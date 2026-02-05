using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PlacementPrefabData
{
    public int TemplateIndex;
    public GameObject Prefab;
}

[CreateAssetMenu(fileName = "PlacementPrefabDataConfig", menuName = "Scriptable Objects/PlacementPrefabDataConfig")]
public class PlacementPrefabDataConfig : ScriptableObject
{
    [field: SerializeField]
    private PlacementPrefabData[] _placementPrefabDataConfigArray;

    private Dictionary<int, PlacementPrefabData> _lookupCache;

    public PlacementPrefabData GetPlacementPrefaConfigData(int index)
    {
        if (_lookupCache == null)
        {
            _lookupCache = new Dictionary<int, PlacementPrefabData>();
            foreach (var data in _placementPrefabDataConfigArray)
            {
                if (_lookupCache.ContainsKey(data.TemplateIndex) == false)
                {
                    _lookupCache.Add(data.TemplateIndex, data);
                }
            }
        }

        if (_lookupCache.TryGetValue(index, out PlacementPrefabData result))
            return result;

        return default;
    }
}
