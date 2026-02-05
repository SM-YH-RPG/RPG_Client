using UnityEngine;
using UnityEngine.InputSystem;

public class SpecialSkillState : GroundedSkillState, IAttackTypeSate
{
    public override void OnEnter()
    {
        base.OnEnter();

        _currentAnimationHash = _stateMachine.PlayerCtrl.CharacterData.SkillGroup.SpecialSkill.AnimationStateHash;

        var playerCtrl = _stateMachine?.PlayerCtrl;
        _stateMachine.PlayerCtrl.CombatCtrl.DisableAllHitboxes();
        _animationCtrl.CrossFade(_currentAnimationHash);

        UseSkill(ESkillType.SpecialSkill);
    }

    public override void OnExit()
    {
        base.OnExit();
        //_stateMachine.PlayerCtrl.CombatCtrl.DisableAllHitboxes();
    }

    public override float CalculateDamageRate()
    {
        var skillCtrl = _stateMachine.PlayerCtrl.SkillCtrl;
        float damageRate = skillCtrl.GetConfigDamageRate(ESkillType.SpecialSkill);
        return damageRate;
    }
}