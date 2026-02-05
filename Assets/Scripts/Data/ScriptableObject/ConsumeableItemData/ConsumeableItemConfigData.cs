using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public struct ConsumeableItemConfig
{
    public int Index;
    public EConsumableEffectType Type;
    public float AffectValue;
    public float Cooldown;
}

[CreateAssetMenu(fileName = "ConsumeableItemConfigData", menuName = "Scriptable Objects/ConsumeableItemConfigData")]
public class ConsumeableItemConfigData : ScriptableObject
{
    [field: SerializeField]
    private ConsumeableItemConfig[] _consumeableItemDataArray;

    public IReadOnlyList<ConsumeableItemConfig> ConsumeableItemDataConfigs => _consumeableItemDataArray;

    private Dictionary<int, ConsumeableItemConfig> _lookupCache;

    public ConsumeableItemConfig GetConsumeableItemData(int index)
    {
        if (_lookupCache == null)
        {
            _lookupCache = new Dictionary<int, ConsumeableItemConfig>();
            foreach (var data in _consumeableItemDataArray)
            {
                if (_lookupCache.ContainsKey(data.Index) == false)
                {
                    _lookupCache.Add(data.Index, data);
                }
            }
        }

        if (_lookupCache.TryGetValue(index, out ConsumeableItemConfig result))
            return result;

        return default;
    }
}
