
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : LazySingleton<ResourcesManager>
{
    private Dictionary<string, Texture2D> _cachedTextureDict = new Dictionary<string, Texture2D>();
    private Dictionary<string, GameObject> _cachedPreviewCharacterDict = new Dictionary<string, GameObject>();
    private Dictionary<string, Sprite> _cachedSpriteDict = new Dictionary<string, Sprite>();
    private Dictionary<string, UniTask<GameObject>> _loadingTasks = new Dictionary<string, UniTask<GameObject>>();

    public void Clear()
    {
        _cachedTextureDict.Clear();
        _cachedPreviewCharacterDict.Clear();
        _loadingTasks.Clear();
        _cachedSpriteDict.Clear();
    }
    
    public async UniTask<Sprite> LoadSpriteAsync(string spriteName)
    {
        if (_cachedSpriteDict.TryGetValue(spriteName, out var cached))
            return cached;

        var request = AddressableManager.Instance.LoadAssetAsync<Sprite>($"{spriteName}");
        
        var sprite = await request;
        if (sprite == null)
            return null;
        if (_cachedSpriteDict.ContainsKey(spriteName) == false)
        {
            _cachedSpriteDict.Add(spriteName, sprite);
        }
        return sprite;
    }

    public UniTask<GameObject> LoadPreviewCharacter(string characterName)
    {
        if (_cachedPreviewCharacterDict.TryGetValue(characterName, out var cached))
            return UniTask.FromResult(cached);

        if (_loadingTasks.TryGetValue(characterName, out var inFlight))
            return inFlight;

        var task = LoadPreviewCharacterInternal(characterName);
        _loadingTasks[characterName] = task;

        return task;
    }

    private async UniTask<GameObject> LoadPreviewCharacterInternal(string characterName)
    {
        try
        {
            var request = AddressableManager.Instance.LoadAssetAsync<GameObject>($"{characterName}");

            var prefab = await request;
            if (prefab == null)
                return null;

            if (_cachedPreviewCharacterDict.ContainsKey(characterName) == false)
            {
                _cachedPreviewCharacterDict.Add(characterName, prefab);
            }
            return prefab;
        }
        finally
        {
            _loadingTasks.Remove(characterName);
        }
    }    
}