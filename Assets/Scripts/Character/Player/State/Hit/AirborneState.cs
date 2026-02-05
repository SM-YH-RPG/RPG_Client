public class AirborneState : AirState
{
    public override void OnEnter()
    {
        base.OnEnter();

        _stateMachine.ChangeState(EPlayerStateType.Idle); // 3번 맞아서 에어본 들어오면 다음 공격 맞기 전까지 캐릭터 먹통 이슈..
    }
}