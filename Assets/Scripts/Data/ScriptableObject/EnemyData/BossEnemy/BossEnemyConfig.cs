using UnityEngine;

[CreateAssetMenu(fileName = "BossEnemyConfig", menuName = "Scriptable Objects/BossEnemyConfig")]
public class BossEnemyConfig : EnemyBaseConfig
{
    public BossAttackGroupConfig attackGroupConfig;
    public int[] rewardIndexArray;
    public string bossName;
}
