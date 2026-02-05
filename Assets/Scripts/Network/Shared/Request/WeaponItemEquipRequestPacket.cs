using UnityEngine;

public class WeaponItemEquipRequestPacket : RequestPacket
{
    public long CharacterId { get; set; }
    public long WeaponInstanceId { get; set; }
}
