using UnityEngine;

[CreateAssetMenu(fileName = "BossAttackGroupConfig", menuName = "Scriptable Objects/BossAttackGroupConfig")]
public class BossAttackGroupConfig : ScriptableObject
{
    [field: SerializeField]
    public AttackConfig BiteAttack { get; private set; }

    [field: SerializeField]
    public AttackConfig ClawAttack { get; private set; }

    [field: SerializeField]
    public AttackConfig HeadAttack { get; private set; }
}
