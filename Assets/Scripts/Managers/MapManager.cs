using System.Collections.Generic;
using UnityEngine;

public class DropItem
{
    
}

public class ZoneData
{
    public int x;
    public int y;
    public Texture map;
    public List<DropItem> items;
}

public class MapManager : LazySingleton<MapManager>
{
    private List<ZoneData> zoneDatas = new List<ZoneData>();
}