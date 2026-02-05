using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWeaponItemDetail : UIItemDetailBase
{
    [SerializeField] private Image _gradeLineImage;
    [SerializeField] private StatInfo[] _weaponStatArray;
    [SerializeField] private TextMeshProUGUI _weaponAffectText;
    [SerializeField] private TextMeshProUGUI _weaponeDescText;
    [SerializeField] private TextMeshProUGUI _weaponLevelText;
    [SerializeField] private TextMeshProUGUI _currentEquipCharacterText;
    [SerializeField] private GameObject _currentEquipObject;
    [SerializeField] private Image _currentEquipCharacterImage;

    private void Awake()
    {
        WeaponManager.Instance.OnWeaponDataUpdate += UpdateWeaponDetailView;
    }

    private void OnDestroy()
    {
        WeaponManager.Instance.OnWeaponDataUpdate -= UpdateWeaponDetailView;
    }

    public override void UpdateSelectView(BaseInventoryItem data)
    {
        if (data is WeaponItem weapon)
        {
            UpdateWeaponDetailView(weapon);
        }
    }

    private void UpdateWeaponDetailView(WeaponItem weapon)
    {
        ItemConfigData itemConfig = ItemDataManager.Instance.GetItemConfigData(weapon.Category, weapon.TemplateId);        

        _itemImage.sprite = itemConfig.Sprite;
        _itemName.text = itemConfig.Name;
        _gradeLineImage.color = ItemDataManager.Instance.GetGradeColor((int)weapon.Grade);
        _weaponAffectText.text = itemConfig.AffectDescription;
        _weaponeDescText.text = itemConfig.Description;
        if (weapon.Tier >= WeaponDataManager.Instance.MaxTier)
        {
            _weaponLevelText.color = Color.red;
        }
        else
        {
            _weaponLevelText.color = Color.white;
        }

        _weaponLevelText.text = $"Tier {weapon.Tier}";
        WeaponStatDataInit(weapon);
        if (weapon.EquippedCharacterIndex != -1)
        {
            SetEquipCharacterData(weapon);
        }
        _currentEquipObject.SetActive(weapon.EquippedCharacterIndex != -1);
    }

    private void WeaponStatDataInit(WeaponItem weapon)
    {
        for (int i = 0; i < _weaponStatArray.Length; i++)
        {
            if (i == 0) // mainStat ¼ÂÆÃ
            {
                _weaponStatArray[i].SetEquipItemStatData(weapon.MainStatValue, weapon.MainStatType);
            }
            else
            {
                _weaponStatArray[i].SetEquipItemStatData(weapon.SubStatValue, weapon.SubStatType);
            }
        }
    }

    private async void SetEquipCharacterData(WeaponItem weapon)
    {
        _currentEquipCharacterImage.sprite = await ResourcesManager.Instance.LoadSpriteAsync($"equip_chacacter_icon_{weapon.EquippedCharacterIndex}");
        CharacterConfig config = InGameManager.Instance.GetPlayerController(weapon.EquippedCharacterIndex).CharacterData;
        _currentEquipCharacterText.text = $"{config.Name} ÀåÂø Áß";
    }
}
