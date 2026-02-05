using UnityEngine;

public interface IRespawnable
{
    float GetRespawnDelay();
    void OnDeactivated();
    void OnRespawned();
}