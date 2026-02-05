using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillUsageObserver
{
    void OnSkillUsed(ESkillType skillType, float cooldown);
}

//.. 추가로 등록될 가능성이 있는 옵저버 : 사운드 / 이펙트
public class SkillManager : LazySingleton<SkillManager>
{
    private readonly List<ISkillUsageObserver> _observers = new List<ISkillUsageObserver>();

    public void Subscribe(ISkillUsageObserver observer)
    {
        if (observer != null && !_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void Unsubscribe(ISkillUsageObserver observer)
    {
        if (observer != null)
        {
            _observers.Remove(observer);
        }
    }

    public void NotifySkillUsed(ESkillType skillType, float cooldown)
    {
        foreach (var observer in _observers)
        {
            observer.OnSkillUsed(skillType, cooldown);
        }
    }
}
