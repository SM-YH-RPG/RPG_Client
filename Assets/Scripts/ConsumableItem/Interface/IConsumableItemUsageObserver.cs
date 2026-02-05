public interface IConsumableItemUsageObserver
{
    void OnConsumableItemUsed(EConsumableEffectType Type, int TemplateIndex, float cooldown);    
}
