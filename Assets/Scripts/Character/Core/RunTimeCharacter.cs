
using System.Collections.Generic;

public class RuntimeCharacter
{
    public long InstanceId;
    public int TemplateId;
    public int Level;
    public int Exp;
    public int CurrentHP;
    public int MaxHp;
    public int Damage;
    public int Defence;
    public float ChargeEfficiency;
    public float CriticalPercent;
    public float CriticalDamageRate;
    public Dictionary<int, BaseInventoryItem> EquippedItems;
}