using System;
using UnityEngine;
public enum HitShapeType
{
    None,
    Sphere,
    Box,
    Sector
}

public interface IAttackTypeSate
{
    public float CalculateDamageRate();
}

[Serializable]
public struct HitShapeData
{
    public HitShapeType Type;

    [Header("공용 데이터")]
    public Vector3 Offset;
    public LayerMask Layers;

    [Header("원형 / 부채꼴 데이터")]
    public float Radius;

    [Header("부채꼴 데이터")]
    public float Angle;

    [Header("박스 데이터")]
    public Vector3 HalfExtend;

    [Header("박스 회전")]
    public Quaternion Rotation;
}

[Serializable]
public struct TimedHitData
{
    public HitShapeData Config;
    public float Delay;
}


[CreateAssetMenu(fileName = "AttackConfig", menuName = "Scriptable Objects/AttackConfig")]
public class AttackConfig : ScriptableObject
{
    public TimedHitData[] TimedHitData;

    [field: SerializeField]
    private string AnimationStateName;
    public int AnimationStateHash => Animator.StringToHash(AnimationStateName);

    public float StartTimeNormalized;
    public float EndTimeNormalized;

    public float DamageRate;
}
