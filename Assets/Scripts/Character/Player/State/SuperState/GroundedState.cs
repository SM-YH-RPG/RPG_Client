using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class GroundedState : LocoMotionState
{
    private bool _isJumpButtonPressed = false;

    public override void OnUpdate()
    {
        base.OnUpdate();

        if(_stateMachine.PlayerCtrl.IsGrounded == false)
        {
            //_stateMachine.ChangeState(EPlayerStateType.Falling);
        }
    }

    public override void OnInput()
    {
        base.OnInput();

        if (_isJumpButtonPressed)
        {
            if (_stateMachine.GetCurrentState() is RunState)
            {
                return;
            }

            _isJumpButtonPressed = false;
            _stateMachine.ChangeState(EPlayerStateType.Jump);
        }
    }

    protected override void AddInputListeners()
    {
        base.AddInputListeners();

        _inputActions.Jump.started += OnJumpStarted;
        _inputActions.Jump.canceled += OnJumpCanceled;
        _inputActions.Move.performed += OnMovePerformed;
        _inputActions.Attack.performed += OnAttackPerformed;
        _inputActions.StrongAttack.performed += OnAttackPerformed;
        _inputActions.Skill.performed += OnSkillPerformed;
        _inputActions.UltimateSkill.performed += OnUltimateSkillPerformed;
    }

    protected override void RemoveInputListeners()
    {
        base.RemoveInputListeners();

        _inputActions.Jump.started -= OnJumpStarted;
        _inputActions.Jump.canceled -= OnJumpCanceled;
        _inputActions.Move.performed -= OnMovePerformed;
        _inputActions.Attack.performed -= OnAttackPerformed;
        _inputActions.StrongAttack.performed -= OnAttackPerformed;
        _inputActions.Skill.performed -= OnSkillPerformed;
        _inputActions.UltimateSkill.performed -= OnUltimateSkillPerformed;
    }

    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        _isJumpButtonPressed = true;
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        _isJumpButtonPressed = false;
    }


    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        if (_stateMachine.IsWalkingMode)
        {
            if(_stateMachine.GetCurrentState() is RunState)
            {
                return;
            }

            _stateMachine.ChangeState(EPlayerStateType.Walk);
        }
        else
        {
            _stateMachine.ChangeState(EPlayerStateType.Run);
        }   
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (context.interaction is HoldInteraction)
        {
            _stateMachine.ChangeState(EPlayerStateType.StrongAttack);
        }
        else
        {
#if UNITY_ANDROID || UNITY_IOS
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
#endif
            _stateMachine.ChangeState(EPlayerStateType.WeakAttack);
        }
    }

    /// <summary>
    /// E스킬 발동 함수
    /// </summary>
    /// <param name="context"></param>
    private void OnSkillPerformed(InputAction.CallbackContext context)
    {
        if (_stateMachine.PlayerCtrl.SkillCtrl.IsPossibleUseSkill(ESkillType.Skill)) // 없으면 쿨타임은 도는데 스킬이 계속 나간다..
        {
            _stateMachine.ChangeState(EPlayerStateType.GroundedSkill);
        }
    }

    /// <summary>
    /// R스킬 발동 함수
    /// </summary>
    /// <param name="context"></param>
    private void OnUltimateSkillPerformed(InputAction.CallbackContext context)
    {
        if (_stateMachine.PlayerCtrl.SkillCtrl.IsPossibleUseSkill(ESkillType.UltimateSkill)) // 없으면 쿨타임은 도는데 스킬이 계속 나간다..
        {
            _stateMachine.ChangeState(EPlayerStateType.UltimateSkill);
        }
    }
}