using System;
using UnityEngine;

public interface IInteractionService
{
    event Action<Transform> OnInteractionTargetChanged;
    event Action<Transform> OnInteractionSourceChanged;

    void NotifyInteractionTargetChanged(Transform targetTransform);
    void NotifyInteractionSourceChanged(Transform sourceTransform);
}