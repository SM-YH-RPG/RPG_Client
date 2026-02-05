using UnityEngine;

public class EffectManager : LazySingleton<EffectManager>
{
    public void SpawnEffect(GameObject effectPrefab, Vector3 targetPosition, Quaternion targetRotation)
    {
        var effect = ObjectPoolManager.Instance.Spawn<EffectController>(
                effectPrefab,
                targetPosition,
                targetRotation
            );

        if (effect != null)
        {
            effect.InitEffect(effectPrefab);
        }
    }
}
