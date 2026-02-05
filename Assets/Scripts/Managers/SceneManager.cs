using Cysharp.Threading.Tasks;
using System.Collections;
using UnitySceneMgr = UnityEngine.SceneManagement.SceneManager;

public class SceneManager : LazySingleton<SceneManager>
{
    public async void ChangeScene(string sceneName)
    {
        UIManager.Instance.CachedClear();

        var loadRequester = UnitySceneMgr.LoadSceneAsync(sceneName);
        await UniTask.WaitUntil(() => loadRequester.isDone);

    }
}