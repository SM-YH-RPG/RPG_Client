using System;
using UnityEngine;

[Serializable]
public struct CharacterStatData
{
    public bool Percent;
    public ECharacterStatType StatType;
    public float Value;
}

[Serializable]
public class CharacterStat
{
    //public int BaseHp;
    //public int BaseDamage;
    //public int BaseDefense;
    //public int BaseResound;
    //public float BaseCriticalRate; // %Ω∫≈»
    //public float BaseCriticalDamage; // %Ω∫≈»
    public CharacterStatData[] StatDataArray;
}

[CreateAssetMenu(fileName = "CharacterConfig", menuName = "Scriptable Objects/CharacterConfig")]
public class CharacterConfig : ScriptableObject
{
    [field: SerializeField]
    public string Name;

    [field: SerializeField]
    public string PrefabName;
    
    [field: SerializeField]
    public int Index;

    [field: SerializeField]
    public EWeaponType WeaponType;

    [field: SerializeField]
    public CharacterStat StatData { get; private set; }

    [field: SerializeField]
    public CommonAnimationData AnimationKey { get; private set; }

    [field:SerializeField]
    public AttackGroupConfig AttackGroup { get; private set; }

    [field: SerializeField]
    public SkillGroupConfig SkillGroup { get; private set; }

    [field:SerializeField]
    public PlayerGroundData GroundData { get; private set; }

    [field: SerializeField]
    public PlayerAirData AirData { get; private set; }
}
