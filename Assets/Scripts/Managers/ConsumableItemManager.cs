using System;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItemManager : LazySingleton<ConsumableItemManager>
{
    private readonly List<IConsumableItemUsageObserver> _observers = new List<IConsumableItemUsageObserver>();    

    public void Subscribe(IConsumableItemUsageObserver observer)
    {
        if (observer != null && !_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void Unsubscribe(IConsumableItemUsageObserver observer)
    {
        if (observer != null)
        {
            _observers.Remove(observer);
        }
    }

    public void NotifyConsumableItemUsed(EConsumableEffectType type, int templateIndex, float cooldown)
    {
        foreach (var observer in _observers)
        {
            observer.OnConsumableItemUsed(type, templateIndex, cooldown);
        }
    }     
}
