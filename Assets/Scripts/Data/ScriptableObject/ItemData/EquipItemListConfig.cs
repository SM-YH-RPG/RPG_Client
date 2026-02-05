using System;
using System.Collections.Generic;
using UnityEngine;

#region Enum
public enum EItemStatType
{
    HP,
    HPRate,
    AttackPower,
    AttackPowerRate,
    Defense,
    DefenseRate,
    ResoundRate,
    CriticalRate,
    CriticalDamageRate,
    End
}
#endregion

[Serializable]
public struct EquipItemConfigData
{
    public int Index;
    public int EquipCost;
    public EItemStatType[] availableMainStat;
    public EItemStatType[] availableSubStat;
}

[CreateAssetMenu(fileName = "EquipItemListConfig", menuName = "Scriptable Objects/EquipItemListConfig")]
public class EquipItemListConfig : ItemListConfig
{
    [field: SerializeField]
    private EquipItemConfigData[] _equipmentItemConfigArray;
    public IReadOnlyList<EquipItemConfigData> EquipmentItemDatas => _equipmentItemConfigArray;

    private Dictionary<int, EquipItemConfigData> _lookupCache;

    public EquipItemConfigData GetEquipmentConfigData(int index)
    {
        if (_lookupCache == null)
        {
            _lookupCache = new Dictionary<int, EquipItemConfigData>();
            foreach (var data in _equipmentItemConfigArray)
            {
                if (_lookupCache.ContainsKey(data.Index) == false)
                {
                    _lookupCache.Add(data.Index, data);
                }
            }
        }

        if (_lookupCache.TryGetValue(index, out EquipItemConfigData result))
        {
            return result;
        }

        return default;
    }
}