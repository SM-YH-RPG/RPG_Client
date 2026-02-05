using System;
using UnityEngine;

public class TargetDetectAction : Node
{
    private Transform _transform;
    private float _detectRange;
    private Action<Transform> _detectedTargetCallback;

    public TargetDetectAction(Transform transform, float detectRage, Action<Transform> callback)
    {
        _transform = transform;
        _detectRange = detectRage;
        _detectedTargetCallback = callback;
    }

    public override EState Execute()
    {
        var overlap = Physics.OverlapSphere(_transform.position, _detectRange, LayerMask.GetMask("Player"));
        if (overlap != null && overlap.Length > 0)
        {
            _detectedTargetCallback?.Invoke(overlap[0].transform);
            return EState.Success;
        }

        return EState.Failure;
    }
}