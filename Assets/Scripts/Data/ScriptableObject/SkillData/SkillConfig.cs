using System;
using UnityEngine;


public enum ESkillType
{
    Skill,
    SpecialSkill,
    UltimateSkill,
    End
}


[Serializable]
public struct SkillInfo
{
    public ESkillType Type;
    public float Cooldown;
}

[CreateAssetMenu(fileName = "SkillConfig", menuName = "Scriptable Objects/SkillConfig")]
public class SkillConfig : AttackConfig
{       
    public SkillInfo SkillInfo;
}
