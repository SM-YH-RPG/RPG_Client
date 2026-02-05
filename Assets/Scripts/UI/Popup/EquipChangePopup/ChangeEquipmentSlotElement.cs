using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ChangeEquipmentSlotElement : MonoBehaviour
{
    private const int EMPTY_CHARACTER_INDEX = -1;

    [SerializeField] private Toggle _toggle;
    [SerializeField] private Image _selectBg;
    [SerializeField] private Image _selectArrow;
    [SerializeField] private Image _equipIcon;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _costText;

    private Action<ChangeEquipmentSlotElement> _OnChangeSelectSlotCallback;
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

    public void InitCurrentEquipmentSlotData(int index, Action<ChangeEquipmentSlotElement> callback)
    {
        _slotIndex = index;
        _OnChangeSelectSlotCallback = callback;
    }

    public void EmptySlotSetting()
    {
        _equipmentItem = null;
        _selectBg.enabled = false;
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

    public bool CheckEqualEquipmentInstanceID(long instanceId)
    {
        if (_equipmentItem != null)
        {
            if (_equipmentItem.InstanceId == instanceId)
                return true;
        }
        return false;
    }

    public void SelectEquipSlot(bool isOn)
    {
        _selectBg.enabled = isOn;
        _selectArrow.enabled = isOn;
        _toggle.isOn = isOn;
        if (isOn)
        {            
            _OnChangeSelectSlotCallback?.Invoke(this);
        }
    }
}
