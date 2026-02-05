using System;
using UnityEngine;

[Serializable]
public class CommonAnimationData
{
    [field: SerializeField]
    protected string _idleStateName = "Idle";

    [field: SerializeField]
    protected string _walkStateName = "Walk";

    [field: SerializeField]
    protected string _runStateName = "Run";

    [field: SerializeField]
    protected string _dashStateName = "Dash";

    [field: SerializeField]
    protected string _deadStateName = "Dead";

    [field: SerializeField]
    protected string _jumpStateName = "Jump";


    [field: SerializeField]
    protected string _hitStateName = "Hit";

    //.. Locomotion Hashes
    public int IdleHash => Animator.StringToHash(_idleStateName);
    public int WalkHash => Animator.StringToHash(_walkStateName);
    public int RunHash => Animator.StringToHash(_runStateName);
    public int DashHash => Animator.StringToHash(_dashStateName);
    public int DeadHash => Animator.StringToHash(_deadStateName);
    public int JumpHash => Animator.StringToHash(_jumpStateName);
    public int HitHash => Animator.StringToHash(_hitStateName);
}