using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TileInfo
{
    public string tileName;
    public string addressKey;
    public Vector2 localOffset;
}

[CreateAssetMenu(fileName = "ZoneTileMap", menuName = "Map/Zone Tile Set")]
public class MapZoneTileData : ScriptableObject
{
    public string zoneName;
    public List<TileInfo> tiles;
}
