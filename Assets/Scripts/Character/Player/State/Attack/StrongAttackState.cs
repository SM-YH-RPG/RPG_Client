using UnityEngine.InputSystem;

public class StrongAttackState : GroundedAttackState, IAttackTypeSate
{    
    public StrongAttackState()
    {
        Priority = (int)EStatePriority.Medium;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        var playerCtrl = _stateMachine?.PlayerCtrl;
        _comboConfigs = playerCtrl.CharacterData.AttackGroup.StrongAttack;
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
        Priority = (int)EStatePriority.High;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        bool isFinished = _animationCtrl.IsAnimationFinished(_currentAnimationHash);
        if (isFinished)
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

        if (_comboMax > 1)
        {
            _inputActions.Attack.performed += OnAttackPerformed;
        }
    }

    protected override void RemoveInputListeners()
    {
        base.RemoveInputListeners();

        if (_comboMax > 1)
        {
            _inputActions.Attack.performed -= OnAttackPerformed;
        }
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (IsAbleToQueueCombo())
        {
            _nextAttackQueued = true;
        }
    }

    public override float CalculateDamageRate()
    {
        if (_currentComboIndex >= 0 && _currentComboIndex < _comboMax)
        {
            var comboConfig = _comboConfigs[_currentComboIndex];
            return comboConfig.DamageRate;
        }

        return 1f;
    }
}