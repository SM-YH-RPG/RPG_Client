using UnityEngine;
using System;

[Serializable]
public class PlayerAirData
{
    [field: Header("BaseData")]
    [field: SerializeField]
    [field: Range(0f, 25f)]
    public float JumpForce { get; private set; } = 7f;

    [field: SerializeField]
    [field: Range(0f, 10f)]
    public float CanAirMoveFactor { get; private set; } = 1f;

    [field: SerializeField]
    public bool CanDoubleJump { get; private set; } = false;

    [field: SerializeField]
    public bool CanAirAttack { get; private set; } = false;
}
