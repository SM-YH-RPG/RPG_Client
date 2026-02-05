using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;

public class InventoryProcessor
{
    #region Common
    private static Dictionary<long, BaseInventoryItem> GetCategoryDict(
        Dictionary<EItemCategory, Dictionary<long, BaseInventoryItem>> inventoryByCategory,
        EItemCategory category,
        bool createIfMissing = false)
    {
        if (inventoryByCategory.TryGetValue(category, out Dictionary<long, BaseInventoryItem> dict) == false)
        {
            if (createIfMissing)
            {
                dict = new Dictionary<long, BaseInventoryItem>();
                inventoryByCategory.Add(category, dict);
            }
            else
            {
                return new Dictionary<long, BaseInventoryItem>();
            }
        }
        return dict;
    }
    #endregion

    #region Add
    public static void AddItem(
        Dictionary<EItemCategory, Dictionary<long, BaseInventoryItem>> inventoryByCategory,
        int templateIndex,
        EItemCategory category,
        EItemGrade grade,
        string name,
        int amountToAdd,
        int tier,
        ref long nextInstanceId)
    {
        if (amountToAdd <= 0)
            return;

        int maxStackSize = ItemDataManager.Instance.GetMaxStackSize(category);
        int remainingAmountToAdd = amountToAdd;

        Dictionary<long, BaseInventoryItem> itemsInCategory = GetCategoryDict(inventoryByCategory, category, createIfMissing: true);
        if (maxStackSize > 1)
        {
            int amountHandled = ProcessExistingStacks(itemsInCategory, templateIndex, maxStackSize, remainingAmountToAdd);
            remainingAmountToAdd -= amountHandled;
        }

        if (remainingAmountToAdd > 0)
        {
            CreateNewItemStacks(itemsInCategory, templateIndex, category, grade, name, maxStackSize, ref remainingAmountToAdd, tier, ref nextInstanceId);            
        }
    }

    private static int ProcessExistingStacks(
        Dictionary<long, BaseInventoryItem> itemsInCategory,
        int templateIndex,        
        int maxStackSize,
        int amountToAdd)
    {
        int amountHandled = 0;
        foreach (var item in itemsInCategory.Values)
        {
            if (item.TemplateId == templateIndex && item.Amount < maxStackSize)
            {
                int spaceAvailableInStack = maxStackSize - item.Amount;
                if (amountToAdd <= spaceAvailableInStack)
                {
                    item.Amount += amountToAdd;
                    amountHandled += amountToAdd;
                    return amountHandled;
                }
                else
                {
                    item.Amount += spaceAvailableInStack;
                    amountToAdd -= spaceAvailableInStack;
                    amountHandled += spaceAvailableInStack;
                }
            }
        }
        return amountHandled;
    }

    private static T CreateBaseItem<T>(int templateIndex, EItemGrade grade, string name, long instanceId, int amount = 1) where T : BaseInventoryItem, new()
    {
        T item = new T
        {
            InstanceId = instanceId,
            TemplateId = templateIndex,
            Grade = grade,
            Name = name,
            Amount = amount
        };

        return item;
    }

    private static T CreateEquipableItem<T>(int templateIndex, EItemGrade grade, string name, long instanceId) where T : EquipableItem, new()
    {
        T item = CreateBaseItem<T>(templateIndex, grade, name, instanceId);
        item.EquippedCharacterIndex = -1;

        return item;
    }

    private static void CreateNewItemStacks(
        Dictionary<long, BaseInventoryItem> itemsInCategory,
        int templateIndex,
        EItemCategory category,
        EItemGrade grade,
        string name,
        int maxStackSize,
        ref int remainingAmountToAdd,
        int tier,
        ref long nextInstanceId)
    {
        while (remainingAmountToAdd > 0)
        {
            int amountForNewStack = Mathf.Min(remainingAmountToAdd, maxStackSize);
            BaseInventoryItem newItemInstance;

            long currentInstanceId = nextInstanceId++;            

            switch (category)
            {                
                case EItemCategory.Weapon:
                    var weaponConfig = ItemDataManager.Instance.GetItemConfigData(category, templateIndex);
                    grade = weaponConfig.template.Grade;
                    var weaponItem = CreateEquipableItem<WeaponItem>(templateIndex, grade, name, currentInstanceId);
                    ItemStatGenerationHelper.GenerateWeaponStats(weaponItem, templateIndex);
                    weaponItem.Tier = tier;
                    weaponItem.Category = category;
                    weaponItem.MaxStackSize = maxStackSize;
                    newItemInstance = weaponItem;
                    break;
                case EItemCategory.Equipment:
                    var equipmentConfig = ItemDataManager.Instance.GetItemConfigData(category, templateIndex);
                    grade = equipmentConfig.template.Grade;
                    var equipItem = CreateEquipableItem<EquipmentItem>(templateIndex, grade, name, currentInstanceId);
                    ItemStatGenerationHelper.GenerateEquipmentStats(equipItem, templateIndex);
                    equipItem.Tier = tier;
                    equipItem.Category = category;
                    equipItem.MaxStackSize = maxStackSize;
                    newItemInstance = equipItem;
                    break;
                case EItemCategory.Consumable:
                    var consumableConfig = ItemDataManager.Instance.GetItemConfigData(category, templateIndex);
                    grade = consumableConfig.template.Grade;
                    var consumableItem = CreateBaseItem<ConsumableItem>(templateIndex, grade, name, currentInstanceId, amountForNewStack);
                    var config = ItemDataManager.Instance.ConsumeableItemConfig.GetConsumeableItemData(consumableItem.TemplateId);
                    consumableItem.ConsumableEffectType = config.Type;
                    consumableItem.ConsumableEffectValue = config.AffectValue;
                    consumableItem.CooldownSeconds = config.Cooldown;
                    consumableItem.Category = category;
                    consumableItem.MaxStackSize = maxStackSize;
                    newItemInstance = consumableItem;
                    break;
                case EItemCategory.Material:
                    var materialConfig = ItemDataManager.Instance.GetItemConfigData(category, templateIndex);
                    grade = materialConfig.template.Grade;
                    newItemInstance = CreateBaseItem<BaseInventoryItem>(templateIndex, grade, name, currentInstanceId, amountForNewStack);
                    newItemInstance.MaxStackSize = maxStackSize;
                    newItemInstance.Category = category;
                    break;
                default:
                    newItemInstance = CreateBaseItem<BaseInventoryItem>(templateIndex, grade, name, currentInstanceId, amountForNewStack);
                    break;
            }            

            itemsInCategory.Add(currentInstanceId, newItemInstance);
            remainingAmountToAdd -= amountForNewStack;
        }
    }
    #endregion

    #region Remove (합성, 강화, 폐기 등에 사용)
    public static bool RemoveItem(
        Dictionary<EItemCategory, Dictionary<long, BaseInventoryItem>> inventoryByCategory,
        EItemCategory category,
        int itemIndex,
        int amountToRemove)
    {
        if (amountToRemove <= 0)
        {
            return true;
        }

        Dictionary<long, BaseInventoryItem> itemsInCategory = GetCategoryDict(inventoryByCategory, category);
        if (itemsInCategory.Count == 0)
            return false;

        int totalAvailable = CalculateTotalAvailable(itemsInCategory, itemIndex);

        if (totalAvailable < amountToRemove)
        {
            return false;
        }

        ProcessItemRemoval(itemsInCategory, itemIndex, amountToRemove);

        if (itemsInCategory.Count == 0)
        {
            inventoryByCategory.Remove(category);
        }

        return true;
    }

    private static int CalculateTotalAvailable(Dictionary<long, BaseInventoryItem> itemsInCategory, int itemIndex)
    {
        int totalAvailable = 0;
        foreach (var instance in itemsInCategory.Values)
        {
            if (instance.TemplateId == itemIndex)
            {
                totalAvailable += instance.Amount;
            }
        }
        return totalAvailable;
    }

    private static void ProcessItemRemoval(Dictionary<long, BaseInventoryItem> itemsInCategory, int itemIndex, int amountToRemove)
    {
        int remainingToRemove = amountToRemove;

        List<long> instanceIDsToModify = new List<long>();
        foreach (var item in itemsInCategory.Values)
        {
            if (item.TemplateId == itemIndex)
            {
                instanceIDsToModify.Add(item.InstanceId);
            }
        }

        // instanceId 내림차순 정렬 (MaxStackCount로 슬릇이 여러개인 아이템은 최근꺼부터 소비되도록..)
        instanceIDsToModify.Sort((a, b) => b.CompareTo(a));

        foreach (long instanceId in instanceIDsToModify)
        {
            if (itemsInCategory.TryGetValue(instanceId, out BaseInventoryItem currentInstance))
            {
                if (currentInstance.Amount <= remainingToRemove)
                {
                    remainingToRemove -= currentInstance.Amount;
                    itemsInCategory.Remove(instanceId);
                }
                else
                {
                    currentInstance.Amount -= remainingToRemove;
                    remainingToRemove = 0;
                }

                if (remainingToRemove == 0)
                {
                    break;
                }
            }
        }
    }
    #endregion
}