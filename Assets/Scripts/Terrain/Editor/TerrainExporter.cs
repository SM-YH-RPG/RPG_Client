using Newtonsoft.Json;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TerrainExporter : MonoBehaviour
{
    [MenuItem("Tools/Export Terrain Heightmap (JSON)")]
    public static void ExportHeightmap()
    {
        Terrain terrain = FindFirstObjectByType<Terrain>();
        if (terrain == null)
        {
            Debug.LogError("장면에 Terrain 오브젝트가 없습니다.");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        float[,] heights = terrainData.GetHeights(0, 0, width, height);

        string json = JsonConvert.SerializeObject(heights, Formatting.Indented);

        string path = "Assets/Resources/TerrainData/terrain_height.json";
        File.WriteAllText(path, json);

        Debug.Log($"Terrain heightmap exported to: {path}");
        AssetDatabase.Refresh();
    }
}