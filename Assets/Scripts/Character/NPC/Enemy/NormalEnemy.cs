using UnityEngine;

public class NormalEnemy : BaseEnemy
{
    #region Inspector
    [SerializeField]
    private EnemyConfig _enemyConfig;

    [SerializeField] //.. debug purpose
    private UINormalEnemyHP _enemyHPBar;

    [SerializeField]
    private GameObject _hpBarPrefab;

    [SerializeField]
    private Vector3 _uiSpawnPosition;
    #endregion

    private IAttackTypeSate _attackState;

    protected override void Awake()
    {
        base.Awake();

        _attackState = new BaseEnemyAttackState(this);
    }

    public override void OnDequeuedFromPool()
    {
        base.OnDequeuedFromPool();

        _enemyHPBar = UIPoolingObjectSpawnManager.Instance.SpawnEnemyHpBar(_hpBarPrefab, transform, _uiSpawnPosition, Quaternion.identity);
    }

    public override void OnEnqueuedToPool()
    {
        if (_enemyHPBar != null)
        {
            _enemyHPBar.ResetHPBar();
            ObjectPoolManager.Instance.ReturnToPool(_hpBarPrefab, _enemyHPBar);
            _enemyHPBar = null;
        }

        base.OnEnqueuedToPool();
    }

    public override void OnDamaged(CombatData combatData)
    {
        base.OnDamaged(combatData);
        
        if (_enemyHPBar != null)
        {
            _enemyHPBar.UpdateHPBar(_currentHP, _baseConfig.MaxHP);

            UIPoolingObjectSpawnManager.Instance.ShowDamageText(transform, _uiSpawnPosition, combatData, Color.white);
        }
    }

    protected override void RewardGenerate()
    {
        int rewardTotalCount = _enemyConfig.rewardIndexArray.Length;
        int rewardCount = UnityEngine.Random.Range(0, rewardTotalCount);
        for (int i = 0; i < rewardCount; i++)
        {
            int rewardArrayIndex = UnityEngine.Random.Range(0, rewardTotalCount);
            int rewardIndex = _enemyConfig.rewardIndexArray[rewardArrayIndex];
            RewardItemManager.Instance.RewardDropItem(rewardIndex);
        }
    }

    public override AttackConfig GetAttackConfig()
    {
        return _enemyConfig.attackConfig;
    }

    public override IAttackTypeSate GetAttackTypeState()
    {
        return _attackState;
    }
}
