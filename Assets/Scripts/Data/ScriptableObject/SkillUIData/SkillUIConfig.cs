using UnityEngine;
using System;

public enum ECharacterSkillOrder
{
    WeekAttack = 0,
    StrongAttack,
    Skill,
    UltimateSkill
}

[Serializable]
public struct SkillUIInfo
{
    public ECharacterSkillOrder orderType;
    public string Name;
    public string Description;
    public Sprite SkillIcon;
}

[CreateAssetMenu(fileName = "SkillUIConfig", menuName = "Scriptable Objects/SkillUIConfig")]
public class SkillUIConfig : ScriptableObject
{
    public bool isVisible = true;
    public SkillUIInfo skillUIInfo;
}
