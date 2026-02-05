using UnityEngine;

public class IdleState : GroundedState
{
    public override int Priority => (int)EStatePriority.Medium;

    public override void OnEnter()
    {
        base.OnEnter();

        SetDirection(Vector3.zero);
        _animationCtrl.CrossFade(_stateMachine.PlayerCtrl.CharacterData.AnimationKey.IdleHash, 0.1f);
    }    
}