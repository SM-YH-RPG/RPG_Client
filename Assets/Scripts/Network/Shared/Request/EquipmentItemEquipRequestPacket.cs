public class EquipmentItemEquipRequestPacket : RequestPacket
{
    public long CharacterId { get; set; }
    public int SlotIndex { get; set; }
    public long EquipmentInstanceId { get; set; }
}