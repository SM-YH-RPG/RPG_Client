using UnityEngine;

public class BossHeadAttackState : BaseEnemyAttackState
{
    private AttackConfig _currentTypeConfig;

    public BossHeadAttackState(BossEnemy enemy) : base(enemy)
    {
        _currentTypeConfig = enemy.GetBossAttackGroupConfig().HeadAttack;
    }

    public override float CalculateDamageRate()
    {
        return _currentTypeConfig.DamageRate;
    }
}
