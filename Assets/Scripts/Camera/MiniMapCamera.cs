using UnityEngine;

public class MiniMapCamera : BaseCamera
{
    [Header("Offset")]
    [SerializeField] private float _height = 15.0f;

    public override void SetTarget(PlayerController target)
    {
        base.SetTarget(target);

        enabled = target != null;
    }

    private void LateUpdate()
    {
        HandlePosition();
    }

    private void HandlePosition()
    {
        Vector3 origin = _target.transform.position;
        Vector3 offset = new Vector3(0f, _height, 0f);
        Vector3 desiredPosition = origin + offset;

        transform.position = desiredPosition;
    }
}
