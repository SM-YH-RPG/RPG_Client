using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MergeCraftDetail : MonoBehaviour
{
    [SerializeField] private GameObject _buttonDimImageObject;
    [SerializeField] private GameObject _needItemPrefab;
    [SerializeField] private Transform _createRoot;
    [SerializeField] private Image _itemIcon;
    [SerializeField] private Image _gradeLine;
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private TextMeshProUGUI _itemAffect;
    [SerializeField] private TextMeshProUGUI _itemCurrentAmount;
    [SerializeField] private TextMeshProUGUI _maxText;
    [SerializeField] private TextMeshProUGUI _mergeAmount;
    [SerializeField] private Slider _itemSlider;
    [SerializeField] private Button _plusButton;
    [SerializeField] private Button _minusButton;
    [SerializeField] private Button _maxButton;
    [SerializeField] private Button _craftButton;

    private MergeItemConfigData _config;
    private ItemConfigData _itemConfig;
    private List<MergeCraftNeedItemElement> _itemList = new List<MergeCraftNeedItemElement>();
    private int _craftCount;

    private void Awake()
    {
        _plusButton.onClick.AddListener(OnClickPlusButton);
        _minusButton.onClick.AddListener(OnClickMinusButton);
        _maxButton.onClick.AddListener(OnClickMaxButton);
        _craftButton.onClick.AddListener(OnClickCraftButton);
        _itemSlider.onValueChanged.AddListener(_ => UpdateMergeItemInfo());
        
        PlayerManager.Instance.Inventory.OnInventoryChanged += OnInventoryChanged;
    }

    private void OnDestroy()
    {        
        PlayerManager.Instance.Inventory.OnInventoryChanged -= OnInventoryChanged;
    }

    public void UpdateDetailView(MergeItemConfigData config)
    {
        _config = config;
        _itemConfig = ItemDataManager.Instance.GetItemConfigData(_config.Category, _config.ItemIndex);
        _itemIcon.sprite = _itemConfig.Sprite;
        _gradeLine.color = ItemDataManager.Instance.GetGradeColor((int)_itemConfig.template.Grade);
        _itemName.text = _itemConfig.Name;
        _itemAffect.text = _itemConfig.AffectDescription;
        _itemCurrentAmount.text = $"보유:{GetCurrentItemCount()}";
        CrateNeedItemElement();
        if (CalculatorMaxCount() == 0)
        {
            _itemSlider.maxValue = 1;
            _itemSlider.value = 1;
        }
        else
        {
            _itemSlider.maxValue = CalculatorMaxCount();
            _itemSlider.value = 0;
        }
        _maxText.text = ((int)_itemSlider.maxValue).ToString();        
        UpdateMergeItemInfo();
    }

    private void OnInventoryChanged()
    {
        _itemCurrentAmount.text = $"보유:{GetCurrentItemCount()}";
        _buttonDimImageObject.SetActive(CheckImpossibleMerge());
    }

    private void CrateNeedItemElement()
    {
        int itemsToShowCount = _config.NeedItemArray.Length;
        while (_itemList.Count < itemsToShowCount)
        {
            GameObject elementObject = Instantiate(_needItemPrefab, _createRoot);
            if (elementObject.TryGetComponent(out MergeCraftNeedItemElement item))
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
                var itemConfig = ItemDataManager.Instance.GetItemConfigData(_config.NeedItemArray[i].Category, _config.NeedItemArray[i].ItemIndex);
                _itemList[i].InitNeedElement(_config.NeedItemArray[i], itemConfig);
            }
        }
    }

    private int GetCurrentItemCount()
    {
        EItemCategory category = _config.Category;
        IReadOnlyList<BaseInventoryItem> itemDatas = PlayerManager.Instance.Inventory.GetInvenItemDataList(category);

        if (itemDatas != null)
        {
            foreach (var item in itemDatas)
            {
                if (item.TemplateId == _config.ItemIndex)
                    return item.Amount;
            }
        }
        return 0;
    }

    private void UpdateMergeItemInfo()
    {
        _craftCount = (int)_itemSlider.value;
        _mergeAmount.text = _craftCount.ToString();
        _buttonDimImageObject.SetActive(CheckImpossibleMerge());
        _craftButton.interactable = _craftCount != 0;
    }

    private int CalculatorMaxCount()
    {
        int maxCount = 0;
        var materialDatas = _config.NeedItemArray;
        for (int i = 0; i < materialDatas.Length; i++)
        {
            int currentCount = InventoryManager.Instance.GetItemCount(materialDatas[i].Category, materialDatas[i].ItemIndex);
            int needCount = materialDatas[i].NeedAmount;
            int calculatorCount = currentCount / needCount;
            if (calculatorCount == 0) // 목록중 하나라도 합성 가능 수량이 0이면 0개..
            {
                maxCount = 0;
                break;
            }
            if (calculatorCount < maxCount || maxCount == 0)
                maxCount = calculatorCount;
        }
        return maxCount;
    }

    private bool CheckImpossibleMerge()
    {
        var materialDatas = _config.NeedItemArray;
        for (int i = 0; i < _config.NeedItemArray.Length; i++)
        {
            int currentCount = InventoryManager.Instance.GetItemCount(materialDatas[i].Category, materialDatas[i].ItemIndex);
            int needCount = materialDatas[i].NeedAmount;
            int calculatorCount = currentCount / needCount;
            if (calculatorCount == 0)
                return true;
        }
        return false;
    }

    private void OnClickMaxButton()
    {
        if (CalculatorMaxCount() == 0)
        {
            _itemSlider.value = 1;
        }
        else
        {
            _itemSlider.value = CalculatorMaxCount();
        }
        UpdateMergeItemInfo();
    }

    private void OnClickPlusButton()
    {
        if (CalculatorMaxCount() == 0)
        {
            _itemSlider.value = 1;
        }
        if ((int)_itemSlider.value < CalculatorMaxCount())
        {
            _itemSlider.value += 1;
        }
        UpdateMergeItemInfo();
    }

    private void OnClickMinusButton()
    {
        if ((int)_itemSlider.value > 0)
        {
            _itemSlider.value -= 1;
        }
        UpdateMergeItemInfo();
    }

    private async void OnClickCraftButton()
    {        
        MergeManager.Instance.AddMergeItem(_config, _itemConfig, _craftCount);
        var resultPopup = await UIManager.Instance.Show<ResultPopup>();
        resultPopup.InitResultItemData((int)_config.Category, _config.ItemIndex, _craftCount, EShopCategory.Mixer);
        UpdateDetailView(_config);
        SaveManager.Instance.RequestSave(ESaveCategory.Mix);
    }
}
