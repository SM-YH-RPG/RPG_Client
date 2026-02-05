using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPopup : BasePopup
{
    #region Const
    private const int EMPTY_COUNT = 0;
    #endregion

    #region Inspector
    [SerializeField] private GameObject _invenItemPrefab;
    
    [SerializeField] private Transform _createRoot;
    
    [SerializeField] private GameObject _invenItemScrollListObject;
    
    [SerializeField] private ToggleGroup _invenItemToggleGroup;
    
    [SerializeField] private TextMeshProUGUI _nonItemText;
    
    [SerializeField] private TextMeshProUGUI _currentGoldText;
    
    [SerializeField] private TextMeshProUGUI _currentCategoryItemCountText;
    
    [SerializeField] private InvenCategoryButton[] _categoryButtons;
    
    [SerializeField] private UIItemDetailBase[] _selectDetailViews;

    [SerializeField] private Button _closeButton;
    
    [SerializeField] private Button _useButton;
    
    [SerializeField] private Button _enhanceButton;
    #endregion

    private EItemCategory _currentSelectedCategory;
    private UIItemDetailBase _currentSelectView;
    private List<InvenItemElement> _itemList = new List<InvenItemElement>();
    private BaseInventoryItem _currentSelectItem;    
    private int _currnetCategoryItemCount;
    private int _selectedInvenSlotIndex;

    private IInventoryManagerService _inventoryManagerService => PlayerManager.Instance.Inventory;

    protected override void Awake()
    {
        base.Awake();

        _itemList.Clear();
        InitCategoryButtons();
        _closeButton.onClick.AddListener(OnClickCloseButton);
        _enhanceButton.onClick.AddListener(OnClickEnhanceButton);
        HandleCurrentGoldUpdated();
        
        PlayerManager.Instance.OnCurrencyValueChanged += HandleCurrentGoldUpdated;

        _inventoryManagerService.OnInventoryChanged += HandleInventoryDataChanged;
        _inventoryManagerService.OnCategorySelected += HandleSelectCategoryUpdated;
        _currnetCategoryItemCount = 0;
    }

    private void OnDestroy()
    {
        PlayerManager.Instance.OnCurrencyValueChanged -= HandleCurrentGoldUpdated;
        
        _inventoryManagerService.OnInventoryChanged -= HandleInventoryDataChanged;
        _inventoryManagerService.OnCategorySelected -= HandleSelectCategoryUpdated;
    }

    private void InitCategoryButtons()
    {
        int count = _categoryButtons.Length;
        for (int i = 0; i < count; i++)
        {
            _categoryButtons[i].InitCategoryButton(i);
        }
    }

    private void VisibleButtonForCategory()
    {
        _useButton.gameObject.SetActive(_currentSelectedCategory == EItemCategory.Consumable);
        _enhanceButton.gameObject.SetActive(_currentSelectedCategory == EItemCategory.Weapon || _currentSelectedCategory == EItemCategory.Equipment);
    }

    private void HandleCurrentGoldUpdated()
    {
        _currentGoldText.text = PlayerManager.Instance.CurrentCurrencyValue.ToString();
    }

    private void HandleInventoryDataChanged()
    {
        UpdateInvenItem(_currentSelectedCategory);
    }

    private void UpdateInvenItem(EItemCategory category)
    {
        IReadOnlyList<BaseInventoryItem> invenItemList = _inventoryManagerService.GetInvenItemDataList(category);

        if (invenItemList == null)
        {
            _invenItemScrollListObject.SetActive(false);
            _nonItemText.gameObject.SetActive(true);
            UpdateSelectViewData(null);
            SetCurrentCategoryItemCount(EMPTY_COUNT, category);
            return;
        }

        int itemsToShowCount = invenItemList.Count;
        if (itemsToShowCount != _currnetCategoryItemCount)
        {
            // 이전 아이템 개수와 달라졌을때..ex.. 아이템 사용으로 remove로 인한 아이템 삭제등
            _selectedInvenSlotIndex = 0;
        }
        _currnetCategoryItemCount = itemsToShowCount;        

        if (_currentSelectedCategory != category)
        {
            _currentSelectedCategory = category;                        
        }        
        
        while (_itemList.Count < itemsToShowCount)
        {
            GameObject elementObject = Instantiate(_invenItemPrefab, _createRoot);
            if (elementObject.TryGetComponent(out InvenItemElement item))
            {
                _itemList.Add(item);
            }
        }

        for (int i = 0; i < _itemList.Count; i++)
        {
            bool isActive = i < itemsToShowCount;
            _itemList[i].gameObject.SetActive(isActive);

            if (isActive)
            {
                _itemList[i].InitItemElement(i, invenItemList[i], _invenItemToggleGroup, UpdateSelectViewData, SetSelectedInvenItemSlotIndex);
            }
        }

        bool isInventoryEmpty = itemsToShowCount == 0;
        
        _invenItemScrollListObject.SetActive(!isInventoryEmpty);
        _nonItemText.gameObject.SetActive(isInventoryEmpty);

        if (isInventoryEmpty == false)
        {
            _itemList[_selectedInvenSlotIndex].OnSelectItem(true);
        }
        else if (isInventoryEmpty)
        {
            UpdateSelectViewData(null);
        }

        SetCurrentCategoryItemCount(itemsToShowCount, category);
        OnChangeCategory(category);
    }

    private void SetSelectedInvenItemSlotIndex(int index)
    {
        _selectedInvenSlotIndex = index;
    }

    private void OnChangeCategory(EItemCategory category)
    {
        UIItemDetailBase newPage = _selectDetailViews[(int)category];
        if (_currentSelectView == newPage)
        {
            return;
        }

        if (_currentSelectView != null)
        {
            _currentSelectView.gameObject.SetActive(false);
        }

        _currentSelectView = newPage;
        _currentSelectView.gameObject.SetActive(true);
        _currentSelectedCategory = category;
        VisibleButtonForCategory();
    }

    private void UpdateSelectViewData(BaseInventoryItem data)
    {
        _currentSelectItem = data;
        _selectDetailViews[(int)_currentSelectedCategory].UpdateSelectView(data);
    }
    
    private void SetCurrentCategoryItemCount(int count, EItemCategory category)
    {
        int maxCount = ItemDataManager.Instance.GetCategoryMaxSlotCount(category);
        _currentCategoryItemCountText.text = $"{count}/{maxCount}";
    }

    public void HandleSelectCategoryUpdated(EItemCategory category)
    {        
        UpdateInvenItem(category);
        _currentSelectedCategory = category;
        OnChangeCategory(category);        
    }

    private void OnClickCloseButton()
    {
        UIManager.Instance.Hide();
    }

    private async void OnClickEnhanceButton()
    {
        if (_currentSelectItem is WeaponItem weapon)
        {
            var popup = await UIManager.Instance.Show<WeaponEnhancePopup>();
            popup.UpdateWeaponData(weapon);
        }

        if (_currentSelectItem is EquipmentItem equip)
        {
            var popup = await UIManager.Instance.Show<EquipmentEnhancePopup>();
            popup.UpdateEquipmentData(equip);
        }
    }
}
