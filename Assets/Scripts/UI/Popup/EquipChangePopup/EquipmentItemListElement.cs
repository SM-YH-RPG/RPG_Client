using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EquipmentItemListElement : MonoBehaviour
{
    [SerializeField] private Toggle _equipmentToggle;
    [SerializeField] private Image _equipIcon;
    [SerializeField] private Image _gradeLineImage;
    [SerializeField] private Image _selectLineImage;
    [SerializeField] private Image _currentEquipCharacterIcon;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private GameObject _currentEquipCharacterObject;

    private Action<EquipmentItem> _OnChangeSelectEquipmentItemCallback;
    private Action<EquipmentItem> _OnUpdateSelectEquipmentDetailCallback;

    private EquipmentItem _equipmentItem;

    private void Awake()
    {
        _equipmentToggle.onValueChanged.AddListener(OnSelectItem);

        EquipmentManager.Instance.OnEquipmentItemDataChanged += UpdateItemDataOutputElement;
    }

    private void OnDestroy()
    {
        EquipmentManager.Instance.OnEquipmentItemDataChanged -= UpdateItemDataOutputElement;
    }

    public void InitEquipmentElement(EquipmentItem equip, ToggleGroup group, Action<EquipmentItem> changeSelectItemCallback, Action<EquipmentItem> updateItemDetailCallback)
    {
        _equipmentItem = equip;

        _OnChangeSelectEquipmentItemCallback = changeSelectItemCallback;
        _OnUpdateSelectEquipmentDetailCallback = updateItemDetailCallback;

        _equipmentToggle.group = group;

        UpdateItemDataOutputElement(equip);
    }    

    private void UpdateItemDataOutputElement(EquipmentItem equip)
    {
        if (_equipmentItem.InstanceId == equip.InstanceId)
        {
            _gradeLineImage.color = ItemDataManager.Instance.GetGradeColor((int)equip.Grade);
            SetEquipmentItemImage();
            UpdateEquipDataOutput();
            if (equip.Tier >= EquipmentDataManager.Instance.MaxTier)
            {
                _levelText.color = Color.red;
            }
            else
            {
                _levelText.color = Color.white;
            }

            _levelText.text = $"Tier {equip.Tier}";
            _costText.text = equip.EquipCost.ToString();
        }
    }

    private void SetEquipmentItemImage()
    {        
        ItemConfigData itemConfig = ItemDataManager.Instance.GetItemConfigData(_equipmentItem.Category, _equipmentItem.TemplateId);
        _equipIcon.sprite = itemConfig.Sprite;
    }

    public bool CheckEquipmentInstanceID(long instanceId)
    {
        return _equipmentItem.InstanceId == instanceId;
    }

    public async void UpdateEquipDataOutput()
    {
        if (_equipmentItem.IsEquipped)
        {            
            _currentEquipCharacterIcon.sprite = await ResourcesManager.Instance.LoadSpriteAsync($"equip_chacacter_icon_{_equipmentItem.EquippedCharacterIndex}");
            _currentEquipCharacterObject.SetActive(true);
        }
        else
        {
            _currentEquipCharacterObject.SetActive(false);
        }
    }

    public void OnSelectItem(bool _isOn)
    {
        _selectLineImage.enabled = _isOn;
        _equipmentToggle.isOn = _isOn;
        if (_isOn)
        {
            _OnChangeSelectEquipmentItemCallback?.Invoke(_equipmentItem);
            _OnUpdateSelectEquipmentDetailCallback.Invoke(_equipmentItem);
        }
    }
}
