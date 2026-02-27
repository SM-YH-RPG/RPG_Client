using UnityEngine;

public class DestoryableObject : ClientPlacementObjectBase, IAttackTarget, IRespawnable
{
    #region Inspector
    [SerializeField]
    private string _name;

    [SerializeField]
    private float _respawnDelay = 10f;

    [SerializeField]
    private int _rewardIndex;
    #endregion

    //.. FIXME :: Config에서 받아와야함
    [SerializeField]
    private int _maxHP = 10;
    
    private int _currentHP = 10;

    private IRespawnManagerService _respawnService => RespawnManager.Instance;

    public float GetRespawnDelay() => _respawnDelay;

    private void Start()
    {
        _currentHP = _maxHP;
        OnRespawned();
    }

    public void OnDamaged(CombatData combatData)
    {
        _currentHP -= combatData.damage;
        if (_currentHP <= 0f)
        {
            DestroyObject();
        }
    }

    private void DestroyObject()
    {
        _respawnService.ScheduleRespawn(this);
        //.. TODO : 파괴 이펙트 재생
        
        RewardItemManager.Instance.RewardDropItem(_rewardIndex);
    }

    public virtual void OnDeactivated()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnRespawned()
    {
        gameObject.SetActive(true);
        _currentHP = _maxHP;
    }
}