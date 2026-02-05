using UnityEngine;

[CreateAssetMenu(fileName = "AttackGroupConfig", menuName = "Scriptable Objects/AttackGroupConfig")]
public class AttackGroupConfig : ScriptableObject
{
    [field: SerializeField]
    public AttackConfig[] WeakAttack { get; private set; }

    [field: SerializeField]
    public AttackConfig[] StrongAttack { get; private set; }

    [field: SerializeField]
    public AttackConfig FallingAttack { get; private set; }

    [field: SerializeField]
    public AttackConfig MidAirAttack { get; private set; }
}
