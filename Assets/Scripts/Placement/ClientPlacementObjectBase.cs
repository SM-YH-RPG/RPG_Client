using UnityEngine;

public abstract class ClientPlacementObjectBase : MonoBehaviour
{
    public int _templateId;

    protected long _instanceId;

    [SerializeField] protected int _rewardIndex;

    public void SetActiveGameObject(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void SetInstanceId(long instanceId)
    {
        _instanceId = instanceId;
    }

    public long GetInstanceId()
    {
        return _instanceId;
    }
}
