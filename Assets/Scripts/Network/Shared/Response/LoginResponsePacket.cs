using Newtonsoft.Json;
using System.Collections.Generic;

public class LoginResponsePacket : ResponsePacket
{
    public UserInfo User;    
}

public class UserInfo
{
    public int UniqueId { get; set; }
    public string IdentifyCode { get; set; }

    public string Name { get; set; }

    public int Gold { get; set; }

    public int SelectPresetID { get; set; }

    public Dictionary<int, RuntimeCharacter> OwnsCharacterList { get; set; }
        = new Dictionary<int, RuntimeCharacter>();

    public Dictionary<int, PartyInfo> PartyPresets { get; set; }
        = new Dictionary<int, PartyInfo>();

    //[JsonConverter(typeof(InventoryItemsConverter))]
    public Dictionary<EItemCategory, Dictionary<long, BaseInventoryItem>> InventoryItems { get; set; }
        = new Dictionary<EItemCategory, Dictionary<long, BaseInventoryItem>>();
}

public class PartyInfo
{
    public int Index { get; set; }
    public string PresetName { get; set; }
    public List<int> Members { get; set; }
}
