using UnityEngine;

public class FallingState : AirState
{
    public override void OnEnter()
    {
        base.OnEnter();

        var currentVelocity = _stateMachine.PlayerCtrl.CharacterVelocity;
        currentVelocity.y = -Physics.gravity.y;
        _stateMachine.PlayerCtrl.CharacterVelocity = currentVelocity;
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnUpdate()
    {
        if(_characterCtrl.isGrounded)
        {
            DetermineMovementState();
        }
    }

    protected override void AddInputListeners()
    {
        base.AddInputListeners();

        _inputActions.Jump.started += OnJumpStarted;
    }


    protected override void RemoveInputListeners()
    {
        base.RemoveInputListeners();

        _inputActions.Jump.started -= OnJumpStarted;
    }

    private void OnJumpStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        //.. TODO :: enough stamina -> Flying State
    }
}