using System;
using UnityEngine;

//.. FIXME :: 이것도 고치자?
public struct ShopItemData : IComparable<ShopItemData>
{
    public int CategoryIndex;
    public int Index;    
    public int Price;
    public int PurchaseLimitCount;
    
    public int CompareTo(ShopItemData other)
    {
        return this.Index.CompareTo(other.Index); // index 오름차순 정렬
    }
}
