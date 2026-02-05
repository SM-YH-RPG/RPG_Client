using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class WeakAttackState : GroundedAttackState, IAttackTypeSate
{
    public WeakAttackState()
    {
        Priority = (int)EStatePriority.Medium;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        var playerCtrl = _stateMachine?.PlayerCtrl;
        _comboConfigs = playerCtrl.CharacterData.AttackGroup.WeakAttack;
        _comboMax = _comboConfigs.Length;

        _nextAttackQueued = false;

        _currentAnimationHash = GetCurrentAnimationHashInternal();
        if (_currentAnimationHash != -1)
        {
            _animationCtrl.CrossFade(_currentAnimationHash);
        }
        else
        {
            _stateMachine.ChangeState(EPlayerStateType.Idle);
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        float time = _animationCtrl.GetCurrentAnimationTime();
        if (time > 0.5f)
        {
            Priority = (int)EStatePriority.Medium;
        }
        else
        {
            Priority = (int)EStatePriority.High;
        }
    }

    public override void OnExit()
    {
        _comboConfigs = null;
        base.OnExit();
    }

    protected override void AddInputListeners()
    {
        base.AddInputListeners();

        _inputActions.Attack.performed += OnAttackPerformed;
        _inputActions.Sprint.performed += OnSprintPerformed;
        _inputActions.StrongAttack.performed += OnAttackPerformed;
    }

    protected override void RemoveInputListeners()
    {
        base.RemoveInputListeners();

        _inputActions.Attack.performed -= OnAttackPerformed;
        _inputActions.StrongAttack.performed -= OnAttackPerformed;
        _inputActions.Sprint.performed -= OnSprintPerformed;
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (context.interaction is HoldInteraction)
        {
            Priority = (int)EStatePriority.Medium;
            _stateMachine.ChangeState(EPlayerStateType.StrongAttack);
        }
        else
        {
            if (IsAbleToQueueCombo())
            {
                _nextAttackQueued = true;
            }
        }
    }

    private void OnSprintPerformed(InputAction.CallbackContext context)
    {
        _stateMachine.ChangeState(EPlayerStateType.Dash);
    }

    public override float CalculateDamageRate()
    {
        if (_currentComboIndex >= 0 && _currentComboIndex < _comboMax)
        {
            if (_comboConfigs != null) // 강공격, 공격연타등 간헐적 null에러 발생 예외처리..(게임 진행에는 문제 X)
            {
                var comboConfig = _comboConfigs[_currentComboIndex];
                return comboConfig.DamageRate;
            }
        }

        return 1f;
    }
}