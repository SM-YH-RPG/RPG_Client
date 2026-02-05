using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct EnhanceMaterialData
{
    public EItemCategory Category;
    public int Index;
    public int Amount;
}


[Serializable]
public struct EnhanceData
{
    public int Index;
    public EItemGrade Grade;
    public EnhanceMaterialData[] MaterialsArray;
    public int Cost;
    public int EnhanceSuccessRate;
}

[CreateAssetMenu(fileName = "EnhanceConfigData", menuName = "Scriptable Objects/EnhanceConfigData")]
public class EnhanceConfigData : ScriptableObject
{
    [field: SerializeField]
    private EnhanceData[] _enhanceDataArray;
    public IReadOnlyList<EnhanceData> EnhanceDataConfigs => _enhanceDataArray;

    private Dictionary<EItemGrade, Dictionary<int, EnhanceData>> _lookupCache;

    public EnhanceData GetEnhanceData(EItemGrade grade, int index)
    {
        if (_lookupCache == null)
        {
            _lookupCache = new Dictionary<EItemGrade, Dictionary<int, EnhanceData>>();
            foreach (var data in _enhanceDataArray)
            {
                if (_lookupCache.ContainsKey(data.Grade) == false)
                {
                    Dictionary<int, EnhanceData> enhanceDataDict = new Dictionary<int, EnhanceData>();
                    enhanceDataDict.Add(data.Index, data);
                    _lookupCache.Add(data.Grade, enhanceDataDict);
                }
                else
                {
                    _lookupCache[data.Grade].Add(data.Index, data);
                }
            }
        }

        if (_lookupCache.TryGetValue(grade, out var dict))
        {
            if (dict.TryGetValue(index, out EnhanceData result))
                return result;
        }

        return default;
    }
}
