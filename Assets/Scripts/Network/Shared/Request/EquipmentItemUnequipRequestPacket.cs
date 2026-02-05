using UnityEngine;

public class EquipmentItemUnequipRequestPacket : RequestPacket
{
    public long CharacterId { get; set; }
    public int SlotIndex { get; set; }
    //public long EquipmentInstanceID; // 장비 고유 ID는 해당 캐릭터 슬릇 인덱스를 통해 아이템으로 서버가 접근 할 수 있으니 필요 없을거같다..
}
