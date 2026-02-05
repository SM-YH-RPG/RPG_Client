public class DeadState : BaseState
{
    public DeadState()
    {
        Priority = (int)EStatePriority.High;
    }

    public override void OnEnter()
    {
        _stateMachine.PlayerCtrl.GetInput().enabled = false;
        _animationCtrl.CrossFade(_stateMachine.PlayerCtrl.CharacterData.AnimationKey.DeadHash);
        base.OnEnter();
    }

    public override void OnExit()
    {
        _stateMachine.PlayerCtrl.GetInput().enabled = true;
        Priority = (int)EStatePriority.High;
        base.OnExit();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        bool isFinished = _animationCtrl.IsAnimationFinished(_stateMachine.PlayerCtrl.CharacterData.AnimationKey.DeadHash);
        if (isFinished)
        {
            Priority = (int)EStatePriority.Medium;
        }
        else
        {
            Priority = (int)EStatePriority.High;
        }
    }

    public override void OnInput()
    {
        base.OnInput();
    }

    protected override void AddInputListeners()
    {
        base.AddInputListeners();
    }

    protected override void RemoveInputListeners()
    {
        base.RemoveInputListeners();
    }
}