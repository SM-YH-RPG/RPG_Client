using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BaseCamera : MonoBehaviour
{
    protected Camera _camera;
    protected Transform _transform;

    [SerializeField]
    protected PlayerController _target;

    protected virtual void Awake()
    {
        TryGetComponent(out _camera);
        TryGetComponent(out _transform);
    }

    public virtual void SetTarget(PlayerController target)
    {
        _target = target;
    }
}