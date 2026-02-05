using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnArea : SpawnAreaBase
{
    #region Const       
    private const float RESPAWN_TIME = 5f;
    #endregion

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        Spawn();
    }

    protected override void Spawn()
    {
        for (int i = 0; i < _spawnCount; i++)
        {
            var idx = UnityEngine.Random.Range(0, _spawnableEnemies.Length);
            var prefab = _prefabDict[(EnemyType)idx];
            enemyList.Add(Spawn(prefab));
        }
    }

    protected override BaseEnemy Spawn(GameObject prefab)
    {
        Vector3 spawnPosition = GetValidatedRandomNavMeshPosition();
        if (spawnPosition == Vector3.zero)
        {
            return null;
        }

        Quaternion spawnRotation = Quaternion.identity;
        var spawnedEnemy = ObjectPoolManager.Instance.Spawn<BaseEnemy>(prefab, spawnPosition, spawnRotation);

        if (spawnedEnemy != null)
        {
            spawnedEnemy.SetOriginPrefab(prefab);
            Debug.Log($"스폰 성공 {prefab.name} :> {spawnPosition}");
        }
        spawnedEnemy.SetDeathCallback(RemoveDeathMonster);
        return spawnedEnemy;
    }

    public override float GetRespawnDelay()
    {
        return RESPAWN_TIME;
    }

    public override void OnDeactivated()
    {
        
    }

    public override void OnRespawned()
    {
        Spawn();
    }

    #region Debug
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _maxSpawnDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
    #endregion
}