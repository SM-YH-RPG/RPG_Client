using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class PatrolSequence : SequenceNode
{
    private const float ARRIVAL_THRESHOLD = 0.1f;                   //.. 도착 임계값
    private const float MIN_DESTINATION_CHANGE_SQR = 0.01f;         //.. 목적지 변경 최소 거리 제곱
    private const float MIN_ANIMATION_READ_TIME = 0.1f;             //.. 애니메이션 최소 재생 시간
    private const float ANIMATION_END_THRESHOLD = 0.98f;            //.. 애니메이션 종료 임계값
    private const float DESTINATION_UPDATE_INTERVAL = 0.2f;         //.. 목적지 갱신 간격
    private const float MAX_WALK_DISTANCE = 15f;                    //.. 복귀 거리
    private const float WAIT_TIME = 2.0f;                           //.. 이동 완료 후 대기 시간

    private readonly int _walkAnimStateHash = Animator.StringToHash("Walk");
    private readonly int _idleAnimStateHash = Animator.StringToHash("Idle");


    private NavMeshAgent _agent;
    private Transform _transform;
    private Animator _animator;
    private float _stoppingDistance;
    private float _patrolRadius = 10f;
    private Vector3 _originPos = Vector3.zero;
    private Vector3 _currentPatrolPoint;

    private float _currentWalkStartTime;
    private float _nextDestinationUpdateTime;

    private bool _isWaiting = false;
    private bool _waitCompleted = false;


    public PatrolSequence(Transform transform, float stoppingDistance, float patrolRadius)
    {
        _transform = transform;
        _stoppingDistance = stoppingDistance;
        _patrolRadius = patrolRadius;

        transform.TryGetComponent(out _animator);
        transform.TryGetComponent(out _agent);

        _originPos = _transform.position;
        _currentPatrolPoint = _originPos;

        childList.Add(new ActionNode(CheckIfArrivedAtPatrolPoint));
        childList.Add(new ActionNode(WaitAtPoint));
        childList.Add(new ActionNode(ChangePatrolPoint));
        childList.Add(new ActionNode(MoveToPatrolPoint));
    }

    private EState CheckIfArrivedAtPatrolPoint()
    {
        if (_agent == null || !_agent.enabled)
            return EState.Failure;

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance + ARRIVAL_THRESHOLD)
        {
            _agent.isStopped = true;
            return EState.Success;
        }

        return EState.Failure;
    }

    private EState ChangePatrolPoint()
    {
        if (Vector3.Distance(_transform.position, _originPos) > MAX_WALK_DISTANCE)
        {
            _currentPatrolPoint = _originPos;
        }
        else
        {
            Vector3 randomDirection = Random.insideUnitSphere * _patrolRadius + _originPos;
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _patrolRadius, NavMesh.AllAreas))
                _currentPatrolPoint = hit.position;
            else
                _currentPatrolPoint = _originPos;
        }

        _nextDestinationUpdateTime = Time.time;
        return EState.Success;
    }

    private EState MoveToPatrolPoint()
    {
        if (_agent == null || _agent.enabled == false)
            return EState.Failure;

        if (Time.time >= _nextDestinationUpdateTime)
        {
            _nextDestinationUpdateTime = Time.time + DESTINATION_UPDATE_INTERVAL;
            if (Vector3.SqrMagnitude(_agent.destination - _currentPatrolPoint) > MIN_DESTINATION_CHANGE_SQR)
            {
                _agent.SetDestination(_currentPatrolPoint);
                if (IsAnimationStateCurrentlyRunning(_walkAnimStateHash) == false)
                {
                    _currentWalkStartTime = Time.time;
                    _animator.CrossFade(_walkAnimStateHash, MIN_ANIMATION_READ_TIME);
                }
            }
        }

        if (_agent.isStopped)
            _agent.isStopped = false;

        _agent.stoppingDistance = _stoppingDistance;

        if (_agent.pathPending == false && _agent.hasPath &&
            _agent.remainingDistance <= _agent.stoppingDistance + ARRIVAL_THRESHOLD)
        {
            return EState.Success;
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
                return true;

            return stateInfo.normalizedTime < ANIMATION_END_THRESHOLD;
        }

        return false;
    }


    private EState WaitAtPoint()
    {
        if (_waitCompleted)
        {
            _waitCompleted = false;
            return EState.Success;
        }

        if (_isWaiting)
            return EState.Running;

        StartWaitTask(_transform.GetCancellationTokenOnDestroy()).Forget();
        return EState.Running;
    }

    private async UniTaskVoid StartWaitTask(CancellationToken cancellationToken)
    {
        _isWaiting = true;
        _animator.CrossFade(_idleAnimStateHash, MIN_ANIMATION_READ_TIME);

        try
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(WAIT_TIME), cancellationToken: cancellationToken);
        }
        catch (System.OperationCanceledException)
        {
            throw;
        }

        _isWaiting = false;
        _waitCompleted = true;
    }
}