using System.Collections.Generic;
using UnityEngine;

public class UpdatePartyPresetRequestPacket : RequestPacket
{        
    public int UpdatePartyPresetId;
    public List<long> PartyMemberIds;
    public bool IsActivePartyChange;
}
