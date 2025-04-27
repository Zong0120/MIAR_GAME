using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

[System.Serializable]
public class MapParent
{
    public string zoneName;
    public Transform _mapParent;
    public GameObject tilePrefab;

    public GameObject _connectedCollider;
    public List<GameObject> _zoneObjects;
    private Queue<GameObject> tilePool = new();

    public GameObject GetTile(Vector3 localPos, Sprite sprite)
    {
        GameObject obj = tilePool.Count > 0 ? tilePool.Dequeue() : GameObject.Instantiate(tilePrefab);
        obj.transform.SetParent(_mapParent);
        obj.transform.localPosition = localPos;

        var sr = obj.GetComponent<SpriteRenderer>();
        sr.sprite = sprite;

        obj.SetActive(true);
        return obj;
    }

    public void ReturnTile(GameObject obj)
    {
        obj.SetActive(false);
        tilePool.Enqueue(obj);
    }
}

public class MapZoneManager : MonoBehaviour
{
    public MapZoneData startZone;
    public MapParent[] mapParent;
    private Dictionary<string, MapParent> mapParentLookup = new();

    private Dictionary<string, GameObject> visibleTiles = new();
    private Dictionary<string, GameObject> preloadedTiles = new();
    private Dictionary<string, AsyncOperationHandle<Sprite>> loadedHandles = new();

    private MapZoneData currentZone;
    public float viewDistance = 30f;
    public float unloadDelay = 5f;
    public int tilesPerFrame = 3; // 每幀最多預載 tile 數量
    public int objectsPerFrame = 2; // 每幀最多開啟 zoneObjects 數量
    private Transform player;

    private int currentLoadSessionId = 0;

    void Start()
    {
        foreach (var entry in mapParent)
        {
            if (!mapParentLookup.ContainsKey(entry.zoneName))
                mapParentLookup.Add(entry.zoneName, entry);
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        LoadAndActivateZone(startZone);
    }

    void Update()
    {
        UpdateTileVisibility();
    }

    public void LoadAndActivateZone(MapZoneData zone)
    {
        if (zone == currentZone) return;

        currentLoadSessionId++;
        int sessionId = currentLoadSessionId;

        // 先關閉前一個 zone 的物件
        if (currentZone != null && mapParentLookup.TryGetValue(currentZone.zoneName, out var prevZoneParent))
        {
            if (prevZoneParent._connectedCollider != null)
                prevZoneParent._connectedCollider.SetActive(false);

            if (prevZoneParent._zoneObjects != null)
            {
                foreach (var obj in prevZoneParent._zoneObjects)
                    obj.SetActive(false);
            }
        }

        currentZone = zone;
        StartCoroutine(ActivateZone(zone, sessionId));
    }

    private IEnumerator ActivateZone(MapZoneData zone, int sessionId)
    {
        var newVisibleTiles = new HashSet<string>();
        var preloadOnlyTiles = new HashSet<string>();

        if (!mapParentLookup.TryGetValue(zone.zoneName, out var zoneParent))
            yield break;

        // 開啟 connectedCollider
        if (zoneParent._connectedCollider != null)
            zoneParent._connectedCollider.SetActive(true);

        // 載入主要區塊 tile
        int tileCount = 0;
        foreach (var tile in zone.tileData.tiles)
        {
            newVisibleTiles.Add(tile.tileName);

            if (visibleTiles.ContainsKey(tile.tileName))
            {
                visibleTiles[tile.tileName].SetActive(true);
            }
            else if (preloadedTiles.TryGetValue(tile.tileName, out var go))
            {
                preloadedTiles.Remove(tile.tileName);
                visibleTiles[tile.tileName] = go;
                go.SetActive(true);
            }
            else
            {
                StartCoroutine(LoadTile(tile, zoneParent, true, sessionId));
                tileCount++;
                if (tileCount >= tilesPerFrame)
                {
                    tileCount = 0;
                    yield return null;
                }
            }
        }

        // 分批啟用 zone 內的物件
        if (zoneParent._zoneObjects != null)
        {
            for (int i = 0; i < zoneParent._zoneObjects.Count; i++)
            {
                zoneParent._zoneObjects[i].SetActive(true);
                if ((i + 1) % objectsPerFrame == 0)
                    yield return null;
            }
        }

        // 預載 connected zones（但不啟用 zoneObjects 和 collider）
        foreach (var connected in zone.connectedZones)
        {
            if (!mapParentLookup.TryGetValue(connected.zoneName, out var cParent)) continue;

            // 預熱 connected zone tiles
            int preloadCount = 0;
            foreach (var tile in connected.tileData.tiles)
            {
                preloadOnlyTiles.Add(tile.tileName);
                if (!visibleTiles.ContainsKey(tile.tileName) && !preloadedTiles.ContainsKey(tile.tileName))
                {
                    StartCoroutine(LoadTile(tile, cParent, false, sessionId));
                    preloadCount++;
                    if (preloadCount >= tilesPerFrame)
                    {
                        preloadCount = 0;
                        yield return null;
                    }
                }
            }

            // 預熱 zoneObjects（但不啟用）
            if (cParent._zoneObjects != null)
            {
                foreach (var obj in cParent._zoneObjects)
                    obj.SetActive(false);
            }

            if (cParent._connectedCollider != null)
                cParent._connectedCollider.SetActive(false);
        }

        // 清除不再需要的 tile
        var totalNeeded = newVisibleTiles.Union(preloadOnlyTiles);

        var toRemove = new List<string>();
        foreach (var key in visibleTiles.Keys)
        {
            if (!newVisibleTiles.Contains(key))
            {
                GameObject obj = visibleTiles[key];
                obj.SetActive(false);
                preloadedTiles[key] = obj;
                toRemove.Add(key);
            }
        }
        foreach (var key in toRemove)
            visibleTiles.Remove(key);

        var preloadToRemove = new List<string>();
        foreach (var key in preloadedTiles.Keys)
        {
            if (!totalNeeded.Contains(key))
            {
                GameObject obj = preloadedTiles[key];
                StartCoroutine(UnloadTileAfterDelay(key, obj, unloadDelay, sessionId));
                preloadToRemove.Add(key);
            }
        }
        foreach (var key in preloadToRemove)
            preloadedTiles.Remove(key);
    }



    private IEnumerator LoadTile(TileInfo tile, MapParent zoneParent, bool setActive, int sessionId)
    {
        var handle = Addressables.LoadAssetAsync<Sprite>(tile.addressKey);
        yield return handle;

        if (sessionId != currentLoadSessionId) yield break;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject obj = zoneParent.GetTile(tile.localOffset, handle.Result);

            obj.GetComponent<SpriteRenderer>().enabled =false;
            obj.SetActive(true);
            yield return null;
            obj.SetActive(setActive);
            obj.GetComponent<SpriteRenderer>().enabled = true;

            if (setActive)
                visibleTiles[tile.tileName] = obj;
            else
                preloadedTiles[tile.tileName] = obj;
            
            //Debug.Log("✅ 載入 & 預熱完成: " + tile.tileName);
            loadedHandles[tile.tileName] = handle;
        }
        else
        {
            Debug.LogError("❌ 載入失敗: " + tile.addressKey);
        }
    }

    private IEnumerator UnloadTileAfterDelay(string tileName, GameObject tileObj, float delay, int sessionId)
    {
        yield return new WaitForSeconds(delay);

        if (sessionId != currentLoadSessionId) yield break;

        if (tileObj.TryGetComponent(out SpriteRenderer sr) && mapParentLookup.TryGetValue(currentZone.zoneName, out var parent))
        {
            sr.sprite = null;
            parent.ReturnTile(tileObj);
        }

        if (loadedHandles.TryGetValue(tileName, out var handle))
        {
            Addressables.Release(handle);
            loadedHandles.Remove(tileName);
        }
    }

    private void UpdateTileVisibility()
    {
        if (player == null) return;

        foreach (var kvp in visibleTiles)
        {
            var obj = kvp.Value;
            var renderer = obj.GetComponent<SpriteRenderer>();
            if (renderer == null || renderer.sprite == null) continue;

            Bounds bounds = renderer.bounds;
            float closestDist = bounds.SqrDistance(player.position);
            obj.SetActive(closestDist <= viewDistance * viewDistance);
        }
    }
}