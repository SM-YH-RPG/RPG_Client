using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact();
    GameObject GetGameObject();
    string InteractionTitle { get; }
}

public class InteractionDetector : MonoBehaviour
{
    #region Const
    private const float DETECTION_RATE = 0.2f;
    private const int MAX_COLLIDERS = 4;
    #endregion

    #region Inspector
    [SerializeField]
    private LayerMask _interactableLayerMask;

    [SerializeField]
    private float _distance = 3f;

    [SerializeField]
    private float _angle = 45f;

    [SerializeField]
    private bool _drawDebugGizmos = true;
    #endregion

    private Collider[] _hitColliders = new Collider[MAX_COLLIDERS];

    private List<IInteractable> _detectedInteractables = new List<IInteractable>();
    private List<IInteractable> _lastDetectedTargetsCache = new List<IInteractable>();

    public Action<List<IInteractable>> OnTargetsDetected;

    private Transform _detector;

    private float _detectionTimer;

    private IUIManagerService _UIManagerService => UIManager.Instance;

    private class DistanceComparer : IComparer<IInteractable>
    {
        private readonly Vector3 _sourcePosition;

        public DistanceComparer(Vector3 sourcePosition)
        {
            _sourcePosition = sourcePosition;
        }

        public int Compare(IInteractable x, IInteractable y)
        {
            if (x == null || y == null)
                return 0;

            float distanceSqX = (x.GetGameObject().transform.position - _sourcePosition).sqrMagnitude;
            float distanceSqY = (y.GetGameObject().transform.position - _sourcePosition).sqrMagnitude;

            return distanceSqX.CompareTo(distanceSqY);
        }
    }

    private void Awake()
    {
        _detector = transform;
        _UIManagerService.OnShowPopup += HandleUIManagerShowPopup;
        _UIManagerService.OnHidePopup += HandleUIManagerHidePopup;
    }

    private void OnDestroy()
    {
        _UIManagerService.OnShowPopup -= HandleUIManagerShowPopup;
        _UIManagerService.OnHidePopup -= HandleUIManagerHidePopup;
    }

    private void Update()
    {
        _detectionTimer -= Time.deltaTime;
        if (_detectionTimer <= 0f)
        {
            _detectionTimer = DETECTION_RATE;
            DetectAndFilterInteractables();
            List<IInteractable> sortedList = SortPriority();
            if (ListsAreEqual(sortedList, _lastDetectedTargetsCache) == false)
            {
                _lastDetectedTargetsCache = sortedList;
                OnTargetsDetected?.Invoke(sortedList);
            }
        }
    }

    private void HandleUIManagerShowPopup()
    {
        _lastDetectedTargetsCache.Clear();
        OnTargetsDetected?.Invoke(_lastDetectedTargetsCache);
        enabled = false;
    }

    private void HandleUIManagerHidePopup()
    {
        enabled = true;
    }

    private void DetectAndFilterInteractables()
    {
        _detectedInteractables.Clear();
        int numColliders = Physics.OverlapSphereNonAlloc(_detector.position, _distance, _hitColliders, _interactableLayerMask);
        for (int i = 0; i < numColliders; i++)
        {
            Collider hitCollider = _hitColliders[i];

            if (hitCollider.TryGetComponent(out IInteractable interactable))
            {
                GameObject interactableObject = interactable.GetGameObject();

                Vector3 directionToTarget = (interactableObject.transform.position - _detector.position).normalized;
                float angleToTarget = Vector3.Angle(_detector.forward, directionToTarget);

                if (angleToTarget <= _angle)
                {
                    _detectedInteractables.Add(interactable);
                }
            }
        }

        Array.Clear(_hitColliders, 0, numColliders);
    }

    private List<IInteractable> SortPriority()
    {
        if (_detectedInteractables.Count == 0)
        {
            return new List<IInteractable>();
        }

        var sortedInteractablesCache = new List<IInteractable>(_detectedInteractables);
        var comparer = new DistanceComparer(_detector.position);
        sortedInteractablesCache.Sort(comparer);

        return sortedInteractablesCache;
    }

    private bool ListsAreEqual(List<IInteractable> list1, List<IInteractable> list2)
    {
        if (list1 == list2) return true;
        if (list1 == null || list2 == null) return false;
        if (list1.Count != list2.Count) return false;

        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i] != list2[i])
            {
                return false;
            }
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        if (_drawDebugGizmos == false ||
            _detector == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_detector.position, _distance);

        Gizmos.color = Color.cyan;
        Vector3 forward = _detector.forward;
        Vector3 leftDir = Quaternion.Euler(0, -_angle, 0) * forward;
        Vector3 rightDir = Quaternion.Euler(0, _angle, 0) * forward;

        Gizmos.DrawRay(_detector.position, leftDir * _distance);
        Gizmos.DrawRay(_detector.position, rightDir * _distance);
        Gizmos.DrawRay(_detector.position, forward * _distance);
    }
}
