

public class FallingAttackState : AirAttackState
{
    public override void OnEnter()
    {
        base.OnEnter();

        _stateMachine.PlayerCtrl.GetInput().enabled = false;
        _animationCtrl.CrossFade(_stateMachine.PlayerCtrl.CharacterData.AttackGroup.FallingAttack.AnimationStateHash);
    }

    public override void OnExit()
    {
        base.OnExit();

        _stateMachine.PlayerCtrl.GetInput().enabled = true;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}