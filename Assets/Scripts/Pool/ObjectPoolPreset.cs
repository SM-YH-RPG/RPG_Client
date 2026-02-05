using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public enum PoolTargetType
{
    None,
    Enemy,
    Effect,
    Sound,
    UI
}

[Serializable]
public struct PoolPresetData
{
    public PoolTargetType Type;
    public GameObject Prefab;
    public Transform parent;

    public int InitCount;
    public int ExpansionCount;
}

#region TODO :: 여기서 옮겨야 할 것 들
public class BaseEffect : MonoBehaviour, IPoolTarget
{
    public GameObject GameObject => gameObject;

    public void OnEnqueuedToPool()
    {
        gameObject.SetActive(false);
    }

    public void OnDequeuedFromPool()
    {
        gameObject.SetActive(true);
    }
}

public class BaseSound : MonoBehaviour, IPoolTarget
{
    public GameObject GameObject => throw new NotImplementedException();

    public void OnEnqueuedToPool()
    {

    }

    public void OnDequeuedFromPool()
    {

    }
}

public class UIPoolingElement : MonoBehaviour, IPoolTarget
{
    public GameObject GameObject => gameObject;

    public void OnEnqueuedToPool()
    {
        gameObject.SetActive(false);
    }

    public void OnDequeuedFromPool()
    {
        gameObject.SetActive(true);
    }
}
#endregion

public class ObjectPoolPreset : MonoBehaviour
{
    [SerializeField]
    private PoolPresetData[] _presetDataList;

    //.. TODO :: 개선필요
    public void Initialize()
    {
        var destroyToken = this.GetCancellationTokenOnDestroy();
        foreach (var presetData in _presetDataList)
        {
            switch (presetData.Type)
            {
                case PoolTargetType.Enemy:
                    ObjectPoolManager.Instance.AddPoolObject<BaseEnemy>(presetData, destroyToken);
                    break;
                case PoolTargetType.Effect:
                    ObjectPoolManager.Instance.AddPoolObject<BaseEffect>(presetData, destroyToken);
                    break;
                case PoolTargetType.Sound:
                    ObjectPoolManager.Instance.AddPoolObject<BaseSound>(presetData, destroyToken);
                    break;
                case PoolTargetType.UI:
                    ObjectPoolManager.Instance.AddPoolObject<UIPoolingElement>(presetData, destroyToken);
                    break;
                case PoolTargetType.None:

                    break;
            }
        }
    }
}