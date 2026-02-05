using UnityEngine;

public class BaseEnemyAttackState : IAttackTypeSate
{
    private BaseEnemy _enemy;

    public BaseEnemyAttackState(BaseEnemy enemy)
    {
        _enemy = enemy;
    }

    public virtual float CalculateDamageRate()
    {
        return _enemy.GetDamageRate();
    }
}
