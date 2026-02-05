using UnityEngine;

public class UIPoolingObjectSpawnManager : LazySingleton<UIPoolingObjectSpawnManager>
{
    private GameObject _damageTextPrefabAsset;

    public void Initialize(GameObject damageTextPrefab)
    {
        _damageTextPrefabAsset = damageTextPrefab;
    }

    public void ShowDamageText(Transform target, Vector3 uiSpawnPosition, CombatData combatData, Color color)
    {
        var damageText = ObjectPoolManager.Instance.Spawn<UIDamageText>(
                _damageTextPrefabAsset,
                uiSpawnPosition,
                Quaternion.identity
                );

        if (damageText != null)
        {
            damageText.Init(target, uiSpawnPosition + Vector3.up, _damageTextPrefabAsset);
            damageText.SetDamage(combatData.damage, color, combatData.isCritical);
        }
    }

    public UINormalEnemyHP SpawnEnemyHpBar(GameObject hpBarPrefab, Transform targetTransform, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        UINormalEnemyHP hpBar = ObjectPoolManager.Instance.Spawn<UINormalEnemyHP>(hpBarPrefab, spawnPosition, spawnRotation);

        if (hpBar != null)
        {
            hpBar.Init(targetTransform, spawnPosition);
            return hpBar;
        }

        return null;
    }
}
