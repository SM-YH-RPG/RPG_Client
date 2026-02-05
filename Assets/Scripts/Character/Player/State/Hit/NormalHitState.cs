public class NormalHitState : BaseState
{
    public NormalHitState()
    {
        Priority = (int)EStatePriority.High;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _animationCtrl.CrossFade(_stateMachine.PlayerCtrl.CharacterData.AnimationKey.HitHash);

        Priority = (int)EStatePriority.Medium;
        _stateMachine.ChangeState(EPlayerStateType.Idle);
    }

    public override void OnExit()
    {
        base.OnExit();

    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}