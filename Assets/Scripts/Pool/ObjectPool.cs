using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface IPoolTarget
{
    GameObject GameObject { get; }
    void OnEnqueuedToPool();
    void OnDequeuedFromPool();
}

public abstract class ObjectPoolBase
{
    public abstract void Enqueue(IPoolTarget item);
    public abstract IPoolTarget Dequeue(Vector3 position, Quaternion rotation);
}

public class ObjectPool<T> : ObjectPoolBase where T : IPoolTarget
{
    #region Const
    private const float EXPANSION_THRESHOLD_PERCENT = 0.25f;
    private const int DEFAULT_EXPANSION_COUNT = 5;
    #endregion

    private GameObject _prefabReference;
    private Transform _parentTransform;

    private int _totalCapacity;
    private int _expansionCount;

    private bool _isExpanding = false;

    private Queue<T> _queue = new Queue<T>();
    private CancellationToken _appCancellationToken;

    public void Generate(GameObject prefab, Transform parent, int initCount, int expansionCount, CancellationToken token)
    {
        _appCancellationToken = token;

        _prefabReference = prefab;
        _parentTransform = parent;

        _expansionCount = expansionCount;
        _totalCapacity = initCount;

        ExpandPoolSynchronous(initCount);
    }

    public override void Enqueue(IPoolTarget item)
    {
        if (item is T typedItem)
        {
            EnqueueGeneric(typedItem);
        }
    }

    public void EnqueueGeneric(T item)
    {
        item.GameObject.SetActive(false);
        item.OnEnqueuedToPool();
        _queue.Enqueue(item);
    }

    public override IPoolTarget Dequeue(Vector3 position, Quaternion rotation)
    {
        return DequeueGeneric(position, rotation);
    }

    public T DequeueGeneric(Vector3 position, Quaternion rotation)
    {
        if (_queue.Count == 0)
        {
            ExpandPoolSynchronous(DEFAULT_EXPANSION_COUNT);
        }

        if (!_isExpanding && (float)_queue.Count / _totalCapacity < EXPANSION_THRESHOLD_PERCENT)
        {
            ExpandPoolAsync(_expansionCount, _appCancellationToken).Forget();
        }

        var targetComponent = _queue.Dequeue();

        targetComponent.GameObject.transform.position = position;
        targetComponent.GameObject.transform.rotation = rotation;
        targetComponent.GameObject.SetActive(true);

        targetComponent.OnDequeuedFromPool();

        return targetComponent;
    }

    private void ExpandPoolSynchronous(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var newInstance = GameObject.Instantiate(_prefabReference, _parentTransform);
            if (newInstance.TryGetComponent(out T component))
            {
                EnqueueGeneric(component);
            }
        }

        _totalCapacity += count;
    }

    private async UniTask ExpandPoolAsync(int count, CancellationToken token)
    {
        if (_isExpanding || token.IsCancellationRequested)
            return;

        _isExpanding = true;

        try
        {
            for (int i = 0; i < count; i++)
            {
                if (token.IsCancellationRequested)
                    break;

                var newInstance = GameObject.Instantiate(_prefabReference, _parentTransform);
                if (newInstance.TryGetComponent(out T component))
                {
                    EnqueueGeneric(component);
                }

                await UniTask.Yield();
            }
            _totalCapacity += count;
        }
        catch (OperationCanceledException)
        {

        }
        finally
        {
            _isExpanding = false;
        }
    }
}