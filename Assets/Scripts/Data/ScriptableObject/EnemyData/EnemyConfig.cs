using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Scriptable Objects/EnemyConfig")]
public class EnemyConfig : EnemyBaseConfig
{
    public AttackConfig attackConfig;
    public int[] rewardIndexArray;
}
