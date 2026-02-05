using UnityEngine;
using UnityEngine.InputSystem;

public class LocoMotionState : BaseState
{
    protected const float MinDesiredThreshold = 0.01f;

    protected CameraDirector _camera;
    private PlayerController _playerCtrl;

    protected float _maxSlopeAngle = 45f;
    protected float _slopeSlideSpeed = 8f;

    private float _baseSpeed;
    protected float _currentMovementSpeed;
    protected Vector3 _moveDirection;

    public override void Initialize(PlayerStateMachine stateMachine)
    {
        base.Initialize(stateMachine);

        _playerCtrl = stateMachine.PlayerCtrl;
        _baseSpeed = _playerCtrl.CharacterData.GroundData.BaseSpeed;
        _camera = _playerCtrl.Camera;
    }

    public override void OnUpdate()
    {
        if (_camera == null)
            return;

        base.OnUpdate();

        CalculateMovementVector();
    }

    public override void OnInput()
    {
#if UNITY_ANDROID || UNITY_IOS
        ReadMoveInputOnJoyPad(_playerCtrl.MoveDirection);
#else
        if (_inputActions.Move.IsPressed())
        {
            ReadMoveInput();
        }
#endif
    }

    public override void OnPhysicsUpdate()
    {
        base.OnPhysicsUpdate();
    }

    protected override void AddInputListeners()
    {
        base.AddInputListeners();

        _inputActions.Move.canceled += OnMoveCanceled;
    }

    protected override void RemoveInputListeners()
    {
        base.RemoveInputListeners();

        _inputActions.Move.canceled -= OnMoveCanceled;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveDirection = Vector3.zero;
    }

    private void ReadMoveInput()
    {
        var inputVector2 = _inputActions.Move.ReadValue<Vector2>();
        _moveDirection = new Vector3(inputVector2.x, 0, inputVector2.y);
    }

    private void ReadMoveInputOnJoyPad(Vector3 inputVector)
    {
        _moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
    }

    protected void UpdateMovementSpeed(float speed)
    {
        _currentMovementSpeed = speed;
    }

    private void CalculateMovementVector()
    {
        float inputMagnitude = _moveDirection.sqrMagnitude;
        Vector3 finalMovementXZ = Vector3.zero;

        if (inputMagnitude >= MinDesiredThreshold)
        {
            Vector3 forward = _camera.GetForward();
            Vector3 right = _camera.GetRight();

            Vector3 desiredDirection = (forward * _moveDirection.z + right * _moveDirection.x).normalized;

            finalMovementXZ = desiredDirection * _currentMovementSpeed * _baseSpeed;

            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
            Quaternion smoothRotation = Quaternion.Slerp(
                _transform.rotation,
                targetRotation,
                Time.deltaTime * _stateMachine.PlayerCtrl.CharacterData.GroundData.BaseRotationDamping);

            _transform.rotation = smoothRotation;
        }

        Vector3 currentVelocity = _stateMachine.PlayerCtrl.CharacterVelocity;
        currentVelocity.x = finalMovementXZ.x;
        currentVelocity.z = finalMovementXZ.z;
        _stateMachine.PlayerCtrl.CharacterVelocity = currentVelocity;
    }

    protected void SetDirection(Vector3 _direction)
    {
        _moveDirection = _direction;
    }
}