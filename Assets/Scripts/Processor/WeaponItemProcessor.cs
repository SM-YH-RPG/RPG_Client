using System.Collections.Generic;
using UnityEngine;

public class WeaponItemProcessor
{
    private const int WEAPON_SLOT_INDEX = 0;
    private const int EMPTY_SLOT_ID = -1;

    public static bool TryWeaponEnhance(long instanceId, EnhanceData enhanceData, out WeaponItem weapon)
    {
        ConsumeEnhanceMaterialItems(enhanceData);
        weapon = (WeaponItem)InventoryManager.Instance.GetInventoryItemInstance(instanceId);
        int rate = Random.Range(0, 100);
        bool isSuccess = rate <= enhanceData.EnhanceSuccessRate;
        if (isSuccess)
        {
            ApplyEnhanceData(instanceId, out weapon);
        }

        return isSuccess;
    }

    private static void ApplyEnhanceData(long instanceId, out WeaponItem weapon)
    {
        weapon = (WeaponItem)InventoryManager.Instance.GetInventoryItemInstance(instanceId);
        if (weapon != null)
        {
            weapon.MainStatValue = CalculateNewStat(weapon.MainStatType, weapon.MainStatValue, weapon);
            weapon.SubStatValue = CalculateNewStat(weapon.SubStatType, weapon.SubStatValue, weapon);
            weapon.Tier += 1;
        }
    }    

    public static float CalculateNewStat(EItemStatType type, float currentValue, WeaponItem weapon)
    {
        float gradeWeight = (int)weapon.Grade + 1;
        float tierFactor = weapon.Tier + 1;

        float multiplier = IsRateStat(type) ? 0.5f : 1.0f;

        return currentValue + (gradeWeight * tierFactor * multiplier);
    }

    public static void ExecuteEquipWeapon(RuntimeCharacter characater, WeaponItem weapon, Dictionary<int, long> weaponEquippedDict)
    {
        characater.EquippedItems[WEAPON_SLOT_INDEX] = weapon;
        weapon.EquippedCharacterIndex = characater.TemplateId;

        weaponEquippedDict[characater.TemplateId] = weapon.InstanceId;        
    }

    public static void ExecuteUnEquipWeapon(RuntimeCharacter character, long instanceId, Dictionary<int, long> weaponEquippedDict)
    {
        character.EquippedItems.Remove(0);

        if (InventoryManager.Instance.GetInventoryItemInstance(instanceId) is WeaponItem weapon)
        {
            weapon.EquippedCharacterIndex = EMPTY_SLOT_ID;
        }

        weaponEquippedDict.Remove(character.TemplateId);
    }

    private static void ConsumeEnhanceMaterialItems(EnhanceData enhanceData)
    {
        PlayerManager.Instance.UpdateCurrentCurrencyValue(PlayerManager.Instance.CurrentCurrencyValue - enhanceData.Cost);
        foreach (var material in enhanceData.MaterialsArray)
        {
            InventoryManager.Instance.RemoveItem(material.Category, material.Index, material.Amount);
        }
    }

    private static bool IsRateStat(EItemStatType type)
    {
        return type switch
        {
            EItemStatType.HPRate or
            EItemStatType.AttackPowerRate or
            EItemStatType.DefenseRate or
            EItemStatType.ResoundRate or
            EItemStatType.CriticalRate or
            EItemStatType.CriticalDamageRate => true,
            _ => false
        };
    }
}
