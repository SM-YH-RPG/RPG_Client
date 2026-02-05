using UnityEngine;
using UnityEngine.InputSystem;

public enum EStatePriority : int
{
    Low = 0,
    Medium = 50,
    High = 100
}

public class BaseState : IState
{
    protected float MIN_ANIMATION_READ_TIME = 0.1f;    

    protected PlayerStateMachine _stateMachine = null;
    protected CharacterController _characterCtrl = null;
    protected AnimationController _animationCtrl = null;
    protected InteractionDetector _interactionCtrl = null;
    protected Transform _transform;

    protected float _enteredTime;

    protected InputActionSystem.PlayerActions _inputActions;

    public virtual int Priority { get; protected set; } = (int)EStatePriority.Low;

    public virtual void Initialize(PlayerStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
        _characterCtrl = _stateMachine.PlayerCtrl.CharacterCtrl;
        _animationCtrl = _stateMachine.PlayerCtrl.GetAnimator();
        _interactionCtrl = _stateMachine.PlayerCtrl.InteractionDetector;
        _inputActions = _stateMachine.PlayerCtrl.GetInput().PlayerActions;

        _transform = _characterCtrl.transform;
    }

    public virtual void OnEnter()
    {
#if UNITY_EDITOR
        Debug.Log($"ENTER STATE : {GetType().Name}");
#endif
        _enteredTime = Time.time;

        AddInputListeners();
    }

    public virtual void OnExit()
    {
#if UNITY_EDITOR
        Debug.Log($"EXIT STATE : {GetType().Name}");
#endif
        RemoveInputListeners();
    }
    
    public virtual void OnUpdate() { }
    public virtual void OnPhysicsUpdate() { }
    public virtual void OnInput() { }

    protected virtual void AddInputListeners()
    {
        _inputActions.MoveChange.started += OnMoveChangeStarted;
    }


    protected virtual void RemoveInputListeners()
    {
        _inputActions.MoveChange.started -= OnMoveChangeStarted;
    }

    private void OnMoveChangeStarted(InputAction.CallbackContext context)
    {
        _stateMachine.IsWalkingMode = !_stateMachine.IsWalkingMode;
        if (_stateMachine.GetCurrentState() is GroundedState && !(_stateMachine.GetCurrentState() is IdleState))
        {
            if (_stateMachine.IsWalkingMode)
            {
                _stateMachine.ChangeState(EPlayerStateType.Walk);
            }
            else
            {
                _stateMachine.ChangeState(EPlayerStateType.Run);
            }
        }
    }

    protected virtual void DetermineMovementState()
    {
        var playerCtrl = _stateMachine.PlayerCtrl;
        var actions = playerCtrl.GetInput().PlayerActions;

        float moveInputMagnitudeSq = actions.Move.ReadValue<Vector2>().sqrMagnitude;
        const float MoveInputThreshold = 0.01f;

        if (moveInputMagnitudeSq > MoveInputThreshold * MoveInputThreshold)
        {
            EPlayerStateType nextState = _stateMachine.IsWalkingMode ? EPlayerStateType.Walk: EPlayerStateType.Run;
            _stateMachine.ChangeState(nextState);
        }
        else
        {
            _stateMachine.ChangeState(EPlayerStateType.Idle);
        }
    }
}