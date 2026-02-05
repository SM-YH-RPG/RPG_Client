#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public static class WorldPlacementExporter
{
    [Serializable]
    private class WorldPlacementExport
    {        
        public List<NpcDto> npcs = new();
        public List<InteractionDto> gatherableInteractions = new();
        public List<InteractionDto> destoryableInteractions = new();
        public List<SpawnerDto> spawners = new();
    }

    [Serializable]
    private class TransformDto
    {
        public float[] pos;   // [x,y,z]
        public float[] rot;    // [x,y,z]
    }

    [Serializable]
    private class NpcDto : TransformDto
    {
        public int templateId;
        public EShopCategory shopCategory;
    }

    [Serializable]
    private class InteractionDto : TransformDto
    {
        public int templateId;
    }

    [Serializable]
    private class SpawnerDto : TransformDto
    {
        public int templateId;
        public float maxSpawnDistance;
        public float minSpawnSeparationDistance;
        public int spawnCount;
        public List<string> enemyTypes;
    }

    [MenuItem("Tools/World/Export Placement JSON")]
    public static void ExportPlacementJson()
    {
        // 씬에서 Placement 컴포넌트 수집 (비활성 포함)
        var npcs = Resources.FindObjectsOfTypeAll<ConsumptionStoreNPC>()
            .Where(IsInValidScene).ToArray();
        var gatherableInteractions = Resources.FindObjectsOfTypeAll<GatherableObject>()
            .Where(IsInValidScene).ToArray();
        var destroyableInteractions = Resources.FindObjectsOfTypeAll<DestoryableObject>()
            .Where(IsInValidScene).ToArray();
        var spawners = Resources.FindObjectsOfTypeAll<SpawnAreaBase>()
            .Where(IsInValidScene).ToArray();

        var all = new List<ClientPlacementObjectBase>();
        all.AddRange(npcs);
        all.AddRange(gatherableInteractions);
        all.AddRange(destroyableInteractions);
        all.AddRange(spawners);

        var export = new WorldPlacementExport
        {            
            npcs = npcs.Select(ToNpcDto).ToList(),
            gatherableInteractions = gatherableInteractions.Select(ToInteractionDto).ToList(),
            destoryableInteractions = destroyableInteractions.Select(ToInteractionDto).ToList(),
            spawners = spawners.Select(ToSpawnerDto).ToList(),
        };

        string defaultName = $"WorldInfo.json";
        string path = EditorUtility.SaveFilePanel(
            "Save World Placement JSON",
            Application.dataPath,
            defaultName,
            "json"
        );

        if (string.IsNullOrWhiteSpace(path))
            return;

        // JSON 생성
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        string json = JsonConvert.SerializeObject(export, settings);
        File.WriteAllText(path, json);

        Debug.Log($"World Placement JSON exported: {path}");
        EditorUtility.DisplayDialog("Export Complete", $"Exported:\n{path}", "OK");
    }

    private static bool IsInValidScene(Component c)
    {
        // 프리팹 에셋/프리뷰 오브젝트 제외
        if (c == null) return false;
        if (EditorUtility.IsPersistent(c)) return false; // 에셋이면 제외
        return c.gameObject.scene.IsValid() && c.gameObject.scene.isLoaded;
    }

    private static float[] Pos(Transform t) => new[] { t.position.x, t.position.y, t.position.z };
    private static float[] Rot(Transform t) => new[] { t.eulerAngles.x, t.eulerAngles.y, t.eulerAngles.z };

    private static NpcDto ToNpcDto(ConsumptionStoreNPC p) => new()
    {               
        templateId = p._templateId,
        shopCategory = p.ShopCategory,
        pos = Pos(p.transform),
        rot = Rot(p.transform)
    };

    private static InteractionDto ToInteractionDto(ClientPlacementObjectBase p) => new()
    {
        templateId = p._templateId,
        pos = Pos(p.transform),
        rot = Rot(p.transform)
    };

    private static SpawnerDto ToSpawnerDto(SpawnAreaBase p) => new()
    {
        templateId = p._templateId,
        maxSpawnDistance = p.MaxSpawnDistance,
        minSpawnSeparationDistance = p.MinSpawnSeparationDistance,
        spawnCount = p.SpawnCount,
        enemyTypes = p.GetSpawnEnemyTypeList(),
        pos = Pos(p.transform),
        rot = Rot(p.transform)
    };
}
#endif
