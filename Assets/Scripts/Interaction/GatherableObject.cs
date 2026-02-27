using UnityEngine;

public enum EGatherType
{
    None = 0,
    Food = 1,
    Resource = 2,
}

public class GatherableObject : InteractionObject, IRespawnable
{
    #region Inspector
    [SerializeField]
    private EGatherType _gatherType = EGatherType.None;
    public EGatherType GatherType => _gatherType;

    //.. FIXME :: 리워드 시스템 활용
    [SerializeField]
    private int _gatherAmount = 1;
    public int GatherAmount => _gatherAmount;

    [SerializeField]
    private float _respawnDelay = 10f;

    [SerializeField]
    private int _rewardIndex;
    #endregion

    private IRespawnManagerService _respawnService => RespawnManager.Instance;

    public float GetRespawnDelay() => _respawnDelay;

    private void Start()
    {
        OnRespawned();
    }

    public override void Interact()
    {
        _respawnService.ScheduleRespawn(this);

        RewardItemManager.Instance.RewardDropItem(_rewardIndex);
    }

    public virtual void OnDeactivated()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnRespawned()
    {
        gameObject.SetActive(true);
    }
}