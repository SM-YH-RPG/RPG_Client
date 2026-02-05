using System.Collections.Generic;
using UnityEngine;
using System;

public class HitBoxDebugDecorator : IHitBoxGenerator
{
    private readonly IHitBoxGenerator _wrappedGenerator;
    private readonly HitBoxGizmoDrawer _gizmoDrawer;
    private Transform _transform;

    public HitBoxDebugDecorator(IHitBoxGenerator generator, HitBoxGizmoDrawer drawer)
    {
        _wrappedGenerator = generator ?? throw new ArgumentNullException(nameof(generator));
        _gizmoDrawer = drawer ?? throw new ArgumentNullException(nameof(drawer));
    }

    public void Initialize(Transform transform)
    {
        _transform = transform;
        _wrappedGenerator.Initialize(transform);
    }

    public List<IAttackTarget> ExecuteHitCheck(HitShapeData data)
    {
        var targets = _wrappedGenerator.ExecuteHitCheck(data);
        _gizmoDrawer.SetGizmoData(data, _transform);

        return targets;
    }

    public void ClearDebugGizmos()
    {
        _gizmoDrawer.ClearGizmos();
    }
}