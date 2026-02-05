using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType
{
    Slime,
    Turtle,
    BlueSlime,
    StarFish,
    Boss,
}

[Serializable]
public struct EnemySpawnData
{
    public EnemyType EnemyType;
    public GameObject Prefab;
}

public abstract class SpawnAreaBase : ClientPlacementObjectBase, IRespawnable
{
    #region Const
    private const int MAX_SPAWN_ATTEMPTS = 10;
    #endregion

    #region Inspector
    [SerializeField]
    protected EnemySpawnData[] _spawnableEnemies;

    [SerializeField]
    protected float _maxSpawnDistance = 5f;
    public float MaxSpawnDistance => _maxSpawnDistance;

    [SerializeField]
    protected LayerMask _groundLayerMask;

    [SerializeField]
    protected int _spawnCount;
    public int SpawnCount => _spawnCount;

    [SerializeField]
    protected float _minSpawnSeparationDistance = 2.0f;
    public float MinSpawnSeparationDistance => _minSpawnSeparationDistance;

    [SerializeField]
    protected LayerMask _enemyLayerMask;
    #endregion

    protected Dictionary<EnemyType, GameObject> _prefabDict = new Dictionary<EnemyType, GameObject>();
    protected List<string> _enemyTypeList = new List<string>();

    protected List<BaseEnemy> enemyList = new List<BaseEnemy>();
    protected IRespawnManagerService _respawnService => RespawnManager.Instance;

    protected virtual void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        foreach (var data in _spawnableEnemies)
        {
            if (_prefabDict.ContainsKey(data.EnemyType) == false)
            {
                _prefabDict.Add(data.EnemyType, data.Prefab);

                if (data.Prefab.GetComponent<BaseEnemy>() == null)
                {
                    Debug.LogWarning($"프리팹 {data.Prefab.name} 에 Base Enemy 컴포넌트가 할당되어 있지 않습니다.");
                }
            }
            else
            {
                Debug.LogWarning($"중복 등록된 타입 (Duplicate Type) {data.EnemyType} :> {this.gameObject.name}", this);
            }
        }
    }    

    protected Vector3 GetValidatedRandomNavMeshPosition()
    {
        for (int attempts = 0; attempts < MAX_SPAWN_ATTEMPTS; attempts++)
        {
            Vector3 randomPoint = transform.position + UnityEngine.Random.insideUnitSphere * _maxSpawnDistance;
            randomPoint.y = transform.position.y + 2f;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, _maxSpawnDistance, NavMesh.AllAreas))
            {
                bool isAreaClear = !Physics.CheckSphere(hit.position, _minSpawnSeparationDistance, _enemyLayerMask);

                if (isAreaClear)
                {
                    return hit.position;
                }
            }
        }

        return Vector3.zero;
    }

    protected void RemoveDeathMonster(BaseEnemy baseEnemy)
    {
        enemyList.Remove(baseEnemy);
        if (enemyList.Count == 0)
        {
            _respawnService.ScheduleRespawn(this);
        }
    }

    public List<string> GetSpawnEnemyTypeList()
    {
        _enemyTypeList.Clear();
        for (int i = 0; i < _spawnableEnemies.Length; i++)
        {
            _enemyTypeList.Add(_spawnableEnemies[i].EnemyType.ToString());
        }
        return _enemyTypeList;
    }

    public void InitSpawnData(WorldPlacementBase worldInfo)
    {
        // Local SpawnData Init
        if (worldInfo is SpawnerData spawner)
        {
            _maxSpawnDistance = spawner.maxSpawnDistance;
            _spawnCount = spawner.spawnCount;
            _minSpawnSeparationDistance = spawner.minSpawnSeparationDistance;
        }
    }

    protected abstract void Spawn();

    protected abstract BaseEnemy Spawn(GameObject prefab);

    public abstract float GetRespawnDelay();

    public abstract void OnDeactivated();    

    public abstract void OnRespawned();
}
