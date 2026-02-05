using UnityEngine;

[CreateAssetMenu(fileName = "SkillGroupConfig", menuName = "Scriptable Objects/SkillGroupConfig")]
public class SkillGroupConfig : ScriptableObject
{
    [field: SerializeField]
    public SkillUIGroupConfig SkillUIGroup;

    [field: SerializeField]
    public SkillConfig Skill { get; private set; }

    [field: SerializeField]
    public SkillConfig SpecialSkill { get; private set; }

    [field: SerializeField]
    public SkillConfig UltimateSkill { get; private set; }

    [field: SerializeField]
    public SkillConfig AirSkill { get; private set; }
}
