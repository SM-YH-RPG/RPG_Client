using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections.Generic;

public class AttackSequence : SequenceNode
{
    private readonly Animator _animator;
    private readonly Transform _transform;
    private readonly Func<Transform> _getDetectedPlayer;
    private readonly Action<int> _randomAttackIndexCallback;
    private readonly NavMeshAgent _agent;
    private readonly float _AttackRangeSqr;

    private readonly int _attackAnimStateHash = Animator.StringToHash("Attack01");
    private readonly int _attack2AnimStateHash = Animator.StringToHash("Attack02");
    private readonly int _attack3AnimStateHash = Animator.StringToHash("Attack03");

    private float _currentAttackStartTime;
    private int _attackCount;
    private int _currentAttackAnimHash;

    private Transform _currentTarget;
    private List<int> _attackAnimStateHashList;

    private const float MIN_ANIMATION_READ_TIME = 0.1f;
    private const float ANIMATION_END_THRESHOLD = 0.98f;    

    public AttackSequence(Transform transform, Func<Transform> getDetectedPlayer, float attackRange, int attackCount = 1, Action<int> randomAttackIndexCallback = null)
    {
        _attackAnimStateHashList = new List<int>();

        _transform = transform ?? throw new ArgumentNullException(nameof(transform));
        _getDetectedPlayer = getDetectedPlayer ?? throw new ArgumentNullException(nameof(getDetectedPlayer));

        _transform.TryGetComponent(out _animator);
        _transform.TryGetComponent(out _agent);

        _AttackRangeSqr = attackRange * attackRange;

        _attackAnimStateHashList.Add(_attackAnimStateHash);
        _attackAnimStateHashList.Add(_attack2AnimStateHash);
        _attackAnimStateHashList.Add(_attack3AnimStateHash);

        _attackCount = attackCount;
        _randomAttackIndexCallback = randomAttackIndexCallback;

        _currentAttackAnimHash = 0;

        AddChild(new ActionNode(CheckEnemyWithinMeleeAttackRange));
        AddChild(new ActionNode(CheckIfAttackIsAlreadyRunning));
        AddChild(new ActionNode(DoAttack));
    }

    public override EState Execute()
    {
        foreach (Node node in childList)
        {
            EState state = node.Execute();
            if (state != EState.Success)
            {
                return state;
            }
        }

        return EState.Success;
    }

    private bool IsAnimationStateCurrentlyRunning(int animationHash)
    {
        if (_animator == null)
            return false;

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash == animationHash)
        {
            if (Time.time - _currentAttackStartTime < MIN_ANIMATION_READ_TIME)
            {
                return true;
            }

            return stateInfo.normalizedTime < ANIMATION_END_THRESHOLD;
        }

        return false;
    }

    private EState CheckIfAttackIsAlreadyRunning()
    {
        if (IsAnimationStateCurrentlyRunning(_currentAttackAnimHash))
        {
            return EState.Running;
        }

        return EState.Success;
    }

    private EState CheckEnemyWithinMeleeAttackRange()
    {
        _currentTarget = _getDetectedPlayer.Invoke();

        if (_currentTarget == null)
        {
            return EState.Failure;
        }

        Vector3 directionToTarget = _currentTarget.position - _transform.position;
        if (directionToTarget.sqrMagnitude < _AttackRangeSqr)
        {
            return EState.Success;
        }

        _currentTarget = null;
        return EState.Failure;
    }

    private EState DoAttack()
    {
        if (_currentTarget != null)
        {
            if (_agent != null && _agent.enabled)
            {
                _agent.isStopped = true;
            }

            int attackIndex = UnityEngine.Random.Range(0, _attackCount);
            _currentAttackAnimHash = _attackAnimStateHashList[attackIndex];
            _randomAttackIndexCallback?.Invoke(attackIndex);
            RotateTowardsTarget();

            _currentAttackStartTime = Time.time;
            _animator.CrossFade(_currentAttackAnimHash, 0f, 0, 0f);

            return EState.Success;
        }
        return EState.Failure;
    }

    private void RotateTowardsTarget()
    {
        Vector3 dir = _currentTarget.position - _transform.position;
        dir.y = 0f;        
        _transform.rotation = Quaternion.LookRotation(dir);
    }
}