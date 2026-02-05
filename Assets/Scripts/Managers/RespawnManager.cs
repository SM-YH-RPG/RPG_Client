using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface IRespawnManagerService
{
    void ScheduleRespawn(IRespawnable respawnableObject);
}

public class RespawnManager : LazySingleton<RespawnManager>, IRespawnManagerService
{
    private Dictionary<IRespawnable, CancellationTokenSource> _respawnTasks = new Dictionary<IRespawnable, CancellationTokenSource>();

    public void ScheduleRespawn(IRespawnable respawnableObject)
    {
        if (_respawnTasks.ContainsKey(respawnableObject))
        {
            _respawnTasks[respawnableObject].Cancel();
            _respawnTasks[respawnableObject].Dispose();
        }

        var cts = new CancellationTokenSource();
        _respawnTasks[respawnableObject] = cts;

        RespawnAfterDelayAsync(respawnableObject, cts.Token).Forget();
    }

    private async UniTask RespawnAfterDelayAsync(IRespawnable targetObject, CancellationToken cancellationToken)
    {
        targetObject.OnDeactivated();

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(targetObject.GetRespawnDelay()),
                                cancellationToken: cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
            {
                targetObject.OnRespawned();
                _respawnTasks.Remove(targetObject);
            }
        }
        catch (OperationCanceledException)
        {

        }
    }
}