using UnityEngine;
using System;

public class EnemyHPHandler : LazySingleton<EnemyHPHandler>
{    
    public event Action<string, int, int> OnBossEnemyHPChanged;

    public void NotifyBossEnemyHPChanged(string bossName, int currentHP, int maxHP)
    {
        OnBossEnemyHPChanged?.Invoke(bossName, currentHP, maxHP);
    }
}
