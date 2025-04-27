using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewMapZone", menuName = "Map/Zone Data")]
public class MapZoneData : ScriptableObject
{
    public string zoneName;                // 例如 "1F-Up"
    public List<MapZoneData> connectedZones;  // 相鄰區塊
    public MapZoneTileData tileData;       // 指向該區塊的 tile 切割資訊
}