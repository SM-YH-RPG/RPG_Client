using System;
using System.Collections.Generic;

public class WeaponItem : EquipableItem
{
    public EItemStatType MainStatType { get; set; }
    public float MainStatValue { get; set; }

    public EItemStatType SubStatType { get; set; }
    public float SubStatValue { get; set; }
}

public class WeaponManager : BaseEquipManager<WeaponManager>
{
    #region Const
    private const int WEAPON_SLOT = 0;
    private const int MAX_TIER = 5;
    private const int EMPTY_SLOT_ID = -1;    
    #endregion

    private Dictionary<int, long> _equippedWeapons = new Dictionary<int, long>();

    #region Action
    public event Action<int, long> OnWeaponEquipped;
    public event Action<int, long> OnWeaponUnequipped;
    public event Action<WeaponItem> OnWeaponDataUpdate;
    #endregion

    #region Public
    public void Initialize()
    {
        var characters = PlayerManager.Instance.CharacterService.HaveCharacterDict;

        foreach (var character in characters.Values)
        {
            var equippedWeapon = character.EquippedItems[WEAPON_SLOT];
            InitCharacterEquippedWeaponItemData(character.TemplateId, equippedWeapon.InstanceId);
        }
    }

    public override bool EquipItem(int characterIndex, long instanceId, int slotIndex = 0)
    {
        if (IsItemValidAndOfType(instanceId, out WeaponItem targetWeapon) == false)
            return false;

        int previousOwnerIndex = targetWeapon.EquippedCharacterIndex;
        if (previousOwnerIndex != -1 && previousOwnerIndex != characterIndex)
        {
            TryUnequipWeapon(instanceId, previousOwnerIndex);
        }

        if (_equippedWeapons.TryGetValue(characterIndex, out long currentlyEquippedId))
        {
            if (currentlyEquippedId != instanceId)
            {
                TryUnequipWeapon(currentlyEquippedId, characterIndex);
            }
            else
            {
                return true;
            }
        }

        ExecuteEquipWeapon(characterIndex, targetWeapon);

        return true;
    }

    public override bool TryUnequipWeapon(long instanceId, int ownerCharacterIndex)
    {
        var charUniqueKey = CharacterManager.Instance.GetCharacterUniqueIndex(ownerCharacterIndex);
        if (CharacterManager.Instance.HaveCharacterDict.TryGetValue(charUniqueKey, out var character))
        {
            WeaponItemProcessor.ExecuteUnEquipWeapon(character, instanceId, _equippedWeapons);

            OnWeaponUnequipped?.Invoke(ownerCharacterIndex, instanceId);
            NotifyCharacterStatUpdate(ownerCharacterIndex);

            return true;
        }

        return false;
    }

    public bool TryEnhanceWeaponItem(long instanceId, EnhanceData enhanceData, out WeaponItem weapon)
    {
        if (CanEnhance(enhanceData) == false)
        {
            weapon = null;
            return false;
        }

        bool isSuccess = WeaponItemProcessor.TryWeaponEnhance(instanceId, enhanceData, out weapon);
        if (isSuccess)
        {
            OnWeaponDataUpdate?.Invoke(weapon);
            if (weapon.EquippedCharacterIndex != -1)
            {
                NotifyCharacterStatUpdate(weapon.EquippedCharacterIndex);
            }
        }

        return isSuccess;
    }

    public long GetCharacterEquippedWeaponInstanceID(int characterIndex)
    {
        if (_equippedWeapons.TryGetValue(characterIndex, out long instanceId))
        {
            return instanceId;
        }

        return EMPTY_SLOT_ID;
    }

    public (float main, float sub) GetNextTierStats(WeaponItem weapon)
    {
        if (weapon.Tier >= MAX_TIER)
            return (weapon.MainStatValue, weapon.SubStatValue);

        float nextMain = WeaponItemProcessor.CalculateNewStat(weapon.MainStatType, weapon.MainStatValue, weapon);
        float nextSub = WeaponItemProcessor.CalculateNewStat(weapon.SubStatType, weapon.SubStatValue, weapon);

        return (nextMain, nextSub);
    }

    public void InitCharacterEquippedWeaponItemData(int characterIndex, long instanceId)
    {
        if (_equippedWeapons.ContainsKey(characterIndex))
        {
            _equippedWeapons[characterIndex] = instanceId;
        }
        else
        {
            _equippedWeapons.Add(characterIndex, instanceId);
        }
    }
    #endregion

    private void ExecuteEquipWeapon(int charIndex, WeaponItem weapon)
    {
        var charUniqueKey = CharacterManager.Instance.GetCharacterUniqueIndex(charIndex);
        if (CharacterManager.Instance.HaveCharacterDict.TryGetValue(charUniqueKey, out var character))
        {
            WeaponItemProcessor.ExecuteEquipWeapon(character, weapon, _equippedWeapons);

            OnWeaponEquipped?.Invoke(charIndex, weapon.InstanceId);
            NotifyCharacterStatUpdate(charIndex);
        }
    }
}