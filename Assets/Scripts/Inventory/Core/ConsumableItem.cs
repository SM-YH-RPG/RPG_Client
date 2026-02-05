public enum EConsumableEffectType
{
    HPRecovery,
    End
}


public class ConsumableItem : BaseInventoryItem
{
    public EConsumableEffectType ConsumableEffectType;
    public float ConsumableEffectValue;
    public float CooldownSeconds;
}
