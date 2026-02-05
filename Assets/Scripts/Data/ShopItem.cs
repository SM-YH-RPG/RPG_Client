using UnityEngine;

public enum EShopCategory
{
    None = 0,
    Consumption = 1,
    Weapon = 2,
    Equipment = 3,
    Enhancement = 4,
    Mixer = 5,
}

public class ShopItem
{
    public int ShopItemId { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public int ProvideItemId { get; set; }
    public EShopCategory Category
    {
        get; set;
    }
}
