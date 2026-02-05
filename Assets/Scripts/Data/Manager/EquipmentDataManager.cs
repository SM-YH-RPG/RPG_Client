using System.Collections.Generic;

public class EquipmentDataManager : LazySingleton<EquipmentDataManager>
{
    private const int MAX_TIER = 5;
    public int MaxTier => MAX_TIER;

    private Dictionary<int, EquipItemConfigData> _equipmentConfigCache = new Dictionary<int, EquipItemConfigData>();

    private EnhanceConfigData _equipmentEnhanceConfig;
    public EnhanceConfigData EquipmentEnhanceConfig => _equipmentEnhanceConfig;

    public void Initialize(EquipItemListConfig equipItemListConfig)
    {
        if (equipItemListConfig == null) return;

        _equipmentConfigCache.Clear();
        foreach (var data in equipItemListConfig.EquipmentItemDatas)
        {
            if (_equipmentConfigCache.ContainsKey(data.Index) == false)
            {
                _equipmentConfigCache.Add(data.Index, data);
            }
        }
    }

    public EquipItemConfigData GetEquipmentConfigByIndex(int index)
    {
        if (_equipmentConfigCache.TryGetValue(index, out EquipItemConfigData config))
        {
            return config;
        }

        return default;
    }

    public void SetEquipmentEnhanceConfigData(EnhanceConfigData equipmentEnhanceConfig)
    {
        _equipmentEnhanceConfig = equipmentEnhanceConfig;
    }
}