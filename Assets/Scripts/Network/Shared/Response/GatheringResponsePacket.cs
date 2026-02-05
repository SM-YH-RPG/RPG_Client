using System.Collections.Generic;
using UnityEngine;

public class GatheringResponsePacket : ResponsePacket
{
    public Dictionary<int, int> Items; // Key : TemplateId , value : amount
}
