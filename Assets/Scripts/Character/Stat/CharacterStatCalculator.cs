using System;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterStatCalculator
{
    private const int MAX_EQUIPMENT_SLOT_COUNT = 5;
    private const int EMPTY_EQUIPMENTITEM_ID = -1;

    private static int _currentCharacterIndex;
    private static WeaponItem _weapon;
    private static List<EquipmentItem> _equipmentList = new List<EquipmentItem>();

    public static void UpdateCharacterStatData(int characterIndex)
    {
        _currentCharacterIndex = characterIndex;

        long weaponInstanceId = WeaponManager.Instance.GetCharacterEquippedWeaponInstanceID(characterIndex);
        {
            var item = PlayerManager.Instance.Inventory.GetInventoryItemInstance(weaponInstanceId);
            if (item == null)
            {
                _weapon = null;
            }
            else if (item is WeaponItem weapon)
            {
                _weapon = weapon;
            }
        }

        _equipmentList.Clear();

        for (int i = 0; i < MAX_EQUIPMENT_SLOT_COUNT; i++)
        {
            long equipId = EquipmentManager.Instance.GetSlotEquippedIntanceID(characterIndex, i);
            if (equipId != EMPTY_EQUIPMENTITEM_ID)
            {
                var item = PlayerManager.Instance.Inventory.GetInventoryItemInstance(equipId);
                if (item is EquipmentItem equip)
                {
                    _equipmentList.Add(equip);
                }
            }
        }

        CalculateStatData();
    }

    private static void CalculateStatData()
    {
        if (_weapon != null)
        {
            AccumulateItemStat(_weapon.MainStatType, _weapon.MainStatValue);
            AccumulateItemStat(_weapon.SubStatType, _weapon.SubStatValue);
        }

        int count = _equipmentList.Count;
        for (int i = 0; i < count; i++)
        {
            AccumulateItemStat(_equipmentList[i].RandomMainStatType, _equipmentList[i].RandomMainStatValue);
            AccumulateItemStat(_equipmentList[i].MainStatType, _equipmentList[i].MainStatValue);

            foreach (var item in _equipmentList[i].SubStatDict)
            {
                AccumulateItemStat(item.Key, item.Value);
            }
        }

        CalculateItemStatValue();
    }

    private static void AccumulateItemStat(EItemStatType itemStatType, float itemStatValue)
    {
        ECharacterStatType charStatType = ConvertToCharacterStat(itemStatType);

        if (IsRateStat(itemStatType))
        {
            // 공명효율,크리 계열은 퍼센트(확률/배율) 더하기..
            if (charStatType == ECharacterStatType.Resound || charStatType == ECharacterStatType.CriticalRate || charStatType == ECharacterStatType.CriticalDamage)
            {
                PlayerManager.Instance.CharacterService.PlusAddDict[_currentCharacterIndex][charStatType] += itemStatValue;
            }
            else
            {
                PlayerManager.Instance.CharacterService.MultipleAddDict[_currentCharacterIndex][charStatType] += itemStatValue * 0.01f;
            }
        }
        else
        {
            PlayerManager.Instance.CharacterService.PlusAddDict[_currentCharacterIndex][charStatType] += itemStatValue;
        }
    }

    private static ECharacterStatType ConvertToCharacterStat(EItemStatType itemStat)
    {
        return itemStat switch
        {
            EItemStatType.HP or EItemStatType.HPRate
                => ECharacterStatType.HP,

            EItemStatType.AttackPower or EItemStatType.AttackPowerRate
                => ECharacterStatType.AttackPower,

            EItemStatType.Defense or EItemStatType.DefenseRate
                => ECharacterStatType.Defense,

            EItemStatType.ResoundRate
                => ECharacterStatType.Resound,

            EItemStatType.CriticalRate
                => ECharacterStatType.CriticalRate,

            EItemStatType.CriticalDamageRate
                => ECharacterStatType.CriticalDamage,

            _ => throw new System.NotImplementedException(),
        };
    }

    private static bool IsRateStat(EItemStatType statType)
    {
        return statType switch
        {
            EItemStatType.HPRate
            or EItemStatType.AttackPowerRate
            or EItemStatType.DefenseRate
            or EItemStatType.ResoundRate
            or EItemStatType.CriticalRate
            or EItemStatType.CriticalDamageRate
                => true,

            _ => false
        };
    }

    private static void CalculateItemStatValue()
    {
        foreach (ECharacterStatType type in Enum.GetValues(typeof(ECharacterStatType)))
        {
            if (type == ECharacterStatType.End) continue;

            float baseValue = PlayerManager.Instance.CharacterService.CharacterBaseStatDict[_currentCharacterIndex][type];
            float flat = PlayerManager.Instance.CharacterService.PlusAddDict[_currentCharacterIndex][type];
            float rate = PlayerManager.Instance.CharacterService.MultipleAddDict[_currentCharacterIndex][type];

            if (type == ECharacterStatType.Resound || type == ECharacterStatType.CriticalRate || type == ECharacterStatType.CriticalDamage)
            {
                // 공명효율,크리는 포인트 더하기로 최종
                PlayerManager.Instance.CharacterService.FinalStatDict[_currentCharacterIndex][type] = baseValue + flat;
            }
            else
            {
                PlayerManager.Instance.CharacterService.FinalStatDict[_currentCharacterIndex][type] = (baseValue + flat) * (1f + rate);
            }
        }
    }

    public static float CalculateBaseStatFactorByLevel(int level, ECharacterStatType type)
    {
        float factor = GetLevelFactor(level);

        if (level <= 1)
        {
            return factor;
        }

        switch (type)
        {
            case ECharacterStatType.HP:
                break;
            case ECharacterStatType.AttackPower:
            case ECharacterStatType.Defense:
                factor *= 0.85f;
                break;
            case ECharacterStatType.Resound: // 증가 없음                
            case ECharacterStatType.CriticalRate: // 증가 없음                
            case ECharacterStatType.CriticalDamage: // 증가 없음                
                return 1f;
        }

        return factor;
    }

    private static float GetLevelFactor(int level)
    {
        // 1 -> 1.00
        // 5 -> 1.45
        // 10 -> 2.30 배..
        return 1f + Mathf.Pow(level - 1, 1.2f) * 0.15f;
    }
}
