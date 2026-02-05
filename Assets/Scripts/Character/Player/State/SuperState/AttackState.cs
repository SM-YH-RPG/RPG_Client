public class AttackState : BaseState
{
    protected int _currentComboIndex = 0;
    public int CurrentComboIndex => _currentComboIndex;

    protected AttackConfig[] _comboConfigs;
    protected int _comboMax = 0;

    protected int _currentAnimationHash = -1;

    protected bool _nextAttackQueued = false;

    public override void OnEnter()
    {
        base.OnEnter();

        _currentComboIndex = 0;

        var currentVelocity = _stateMachine.PlayerCtrl.CharacterVelocity;
        currentVelocity.x = 0f;
        currentVelocity.z = 0f;
        _stateMachine.PlayerCtrl.CharacterVelocity = currentVelocity;
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    //.. 공격 스타일 별로 배율 관련 버프를 처리 하기 위해 가상화
    public virtual float CalculateDamageRate() { return 1f; }
}