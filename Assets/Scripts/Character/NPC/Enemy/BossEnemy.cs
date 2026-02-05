using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : BaseEnemy
{
    private const int ATTACK_ANIM_COUNT = 3;

    private enum EattackAnimType
    {
        Bite,
        Claw,
        Head
    }

    #region Inspector
    [SerializeField] private BossEnemyConfig _bossConfig;   
    [SerializeField] private Vector3 _uiSpawnPosition;
    #endregion

    private EattackAnimType _currentActionAttackType;

    private IAttackTypeSate _biteAttackType;
    private IAttackTypeSate _clawAttackType;
    private IAttackTypeSate _headAttackType;

    protected override void Awake()
    {
        base.Awake();

        _biteAttackType = new BossBiteAttackState(this);
        _clawAttackType = new BossClawAttackState(this);
        _headAttackType = new BossHeadAttackState(this);
    }

    private void SetActionAttackType(int index)
    {
        _currentActionAttackType = (EattackAnimType)index;
    }

    public override void OnDamaged(CombatData combatData)
    {
        base.OnDamaged(combatData);

        EnemyHPHandler.Instance.NotifyBossEnemyHPChanged(_bossConfig.bossName, _currentHP, _bossConfig.MaxHP);

        UIPoolingObjectSpawnManager.Instance.ShowDamageText(transform, _uiSpawnPosition, combatData, Color.white);
    }

    protected override void InitializeBT()
    {
        var rootNode = new RootNode();
        var mainSelector = new SelectorNode(rootNode, null);

        var attackSequence = new AttackSequence(
            transform,
            () => _detectedPlayer,
            _baseConfig.AttackRange,
            ATTACK_ANIM_COUNT,
            SetActionAttackType);

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

    protected override void RewardGenerate()
    {
        int rewardTotalCount = _bossConfig.rewardIndexArray.Length;
        int rewardCount = UnityEngine.Random.Range(0, rewardTotalCount);
        for (int i = 0; i < rewardCount; i++)
        {
            int rewardArrayIndex = UnityEngine.Random.Range(0, rewardTotalCount);
            int rewardIndex = _bossConfig.rewardIndexArray[rewardArrayIndex];
            RewardItemManager.Instance.RewardDropItem(rewardIndex);
        }
    }

    public override AttackConfig GetAttackConfig()
    {
        switch (_currentActionAttackType)
        {
            case EattackAnimType.Bite:
                return _bossConfig.attackGroupConfig.BiteAttack;
            case EattackAnimType.Claw:
                return _bossConfig.attackGroupConfig.ClawAttack;
            case EattackAnimType.Head:
                return _bossConfig.attackGroupConfig.HeadAttack;
            default:
                return _bossConfig.attackGroupConfig.BiteAttack;
        }
    }

    public override IAttackTypeSate GetAttackTypeState()
    {
        switch (_currentActionAttackType)
        {
            case EattackAnimType.Bite:
                return _biteAttackType;
            case EattackAnimType.Claw:
                return _clawAttackType;
            case EattackAnimType.Head:
                return _headAttackType;
            default:
                return _biteAttackType;
        }
    }

    public BossAttackGroupConfig GetBossAttackGroupConfig()
    {
        return _bossConfig.attackGroupConfig;
    }
}
