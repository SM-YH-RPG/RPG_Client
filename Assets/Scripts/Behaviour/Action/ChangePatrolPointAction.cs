using System;
using UnityEngine;
using UnityEngine.AI;

public class ChangePatrolPointAction : Node
{
    private float _patrolRadius;
    private Func<Vector3> _getOriginPos;
    private Action<Vector3> _setPatrolPointCallback;

    public ChangePatrolPointAction(float patrolRadius, Func<Vector3> getOriginPos, Action<Vector3> setPatrolPointCallback)
    {
        _patrolRadius = patrolRadius;
        _getOriginPos = getOriginPos;
        _setPatrolPointCallback = setPatrolPointCallback;
    }

    public override EState Execute()
    {
        Vector3 originPos = _getOriginPos.Invoke();
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * _patrolRadius;
        randomDirection += originPos;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDirection, out hit, _patrolRadius, NavMesh.AllAreas))
        {
            _setPatrolPointCallback?.Invoke(hit.position);
            return EState.Success;
        }

        _setPatrolPointCallback?.Invoke(originPos);
        return EState.Failure;
    }
}