using UnityEngine;

public class PlayerDamageCalculator : IDamageCalculator
{
    private PlayerController _playerCtrl;
    private bool _isCritical;

    public PlayerDamageCalculator(PlayerController playerCtrl)
    {
        _playerCtrl = playerCtrl;
    }    

    public int CalculateDamage()
    {
        _isCritical = false;
        float rate = _playerCtrl.GetAttackTypeState().CalculateDamageRate();
        int finalDamage = _playerCtrl.CurrentCharacter.Damage;
        if (CheckDamageCritical())
        {
            _isCritical = true;
            finalDamage += Mathf.RoundToInt(finalDamage * (_playerCtrl.CurrentCharacter.CriticalDamageRate * 0.01f));
        }
        finalDamage = Mathf.RoundToInt(finalDamage * rate);
        return finalDamage;
    }

    private bool CheckDamageCritical()
    {
        float ciriticalRate = _playerCtrl.CurrentCharacter.CriticalPercent;
        float RandomCritical = Random.Range(0f, 100f);
        return RandomCritical <= ciriticalRate;
    }

    public CombatData GetCombatData()
    {
        CombatData combat = new CombatData();
        combat.damage = CalculateDamage();
        combat.isCritical = _isCritical;

        return combat;
    }    
}
