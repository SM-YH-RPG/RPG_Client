using UnityEngine;

public abstract class ClientPlacementObjectBase : MonoBehaviour
{
    public int _templateId; // WorldInfo Object Prefab Template Index

    protected long _instanceId;    

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
