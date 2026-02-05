using System.Collections.Generic;

public static class ItemStatGenerationHelper
{
    private const int EMPTY_ITEM_ID = -1;    

    public static void GenerateEquipmentStats(EquipmentItem equipItem, int itemIndex)
    {
        EquipItemConfigData configData = EquipmentDataManager.Instance.GetEquipmentConfigByIndex(itemIndex);
        ItemConfigData itemConfig = ItemDataManager.Instance.GetItemConfigData(EItemCategory.Equipment, itemIndex);
        if (configData.Index == EMPTY_ITEM_ID)
        {
            return;
        }

        EItemStatType[] randomStatType = new EItemStatType[]
            {
            EItemStatType.HPRate,
            EItemStatType.DefenseRate,
            EItemStatType.AttackPowerRate,
            EItemStatType.ResoundRate,
            EItemStatType.CriticalDamageRate
            };

        EItemStatType RandomMainStatType = StatGenerationHelper.GetRandomMainStatType(randomStatType, configData.EquipCost);
        float RandomMainStatValue = StatGenerationHelper.GenerateRandomStatValue(RandomMainStatType, itemConfig.template.Grade);
        float mainStatValue = StatGenerationHelper.GenerateRandomStatValue(configData.availableMainStat[0], itemConfig.template.Grade);

        Dictionary<EItemStatType, float> subStats = StatGenerationHelper.GenerateRandomSubStats(itemConfig.template.Grade, configData.availableSubStat);

        equipItem.RandomMainStatType = RandomMainStatType;
        equipItem.RandomMainStatValue = RandomMainStatValue;
        equipItem.MainStatType = configData.availableMainStat[0]; // 메인 고정스탯 1개
        equipItem.MainStatValue = mainStatValue;
        equipItem.SubStatDict = subStats;

        equipItem.EquipCost = configData.EquipCost;
    }

    public static void GenerateWeaponStats(WeaponItem weaponItem, int itemIndex)
    {
        WeaponItemConfigData configData = WeaponDataManager.Instance.GetWeaponConfigByIndex(itemIndex);
        ItemConfigData itemConfig = ItemDataManager.Instance.GetItemConfigData(EItemCategory.Weapon, itemIndex);
        if (configData.Index == EMPTY_ITEM_ID)
        {
            return;
        }

        float mainStatValue = StatGenerationHelper.GenerateRandomStatValue(configData.MainStatType, itemConfig.template.Grade);
        float subStatValue = StatGenerationHelper.GenerateRandomSubStat(itemConfig.template.Grade, configData.SubStatType);

        weaponItem.MainStatType = configData.MainStatType;
        weaponItem.MainStatValue = mainStatValue;
        weaponItem.SubStatType = configData.SubStatType;
        weaponItem.SubStatValue = subStatValue;
    }

    public static void InitWeaponSubStat(WeaponItem weaponItem, Dictionary<EItemStatType, float> subStatDict)
    {
        foreach (var stat in subStatDict)
        {
            weaponItem.SubStatType = stat.Key;
            weaponItem.SubStatValue = stat.Value;
        }
    }
}
