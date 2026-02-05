using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class CameraDirector : BaseCamera
{
    #region Const
    private const float INPUT_THRESHOLD = 0.05f;
    private const float DELTA_TIME_SCALER = 100f;
    #endregion

    [Header("Target Following & Offset")]
    [SerializeField]
    private float _distance = 5.0f;
    
    [SerializeField]
    private float _height = 2.0f;

    [Header("Zoom Settings")]
    [SerializeField]
    private float _minDistance = 1.0f;
    
    [SerializeField]
    private float _maxDistance = 10.0f;
    
    [SerializeField]
    private float _zoomSpeed = 0.5f;

    [Header("Rotation & Input")]
    [SerializeField]
    private Vector2 _rotationSpeed = new Vector2(0.1f, 0.1f);

    [SerializeField]
    private float _minPitch = -30f;

    [SerializeField]
    private float _maxPitch = 60f;

    [Header("InteractionSeeting")]
    [SerializeField]
    private float _npcHeightOffset = 1.6f;    

    [SerializeField]
    private bool _useLocal = false;

    [SerializeField]
    private float _distanceToTarget = 3f;

    private Transform _npcTransform;
    private Transform _playerTransform;

    public float _zoomFOV = 45f;
    public float _zoomDuration = 1f;
    public float _targetDistance = 2f;

    private float _originalFOV;
    private Vector3 _originCameraLocalPosition;
    private Quaternion _originCameraLocalRotation;

    private UniTask _currentCameraTask;
    private bool _isAnimating = false;

    private float _horizontal = 0.0f;
    private float _vertical = 0.0f;

    #region UIBlocking
    private Finger _activeFinger;
    private float _dragStartThreshold = 12f;

    private Vector2 _startPos;

    private bool _touchStartOnUI = false;
    private bool _isRotating = false;
    #endregion

    #region MobileZoom
    private Vector2 _prevPos0;
    private Vector2 _prevPos1;
    private bool _hasPrev = false;
    #endregion

    public Vector3 GetForward() => Quaternion.Euler(0, _horizontal, 0) * Vector3.forward;
    public Vector3 GetRight() => Quaternion.Euler(0, _horizontal, 0) * Vector3.right;

    private IInteractionService _interactionService => PlayerManager.Instance.InteractionService;

    private void OnEnable()
    {
#if UNITY_ANDROID || UNITY_IOS
        EnhancedTouchSupport.Enable();
#endif
    }

    private void OnDisable()
    {
#if UNITY_ANDROID || UNITY_IOS
        EnhancedTouchSupport.Disable();
#endif
    }

    private void Start()
    {
        _interactionService.OnInteractionTargetChanged += HandleInteractionTargetChanged;
    }

    public override void SetTarget(PlayerController target)
    {
        base.SetTarget(target);

        enabled = target != null;

        _playerTransform = target.transform;

        //_horizontal = _target.transform.eulerAngles.y;
        //_vertical = 0f;
    }

    private void LateUpdate()
    {
        if (_target == null || _npcTransform != null || _isAnimating)
            return;

        UIBlocking();
        HandleZoom();
        HandleRotation();
        HandlePositionAndRotation();
    }

    private void OnDestroy()
    {
        _interactionService.OnInteractionTargetChanged -= HandleInteractionTargetChanged;
    }

    private void HandleInteractionTargetChanged(Transform target)
    {
        _npcTransform = target;
        if (target == null)
        {
            //.. 인터렉션 종료
            EndInteractionCamera().Forget();
        }
        else
        {
            //.. 인터렉션 시작
            StartInteractionCamera();
        }
    }

    private async UniTask EndInteractionCamera()
    {
        if (_currentCameraTask.Status == UniTaskStatus.Pending)
        {
            await _currentCameraTask;
        }

        ResetCameraPositionAsync().Forget();
    }

    private void StartInteractionCamera()
    {
        if (_camera == null || _npcTransform == null)
            return;

        _originalFOV = _camera.fieldOfView;        
        if(_useLocal)
        {
            _originCameraLocalPosition = _transform.localPosition;
            _originCameraLocalRotation = _transform.localRotation;
        }
        else
        {
            _originCameraLocalPosition = _transform.position;
            _originCameraLocalRotation = _transform.rotation;
        }

        _currentCameraTask = MoveAndZoomCameraAsync();
    }

    private async UniTask MoveAndZoomCameraAsync()
    {
        _isAnimating = true;

        Vector3 playerEyeLevelPoint = _playerTransform.position + Vector3.up * 1.6f;

        Vector3 npcTargetPoint = _npcTransform.position + Vector3.up * _npcHeightOffset;

        Quaternion targetRotation = Quaternion.LookRotation(npcTargetPoint - playerEyeLevelPoint);

        Vector3 targetPosition = playerEyeLevelPoint + (targetRotation * Vector3.forward) * _distanceToTarget;

        await UniTask.WhenAll(
            SmoothMoveAsync(_camera.transform, targetPosition, targetRotation, _zoomDuration),
            SmoothFOVAsync(_camera, _zoomFOV, _zoomDuration)
        );

        _isAnimating = false;
    }

    private async UniTaskVoid ResetCameraPositionAsync()
    {
        _isAnimating = true;

        await UniTask.WhenAll(
            SmoothMoveAsync(_transform, _originCameraLocalPosition, _originCameraLocalRotation, _zoomDuration),
            SmoothFOVAsync(_camera, _originalFOV, _zoomDuration)
        );

        _isAnimating = false;
    }

    private async UniTask SmoothMoveAsync(Transform targetTrans, Vector3 endPos, Quaternion endRot, float duration)
    {
        float timer = 0f;
        Vector3 startPos = _useLocal ? targetTrans.localPosition : targetTrans.position;
        Quaternion startRot = _useLocal ? targetTrans.localRotation : targetTrans.rotation;

        while (timer < duration)
        {
            float t = timer / duration;
            if (_useLocal)
            {
                targetTrans.localPosition = Vector3.Lerp(startPos, endPos, t);
                targetTrans.localRotation = Quaternion.Slerp(startRot, endRot, t);
            }
            else
            {
                targetTrans.position = Vector3.Lerp(startPos, endPos, t);
                targetTrans.rotation = Quaternion.Slerp(startRot, endRot, t);
            }

            timer += Time.deltaTime;
            await UniTask.Yield();
        }

        if (_useLocal)
        {
            targetTrans.localPosition = endPos;
            targetTrans.localRotation = endRot;
        }
        else
        {
            targetTrans.position = endPos;
            targetTrans.rotation = endRot;
        }
    }

    private async UniTask SmoothFOVAsync(Camera camera, float endFOV, float duration)
    {
        float timer = 0f;
        float startFOV = camera.fieldOfView;

        while (timer < duration)
        {
            float t = timer / duration;
            camera.fieldOfView = Mathf.Lerp(startFOV, endFOV, t);
            timer += Time.deltaTime;
            await UniTask.Yield();
        }

        camera.fieldOfView = endFOV;
    }


    private void HandleZoom()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (TryPinchToZoom(out float zoomInput))
        {
            ZoomProcess(zoomInput);
        }
#else
        var zoomInput = _target.GetInput().PlayerActions.Zoom.ReadValue<Vector2>().y;
        ZoomProcess(zoomInput);
#endif
    }

    private void ZoomProcess(float zoomInput)
    {
        if (Mathf.Approximately(zoomInput, 0f) == false)
        {
            _distance -= zoomInput * _zoomSpeed * Time.deltaTime * DELTA_TIME_SCALER;
            _distance = Mathf.Clamp(_distance, _minDistance, _maxDistance);
        }
    }

    private void HandleRotation()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (_touchStartOnUI || _isRotating == false)
            return;

        if (Touch.activeTouches.Count >= 2)
            return;
#endif

        var lookInput = _target.GetInput().PlayerActions.Look.ReadValue<Vector2>();
        if (lookInput.sqrMagnitude >= INPUT_THRESHOLD * INPUT_THRESHOLD)
        {
            var rotateValue = lookInput * _rotationSpeed * Time.deltaTime * DELTA_TIME_SCALER;

            _horizontal += rotateValue.x;
            _vertical -= rotateValue.y;
        }

        _vertical = Mathf.Clamp(_vertical, _minPitch, _maxPitch);
    }

    private void HandlePositionAndRotation()
    {
        Vector3 origin = _target.transform.position;
        Quaternion cameraRotation = Quaternion.Euler(_vertical, _horizontal, 0f);
        Vector3 offset = new Vector3(0, _height, -_distance);
        Vector3 desiredPosition = origin + (cameraRotation * offset);

        transform.position = desiredPosition;
        transform.rotation = cameraRotation;
    }

    private void UIBlocking()
    {        
#if UNITY_ANDROID || UNITY_IOS
        if (_activeFinger == null)
        {
            foreach (var touch in Touch.activeTouches)
            {
                if (touch.phase != UnityEngine.InputSystem.TouchPhase.Began)
                    continue;

                _activeFinger = touch.finger;
                _startPos = touch.screenPosition;
                _touchStartOnUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.finger.currentTouch.touchId);
                
                break;
            }
        }

        if (_activeFinger != null)
        {
            foreach (var touch in Touch.activeTouches)
            {
                if (touch.finger != _activeFinger)
                    continue;

                var currentPosition = touch.screenPosition;

                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended || touch.phase == UnityEngine.InputSystem.TouchPhase.Canceled)
                {
                    _touchStartOnUI = false;
                    _isRotating = false;
                    _activeFinger = null;                    
                }

                if (_isRotating == false)
                {
                    float dragSqr = (currentPosition - _startPos).sqrMagnitude;
                    if (dragSqr >= _dragStartThreshold * _dragStartThreshold)
                    {
                        _isRotating = true;
                    }                    
                }
            }
        }
#endif
    }

    private bool TryPinchToZoom(out float zoomInput)
    {
        zoomInput = 0f;

        if (Touch.activeTouches.Count < 2)
        {
            _hasPrev = false;
            return false;
        }

        var touch0 = Touch.activeTouches[0];
        var touch1 = Touch.activeTouches[1];

        if (touch0.phase == UnityEngine.InputSystem.TouchPhase.Ended ||
            touch1.phase == UnityEngine.InputSystem.TouchPhase.Ended ||
            touch0.phase == UnityEngine.InputSystem.TouchPhase.Canceled ||
            touch1.phase == UnityEngine.InputSystem.TouchPhase.Canceled)
            return false;

        
        float currentDistance = Vector2.Distance(touch0.screenPosition, touch1.screenPosition);
        float previousDistance = 0f;

        if (_hasPrev)
        {
            previousDistance = Vector2.Distance(_prevPos0, _prevPos1);
            zoomInput = currentDistance - previousDistance;
        }

        _prevPos0 = touch0.screenPosition;
        _prevPos1 = touch1.screenPosition;
        _hasPrev = true;
        
        return Mathf.Abs(zoomInput) > 0.01f;
    }
}