using UnityEngine.AI;
using UnityEngine;
using System;

public class MoveToPatrolAction : Node
{
    private NavMeshAgent _agent;
    private Func<Vector3> _getPatrolPoint; // Callback to get the target point dynamically
    private float _stoppingDistance;

    public MoveToPatrolAction(NavMeshAgent agent, Func<Vector3> getPatrolPoint, float stoppingDistance)
    {
        _agent = agent;
        _getPatrolPoint = getPatrolPoint;
        _stoppingDistance = stoppingDistance;
    }

    public override EState Execute()
    {
        if (_agent == null || !_agent.enabled) return EState.Failure;

        Vector3 targetPos = _getPatrolPoint.Invoke();

        _agent.stoppingDistance = _stoppingDistance;
        _agent.SetDestination(targetPos);

        if (_agent.remainingDistance <= _agent.stoppingDistance + 0.1f && !_agent.pathPending && _agent.hasPath)
        {
            return EState.Success;
        }

        return EState.Running;
    }
}