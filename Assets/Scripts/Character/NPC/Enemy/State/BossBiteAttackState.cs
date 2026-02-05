using UnityEngine;

public class BossBiteAttackState : BaseEnemyAttackState
{
    private AttackConfig _currentTypeConfig;

    public BossBiteAttackState(BossEnemy enemy) : base(enemy) 
    {
        _currentTypeConfig = enemy.GetBossAttackGroupConfig().BiteAttack;
    }

    public override float CalculateDamageRate()
    {
        return _currentTypeConfig.DamageRate;
    }
}
