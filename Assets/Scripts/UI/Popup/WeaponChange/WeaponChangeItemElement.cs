using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChangeItemElement : MonoBehaviour
{
    [SerializeField] private Toggle _weaponToggle;
    [SerializeField] private Image _weaponIcon;
    [SerializeField] private Image _currentEquipCharacterIcon;
    [SerializeField] private Image _gradeLineImage;
    [SerializeField] private Image _selectLineImage;
    [SerializeField] private TextMeshProUGUI _levelText;    
    [SerializeField] private GameObject _currentEquipCharacterObject;

    private Action<WeaponItem> _OnChangeSelectWeaponItemCallback;
    private Action<WeaponItem> _OnUpdateSelectWeaponDetailCallback;

    private WeaponItem _weaponItem;

    private void Awake()
    {
        _weaponToggle.onValueChanged.AddListener(OnSelectItem);

        WeaponManager.Instance.OnWeaponDataUpdate += UpdateItemDataOutputElement;
    }

    private void OnDestroy()
    {
        WeaponManager.Instance.OnWeaponDataUpdate -= UpdateItemDataOutputElement;
    }

    public void InitElementItem(WeaponItem weapon, ToggleGroup group, Action<WeaponItem> selectItemCallback, Action<WeaponItem> updateDetailCallback)
    {
        _weaponItem = weapon;

        _OnChangeSelectWeaponItemCallback = selectItemCallback;
        _OnUpdateSelectWeaponDetailCallback = updateDetailCallback;

        _weaponToggle.group = group;

        UpdateItemDataOutputElement(weapon);
    }    

    private void UpdateItemDataOutputElement(WeaponItem weapon)
    {
        if (_weaponItem.InstanceId == weapon.InstanceId)
        {
            _gradeLineImage.color = ItemDataManager.Instance.GetGradeColor((int)weapon.Grade);
            SetWeaponItemImage();
            UpdateWeaponDataOutput();
            if (weapon.Tier >= WeaponDataManager.Instance.MaxTier)
            {
                _levelText.color = Color.red;
            }
            else
            {
                _levelText.color = Color.white;
            }

            _levelText.text = $"Tier {weapon.Tier}";
        }
    }

    private void SetWeaponItemImage()
    {        
        ItemConfigData itemConfig = ItemDataManager.Instance.GetItemConfigData(_weaponItem.Category, _weaponItem.TemplateId);
        _weaponIcon.sprite = itemConfig.Sprite;
    }

    public bool CheckWeaponInstanceID(long instanceId)
    {
        return _weaponItem.InstanceId == instanceId;
    }

    public async void UpdateWeaponDataOutput()
    {
        if (_weaponItem.IsEquipped)
        {
            _currentEquipCharacterIcon.sprite = await ResourcesManager.Instance.LoadSpriteAsync($"equip_chacacter_icon_{_weaponItem.EquippedCharacterIndex}");
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
        _weaponToggle.isOn = _isOn;
        if (_isOn)
        {
            _OnChangeSelectWeaponItemCallback?.Invoke(_weaponItem);
            _OnUpdateSelectWeaponDetailCallback.Invoke(_weaponItem);
        }
    }
}
