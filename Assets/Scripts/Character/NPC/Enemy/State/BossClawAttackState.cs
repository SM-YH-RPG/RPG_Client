using UnityEngine;

public class BossClawAttackState : BaseEnemyAttackState
{
    private AttackConfig _currentTypeConfig;

    public BossClawAttackState(BossEnemy enemy) : base(enemy)
    {
        _currentTypeConfig = enemy.GetBossAttackGroupConfig().ClawAttack;
    }

    public override float CalculateDamageRate()
    {
        return _currentTypeConfig.DamageRate;
    }
}
