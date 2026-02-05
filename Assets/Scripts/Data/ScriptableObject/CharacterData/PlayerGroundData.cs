using UnityEngine;
using System;

[Serializable]
public class PlayerGroundData
{
    [field: Header("BaseData")]
    [field: SerializeField]
    [field: Range(0f, 25f)]
    public float BaseSpeed { get; private set; } = 5f;
    
    [field: SerializeField]
    [field: Range(0f, 25f)]
    public float BaseRotationDamping { get; private set; } = 1f;

    [field: Header("WalkData")]
    [field: SerializeField]
    [field: Range(0f, 2f)] 
    public float WalkSpeedModifier { get; private set; } = 0.225f;

    [field: Header("RunData")]
    [field: SerializeField]
    [field: Range(0f, 2f)] 
    public float RunSpeedModifier { get; private set; } = 1f;

    [field: Header("DashData")]
    [field: SerializeField]
    [field: Range(0f, 2f)]
    public float DashSpeedModifier { get; private set; } = 1f;

    [field: Header("MotionData")]
    [field: SerializeField]
    [field: Range(0f, 10f)]
    public float waitTimeToMotion { get; private set; } = 0.1f;
}
