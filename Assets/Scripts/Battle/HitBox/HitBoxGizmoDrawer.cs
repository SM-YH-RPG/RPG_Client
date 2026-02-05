using UnityEngine;

public class HitBoxGizmoDrawer : MonoBehaviour
{
    private HitShapeData _lastHitData;
    private Vector3 _lastWorldPosition;
    private Quaternion _lastWorldRotation;
    private HitShapeType _activeShapeType = HitShapeType.None;

    public void SetGizmoData(HitShapeData data, Transform ownerTransform)
    {
        _lastHitData = data;
        _activeShapeType = data.Type;

        _lastWorldPosition = ownerTransform.TransformPoint(data.Offset);
        _lastWorldRotation = ownerTransform.rotation * data.Rotation;
    }

    public void ClearGizmos()
    {
        _activeShapeType = HitShapeType.None;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        switch (_activeShapeType)
        {
            case HitShapeType.Sphere:
                Gizmos.DrawWireSphere(_lastWorldPosition, _lastHitData.Radius);
                break;

            case HitShapeType.Box:
                Quaternion validRotation = (Mathf.Abs(_lastWorldRotation.w) > float.Epsilon) ? _lastWorldRotation : Quaternion.identity;

                Matrix4x4 originalMatrix = Gizmos.matrix;
                Gizmos.matrix = Matrix4x4.TRS(_lastWorldPosition, validRotation, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, _lastHitData.HalfExtend * 2f);
                Gizmos.matrix = originalMatrix;
                break;

            case HitShapeType.Sector:
                Gizmos.DrawWireSphere(_lastWorldPosition, _lastHitData.Radius);
                Vector3 forward = transform.forward;
                float halfAngle = _lastHitData.Angle / 2f;

                Vector3 leftDir = Quaternion.Euler(0, -halfAngle, 0) * forward * _lastHitData.Radius;
                Vector3 rightDir = Quaternion.Euler(0, halfAngle, 0) * forward * _lastHitData.Radius;

                Gizmos.DrawRay(_lastWorldPosition, leftDir);
                Gizmos.DrawRay(_lastWorldPosition, rightDir);
                Gizmos.DrawRay(_lastWorldPosition, forward * _lastHitData.Radius);
                break;

            case HitShapeType.None:

                break;
        }
    }
}