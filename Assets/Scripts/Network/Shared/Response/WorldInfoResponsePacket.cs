using System.Collections.Generic;
using UnityEngine;

public class WorldInfoResponsePacket : ResponsePacket
{
    public WorldPlacementData worldData;
}

public class WorldPlacementData
{
    public List<NpcData> Npcs { get; set; } = new();
    public List<InteractionData> GatherableInteractions { get; set; } = new();
    public List<InteractionData> DestoryableInteractions { get; set; } = new();
    public List<SpawnerData> Spawners { get; set; } = new();
}

public class WorldPlacementBase
{
    public long instanceId { get; set; }  // 서버 발급
    public int templateId { get; set; }

    public float[] pos { get; set; }
    public float[] rot { get; set; }
}

public class NpcData : WorldPlacementBase
{
    public EShopCategory shopCategory;
}

public class InteractionData : WorldPlacementBase
{

}

public class SpawnerData : WorldPlacementBase
{
    public float maxSpawnDistance;
    public float minSpawnSeparationDistance;
    public int spawnCount;
    public List<string> enemyTypes;
}
