using UnityEngine;

[CreateAssetMenu(fileName = "SkillUIGroupConfig", menuName = "Scriptable Objects/SkillUIGroupConfig")]
public class SkillUIGroupConfig : ScriptableObject
{
    [field: SerializeField]
    public SkillUIConfig WeekAttack { get; private set; }

    [field:SerializeField]
    public SkillUIConfig StrongAttack { get; private set; }

    [field: SerializeField]
    public SkillUIConfig Skill { get; private set; }

    [field: SerializeField]
    public SkillUIConfig SpecialSkill { get; private set; }

    [field: SerializeField]
    public SkillUIConfig UltimateSkill { get; private set; }

    [field: SerializeField]
    public SkillUIConfig AirSkill { get; private set; }
}
