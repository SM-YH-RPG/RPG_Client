using UnityEngine;

public class PurchaseItemRequestPacket : RequestPacket
{
    public EShopCategory ShopType;
    public int TemplateId;   
    public int Amount;
}
