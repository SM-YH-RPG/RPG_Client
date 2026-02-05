using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class DiskSpaceChecker
{
#if UNITY_ANDROID && !UNITY_EDITOR
    [DllImport("UnityDiskUtils")]
    private static extern long GetAvailableDiskSpaceInBytes();
#endif

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern long GetAvailableDiskSpaceInBytes();
#endif

    private static long GetAvailableSpaceBytes()
    {
        if (Application.isEditor)
        {
            return 10L * 1024 * 1024 * 1024;
        }

#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        return GetAvailableDiskSpaceInBytes();
#else
        return -1;
#endif
    }

    public static long GetAvailableFreeSpace(string path = "")
    {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        return GetAvailableSpaceBytes();
#else
        try
        {
            DriveInfo drive = new DriveInfo(path);
            if (drive.IsReady)
            {
                return drive.AvailableFreeSpace;
            }
            else
            {
                return -1;
            }
        }
        catch (Exception ex)
        {
            return -1;
        }
#endif
    }

    public static long GetAvailableSpaceForPersistentData()
    {
        return GetAvailableFreeSpace(Application.persistentDataPath);
    }
}

public class AddressablesDownloader
{
    public async UniTask<long> GetDownloadSizeAsync(object key)
    {
        AsyncOperationHandle<long> handle = Addressables.GetDownloadSizeAsync(key);
        try
        {
            long downloadSize = await handle.ToUniTask();

            Debug.Log($"Download Size : {downloadSize}");
            return downloadSize;
        }
        catch (Exception ex)
        {
            Debug.LogError($"다운로드 크기 못 받아옴 {key}: {ex.Message}");
            throw; 
        }
        finally
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
    }

    public async UniTask DownloadRemoteDependenciesAsync(object key, CancellationToken cancellationToken = default)
    {
        AsyncOperationHandle handle = Addressables.DownloadDependenciesAsync(key);

        try
        {
            await handle.ToUniTask(Progress.Create<float>(p =>
            {
                //.. TODO :: 다운로드 UI
            }), cancellationToken: cancellationToken);

            Debug.Log($"다운 완료 : {key}.");
        }
        catch (OperationCanceledException)
        {
            
            throw;
        }
        catch (Exception ex)
        {
            Debug.Log($"ERROR:{ex.Message}");
            throw;
        }
        finally
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
    }
}

public class AddressableManager : LazySingleton<AddressableManager>
{
    private readonly Dictionary<string, UnityEngine.Object> _loadedAssets = new Dictionary<string, UnityEngine.Object>();

    private readonly Dictionary<string, UniTaskCompletionSource<UnityEngine.Object>> _loadingTasks = new Dictionary<string, UniTaskCompletionSource<UnityEngine.Object>>();

    private AddressablesDownloader _downloader = new AddressablesDownloader();
    public AddressablesDownloader Downloader => _downloader;

    public async UniTask<T> LoadAssetAsync<T>(string address) where T : UnityEngine.Object
    {
        if (_loadedAssets.ContainsKey(address))
        {
            return (T)_loadedAssets[address];
        }

        if (_loadingTasks.TryGetValue(address, out UniTaskCompletionSource<UnityEngine.Object> tcs))
        {
            UnityEngine.Object result = await tcs.Task;
            return (T)result;
        }

        var newTcs = new UniTaskCompletionSource<UnityEngine.Object>();
        _loadingTasks.Add(address, newTcs);

        try
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
            T resultAsset = await handle.ToUniTask();

            if (resultAsset != null)
            {
                _loadedAssets.Add(address, resultAsset);
                newTcs.TrySetResult(resultAsset);
                return resultAsset;
            }
            else
            {
                throw new Exception($"Failed to load asset: {address} resulted in null.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"에셋 로딩 중 오류 발생: {ex.Message}");
            newTcs.TrySetException(ex);
            throw;
        }
        finally
        {
            if (_loadingTasks.ContainsKey(address) && _loadingTasks[address] == newTcs)
            {
                _loadingTasks.Remove(address);
            }
        }
    }

    public void ReleaseAsset(string address)
    {
        if (_loadedAssets.ContainsKey(address))
        {
            object assetToRelease = _loadedAssets[address];
            Addressables.Release(assetToRelease);

            _loadedAssets.Remove(address);
        }
    }

    public void Clear()
    {
        List<string> addressesToRelease = new List<string>(_loadedAssets.Keys);
        foreach (var address in addressesToRelease)
        {
            ReleaseAsset(address);
        }

    }
}