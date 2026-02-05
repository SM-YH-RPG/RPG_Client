using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ObjectPoolManager : LazySingleton<ObjectPoolManager>
{
    private Dictionary<GameObject, ObjectPoolBase> _pools = new Dictionary<GameObject, ObjectPoolBase>();

    public void AddPoolObject<T>(PoolPresetData data, CancellationToken token) where T : class, IPoolTarget
    {
        if (data.Prefab.GetComponent<T>() == null)
        {
            return;
        }

        if (_pools.ContainsKey(data.Prefab) == false)
        {
            var newPool = new ObjectPool<T>();
            newPool.Generate(data.Prefab, data.parent, data.InitCount, data.ExpansionCount, token);
            _pools.Add(data.Prefab, newPool);
        }
    }

    public T Spawn<T>(GameObject prefab, Vector3 position, Quaternion rotation) where T : class, IPoolTarget
    {
        if (_pools.TryGetValue(prefab, out ObjectPoolBase basePool))
        {
            IPoolTarget pooledItem = basePool.Dequeue(position, rotation);

            return pooledItem as T;
        }

        return null;
    }

    public void ReturnToPool<T>(GameObject prefab, T item) where T : class, IPoolTarget
    {
        if (_pools.TryGetValue(prefab, out ObjectPoolBase basePool))
        {
            basePool.Enqueue(item);
        }
        else
        {
            GameObject.Destroy(item.GameObject);
        }
    }
}