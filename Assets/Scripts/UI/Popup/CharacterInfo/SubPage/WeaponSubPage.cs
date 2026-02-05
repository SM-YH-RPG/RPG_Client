using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSubPage : InfoSubPage
{
    private const int EMPTY_ITEM_ID = 0;

    #region Inspector
    [SerializeField]
    private Camera _previewCamera;

    [SerializeField]
    private TextMeshProUGUI _weaponName;

    [SerializeField]
    private TextMeshProUGUI _weaponLevel;

    [SerializeField]
    private StatInfo[] _weaponStats;

    [SerializeField]
    private TextMeshProUGUI _weaponAffect;

    [SerializeField]
    private TextMeshProUGUI _weaponDesc;

    [SerializeField]
    private Button _equipButton;

    [SerializeField]
    private Button _enhanceButton;
    #endregion

    private long _currentEquipWeaponInstanceID = 0;
    private WeaponItem _weapon = null;

    private void Awake()
    {
        SetupPreview();

        _currentEquipWeaponInstanceID = WeaponManager.Instance.GetCharacterEquippedWeaponInstanceID(_selectedElementIndex);
        if (_currentEquipWeaponInstanceID != EMPTY_ITEM_ID)
        {
            SetWeaponInfo(_selectedElementIndex, _currentEquipWeaponInstanceID);
        }

        _equipButton.onClick.AddListener(OnClickChangeButton);
        _enhanceButton.onClick.AddListener(OnClickEnhanceButton);

        WeaponManager.Instance.OnWeaponEquipped += SetWeaponInfo;
        WeaponManager.Instance.OnWeaponUnequipped += SetWeaponInfo;
        WeaponManager.Instance.OnWeaponDataUpdate += UpdateWeaponInfoDetail;
    }

    private void OnDestroy()
    {
        WeaponManager.Instance.OnWeaponEquipped -= SetWeaponInfo;
        WeaponManager.Instance.OnWeaponUnequipped -= SetWeaponInfo;
        WeaponManager.Instance.OnWeaponDataUpdate -= UpdateWeaponInfoDetail;
    }

    protected override void SetupPreview()
    {
        _preview = new Preview3D(_previewCamera);        
    }

    protected override void HandleElementSelectIndexChanged(int index)
    {
        base.HandleElementSelectIndexChanged(index);
        _currentEquipWeaponInstanceID = WeaponManager.Instance.GetCharacterEquippedWeaponInstanceID(index);
        SetWeaponInfo(index, _currentEquipWeaponInstanceID);
    }

    private void EmptyItemDetail()
    {        
        for (int i = 0; i < _weaponStats.Length; i++)
        {
            _weaponStats[i].gameObject.SetActive(false);
        }
        _weaponName.text = string.Empty;
        _weaponLevel.text = string.Empty;
        _weaponAffect.text = string.Empty;
        _weaponDesc.text = string.Empty;
    }


    private void SetWeaponInfo(int characterIndex, long instanceId)
    {
        if (instanceId <= EMPTY_ITEM_ID)
        {
            EmptyItemDetail();
            return;
        }
        var item = PlayerManager.Instance.Inventory.GetInventoryItemInstance(instanceId);
        if (item is WeaponItem weapon)
        {
            _weapon = weapon;
            UpdateWeaponInfoDetail(weapon);
        }
    }

    private void UpdateWeaponInfoDetail(WeaponItem weapon)
    {
        if (weapon != null)
        {
            if (weapon.InstanceId == _weapon.InstanceId)
            {
                ItemConfigData itemConfig = ItemDataManager.Instance.GetItemConfigData(weapon.Category, weapon.TemplateId);
                _weaponName.text = itemConfig.Name;
                if (weapon.Tier >= WeaponDataManager.Instance.MaxTier)
                {
                    _weaponLevel.color = Color.red;
                }
                else
                {
                    _weaponLevel.color = Color.white;
                }

                _weaponLevel.text = $"Tier {weapon.Tier}";
                _weaponAffect.text = itemConfig.AffectDescription;
                _weaponDesc.text = itemConfig.Description;
                SetWeaponStatData(weapon);
            }
        }
    }

    private void SetWeaponStatData(WeaponItem weapon)
    {
        for (int i = 0; i < _weaponStats.Length; i++)
        {
            if (i == 0) // mainStat ¼ÂÆÃ
            {
                _weaponStats[i].SetEquipItemStatData(weapon.MainStatValue, weapon.MainStatType);
            }
            else
            {
                _weaponStats[i].SetEquipItemStatData(weapon.SubStatValue, weapon.SubStatType);
            }
            _weaponStats[i].gameObject.SetActive(true);
        }
    }

    private async void OnClickChangeButton()
    {
        var popup = await UIManager.Instance.Show<WeaponChangePopup>();
        popup.UpdateWeaponItemListOfWeaponType(_selectedElementIndex);
    }

    private async void OnClickEnhanceButton()
    {
        var popup = await UIManager.Instance.Show<WeaponEnhancePopup>();
        popup.UpdateWeaponData(_weapon);
    }
}
