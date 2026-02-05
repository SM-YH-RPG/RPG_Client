using UnityEngine.InputSystem;

public class WalkState : GroundedState
{
    public WalkState()
    {
        Priority = (int)EStatePriority.Medium;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        UpdateMovementSpeed(_stateMachine.PlayerCtrl.CharacterData.GroundData.WalkSpeedModifier);
        _animationCtrl.CrossFade(_stateMachine.PlayerCtrl.CharacterData.AnimationKey.WalkHash);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    protected override void AddInputListeners()
    {
        base.AddInputListeners();

        _inputActions.Move.canceled += OnMoveCanceled;
        _inputActions.Sprint.performed += OnSprintPerformed;
    }

    protected override void RemoveInputListeners()
    {
        base.RemoveInputListeners();

        _inputActions.Move.canceled -= OnMoveCanceled;
        _inputActions.Sprint.performed -= OnSprintPerformed;
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
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