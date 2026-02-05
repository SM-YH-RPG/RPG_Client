using System.Collections.Generic;
using UnityEngine;

public interface IHitBoxGenerator
{
    void Initialize(Transform transform);
    List<IAttackTarget> ExecuteHitCheck(HitShapeData data);
}