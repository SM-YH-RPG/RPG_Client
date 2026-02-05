using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopupEquipmentSlotElement : MonoBehaviour
{
    private const int EMPTY_CHARACTER_INDEX = -1;

    [SerializeField] private Toggle _toggle;    
    [SerializeField] private Image _selectArrow;
    [SerializeField] private Image _equipIcon;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _costText;

    private Action<InfoPopupEquipmentSlotElement> _onSelectSlotCallback;
    private EquipmentItem _equipmentItem;

    private int _slotIndex;
    public int SlotIndex => _slotIndex;

    private void Awake()
    {
        EmptySlotSetting();

        _toggle.onValueChanged.AddListener(SelectEquipSlot);

        EquipmentManager.Instance.OnEquipmentItemDataChanged += SetEquipmentItemDataOnSlot;
    }

    private void OnDestroy()
    {
        EquipmentManager.Instance.OnEquipmentItemDataChanged -= SetEquipmentItemDataOnSlot;
    }

    public void InitEquipmentSlot(int index, Action<InfoPopupEquipmentSlotElement> selectSlotCallback)
    {
        _slotIndex = index;

        _onSelectSlotCallback = selectSlotCallback;
    }

    public void EmptySlotSetting()
    {
        _equipmentItem = null;        
        _selectArrow.enabled = false;
        _equipIcon.enabled = false;
        _levelText.text = string.Empty;
        _costText.text = string.Empty;
    }

    public void UpdateCurrentEquipSlotData(EquipmentItem equip)
    {
        _equipmentItem = equip;
        if (equip.EquippedCharacterIndex == EMPTY_CHARACTER_INDEX)
        {
            EmptySlotSetting();
        }
        else
        {
            SetEquipmentItemDataOnSlot(equip);
        }
    }

    private void SetEquipmentItemDataOnSlot(EquipmentItem equip)
    {
        if (equip.InstanceId == EquipmentManager.Instance.GetSlotEquippedIntanceID(equip.EquippedCharacterIndex, _slotIndex))
        {            
            ItemConfigData itemConfig = ItemDataManager.Instance.GetItemConfigData(equip.Category, equip.TemplateId);
            _equipIcon.enabled = true;
            _equipIcon.sprite = itemConfig.Sprite;
            if (equip.Tier >= EquipmentDataManager.Instance.MaxTier)
            {
                _levelText.color = Color.red;
            }
            else
            {
                _levelText.color = Color.white;
            }

            _levelText.text = $"T {equip.Tier}";
            _costText.text = equip.EquipCost.ToString();
        }
    }

    public void SelectEquipSlot(bool isOn)
    {        
        _selectArrow.enabled = isOn;
        _toggle.isOn = isOn;
        if (isOn)
        {
            _onSelectSlotCallback?.Invoke(this);
        }
    }
}
