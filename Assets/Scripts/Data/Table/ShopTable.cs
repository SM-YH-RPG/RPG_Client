using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class ShopTable : LazySingleton<ShopTable>
{
    private ShopItemData[] _shopItemDataArray;
    private Dictionary<int, ShopItemData> _shopItemDict;

    public async UniTask LoadTable()
    {
        _shopItemDataArray = await DataLoader.LoadJson<ShopItemData[]>("ShopItemTable");
        _shopItemDict = new Dictionary<int, ShopItemData>();
        foreach (var item in _shopItemDataArray)
        {
            _shopItemDict.Add(item.Index, item);
        }
    }

    public ShopItemData[] GetShopItemDatas()
    {
        return _shopItemDataArray;
    }

    public ShopItemData GetShopItemData(int _index)
    {
        if (_shopItemDict.ContainsKey(_index))
            return _shopItemDict[_index];
        return default;
    }
}
