using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSubPage : InfoSubPage
{
    private const int EMPTY_ITEM_ID = -1;

    #region Inspector
    [SerializeField] private Image _thumbnail;

    [SerializeField] private StatInfo[] _equipmentStats;

    [SerializeField] private TextMeshProUGUI _abilityName;

    [SerializeField] private TextMeshProUGUI _abliltyDesc;

    [SerializeField] private TextMeshProUGUI _affectName;

    [SerializeField] private TextMeshProUGUI _affectDesc;

    [SerializeField] private TextMeshProUGUI _currentCostValue;

    [SerializeField] private InfoPopupEquipmentSlotElement[] _equipmentSlotArray;

    [SerializeField] private Button _equipButton;    
    #endregion
    
    private EquipmentItem _equipmentItem = null;
    private InfoPopupEquipmentSlotElement _currentSlot;

    private void Awake()
    {
        SetupPreview();
        UpdateEquipmentSlotData();
        _equipButton.onClick.AddListener(OnClickChangeButton);

        EquipmentManager.Instance.OnEquipmentChanged += UpdateEquipmentSlotData;
        EquipmentManager.Instance.OnEquipmentItemDataChanged += UpdateEquipmentInfoDetail;
    }

    private void OnDestroy()
    {
        EquipmentManager.Instance.OnEquipmentChanged -= UpdateEquipmentSlotData;
        EquipmentManager.Instance.OnEquipmentItemDataChanged -= UpdateEquipmentInfoDetail;
    }

    protected override void SetupPreview()
    {
        _preview = new Preview2D(_thumbnail);
    }

    protected override void HandleElementSelectIndexChanged(int index)
    {
        base.HandleElementSelectIndexChanged(index);
        UpdateEquipmentSlotData();
    }

    private void EmptyItemDetail()
    {
        _thumbnail.gameObject.SetActive(false);
        for (int i = 0; i < _equipmentStats.Length; i++)
        {
            _equipmentStats[i].gameObject.SetActive(false);
        }
        _abilityName.text = string.Empty;
        _abliltyDesc.text = string.Empty;
        _affectName.text = string.Empty;
        _affectDesc.text = string.Empty;
    }

    private void UpdateEquipmentSlotData()
    {
        int cost = 0;
        for (int i = 0; i < _equipmentSlotArray.Length; i++)
        {
            _equipmentSlotArray[i].InitEquipmentSlot(i, SetCurrentInfoSlot);
            long instanceId = EquipmentManager.Instance.GetSlotEquippedIntanceID(_selectedElementIndex, _equipmentSlotArray[i].SlotIndex);
            if (instanceId != EMPTY_ITEM_ID)
            {
                var item = PlayerManager.Instance.Inventory.GetInventoryItemInstance(instanceId);
                if (item is EquipmentItem equip)
                {
                    _equipmentSlotArray[i].UpdateCurrentEquipSlotData(equip);
                    cost += equip.EquipCost;
                }
            }
            else
            {
                _equipmentSlotArray[i].EmptySlotSetting();
            }            
            _equipmentSlotArray[i].SelectEquipSlot(false);
        }
        _currentCostValue.text = cost.ToString();
        _equipmentSlotArray[0].SelectEquipSlot(true);
    }

    private void SetEquipmentInfo()
    {
        long instanceId = EquipmentManager.Instance.GetSlotEquippedIntanceID(_selectedElementIndex, _currentSlot.SlotIndex);

        if (instanceId == EMPTY_ITEM_ID)
        {
            EmptyItemDetail();
            _equipmentItem = null;
            return;
        }

        var item = PlayerManager.Instance.Inventory.GetInventoryItemInstance(instanceId);
        if (item is EquipmentItem equip)
        {
            _equipmentItem = equip;
            UpdateEquipmentInfoDetail(equip);
        }
    }

    private void UpdateEquipmentInfoDetail(EquipmentItem equip)
    {
        if (_equipmentItem != null)
        {
            if (_equipmentItem.InstanceId == equip.InstanceId)
            {                
                ItemConfigData itemConfig = ItemDataManager.Instance.GetItemConfigData(equip.Category, equip.TemplateId);
                _thumbnail.gameObject.SetActive(true);
                _thumbnail.sprite = itemConfig.Sprite;
                _abilityName.text = "장비 설명";
                _abliltyDesc.text = itemConfig.Description;
                _affectName.text = "장비 효과";
                _affectDesc.text = itemConfig.AffectDescription;
                SetEquipmentStatData(equip);
            }
        }
    }

    private void SetEquipmentStatData(EquipmentItem equip)
    {
        int statCount = equip.SubStatDict.Count + 2; // Sub스탯 + main스탯 2개
        int statIndex = 0;

        SetStatData(statIndex++, equip.RandomMainStatType, equip.RandomMainStatValue);
        SetStatData(statIndex++, equip.MainStatType, equip.MainStatValue);

        foreach (var (statType, value) in equip.SubStatDict)
        {
            SetStatData(statIndex++, statType, value);
        }

        for (int i = statCount; i < _equipmentStats.Length; i++)
        {
            _equipmentStats[i].gameObject.SetActive(false);
        }
    }

    private void SetStatData(int index, EItemStatType type, float value)
    {
        StatInfo stat = _equipmentStats[index];
        stat.SetEquipItemStatData(value, type);
        stat.gameObject.SetActive(true);
    }

    private void SetCurrentInfoSlot(InfoPopupEquipmentSlotElement slot)
    {
        _currentSlot = slot;
        SetEquipmentInfo();
    }
    
    private async void OnClickChangeButton()
    {
        var popup = await UIManager.Instance.Show<EquipmentChangePopup>();
        popup.InitEquipItemData(_selectedElementIndex, _equipmentItem, _currentSlot.SlotIndex);
    }
}
