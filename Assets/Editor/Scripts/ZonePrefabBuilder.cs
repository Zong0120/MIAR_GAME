#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using UnityEngine;

public class ZonePrefabBuilder : MonoBehaviour
{
    [MenuItem("Tools/Map/Generate Zone Prefabs")]
    public static void BuildAllZonePrefabs()
    {
        string prefabPath = "Assets/GeneratedZones/";
        if (!Directory.Exists(prefabPath))
            Directory.CreateDirectory(prefabPath);

        var allZones = Resources.LoadAll<MapZoneData>("MapZones");
        foreach (var zone in allZones)
        {
            var go = new GameObject(zone.zoneName);
            var tileParent = new GameObject("TileContainer").transform;
            tileParent.SetParent(go.transform);

            foreach (var tile in zone.tileData.tiles)
            {
                var tileGO = new GameObject(tile.tileName);
                tileGO.transform.SetParent(tileParent);
                tileGO.transform.localPosition = tile.localOffset;

                var sr = tileGO.AddComponent<SpriteRenderer>();
#if UNITY_EDITOR
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(tile.addressKey));
                sr.sprite = sprite;
#endif
            }

            var special = new GameObject("SpecialObjects");
            special.transform.SetParent(go.transform);
            special.SetActive(false);

            string fullPath = prefabPath + zone.zoneName + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(go, fullPath);
            DestroyImmediate(go);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✅ 所有 Zone Prefab 建立完成！");
    }
}
#endif