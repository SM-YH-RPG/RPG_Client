using UnityEngine;

public class MonsterDamageCalculator : IDamageCalculator
{
    private BaseEnemy _enemy;

    public MonsterDamageCalculator(BaseEnemy enemy)
    {
        _enemy = enemy;
    }

    public int CalculateDamage()
    {
        float rate = _enemy.GetAttackTypeState().CalculateDamageRate();
        int damage = _enemy.GetEnemyDamage();
        damage = Mathf.RoundToInt(damage * rate);
        return damage;
    }

    public CombatData GetCombatData()
    {
        CombatData combat = new CombatData();
        combat.damage = CalculateDamage();
        combat.isCritical = false;

        return combat;
    }
}
