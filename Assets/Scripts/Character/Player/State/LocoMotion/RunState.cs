using UnityEngine.InputSystem;

public class RunState : GroundedState
{
    public RunState()
    {
        Priority = (int)EStatePriority.Medium;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (_stateMachine.PlayerCtrl.CheckRunStaminaCostAndReduction())
        {
            Priority = (int)EStatePriority.Medium;
            UpdateMovementSpeed(_stateMachine.PlayerCtrl.CharacterData.GroundData.RunSpeedModifier);

            _animationCtrl.CrossFade(_stateMachine.PlayerCtrl.CharacterData.AnimationKey.RunHash);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        _stateMachine.PlayerCtrl.CheckRunStaminaCostAndReduction();
    }

    protected override void AddInputListeners()
    {
        base.AddInputListeners();

        _inputActions.Move.canceled += OnMoveCanceled;
        _inputActions.Sprint.started += OnSprintStarted;
    }

    protected override void RemoveInputListeners()
    {
        base.RemoveInputListeners();

        _inputActions.Move.canceled -= OnMoveCanceled;
        _inputActions.Sprint.started -= OnSprintStarted;
    }

    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        Priority = (int)EStatePriority.High;
        _stateMachine.ChangeState(EPlayerStateType.Dash);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        Priority = (int)EStatePriority.Medium;
        _stateMachine.ChangeState(EPlayerStateType.Idle);
    }
}