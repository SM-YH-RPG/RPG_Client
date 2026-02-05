using System;
using System.Collections.Generic;
using UnityEngine;

public enum EPlayerStateType
{
    Idle,
    Walk,
    Run,
    Dash,
    Jump,
    Falling,
    WeakAttack,
    StrongAttack,
    GroundedSkill,
    AirSkill,
    SpecialSkill,
    UltimateSkill,
    MidAirAttack,
    FallingAttack,
    Hit,
    Airborne,
    Death
}

public class PlayerStateMachine
{
    public PlayerController PlayerCtrl { get; private set; }

    private IState _currentState;
    private readonly Dictionary<EPlayerStateType, IState> _stateDict = new Dictionary<EPlayerStateType, IState>();

    public bool IsWalkingMode;    

    public PlayerStateMachine(PlayerController controller)
    {
        PlayerCtrl = controller;

        RegisterState<IdleState>(EPlayerStateType.Idle);
        RegisterState<WalkState>(EPlayerStateType.Walk);
        RegisterState<RunState>(EPlayerStateType.Run);
        RegisterState<DashState>(EPlayerStateType.Dash);
        RegisterState<JumpState>(EPlayerStateType.Jump);
        RegisterState<FallingState>(EPlayerStateType.Falling);

        RegisterState<WeakAttackState>(EPlayerStateType.WeakAttack);
        RegisterState<StrongAttackState>(EPlayerStateType.StrongAttack);

        RegisterState<GroundedSkillState>(EPlayerStateType.GroundedSkill);
        RegisterState<AirSkillState>(EPlayerStateType.AirSkill);
        RegisterState<SpecialSkillState>(EPlayerStateType.SpecialSkill);
        RegisterState<UltimateSkillState>(EPlayerStateType.UltimateSkill);

        RegisterState<MidAirAttackState>(EPlayerStateType.MidAirAttack);
        RegisterState<FallingAttackState>(EPlayerStateType.FallingAttack);

        RegisterState<NormalHitState>(EPlayerStateType.Hit);
        RegisterState<AirborneState>(EPlayerStateType.Airborne);
        RegisterState<DeadState>(EPlayerStateType.Death);
    }

    public void Update()
    {
        _currentState?.OnUpdate();        
    }

    public void PhysicsUpdate()
    {
        _currentState?.OnPhysicsUpdate();
    }

    public void Input()
    {
        _currentState?.OnInput();
    }

    public void ChangeState(EPlayerStateType stateType)
    {
        var nextState = GetState(stateType);
        if (_currentState == nextState)
            return;

        if (_currentState != null && _currentState.Priority > nextState.Priority)
            return;

        _currentState?.OnExit();
        _currentState = nextState;
        _currentState?.OnEnter();
    }

    public IState GetCurrentState()
    {
        return _currentState;
    }

    private IState GetState(EPlayerStateType stateType)
    {
        if(_stateDict.TryGetValue(stateType, out IState state))
        {
            return state;
        }

        return null;
    }

    private void RegisterState<T>(EPlayerStateType stateType) where T : IState, new()
    {
        if(_stateDict.ContainsKey(stateType))
            return;

        T state = new T();
        state.Initialize(this);

        _stateDict.Add(stateType, state);
    }
}
