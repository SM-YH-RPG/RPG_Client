using UnityEngine;

public interface IState
{
    void Initialize(PlayerStateMachine stateMachine);
    void OnEnter();
    void OnUpdate();
    void OnPhysicsUpdate();
    void OnExit();

    void OnInput();

    //.. 상태 우선순위, 높을수록 우선 적용, 낮으면 무시
    public int Priority => 0;
}
