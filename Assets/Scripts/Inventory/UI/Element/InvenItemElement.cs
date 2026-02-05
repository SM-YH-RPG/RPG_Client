using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvenItemElement : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;
    [SerializeField] private Image _selectImage;
    [SerializeField] private Image _itemImage;
    [SerializeField] private Image _equipCharacterIcon;
    [SerializeField] private Image _itemGradeLine;
    [SerializeField] private TextMeshProUGUI _itemText;
    [SerializeField] private GameObject _equipCharacterObject;

    private Action<BaseInventoryItem> _OnChangeSelectItemCallback;
    private Action<int> _OnSelectedInvenSlotIndexCallback;
    private BaseInventoryItem _data;
    private int _invenSlotIndex = 0;

    private void Awake()
    {
        _toggle.onValueChanged.AddListener(OnSelectItem);
        WeaponManager.Instance.OnWeaponDataUpdate += UpdateItemDataOutputElement;
        EquipmentManager.Instance.OnEquipmentItemDataChanged += UpdateItemDataOutputElement;
    }

    private void OnDestroy()
    {
        WeaponManager.Instance.OnWeaponDataUpdate -= UpdateItemDataOutputElement;
        EquipmentManager.Instance.OnEquipmentItemDataChanged -= UpdateItemDataOutputElement;
    }

    public void InitItemElement(int index, BaseInventoryItem data, ToggleGroup toggleGroup, Action<BaseInventoryItem> _callback, Action<int> _slotIndexCallback)
    {
        _invenSlotIndex = index;
        _data = data;
        _toggle.group = toggleGroup;

        _equipCharacterObject.SetActive(false);
        UpdateItemDataOutputElement(_data);

        _OnChangeSelectItemCallback = _callback;
        _OnSelectedInvenSlotIndexCallback = _slotIndexCallback;
    }

    private void SetItemIconImage(BaseInventoryItem data)
    {
        ItemConfigData config = ItemDataManager.Instance.GetItemConfigData(data.Category, data.TemplateId);
        _itemImage.sprite = config.Sprite;        
    }

    private async void UpdateItemDataOutputElement(BaseInventoryItem data)
    {
        if (_data.InstanceId == data.InstanceId)
        {
            _itemGradeLine.color = ItemDataManager.Instance.GetGradeColor((int)data.Grade);
            SetItemIconImage(data);

            if (data is EquipableItem equipItem)
            {
                if (equipItem.IsEquipped)
                {
                    _equipCharacterIcon.sprite = await ResourcesManager.Instance.LoadSpriteAsync($"equip_chacacter_icon_{equipItem.EquippedCharacterIndex}");
                    _equipCharacterObject.SetActive(true);
                }
                else
                {
                    _equipCharacterObject.SetActive(false);
                }

                if (equipItem.Tier >= EquipmentDataManager.Instance.MaxTier)
                {
                    _itemText.color = Color.red;
                }
                else
                {
                    _itemText.color = Color.white;
                }

                _itemText.text = $"Tier {equipItem.Tier}";
            }
            else
            {
                _itemText.color = Color.white;
            }

            switch (data.Category)
            {
                case EItemCategory.Consumable:
                case EItemCategory.Material:
                    _itemText.text = data.Amount.ToString();
                    break;
            }
        }
    }

    public void OnSelectItem(bool _isOn)
    {
        _selectImage.enabled = _isOn;
        _toggle.isOn = _isOn;
        if (_isOn)
        {
            _OnChangeSelectItemCallback?.Invoke(_data);
            _OnSelectedInvenSlotIndexCallback?.Invoke(_invenSlotIndex);
        }
    }
}
