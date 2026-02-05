using UnityEngine;

public class DashState : LocoMotionState
{    
    public DashState()
    {
        Priority = (int)EStatePriority.High;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (_stateMachine.PlayerCtrl.CheckDashStaminaCostAndReduction())
        {
            UpdateMovementSpeed(_stateMachine.PlayerCtrl.CharacterData.GroundData.DashSpeedModifier);

            _stateMachine.PlayerCtrl.SetInvulnerability(true);
            _animationCtrl.CrossFade(_stateMachine.PlayerCtrl.CharacterData.AnimationKey.DashHash);
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        Priority = (int)EStatePriority.High;
        _stateMachine.PlayerCtrl.SetInvulnerability(false);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (Time.time - _enteredTime < MIN_ANIMATION_READ_TIME)
            return;

        bool isFinished = _animationCtrl.IsAnimationFinished(_stateMachine.PlayerCtrl.CharacterData.AnimationKey.DashHash);
        if (isFinished)
        {
            DetermineMovementState();
        }
    }

    public override void OnPhysicsUpdate()
    {
        base.OnPhysicsUpdate();
    }

    protected override void AddInputListeners()
    {
        base.AddInputListeners();
    }

    protected override void RemoveInputListeners()
    {
        base.RemoveInputListeners();
    }

    protected override void DetermineMovementState()
    {
        var playerCtrl = _stateMachine.PlayerCtrl;
        var actions = playerCtrl.GetInput().PlayerActions;

        float moveInputMagnitudeSq = actions.Move.ReadValue<Vector2>().sqrMagnitude;
        const float MoveInputThreshold = 0.01f;

        if (moveInputMagnitudeSq > MoveInputThreshold * MoveInputThreshold)
        {
            Priority = (int)EStatePriority.Medium;
            _stateMachine.ChangeState(EPlayerStateType.Run);
        }
        else
        {
            Priority = (int)EStatePriority.Low;
            _stateMachine.ChangeState(EPlayerStateType.Idle);
        }
    }
}