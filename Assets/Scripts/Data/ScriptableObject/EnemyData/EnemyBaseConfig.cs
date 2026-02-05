using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBaseConfig", menuName = "Scriptable Objects/EnemyBaseConfig")]
public class EnemyBaseConfig : ScriptableObject
{
    public float DetectRange = 5f;
    public float AttackRange = 1.5f;

    public float MovementSpeed = 1f;

    public float StoppingDistance = 0.5f;

    public float PatrolRadius = 5f;

    public int MaxHP = 100;

    public int Damage = 0;

    public float DamageRate = 1f;

    public int EXP = 100;

    //.. FIXME?? :: 동일하게 가져갈지도.., 모든 적이 동일하다면 Base Enemy로 이동
    public float StiffnessDuration;

    public GameObject HitEffect;    
}
