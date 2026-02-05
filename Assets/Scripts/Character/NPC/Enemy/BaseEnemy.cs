using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;


public abstract class BaseEnemy : MonoBehaviour, IAttackTarget, IPoolTarget, IAttackableCtrl
{
    public enum EState
    {
        Alive,
        Paused,
        Dead,
    }

    private const string HIT_ANIM_STATE_NAME = "Hit";
    private readonly int _hitAnimStateHash = Animator.StringToHash(HIT_ANIM_STATE_NAME);
    private const string DIE_ANIM_STATE_NAME = "Die";
    private readonly int _dieAnimStateHash = Animator.StringToHash(DIE_ANIM_STATE_NAME);

    #region Inspector
    //.. TODO :: Change To Scriptable Object
    [SerializeField]
    protected EnemyBaseConfig _baseConfig = null;

    [SerializeField]
    private float _stiffnessDuration;
    //..

    [SerializeField]
    private NavMeshAgent _agent = null;
    #endregion

    protected GameObject _originPrefab = null;

    private Vector3 _originPos = default;
    private Vector3 _currentPatrolPoint = default;

    protected BehaviourTree _BT = null;
    protected Transform _detectedPlayer = null;
    private Animator _animator = null;
    private Collider _collider = null;

    private EState _currentState = EState.Alive;

    protected int _currentHP;

    private CancellationTokenSource _staggerCTS;

    private Action<BaseEnemy> OnDeathSpawnListRemoveCallback;
    
    private IDamageCalculator _damageCalculator;

    public GameObject GameObject => this.gameObject;

    protected virtual void Awake()
    {
        TryGetComponent(out _animator);
        TryGetComponent(out _agent);
        TryGetComponent(out _collider);

        if (_agent == null)
        {
            enabled = false;
            return;
        }

        if (_agent != null)
        {
            _agent.speed = _baseConfig.MovementSpeed;
            _agent.stoppingDistance = _baseConfig.StoppingDistance;
        }

        _originPos = transform.position;
        _currentPatrolPoint = _originPos;
        
        _damageCalculator = new MonsterDamageCalculator(this);

        PlayerManager.Instance.PartyService.OnPartyCharacterSwappedChangeMonsterCurrentTarget += SwappedDetecedCharacter;        
    }

    private void Start()
    {
        InitializeBT();
    }

    private void OnEnable()
    {
        _currentState = EState.Alive;

        _collider.enabled = true;
        if (_agent != null && _agent.enabled && _agent.isOnNavMesh)
        {
            _agent.isStopped = false;
        }
    }

    private void OnDisable()
    {
        _collider.enabled = false;
        if (_agent != null && _agent.enabled && _agent.isOnNavMesh)
        {
            _agent.isStopped = true;
        }
    }

    private void Update()
    {
        if(_currentState != EState.Alive)
        {
            return;
        }

        _BT.Run();
    }

    private void OnDestroy()
    {
        PlayerManager.Instance.PartyService.OnPartyCharacterSwappedChangeMonsterCurrentTarget -= SwappedDetecedCharacter;        
    }

    public void SetDeathCallback(Action<BaseEnemy> deathCallback)
    {
        OnDeathSpawnListRemoveCallback = deathCallback;
    }

    public void SetOriginPrefab(GameObject origin)
    {
        _originPrefab = origin;
    }

    public virtual void ResetEnemy()
    {
        _currentHP = _baseConfig.MaxHP;
        _currentState = EState.Alive;
        _detectedPlayer = null;
        _currentPatrolPoint = _originPos;

        if (_animator != null)
        {
            _animator.Rebind();
        }

        if (_agent != null && _agent.enabled)
        {
            _agent.isStopped = false;
            _agent.Warp(_originPos);
        }

        enabled = true;
    }

    /// <summary>
    /// Behaviour Tree 초기화
    /// [priority] Attack -> Chase -> Patrol
    /// [info] Attack 시 플레이어가 Attack Range 내에 있는지 확인 후 공격
    /// [info] Chase 시 플레이어를 감지하면 Attack Range 까지 이동
    /// [info] Patrol 시 현재 위치에서 일정 반경 내 랜덤 지점으로 이동
    /// [warining] 서브 클레스에서 오버라이딩 하여 세부 행동 구현 필요
    /// </summary>
    protected virtual void InitializeBT()
    {
        var rootNode = new RootNode();
        var mainSelector = new SelectorNode(rootNode, null);

        var attackSequence = new AttackSequence(
            transform,
            () => _detectedPlayer,
            _baseConfig.AttackRange);

        mainSelector.AddChild(attackSequence);

        mainSelector.AddChild(new SequenceNode
        (
            new List<Node>()
            {
                new TargetDetectAction(
                    transform,
                    _baseConfig.DetectRange,
                    (target)=> {
                        _detectedPlayer = target; 
                    }),
                new MoveToTargetAction(
                    transform,
                    _baseConfig.AttackRange,
                    () => _detectedPlayer),
            }
        ));

        var patrolSequence = new PatrolSequence(
            transform,
            _baseConfig.StoppingDistance,
            _baseConfig.PatrolRadius);

        mainSelector.AddChild(patrolSequence);

        _BT = new BehaviourTree(rootNode);
    }

    public void StopBT()
    {
        if (_currentState == EState.Alive || _currentState == EState.Paused)
        {
            _currentState = EState.Paused;
            if (_agent != null && _agent.enabled)
            {
                _agent.isStopped = true; 
            }
        }
    }

    public void ResumeBT()
    {
        if (_currentState == EState.Paused)
        {
            _currentState = EState.Alive;
            if (_agent != null && _agent.enabled)
            {
                _agent.isStopped = false;
            }
        }
    }

    public virtual void OnDamaged(CombatData combatData)
    {
        _currentHP -= combatData.damage;

        Debug.Log($"Enemy : {gameObject.name} Damaged : <color=#FF0000>{combatData.damage}</color>, Current HP : {_currentHP}");

        if (_currentHP <= 0)
        {
            HandleDeath();
        }
        else
        {
            _animator?.CrossFade(_hitAnimStateHash, 0.1f);
            StopBTAsync(_stiffnessDuration).Forget();

            EffectManager.Instance.SpawnEffect(_baseConfig.HitEffect, transform.position, transform.rotation);
        }
    }

    #region AnimationEvent
    public void AnimationEvent_ReturePool()
    {
        ResetEnemy();
        ObjectPoolManager.Instance.ReturnToPool(_originPrefab, this);
    }
    #endregion

    private void HandleDeath()
    {
        _currentState = EState.Dead;
        if (_animator != null)
        {
            _animator.CrossFade(_dieAnimStateHash, 0.1f);
        }

        _staggerCTS?.Cancel();
        _staggerCTS?.Dispose();
        _staggerCTS = null;

        enabled = false;

        RewardGenerate();
        CharacterLevelManager.Instance.GetExp(_baseConfig.EXP);
        OnDeathSpawnListRemoveCallback?.Invoke(this);
    }        

    protected async UniTaskVoid StopBTAsync(float duration)
    {
        _staggerCTS?.Cancel();
        _staggerCTS?.Dispose();
        _staggerCTS = new CancellationTokenSource();
        CancellationToken token = _staggerCTS.Token;

        try
        {
            StopBT();

            await UniTask.Delay(TimeSpan.FromSeconds(duration), ignoreTimeScale: false, cancellationToken: token);

            if (!token.IsCancellationRequested)
            {
                ResumeBT();
            }
        }
        catch (OperationCanceledException ex)
        {
            Debug.Log($"Exception : <color=#FF0000>{ex.Message}</color>");
        }
        finally
        {
            if (_staggerCTS != null && _staggerCTS.Token == token)
            {
                _staggerCTS.Dispose();
                _staggerCTS = null;
            }
        }
    }

    private void SwappedDetecedCharacter()
    {
        _detectedPlayer = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _baseConfig.AttackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _baseConfig.DetectRange);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_originPos, 0.3f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(_currentPatrolPoint, 0.3f);
    }

    public int GetEnemyDamage()
    {
        return _baseConfig.Damage;
    }

    public float GetDamageRate()
    {
        return _baseConfig.DamageRate;
    }

    public virtual void OnEnqueuedToPool()
    {
        if (_agent != null && _agent.isOnNavMesh)
            _agent.isStopped = true;
    }

    public virtual void OnDequeuedFromPool()
    {
        ResetEnemy();
    }

    protected abstract void RewardGenerate();

    public abstract AttackConfig GetAttackConfig();

    public abstract IAttackTypeSate GetAttackTypeState();    

    public IDamageCalculator GetCalculator()
    {
        return _damageCalculator;
    }
}
