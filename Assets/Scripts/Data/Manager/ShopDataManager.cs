using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopDataManager : LazySingleton<ShopDataManager>
{
    private Dictionary<EShopCategory, List<ShopItem>> _shopItemByCategory = new Dictionary<EShopCategory, List<ShopItem>>();

    private EShopCategory _currentCategory;
    public EShopCategory CurrentCategory => _currentCategory;

    public bool TryCheckShopItemCached(EShopCategory category)
    {
        if (_shopItemByCategory.ContainsKey(category) == false)
            return true;
        return false;
    }

    public void SetDataLoadCurrentCategory(EShopCategory category)
    {
        _currentCategory = category;
    }

    public void InitShopItemListByCategory(EShopCategory category, List<ShopItem> items)
    {
        if (_shopItemByCategory.ContainsKey(category) == false)
        {
            _shopItemByCategory.Add(category, items);
        }
    }    

    public List<ShopItem> GetShopItemList(EShopCategory category)
    {
        if (_shopItemByCategory.TryGetValue(category, out List<ShopItem> items))
        {
            return items;
        }
        return null;
    }
}
