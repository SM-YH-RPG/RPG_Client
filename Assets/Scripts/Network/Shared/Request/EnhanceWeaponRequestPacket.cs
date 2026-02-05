using System.Collections.Generic;
using UnityEngine;

public class EnhanceWeaponRequestPacket : RequestPacket
{
    public long WeaponInstanceID;
    public List<EnhanceMaterialItem> EnhanceMaterialsItemList;
}

public class EnhanceMaterialItem
{
    public long EnhanceMaterialInstanceID;
    public int Amount;
}
