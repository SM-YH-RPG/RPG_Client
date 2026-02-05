using Cysharp.Threading.Tasks;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class DataLoader
{
    //public static async UniTask<T> LoadJson<T>(string fileName) where T : class
    //{
    //    string path = $"Resources/Table/{fileName}.json";
    //    string fullPath = Path.Combine(Application.dataPath, path);
    //    using (StreamReader sr = new StreamReader(fullPath))
    //    {
    //        string json = await sr.ReadToEndAsync();
    //        return JsonConvert.DeserializeObject<T>(json);
    //    }
    //}

    public static UniTask<T> LoadJson<T>(string fileName) where T : class
    {        
        string resourcePath = $"Table/{fileName}";

        TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);
        if (textAsset == null)
        {
            Debug.LogError($"[DataLoader] Missing TextAsset: Assets/Resources/{resourcePath}.json");
            return UniTask.FromResult<T>(null);
        }

        try
        {
            T data = JsonConvert.DeserializeObject<T>(textAsset.text);
            return UniTask.FromResult(data);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DataLoader] JSON parse failed: {resourcePath}\n{e}");
            return UniTask.FromResult<T>(null);
        }
    }
}