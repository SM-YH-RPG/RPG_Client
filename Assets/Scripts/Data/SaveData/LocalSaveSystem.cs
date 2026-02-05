using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class LocalSaveSystem
{
    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "PlayerSaveData.json");

    public static void Save(PlayerSaveData data)
    {
        string json = SaveJson.ToJson(data);
        AtomicWrite(SavePath, json);
    }

    public static bool TryLoad(out PlayerSaveData data)
    {
        data = null;
        if (!File.Exists(SavePath))
            return false;

        string json = File.ReadAllText(SavePath);
        data = SaveJson.FromJson<PlayerSaveData>(json);
        return data != null;
    }

    // 파일 손상 방지(원자적 저장)
    private static void AtomicWrite(string path, string content)
    {
        string tmp = path + ".tmp";
        File.WriteAllText(tmp, content);

        if (File.Exists(path))
            File.Delete(path);

        File.Move(tmp, path);
    }
}

public static class SaveJson
{
    private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,
        
        TypeNameHandling = TypeNameHandling.Auto,
     
        SerializationBinder = new KnownTypesBinder(
            typeof(BaseInventoryItem),
            typeof(WeaponItem),
            typeof(EquipmentItem),
            typeof(ConsumableItem)
        )
    };

    public static string ToJson<T>(T data) =>
        JsonConvert.SerializeObject(data, Settings);

    public static T FromJson<T>(string json) =>
        JsonConvert.DeserializeObject<T>(json, Settings);
}

public sealed class KnownTypesBinder : Newtonsoft.Json.Serialization.ISerializationBinder
{
    private readonly System.Collections.Generic.Dictionary<string, Type> _nameToType;

    public KnownTypesBinder(params Type[] knownTypes)
    {
        _nameToType = new();
        foreach (var t in knownTypes)
            _nameToType[t.FullName] = t;
    }

    public Type BindToType(string assemblyName, string typeName)
        => _nameToType.TryGetValue(typeName, out var t) ? t : null;

    public void BindToName(Type serializedType, out string assemblyName, out string typeName)
    {
        assemblyName = null;                 // assemblyName 저장 안함(유니티 환경에서 안정적)
        typeName = serializedType.FullName;  // FullName으로 매칭
    }
}
