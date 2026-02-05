using System.Collections.Generic;
using UnityEngine;

public class EnhanceEquipmentRequestPacket : RequestPacket
{
    public long EquipmentInstanceID;
    public List<EnhanceMaterialItem> EnhanceMaterialsItemList;
}
