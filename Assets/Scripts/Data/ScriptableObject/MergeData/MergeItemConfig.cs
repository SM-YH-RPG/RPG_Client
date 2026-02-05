using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct NeedItemTemplate
{
    public EItemCategory Category;
    public int ItemIndex;
    public int NeedAmount;
}

[Serializable]
public struct MergeItemConfigData
{    
    public int ItemIndex;
    public EItemCategory Category;
    public NeedItemTemplate[] NeedItemArray;
}

[CreateAssetMenu(fileName = "MergeItemConfig", menuName = "Scriptable Objects/MergeItemConfig")]
public class MergeItemConfig : ScriptableObject
{
    [field: SerializeField]
    private MergeItemConfigData[] _mergeItemConfigArray;
    public IReadOnlyList<MergeItemConfigData> MergeItemConfigs => _mergeItemConfigArray;

    private Dictionary<int, MergeItemConfigData> _lookupCache;

    public MergeItemConfigData GetMergeConfigData(int index)
    {
        if (_lookupCache == null)
        {
            _lookupCache = new Dictionary<int, MergeItemConfigData>();
            foreach (var data in _mergeItemConfigArray)
            {
                if (_lookupCache.ContainsKey(data.ItemIndex) == false)
                {
                    _lookupCache.Add(data.ItemIndex, data);
                }
            }
        }

        if (_lookupCache.TryGetValue(index, out MergeItemConfigData result))
            return result;

        return default;
    }
}
