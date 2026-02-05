using UnityEngine.InputSystem;
using UnityEngine;

public class JumpState : AirState
{
    public override int Priority => (int)EStatePriority.Medium;

    private float _minAirAttackHeight = 5f;

    private float _jumpForce;

    private LayerMask _groundLayer = LayerMask.NameToLayer("Ground");

    public override void OnEnter()
    {
        base.OnEnter();

        _jumpForce = _stateMachine.PlayerCtrl.CharacterData.AirData.JumpForce;

        Vector3 currentVelocity = _stateMachine.PlayerCtrl.CharacterVelocity;
        currentVelocity.y = _jumpForce;
        _stateMachine.PlayerCtrl.CharacterVelocity = currentVelocity;
    }

    public override void OnExit()
    {
        base.OnExit();
    }


    public override void OnUpdate()
    {
        base.OnUpdate();

        if (Time.time - _enteredTime < MIN_ANIMATION_READ_TIME)
            return;

        if (_stateMachine.PlayerCtrl.IsGrounded)
        {
            DetermineMovementState();
        }
    }

    public override void OnInput()
    {
        base.OnInput();
    }

    protected override void AddInputListeners()
    {
        base.AddInputListeners();

        _inputActions.Attack.performed += OnAttackPerformed;
    }

    protected override void RemoveInputListeners()
    {
        base.RemoveInputListeners();

        _inputActions.Attack.performed -= OnAttackPerformed;
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if(IsAirAttackable())
        {
            _stateMachine.ChangeState(EPlayerStateType.MidAirAttack);
        }
        else
        {
            _stateMachine.ChangeState(EPlayerStateType.FallingAttack);
        }
    }

    private bool IsAirAttackable()
    {
        float distanceToGround = CheckGroundDistance();
        return distanceToGround > _minAirAttackHeight;
    }

    private float CheckGroundDistance()
    {
        Vector3 raycastOrigin = _stateMachine.PlayerCtrl.transform.position + Vector3.up;

        Ray ray = new Ray(raycastOrigin, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _groundLayer))
        {
            return hit.distance;
        }

        return float.MaxValue;
    }
}