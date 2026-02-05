using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CustomTypeBinder : DefaultSerializationBinder
{
    public override Type BindToType(string assemblyName, string typeName)
    {
        string assemblyToLoad = Assembly.GetExecutingAssembly().FullName;
        return Type.GetType(String.Format("{0}, {1}", typeName, assemblyToLoad));
    }
}

public static class SerializationHelper
{
    public static string SerializeWithPolymorphism<T>(T dataObject)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };

        return JsonConvert.SerializeObject(dataObject, settings);
    }

    // 제네릭 역직렬화 메서드
    public static T DeserializeWithPolymorphism<T>(string jsonPayload)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        return JsonConvert.DeserializeObject<T>(jsonPayload, settings);
    }
}

public class PacketDispatcher
{
    private readonly Dictionary<string, Action<ResponsePacket>> handlers = new Dictionary<string, Action<ResponsePacket>>();

    public void Initialize()
    {
        ClientPacketRegistry.RegisterAllHandlers(this);
    }

    public void RegisterHandler<T>(Action<T> handlerAction) where T : ResponsePacket
    {
        string typeName = typeof(T).FullName;

        handlers.Add(typeName, (packet) =>
        {
            handlerAction.Invoke((T)packet);
        });
    }

    private readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto,
        Formatting = Formatting.Indented,
        SerializationBinder = new CustomTypeBinder()
    };

    public void DispatchPacket(string jsonMessage)
    {
        try
        {
            var basePacket = JsonConvert.DeserializeObject<ResponsePacket>(jsonMessage, Settings);
            if (basePacket == null || handlers.ContainsKey(basePacket.PacketType) == false)
            {
                Debug.LogWarning($"패킷 불일치 또는 핸들러 없음: {basePacket?.PacketType}");
                return;
            }

            Type specificType = Type.GetType(basePacket.PacketType);
            if (specificType == null)
            {
                Debug.LogError($"알 수 없는 타입: {basePacket.PacketType}");
                return;
            }

            object specificPacket = JsonConvert.DeserializeObject(jsonMessage, specificType, Settings);
            handlers[basePacket.PacketType].Invoke((ResponsePacket)specificPacket);
        }
        catch (JsonException ex)
        {
            Debug.LogError($"JSON ERROR: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"DISPATCH ERROR: {ex.Message}");
        }
    }
}