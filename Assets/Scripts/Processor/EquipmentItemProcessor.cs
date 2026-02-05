using System.Collections.Generic;
using UnityEngine;

public class EquipmentItemProcessor
{
    private const int EMPTY_SLOT_INDEX = -1;

    public static bool TryEquipmentEnhance(long instanceId, EnhanceData enhanceData, out EquipmentItem currentEquipment)
    {
        ConsumeEnhanceMaterialItems(enhanceData);
        currentEquipment = (EquipmentItem)InventoryManager.Instance.GetInventoryItemInstance(instanceId);
        int rate = Random.Range(0, 100);
        bool isSuccess = rate <= enhanceData.EnhanceSuccessRate;
        if (isSuccess)
        {
            ApplyEnhanceData(instanceId, out currentEquipment);
        }

        return isSuccess;
    }

    private static void ApplyEnhanceData(long instanceId, out EquipmentItem currentEquipment)
    {
        currentEquipment = (EquipmentItem)InventoryManager.Instance.GetInventoryItemInstance(instanceId);
        if (currentEquipment != null)
        {
            currentEquipment.MainStatValue = CalculateNewStat(currentEquipment.MainStatType, currentEquipment.MainStatValue, currentEquipment);
            currentEquipment.RandomMainStatValue = CalculateNewStat(currentEquipment.RandomMainStatType, currentEquipment.RandomMainStatValue, currentEquipment);
            var keys = new List<EItemStatType>(currentEquipment.SubStatDict.Keys);
            foreach (var statType in keys)
            {
                currentEquipment.SubStatDict[statType] = CalculateNewStat(statType, currentEquipment.SubStatDict[statType], currentEquipment);
            }
            currentEquipment.Tier += 1;
        }
    }    

    public static float CalculateNewStat(EItemStatType type, float currentValue, EquipmentItem equipment)
    {
        float gradeWeight = (int)equipment.Grade + 1;
        float tierFactor = equipment.Tier + 1;

        float multiplier = IsRateStat(type) ? 0.5f : 1.0f;

        return currentValue + (gradeWeight * tierFactor * multiplier);
    }

    public static void ExecuteEquipItem(Dictionary<int, long[]> equipDict, int characterIndex, int slotIndex, long instanceId)
    {
        long oldItemIdInSlot = equipDict[characterIndex][slotIndex];
        equipDict[characterIndex][slotIndex] = instanceId;

        EquipmentItem item = null;
        if (InventoryManager.Instance.GetInventoryItemInstance(instanceId) is EquipmentItem equipmentItem)
        {
            equipmentItem.EquippedCharacterIndex = characterIndex;
            item = equipmentItem;
        }

        if (oldItemIdInSlot != EMPTY_SLOT_INDEX)
        {
            if (InventoryManager.Instance.GetInventoryItemInstance(oldItemIdInSlot) is EquipmentItem oldItem)
            {                
                oldItem.EquippedCharacterIndex = EMPTY_SLOT_INDEX;
            }
        }

        RuntimeCharacter character = CharacterManager.Instance.HaveCharacterDict[CharacterManager.Instance.GetCharacterUniqueIndex(characterIndex)];
        Dictionary<int, BaseInventoryItem> equippedDict = character.EquippedItems;

        if (equippedDict.ContainsKey(slotIndex + 1) == false)
        {
            equippedDict.Add(slotIndex + 1, item);
        }
        else
        {
            equippedDict[slotIndex + 1] = item;
        }
    }

    public static void ExecuteUnEquipItem(long[] slotArray, int slotIndex, int characterIndex, long instanceId)
    {
        slotArray[slotIndex] = EMPTY_SLOT_INDEX;        
        if (InventoryManager.Instance.GetInventoryItemInstance(instanceId) is EquipmentItem equipItem)
        {
            equipItem.EquippedCharacterIndex = EMPTY_SLOT_INDEX;
        }
        RuntimeCharacter character = CharacterManager.Instance.HaveCharacterDict[CharacterManager.Instance.GetCharacterUniqueIndex(characterIndex)];
        var equippedDict = character.EquippedItems;
        equippedDict.Remove(slotIndex + 1);
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
