using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct RewardItemTemplate
{
    public EItemCategory CategoryType;
    public int TemplateId;
    public int Amount;
}


[Serializable]
public struct RewardItemConfigData
{
    public int Index;
    public RewardItemTemplate[] RewardItemArray;
}

[CreateAssetMenu(fileName = "RewardItemDataConfig", menuName = "Scriptable Objects/RewardItemDataConfig")]
public class RewardItemConfig : ScriptableObject
{
    [field: SerializeField]
    private RewardItemConfigData[] _rewardItemConfigArray;
    public IReadOnlyList<RewardItemConfigData> RewardDatas => _rewardItemConfigArray;

    private Dictionary<int, RewardItemConfigData> _lookupCache;

    public RewardItemConfigData GetRewardConfigData(int index)
    {
        if (_lookupCache == null)
        {
            _lookupCache = new Dictionary<int, RewardItemConfigData>();
            foreach (var data in _rewardItemConfigArray)
            {
                if (_lookupCache.ContainsKey(data.Index) == false)
                {
                    _lookupCache.Add(data.Index, data);
                }
            }
        }

        if (_lookupCache.TryGetValue(index, out RewardItemConfigData result))
        {
            return result;
        }

        return default;
    }
}
