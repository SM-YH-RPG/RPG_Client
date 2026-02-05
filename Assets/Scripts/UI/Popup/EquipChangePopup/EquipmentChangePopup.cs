using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EquipmentChangePopup : BasePopup
{
    private const long EMPTY_ITEM_ID = -1;
    private const int MAX_COST_BUDGET = 12;

    [SerializeField] private Image _equipItemPreviewImage;
    [SerializeField] private ToggleGroup _equipmentItemListToggleGroup;
    [SerializeField] private TextMeshProUGUI _currentCostValueText;
    [SerializeField] private EquipmentCostToggle[] _costToggleArray;
    [SerializeField] private ChangeEquipmentSlotElement[] _currentEquipArray;
    [SerializeField] private StatInfo[] _equipStatArray;
    [SerializeField] private GameObject _equipmentItemListPrefab;
    [SerializeField] private Transform _createRoot;
    [SerializeField] private TextMeshProUGUI _equipName;
    [SerializeField] private TextMeshProUGUI _equipCost;
    [SerializeField] private TextMeshProUGUI _equipLevel;
    [SerializeField] private TextMeshProUGUI _equipDesc;
    [SerializeField] private GameObject _currentEquipCharacterObject;
    [SerializeField] private Image _currentEquipCharacterIcon;
    [SerializeField] private TextMeshProUGUI _currentEquipCharacterText;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _changeButton;
    [SerializeField] private Button _enhanceButton;

    private int _totalCost;
    private int _characterIndex;
    private long _prevEquipmentItemIndex;
    private EquipmentItem _currentSelectEquipment;
    private ChangeEquipmentSlotElement _currentEquipSlot;
    private IReadOnlyList<BaseInventoryItem> _equipList;
    private List<EquipmentItemListElement> _itemList;
    private List<EquipmentItem> costFilteredList;

    protected override void Awake()
    {
        base.Awake();

        _equipList = new List<BaseInventoryItem>();
        _itemList = new List<EquipmentItemListElement>();
        costFilteredList = new List<EquipmentItem>();

        _closeButton.onClick.AddListener(OnClickCloseButton);
        _changeButton.onClick.AddListener(OnClickChangeButton);
        _enhanceButton.onClick.AddListener(OnClickEnhanceButton);

        EquipmentManager.Instance.OnEquipmentChanged += ChangeEquipmentItemData;
        EquipmentManager.Instance.OnEquipmentItemDataChanged += UpdateSelectEquipmentDetail;
    }

    private void OnDestroy()
    {
        EquipmentManager.Instance.OnEquipmentChanged -= ChangeEquipmentItemData;
        EquipmentManager.Instance.OnEquipmentItemDataChanged -= UpdateSelectEquipmentDetail;
    }

    public void InitEquipItemData(int characterIndex, EquipmentItem equipmentItem, int slotIndex)
    {
        _characterIndex = characterIndex;
        _currentSelectEquipment = equipmentItem;
        _equipList = PlayerManager.Instance.Inventory.GetInvenItemDataList(EItemCategory.Equipment);
        InitCostCategory();
        InitCurrentEquipSlot(slotIndex);
        SetCurrentCostValue();
    }

    private void InitCostCategory()
    {
        for (int i = 0; i < _costToggleArray.Length; i++)
        {
            switch (i)
            {
                case 0:
                    _costToggleArray[i].InitCostData(EEquipCost.None, UpdateEquipListFilterCost);
                    break;
                case 1:
                    _costToggleArray[i].InitCostData(EEquipCost.One, UpdateEquipListFilterCost);
                    break;
                case 2:
                    _costToggleArray[i].InitCostData(EEquipCost.Three, UpdateEquipListFilterCost);
                    break;
                case 3:
                    _costToggleArray[i].InitCostData(EEquipCost.Four, UpdateEquipListFilterCost);
                    break;
            }
        }
        _costToggleArray[0].OnSelectToggle(true);
    }
    
    private void InitCurrentEquipSlot(int slotIndex)
    {
        _totalCost = 0;
        for (int i = 0; i < _currentEquipArray.Length; i++)
        {            
            _currentEquipArray[i].InitCurrentEquipmentSlotData(i, SetCurrentEquipSlot);
            long instanceId = EquipmentManager.Instance.GetSlotEquippedIntanceID(_characterIndex, i);
            if (instanceId == EMPTY_ITEM_ID)
            {
                _currentEquipArray[i].EmptySlotSetting();
            }
            else
            {
                BaseInventoryItem item = PlayerManager.Instance.Inventory.GetInventoryItemInstance(instanceId);
                if (item is EquipmentItem equip)
                {
                    _currentEquipArray[i].UpdateCurrentEquipSlotData(equip);
                    _totalCost += equip.EquipCost;
                }
            }            
        }
        _currentEquipArray[slotIndex].SelectEquipSlot(true);
    }

    private void UpdateEquipListFilterCost(EEquipCost cost)
    {
        costFilteredList.Clear();

        for (int i = 0; i < _equipList.Count; i++)
        {
            if (_equipList[i] is EquipmentItem equip)
            {
                EquipItemConfigData config = EquipmentDataManager.Instance.GetEquipmentConfigByIndex(_equipList[i].TemplateId);

                if ((int)cost == config.EquipCost)
                {
                    costFilteredList.Add(equip);
                }
                else if ((int)cost == 0) // 전체 아이템..
                {
                    costFilteredList.Add(equip);
                }
            }
        }
        CreateEquipListItem(costFilteredList);
    }

    private void CreateEquipListItem(List<EquipmentItem> equipList)
    {
        int itemsToShowCount = equipList.Count;
        while (_itemList.Count < itemsToShowCount)
        {
            GameObject elementObject = Instantiate(_equipmentItemListPrefab, _createRoot);
            if (elementObject.TryGetComponent(out EquipmentItemListElement item))
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
                _itemList[i].InitEquipmentElement(equipList[i], _equipmentItemListToggleGroup, SetSelectEquipItem, UpdateSelectEquipmentDetail);
                if (_currentSelectEquipment == null)
                {
                    _itemList[0].OnSelectItem(true);                    
                }
                else if (_itemList[i].CheckEquipmentInstanceID(_currentSelectEquipment.InstanceId))
                {
                    _itemList[i].OnSelectItem(true);
                }
            }
        }        
    }

    private void SetSelectEquipItem(EquipmentItem equip)
    {
        _currentSelectEquipment = equip;
    }    

    private void UpdateSelectEquipmentDetail(EquipmentItem equip)
    {
        EquipItemConfigData config = EquipmentDataManager.Instance.GetEquipmentConfigByIndex(equip.TemplateId);
        ItemConfigData itemConfig = ItemDataManager.Instance.GetItemConfigData(equip.Category, equip.TemplateId);
        _equipName.text = itemConfig.Name;
        _equipCost.text = $"COST {config.EquipCost}";
        _equipDesc.text = itemConfig.Description;
        if (equip.Tier >= EquipmentDataManager.Instance.MaxTier)
        {
            _equipLevel.color = Color.red;
        }
        else
        {
            _equipLevel.color = Color.white;
        }

        _equipLevel.text = $"Tier {equip.Tier}";
        UpdateEquipmentStatData(equip);
        if (equip.EquippedCharacterIndex != EMPTY_ITEM_ID)
        {
            SetEquipCharacterData(equip);
        }
        _currentEquipCharacterObject.SetActive(equip.EquippedCharacterIndex != EMPTY_ITEM_ID);
        UpdateEquipmentPreviewImage(equip);
    }

    private void UpdateEquipmentPreviewImage(EquipmentItem equip)
    {        
        ItemConfigData itemConfig = ItemDataManager.Instance.GetItemConfigData(equip.Category, equip.TemplateId);
        _equipItemPreviewImage.sprite = itemConfig.Sprite;
    }

    private void UpdateEquipmentStatData(EquipmentItem equip)
    {
        int statCount = equip.SubStatDict.Count + 2; // Sub스탯 + main스탯 2개
        int statIndex = 0;

        SetStatData(statIndex++, equip.RandomMainStatType, equip.RandomMainStatValue);
        SetStatData(statIndex++, equip.MainStatType, equip.MainStatValue);

        foreach (var (statType, value) in equip.SubStatDict)
        {
            SetStatData(statIndex++, statType, value);
        }

        for (int i = statCount; i < _equipStatArray.Length; i++)
        {
            _equipStatArray[i].gameObject.SetActive(false);
        }
    }

    private void SetStatData(int index, EItemStatType type, float value)
    {
        StatInfo stat = _equipStatArray[index];
        stat.SetEquipItemStatData(value, type);
        stat.gameObject.SetActive(true);
    }

    private async void SetEquipCharacterData(EquipmentItem equip)
    {
        _currentEquipCharacterIcon.sprite = await ResourcesManager.Instance.LoadSpriteAsync($"equip_chacacter_icon_{equip.EquippedCharacterIndex}");
        CharacterConfig config = InGameManager.Instance.GetPlayerController(equip.EquippedCharacterIndex).CharacterData;
        _currentEquipCharacterText.text = $"{config.Name} 장착 중";
    }    

    private void SetCurrentEquipSlot(ChangeEquipmentSlotElement currentSlot)
    {
        _currentEquipSlot = currentSlot;
    }

    private void UpdateEqualEquipmentSlotData()
    {
        // 슬릇중 같은 아이템 있는지 검사
        for (int i = 0; i < _currentEquipArray.Length; i++)
        {
            if (i != _currentEquipSlot.SlotIndex)
            {
                if (_currentEquipArray[i].CheckEqualEquipmentInstanceID(_currentSelectEquipment.InstanceId)) // 선택한 슬릇 제외 다른 슬릇에 같은 장비 껴있다면..
                {                    
                    _currentEquipArray[i].EmptySlotSetting(); // 슬릇 Empty상태로 만들어주기
                    break;
                }
            }
        }
    }

    private void SetCurrentCostValue()
    {
        _currentCostValueText.text = $"{_totalCost}/{MAX_COST_BUDGET}";
    }

    private void OnClickCloseButton()
    {
        UIManager.Instance.Hide();
    }

    private void OnClickChangeButton()
    {        
        _prevEquipmentItemIndex = EquipmentManager.Instance.GetSlotEquippedIntanceID(_characterIndex, _currentEquipSlot.SlotIndex);
        if (_prevEquipmentItemIndex != _currentSelectEquipment.InstanceId) // 장착한 아이템 인덱스가 선택한 아이템과 다르면 교체..
        {
            EquipmentManager.Instance.EquipItem(_characterIndex, _currentSelectEquipment.InstanceId, _currentEquipSlot.SlotIndex);
        }
        else // 선택한 아이템이 장착중인 아이템이면 해제
        {
            EquipmentManager.Instance.TryUnequipWeapon(_currentSelectEquipment.InstanceId, _characterIndex);
        }
    }

    private void ChangeEquipmentItemData()
    {
        _currentEquipSlot.UpdateCurrentEquipSlotData(_currentSelectEquipment); // 선택한 아이템 데이터로 슬릇 셋팅
        _currentEquipSlot.SelectEquipSlot(true);
        InitCurrentEquipSlot(_currentEquipSlot.SlotIndex);
        UpdateEqualEquipmentSlotData(); // 다른 슬릇 검사 및 중복 장비 Empty설정

        SetCurrentCostValue(); // 장착 총합 Cost계산
        for (int i = 0; i < _itemList.Count; i++)
        {
            if (_itemList[i].CheckEquipmentInstanceID(_currentSelectEquipment.InstanceId))
            {
                _itemList[i].UpdateEquipDataOutput(); // 선택한 아이템으로 아이템 장착중 오브젝트 셋팅
            }
            if (_prevEquipmentItemIndex != EMPTY_ITEM_ID)
            {
                if (_itemList[i].CheckEquipmentInstanceID(_prevEquipmentItemIndex))
                {
                    _itemList[i].UpdateEquipDataOutput(); // 이전 아이템 장착중 오브젝트 꺼주기
                }
            }
        }
        UpdateSelectEquipmentDetail(_currentSelectEquipment);
    }

    private async void OnClickEnhanceButton()
    {
        var popup = await UIManager.Instance.Show<EquipmentEnhancePopup>();
        popup.UpdateEquipmentData(_currentSelectEquipment);
    }
}
