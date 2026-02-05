using UnityEngine;
using UnityEngine.InputSystem;

public class UltimateSkillState : GroundedAttackState, IAttackTypeSate
{
    public UltimateSkillState()
    {
        Priority = (int)EStatePriority.Medium;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _currentAnimationHash = _stateMachine.PlayerCtrl.CharacterData.SkillGroup.UltimateSkill.AnimationStateHash;

        var playerCtrl = _stateMachine?.PlayerCtrl;
        _stateMachine.PlayerCtrl.CombatCtrl.DisableAllHitboxes();
        _animationCtrl.CrossFade(_currentAnimationHash);
        Priority = (int)EStatePriority.High;
        UseSkill(ESkillType.UltimateSkill);
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
        base.OnExit();
    }

    public override float CalculateDamageRate()
    {
        var skillCtrl = _stateMachine.PlayerCtrl.SkillCtrl;
        float damageRate = skillCtrl.GetConfigDamageRate(ESkillType.UltimateSkill);
        return damageRate;
    }
}