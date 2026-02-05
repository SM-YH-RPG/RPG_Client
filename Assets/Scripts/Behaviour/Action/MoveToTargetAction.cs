using System;
using UnityEngine;
using UnityEngine.AI;

public class MoveToTargetAction : Node
{
    private const float MIN_ANIMATION_READ_TIME = 0.1f;
    private const float ANIMATION_END_THRESHOLD = 0.98f;

    private NavMeshAgent _agent;
    private Animator _animator;
    private float _attackRange = 2f;
    private Func<Transform> _setTargetCallback;
    private const string WALK_ANIM_STATE_NAME = "Walk";
    private readonly int _walkAnimStateHash = Animator.StringToHash(WALK_ANIM_STATE_NAME);
    private float _currentWalkStartTime;    

    public MoveToTargetAction(Transform transform, float attackRange, Func<Transform> setTargetCallback)
    {
        _setTargetCallback = setTargetCallback;
        _attackRange = attackRange;

        transform.TryGetComponent(out _animator);
        transform.TryGetComponent(out _agent);
    }

    public override EState Execute()
    {
        var target = _setTargetCallback.Invoke();
        if (target == null)
        {
            if (_agent.enabled)
                _agent.isStopped = true;

            return EState.Failure;
        }

        _agent.stoppingDistance = _attackRange;
        Vector3 directionToTarget = target.position - _agent.transform.position;

        if (directionToTarget.sqrMagnitude <= _attackRange * _attackRange)
        {
            if (_agent.enabled)
                _agent.isStopped = true;

            return EState.Success;
        }

        if (_agent.enabled)
        {
            if (_agent.isStopped)
                _agent.isStopped = false;

            var targetPosition = target.position;
            if (Vector3.SqrMagnitude(_agent.destination - targetPosition) > 0.1f * 0.1f)
            {
                _agent.SetDestination(targetPosition);
            }

            if (IsAnimationStateCurrentlyRunning(_walkAnimStateHash) == false)
            {
                _currentWalkStartTime = Time.time;
                _animator.CrossFade(_walkAnimStateHash, 0f, 0, 0f);
            }
        }

        return EState.Running;
    }

    private bool IsAnimationStateCurrentlyRunning(int animationHash)
    {
        if (_animator == null)
            return false;

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash == animationHash)
        {
            if (Time.time - _currentWalkStartTime < MIN_ANIMATION_READ_TIME)
            {
                return true;
            }

            return stateInfo.normalizedTime < ANIMATION_END_THRESHOLD;
        }

        return false;
    }
}