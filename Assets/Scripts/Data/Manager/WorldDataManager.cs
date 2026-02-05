using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldDataManager : LazySingleton<WorldDataManager>
{
    private WorldPlacementData _worldData;
    public WorldPlacementData WorldData => _worldData;

    public void Initialize(WorldPlacementData data)
    {
        _worldData = data;
    }
}
