using System;
using System.Collections.Generic;
using UnityEngine;

public enum EEquipCost : int
{
    None = 0,
    One = 1,
    Three = 3,
    Four = 4
}


[Serializable]
public struct StatRange
{
    public EItemStatType StatType;
    public float MinValue;
    public float MaxValue;
}

[Serializable]
public struct StatTypeRanges
{
    public bool isMain;
    public EItemGrade Grade;
    public EEquipCost EquipCost;

    public List<StatRange> Ranges;
}

[CreateAssetMenu(fileName = "StatusGenConfig", menuName = "Scriptable Objects/StatusGenConfig")]
public class StatusGenConfig : ScriptableObject
{
    [field: SerializeField]
    private StatTypeRanges[] _statTypeRanges;
    public StatTypeRanges[] StatTypeRanges => _statTypeRanges;
}
