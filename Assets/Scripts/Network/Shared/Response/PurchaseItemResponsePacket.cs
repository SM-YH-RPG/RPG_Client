using UnityEngine;

public class PurchaseItemResponsePacket : ResponsePacket
{
    public BaseInventoryItem Item { get; set; }
    public int RemainingGold { get; set; }
}
