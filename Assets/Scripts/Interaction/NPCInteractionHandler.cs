
using UnityEngine;

public class NPCInteractionHandler : MonoBehaviour
{
    [SerializeField]
    private float _lookSpeed = 5f;

    [SerializeField]
    private float _rotationThreshold = 0.1f;

    [SerializeField]
    private bool _isSmoothRotation;

    private Transform _targetPlayerTransform;
    private Quaternion _targetRotation;
    private bool _isLookingAtPlayer = false;

    private void Update()
    {
        if (_isSmoothRotation == false || _targetPlayerTransform == null)
            return;

        if (_isLookingAtPlayer)
        {
            UpdateLookAtPlayer();
            CheckRotationComplete();
        }
    }

    public void StartLookingAtPlayer(Transform playerTarget)
    {
        _targetPlayerTransform = playerTarget;

        Quaternion newTargetRotation = CalculateTargetRotation();
        _targetRotation = newTargetRotation;

        if (_isSmoothRotation)
        {
            _isLookingAtPlayer = true;
        }
        else
        {
            transform.rotation = newTargetRotation;
        }
    }

    private void UpdateLookAtPlayer()
    {
        _targetRotation = CalculateTargetRotation();

        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime * _lookSpeed);
    }

    private Quaternion CalculateTargetRotation()
    {
        if (_targetPlayerTransform == null)
            return transform.rotation;

        Vector3 directionToPlayer = _targetPlayerTransform.position - transform.position;
        directionToPlayer.y = 0;

        if (directionToPlayer == Vector3.zero)
        {
            return transform.rotation;
        }

        return Quaternion.LookRotation(directionToPlayer);
    }

    private void CheckRotationComplete()
    {
        if (Quaternion.Angle(transform.rotation, _targetRotation) < _rotationThreshold)
        {
            transform.rotation = _targetRotation;
            _isLookingAtPlayer = false;
        }
    }
}