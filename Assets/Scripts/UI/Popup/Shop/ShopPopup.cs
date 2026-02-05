using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPopup : BasePopup
{
    #region Inspector
    [SerializeField] private GameObject _shopItemPrefab;
    [SerializeField] private Transform _createRoot;
    
    [SerializeField] private ToggleGroup _shopItemToggleGroup;

    [SerializeField] private TextMeshProUGUI _shopNameText;
    [SerializeField] private TextMeshProUGUI _currentCurrencyValue;
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemDescText;
    [SerializeField] private TextMeshProUGUI _currentItemCountText;
    [SerializeField] private TextMeshProUGUI _purchaseLimitText;
    [SerializeField] private TextMeshProUGUI _purchaseMaxText;    
    [SerializeField] private TextMeshProUGUI _buyItemCountText;
    [SerializeField] private TextMeshProUGUI _buyItemPriceText;

    [SerializeField] private Slider _buyCountSlider;

    [SerializeField] private Button _buyCountPlusButton;
    [SerializeField] private Button _buyCountMinusButton;
    [SerializeField] private Button _purchaseButton;
    [SerializeField] private Button _closeButton;
    #endregion

    private List<SaleShopItemElement> _itemList = new List<SaleShopItemElement>();
    private ShopItemData _currentItemData;
    private ItemConfigData _currentItemConfig;    
    private EShopCategory _currentShopCategory;
    private int buyCount;

    private IInventoryManagerService _inventoryManagerService => PlayerManager.Instance.Inventory;

    private IInteractionService _interactionService => PlayerManager.Instance.InteractionService;

    protected override void Awake()
    {
        base.Awake();

        _buyCountPlusButton.onClick.AddListener(OnClickBuyCountPlusButton);
        _buyCountMinusButton.onClick.AddListener(OnClickBuyCountMinusButton);
        _purchaseButton.onClick.AddListener(OnClickPurchaseButton);
        _closeButton.onClick.AddListener(OnClickCloseButton);
        _buyCountSlider.onValueChanged.AddListener(_ => UpdateBuyItemInfo());

        PlayerManager.Instance.OnCurrencyValueChanged += UpdateCurrentCurrencyValue;
        _inventoryManagerService.OnInventoryChanged += OnInventoryChanged;        

        CreateShopItems();
        UpdateCurrentCurrencyValue();
        _itemList.Clear();
    }

    private void OnDestroy()
    {
        PlayerManager.Instance.OnCurrencyValueChanged -= UpdateCurrentCurrencyValue;
        _inventoryManagerService.OnInventoryChanged -= OnInventoryChanged;
    }

    public override void Hide()
    {
        _interactionService.NotifyInteractionTargetChanged(null);
        InGameManager.Instance.NotifyObjectSetActiveInShop(true);

        base.Hide();
    }

    public void SetShopNameText(string uiName)
    {
        _shopNameText.text = uiName;
    }

    public void SetShopCategoryType(EShopCategory category)
    {
        InGameManager.Instance.NotifyObjectSetActiveInShop(false);
        _currentShopCategory = category;
        _buyCountSlider.value = _buyCountSlider.minValue;
    }

    private void OnInventoryChanged()
    {
        _currentItemCountText.text = GetCurrentItemCount().ToString();
        UpdatePurchaseButtonState();
    }

    private void CreateShopItems()
    {
        ShopItemData[] datas = ShopTable.Instance.GetShopItemDatas();        
        int dataCount = dataCount = datas.Length;                  

        while (_itemList.Count < dataCount)
        {
            GameObject obj = Instantiate(_shopItemPrefab, _createRoot);
            if (obj.TryGetComponent(out SaleShopItemElement item))
            {
                _itemList.Add(item);
            }
        }

        for (int i = 0; i < _itemList.Count; i++)
        {
            bool isActive = i < dataCount;
            _itemList[i].gameObject.SetActive(isActive);

            if (isActive)
            {
                _itemList[i].InitItemElement(datas[i], _shopItemToggleGroup, UpdateSelectItemData);
            }
        }

        if (_itemList.Count > 0)
        {
            _itemList[0].SetToggleSelect();
            UpdateSelectItemData(datas[0]);
        }
    }

    private void UpdateSelectItemData(ShopItemData data)
    {
        _currentItemData = data;        
        ItemConfigData config = ItemDataManager.Instance.GetItemConfigData((EItemCategory)data.CategoryIndex, data.Index);
        _currentItemConfig = config;

        _itemDescText.text = config.Description;

        _currentItemCountText.text = GetCurrentItemCount().ToString();
        UpdateCurrentCurrencyValue();

        _purchaseLimitText.text = $"구매 제한 {data.PurchaseLimitCount}/{data.PurchaseLimitCount}";

        _buyCountSlider.minValue = 1;
        _buyCountSlider.maxValue = data.PurchaseLimitCount > 0 ? data.PurchaseLimitCount : 99;
        _buyCountSlider.value = 1;

        _purchaseMaxText.text = _buyCountSlider.maxValue.ToString();

        UpdateBuyItemInfo();
    }

    private int GetCurrentItemCount()
    {
        EItemCategory category = (EItemCategory)_currentItemData.CategoryIndex;
        IReadOnlyList<BaseInventoryItem> itemDatas = _inventoryManagerService.GetInvenItemDataList(category);

        if (itemDatas != null)
        {
            foreach (var item in itemDatas)
            {
                if (item.TemplateId == _currentItemData.Index)
                    return item.Amount;
            }
        }

        return 0;
    }

    private void UpdatePurchaseButtonState()
    {
        _purchaseButton.interactable = CheckPossiblePurchase();
    }

    private void UpdateBuyItemInfo()
    {
        buyCount = (int)_buyCountSlider.value;
        int totalPrice = _currentItemData.Price * buyCount;       
        _itemNameText.text = $"{_currentItemConfig.Name}x{buyCount}";
        _buyItemCountText.text = buyCount.ToString();
        _buyItemPriceText.text = totalPrice.ToString();

        UpdatePurchaseButtonState();
    }

    private void UpdateCurrentCurrencyValue()
    {
        _currentCurrencyValue.text = PlayerManager.Instance.CurrentCurrencyValue.ToString();
        UpdatePurchaseButtonState();
    }

    private bool CheckPossiblePurchase()
    {
        buyCount = (int)_buyCountSlider.value;
        int totalPrice = _currentItemData.Price * buyCount;        
        return PlayerManager.Instance.CurrentCurrencyValue >= totalPrice;
    }

    private void OnClickBuyCountPlusButton()
    {
        _buyCountSlider.value += 1;
        UpdateBuyItemInfo();
    }

    private void OnClickBuyCountMinusButton()
    {
        _buyCountSlider.value -= 1;
        UpdateBuyItemInfo();
    }

    private void OnClickPurchaseButton()
    {
        buyCount = (int)_buyCountSlider.value;
        int totalPrice = _currentItemData.Price * buyCount;        

        if (CheckPossiblePurchase() == false)
        {
            return;
        }

        PlayerManager.Instance.UpdateCurrentCurrencyValue(PlayerManager.Instance.CurrentCurrencyValue -totalPrice);
        var itemConfig = ItemDataManager.Instance.GetItemConfigData((EItemCategory)_currentItemData.CategoryIndex, _currentItemData.Index);
        _inventoryManagerService.AddItem(_currentItemData.Index, (EItemCategory)_currentItemData.CategoryIndex, itemConfig.template.Grade, itemConfig.Name, buyCount);
        ShowResultPopup();
    }

    private async void ShowResultPopup()
    {
        var resultPopup = await UIManager.Instance.Show<ResultPopup>();
        resultPopup.InitResultItemData(_currentItemData.CategoryIndex, _currentItemData.Index, buyCount, _currentShopCategory);
    }    

    private void OnClickCloseButton()
    {
        InGameManager.Instance.NotifyObjectSetActiveInShop(true);
        UIManager.Instance.Hide();
    }
}
