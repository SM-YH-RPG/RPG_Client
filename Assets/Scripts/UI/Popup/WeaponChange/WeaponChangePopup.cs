using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChangePopup : BasePopup
{
    private const long EMPTY_ITEM_ID = -1;

    [SerializeField] private GameObject _weaponItemElementPrefab;
    [SerializeField] private ToggleGroup _toggleGroup;
    [SerializeField] private Transform _createRoot;
    [SerializeField] private Image _weaponTypeIcon;
    [SerializeField] private TextMeshProUGUI _weaponNaem;
    [SerializeField] private TextMeshProUGUI _weaponLevel;
    [SerializeField] private TextMeshProUGUI _weaponAffectText;
    [SerializeField] private TextMeshProUGUI _weaponDesc;
    [SerializeField] private TextMeshProUGUI _equipCharacterText;
    [SerializeField] private StatInfo[] _weaponStatArray;
    [SerializeField] private GameObject _equipCharacterObject;
    [SerializeField] private Image _equipCharacterIcon;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _changeButton;
    [SerializeField] private Button _enhanceButton;

    private IReadOnlyList<BaseInventoryItem> _weaponList;    
    private List<WeaponChangeItemElement> _itemList;
    private int _CharacterIndex;
    private WeaponItem _currentSelectWeapon;    

    protected override void Awake()
    {
        base.Awake();

        _weaponList = new List<BaseInventoryItem>();
        _itemList = new List<WeaponChangeItemElement>();

        _closeButton.onClick.AddListener(OnClickCloseButton);
        _changeButton.onClick.AddListener(OnClickChangeButton);
        _enhanceButton.onClick.AddListener(OnClickEnhanceButton);

        PlayerManager.Instance.Inventory.OnInventoryChanged += UpdateInventoryChange;
        WeaponManager.Instance.OnWeaponDataUpdate += UpdateSelectWeaponDetail;
    }

    private void OnDestroy()
    {
        PlayerManager.Instance.Inventory.OnInventoryChanged -= UpdateInventoryChange;
        WeaponManager.Instance.OnWeaponDataUpdate -= UpdateSelectWeaponDetail;
    }

    public async void UpdateWeaponItemListOfWeaponType(int characterIndex)
    {
        _CharacterIndex = characterIndex;
        _weaponList = PlayerManager.Instance.Inventory.GetInvenItemDataList(EItemCategory.Weapon);
        List<WeaponItem> filteredList = new List<WeaponItem>();
        CharacterConfig characterConfig = InGameManager.Instance.GetPlayerController(_CharacterIndex).CharacterData;
        for (int i = 0; i < _weaponList.Count; i++)
        {            
            if (_weaponList[i] is WeaponItem weapon)
            {
                WeaponItemConfigData config = WeaponDataManager.Instance.GetWeaponConfigByIndex(_weaponList[i].TemplateId);
                if (config.Type == characterConfig.WeaponType)
                {
                    filteredList.Add(weapon);
                }
            }
        }
        
        _weaponTypeIcon.sprite = await WeaponDataManager.Instance.GetWeaponTypeSprite(characterConfig.WeaponType);
        CreateWeaponListItem(filteredList);
    }

    private void UpdateInventoryChange()
    {
        _weaponList = PlayerManager.Instance.Inventory.GetInvenItemDataList(EItemCategory.Weapon);
        List<WeaponItem> filteredList = new List<WeaponItem>();
        CharacterConfig characterConfig = InGameManager.Instance.GetPlayerController(_CharacterIndex).CharacterData;
        for (int i = 0; i < _weaponList.Count; i++)
        {
            if (_weaponList[i] is WeaponItem weapon)
            {
                WeaponItemConfigData config = WeaponDataManager.Instance.GetWeaponConfigByIndex(_weaponList[i].TemplateId);
                if (config.Type == characterConfig.WeaponType)
                {
                    filteredList.Add(weapon);
                }
            }
        }
        CreateWeaponListItem(filteredList);
    }

    private void CreateWeaponListItem(List<WeaponItem> weaponList)
    {
        SortWeaponItemList(weaponList);
        int itemsToShowCount = weaponList.Count;
        while (_itemList.Count < itemsToShowCount)
        {
            GameObject elementObject = Instantiate(_weaponItemElementPrefab, _createRoot);
            if (elementObject.TryGetComponent(out WeaponChangeItemElement item))
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
                _itemList[i].InitElementItem(weaponList[i], _toggleGroup, SetCurrentSelectWeaponItem, UpdateSelectWeaponDetail);
                if (_currentSelectWeapon == null)
                {
                    _itemList[0].OnSelectItem(true);
                }
                else if (_itemList[i].CheckWeaponInstanceID(_currentSelectWeapon.InstanceId))
                {
                    _itemList[i].OnSelectItem(true);
                }
            }
        }
    }

    private void SortWeaponItemList(List<WeaponItem> weaponList)
    {
        // 등급 내림차순 -> 티어 -> InstanceID 내림차순
        weaponList.Sort((a, b) =>
        {
            // 1. 등급 내림차순
            int result = b.Grade.CompareTo(a.Grade);
            if (result != 0) return result;

            // 2. 티어 내림차순
            result = b.Tier.CompareTo(a.Tier);
            if (result != 0) return result;

            // 3. InstanceID 내림차순
            return b.InstanceId.CompareTo(a.InstanceId);
        });
    }

    private void SetCurrentSelectWeaponItem(WeaponItem weapon)
    {
        _currentSelectWeapon = weapon;
    }

    private void UpdateSelectWeaponDetail(WeaponItem weapon)
    {        
        ItemConfigData itemConfig = ItemDataManager.Instance.GetItemConfigData(weapon.Category, weapon.TemplateId);

        _weaponNaem.text = weapon.Name;        
        _weaponAffectText.text = itemConfig.AffectDescription;
        _weaponDesc.text = itemConfig.Description;
        if (weapon.Tier >= WeaponDataManager.Instance.MaxTier)
        {
            _weaponLevel.color = Color.red;
        }
        else
        {
            _weaponLevel.color = Color.white;
        }

        _weaponLevel.text = $"Tier {weapon.Tier}";
        UpdateWeaponStatData(weapon);
        if (weapon.EquippedCharacterIndex != EMPTY_ITEM_ID)
        {
            SetEquipCharacterData(weapon);
        }
        _equipCharacterObject.SetActive(weapon.EquippedCharacterIndex != EMPTY_ITEM_ID);
    }

    private void UpdateWeaponStatData(WeaponItem weapon)
    {
        for (int i = 0; i < _weaponStatArray.Length; i++)
        {
            if (i == 0) // mainStat 셋팅
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
        _equipCharacterIcon.sprite = await ResourcesManager.Instance.LoadSpriteAsync($"equip_chacacter_icon_{weapon.EquippedCharacterIndex}");
        CharacterConfig config = InGameManager.Instance.GetPlayerController(weapon.EquippedCharacterIndex).CharacterData;
        _equipCharacterText.text = $"{config.Name} 장착 중";
    }

    

    private void OnClickCloseButton()
    {
        UIManager.Instance.Hide();
    }

    private void OnClickChangeButton()
    {
        long prevWeaponInstanceId = EMPTY_ITEM_ID;
        long instanceId = WeaponManager.Instance.GetCharacterEquippedWeaponInstanceID(_CharacterIndex);
        if (instanceId != EMPTY_ITEM_ID)
        {
            prevWeaponInstanceId = instanceId;
        }

        if (prevWeaponInstanceId != _currentSelectWeapon.InstanceId)
        {
            WeaponManager.Instance.EquipItem(_CharacterIndex, _currentSelectWeapon.InstanceId);

            for (int i = 0; i < _itemList.Count; i++)
            {
                if (_itemList[i].CheckWeaponInstanceID(_currentSelectWeapon.InstanceId))
                {
                    _itemList[i].UpdateWeaponDataOutput();
                }
                if (prevWeaponInstanceId != EMPTY_ITEM_ID)
                {
                    if (_itemList[i].CheckWeaponInstanceID(prevWeaponInstanceId))
                    {
                        _itemList[i].UpdateWeaponDataOutput();
                    }
                }
            }
            UpdateSelectWeaponDetail(_currentSelectWeapon);
        }
    }

    private async void OnClickEnhanceButton()
    {
        var popup = await UIManager.Instance.Show<WeaponEnhancePopup>();
        popup.UpdateWeaponData(_currentSelectWeapon);
    }
}
