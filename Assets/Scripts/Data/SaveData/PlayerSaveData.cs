using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class PlayerSaveData
{
    public int Version = 1;

    public int Gold;
    public int LastSelectedPartyPresetIndex;
    public int LastSelectedIndexInParty;
    public long InventoryInstanceId;

    public Dictionary<EItemCategory, Dictionary<long, BaseInventoryItem>> Inventory = new();
    
    public Dictionary<long, RuntimeCharacter> Characters = new();
    
    public Dictionary<int, Party> Parties = new();
}

