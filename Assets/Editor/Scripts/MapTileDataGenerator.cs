#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class MapTileDataGenerator : EditorWindow
{
    [MenuItem("Tools/Generate Map Zone Tile Data")]
    public static void ShowWindow()
    {
        GetWindow<MapTileDataGenerator>("Map Tile Data Generator");
    }

    private Texture2D sourceTexture;
    private string zoneName;

    void OnGUI()
    {
        GUILayout.Label("Generate Tile Info from Sprites", EditorStyles.boldLabel);
        sourceTexture = (Texture2D)EditorGUILayout.ObjectField("Sprite Texture", sourceTexture, typeof(Texture2D), false);
        zoneName = EditorGUILayout.TextField("Zone Name", zoneName);

        if (GUILayout.Button("Generate Tile Data"))
        {
            GenerateTileData();
        }
    }

    void GenerateTileData()
    {
        if (sourceTexture == null || string.IsNullOrEmpty(zoneName))
        {
            Debug.LogError("Missing input.");
            return;
        }

        string path = AssetDatabase.GetAssetPath(sourceTexture);
        Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);

        List<TileInfo> tiles = new();

        foreach (var asset in assets)
        {
            if (asset is Sprite sprite)
            {
                string fullKey = path + "[" + sprite.name + "]";
        
                // 計算正確的 localOffset
                float x = sprite.rect.x / sprite.pixelsPerUnit;
                float y = (sourceTexture.height - sprite.rect.yMax) / sprite.pixelsPerUnit;
        
                TileInfo tile = new TileInfo
                {
                    tileName = sprite.name,
                    addressKey = fullKey,
                    localOffset = new Vector3(x, -y, 0) // 使用正確的 x 和 y
                };
                tiles.Add(tile);
            }
        }

        MapZoneTileData tileData = ScriptableObject.CreateInstance<MapZoneTileData>();
        tileData.zoneName = zoneName;
        tileData.tiles = tiles;

        string folderPath = "Assets/YourGame/Sprites/Terrain";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string tilePath = $"{folderPath}/{zoneName}_TileData.asset";
        AssetDatabase.CreateAsset(tileData, tilePath);

        MapZoneData zoneData = ScriptableObject.CreateInstance<MapZoneData>();
        zoneData.zoneName = zoneName;
        //zoneData.offsetPosition = Vector2.zero;
        zoneData.connectedZones = new List<MapZoneData>();
        zoneData.tileData = tileData;

        string zonePath = $"{folderPath}/{zoneName}_ZoneData.asset";
        AssetDatabase.CreateAsset(zoneData, zonePath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Generated: {tilePath} and {zonePath}");
    }
}
#endif