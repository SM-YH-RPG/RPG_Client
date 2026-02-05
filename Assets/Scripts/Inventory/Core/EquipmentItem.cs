using System.Collections.Generic;

public class EquipmentItem : EquipableItem, IEquipmentItem
{
    public EItemStatType MainStatType { get; set; }
    public float MainStatValue { get; set; }
    public EItemStatType RandomMainStatType { get; set; }
    public float RandomMainStatValue { get; set; }
    public Dictionary<EItemStatType, float> SubStatDict { get; set; } = new Dictionary<EItemStatType, float>();
    public int EquipCost { get; set; }
}