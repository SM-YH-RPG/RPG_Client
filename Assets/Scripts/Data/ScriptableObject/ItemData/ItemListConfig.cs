using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemConfigData
{
    public ItemTemplate template;

    public string Name;
    public string Description;
    public string AffectDescription;
    
    public Sprite Sprite;
}

[CreateAssetMenu(fileName = "ItemListConfig", menuName = "Scriptable Objects/ItemListConfig")]
public class ItemListConfig : ScriptableObject
{
    [field: SerializeField]
    public EItemCategory Category;

    [field: SerializeField]
    public int CategoryMaxSlotCount;

    [field:SerializeField]
    public int MaxStackCount { get; private set; }

    [field: SerializeField]
    private ItemConfigData[] _itemConfigArray;
    public IReadOnlyList<ItemConfigData> ItemDatas => _itemConfigArray;

    private Dictionary<int, ItemConfigData> _lookupCache;

    public ItemConfigData GetConfigData(int index)
    {
        if (_lookupCache == null)
        {
            _lookupCache = new Dictionary<int, ItemConfigData>();
            foreach (var data in _itemConfigArray)
            {
                if (_lookupCache.ContainsKey(data.template.Index) == false)
                {
                    _lookupCache.Add(data.template.Index, data);
                }
            }
        }

        if (_lookupCache.TryGetValue(index, out ItemConfigData result))
        {
            return result;
        }

        return default;
    }
}
