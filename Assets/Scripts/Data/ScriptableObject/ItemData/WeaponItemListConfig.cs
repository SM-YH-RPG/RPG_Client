using System;
using System.Collections.Generic;
using UnityEngine;

public enum EWeaponType
{
    None,
    Katana,
    Gun,
    Kunai,
    Wnad
}

[Serializable]
public struct WeaponItemConfigData
{
    public int Index;
    public EWeaponType Type;
    public EItemStatType MainStatType;    
    public EItemStatType SubStatType;
}

[CreateAssetMenu(fileName = "WeaponItemListConfig", menuName = "Scriptable Objects/WeaponItemListConfig")]
public class WeaponItemListConfig : ItemListConfig
{
    [field: SerializeField]
    private WeaponItemConfigData[] _weaponItemConfigArray;
    public IReadOnlyList<WeaponItemConfigData> WeaponDatas => _weaponItemConfigArray;

    private Dictionary<int, WeaponItemConfigData> _lookupCache;

    public WeaponItemConfigData GetWeaponConfigData(int index)
    {
        if (_lookupCache == null)
        {
            _lookupCache = new Dictionary<int, WeaponItemConfigData>();
            foreach (var item in _weaponItemConfigArray)
            {                
                if (!_lookupCache.TryGetValue(item.Index, out var weaponList))
                {                    
                    _lookupCache.Add(item.Index, weaponList);
                }                
            }
        }

        if (_lookupCache.TryGetValue(index, out WeaponItemConfigData result))
        {
            return result;
        }

        return default;
    }
}
