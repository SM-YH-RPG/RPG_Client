using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDataManager : LazySingleton<WeaponDataManager>
{
    private const int MAX_TIER = 5;
    public int MaxTier => MAX_TIER;

    private Dictionary<int, WeaponItemConfigData> _weaponConfigCache = new Dictionary<int, WeaponItemConfigData>();

    private EnhanceConfigData _weaponEnhanceConfig;
    public EnhanceConfigData WeaponEnhanceConfg => _weaponEnhanceConfig;

    public void Initialize(WeaponItemListConfig weaponItemListConfig)
    {
        if (weaponItemListConfig == null) return;

        _weaponConfigCache.Clear();
        foreach (var data in weaponItemListConfig.WeaponDatas)
        {
            if (_weaponConfigCache.ContainsKey(data.Index) == false)
            {
                _weaponConfigCache.Add(data.Index, data);
            }
        }
    }

    public WeaponItemConfigData GetWeaponConfigByIndex(int index)
    {
        if (_weaponConfigCache.TryGetValue(index, out WeaponItemConfigData config))
        {
            return config;
        }

        return default;
    }

    public async UniTask<Sprite> GetWeaponTypeSprite(EWeaponType type)
    {
        switch (type)
        {
            case EWeaponType.None:
                break;
            case EWeaponType.Katana:                
                return await AddressableManager.Instance.LoadAssetAsync<Sprite>("sword2");
            case EWeaponType.Gun:                
                return await AddressableManager.Instance.LoadAssetAsync<Sprite>("target");                
            case EWeaponType.Kunai:                
                return await AddressableManager.Instance.LoadAssetAsync<Sprite>("battle");                
            case EWeaponType.Wnad:                
                return await AddressableManager.Instance.LoadAssetAsync<Sprite>("fairy-wand");
            default:
                break;
        }
        return null;
    }

    public void SetWeaponEnhanceConfigData(EnhanceConfigData weaponEnhanceConfig)
    {
        _weaponEnhanceConfig = weaponEnhanceConfig;
    }
}