
using UnityEngine;

public class WorldToScreenUIFollower : MonoBehaviour
{
    private Camera _mainCamera;

    public Transform TargetWorldTransform { get; set; }

    public Vector3 WorldOffset = Vector3.zero;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _mainCamera = Camera.main;

        TryGetComponent(out _rectTransform);
    }

    private void Update()
    {
        if (TargetWorldTransform != null && _mainCamera != null)
        {
            Vector3 worldPosition = TargetWorldTransform.position + WorldOffset;
            Vector3 screenPosition = _mainCamera.WorldToScreenPoint(worldPosition);
            _rectTransform.position = screenPosition;
        }
    }
}