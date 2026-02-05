using System;
using UnityEngine;

public class InteractionEventManager : LazySingleton<InteractionEventManager>, IInteractionService
{
    public event Action<Transform> OnInteractionTargetChanged;
    public event Action<Transform> OnInteractionSourceChanged;

    public void NotifyInteractionTargetChanged(Transform targetTransform)
    {
        OnInteractionTargetChanged?.Invoke(targetTransform);
    }

    public void NotifyInteractionSourceChanged(Transform sourceTransform)
    {
        OnInteractionSourceChanged?.Invoke(sourceTransform);
    }
}
