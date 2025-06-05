using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using PlayerInputAction;

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
    public static MapZoneManager Instance { get; private set; }
    public MapZoneData startZone;
    public MapParent[] mapParent;
    private Dictionary<string, MapParent> mapParentLookup = new();

    private Dictionary<string, GameObject> visibleTiles = new();
    private Dictionary<string, GameObject> preloadedTiles = new();
    private Dictionary<string, AsyncOperationHandle<Sprite>> loadedHandles = new();

    private MapZoneData currentZone;
    public char currentZoneLevel{ get; set; } = '1';

    public List<GameObject> _1FZoneObjects;
    public List<GameObject> _2FZoneObjects;
    public float viewDistance = 30f;
    public float unloadDelay = 7f;
    public int tilesPerFrame = 3; // 每幀最多預載 tile 數量
    public int objectsPerFrame = 2; // 每幀最多開啟 zoneObjects 數量

    public UnityEngine.UI.Image OverEffectImage;
    private Transform player;
    private int currentLoadSessionId = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        foreach (var entry in mapParent)
        {
            if (!mapParentLookup.ContainsKey(entry.zoneName))
                mapParentLookup.Add(entry.zoneName, entry);
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        LoadAndActivateZone(startZone);
        RendererFeatureManager.Instance.DeathEnd();
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

        OverEffectImage.gameObject.SetActive(true);
        // 過渡效果
        float elapsedTime = 0f;
        while (elapsedTime < 0.2f)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsedTime / 0.2f);
            OverEffectImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        PlayerController.Instance.FreezePlayer();

        if (zone.zoneName[0] != currentZoneLevel)
        {
            Debug.Log("切換樓層: " + zone.zoneName[0]);
            currentZoneLevel = zone.zoneName[0];
            if (currentZoneLevel == '1')
            {
                foreach (var obj in _1FZoneObjects)
                    obj.SetActive(true);
                foreach (var obj in _2FZoneObjects)
                    obj.SetActive(false);
            }
            else if (currentZoneLevel == '2')
            {
                foreach (var obj in _2FZoneObjects)
                    obj.SetActive(true);
                foreach (var obj in _1FZoneObjects)
                    obj.SetActive(false);
            }
        }

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
        PlayerController.Instance.UnFreezePlayer();
        // 過渡效果
        elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / 0.5f);
            OverEffectImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        OverEffectImage.gameObject.SetActive(false);
    }

    public void ShowConnectedZone(MapZoneData zone)
    {
        if (!mapParentLookup.TryGetValue(zone.zoneName, out var zoneParent)) return;

        foreach (var tile in zone.tileData.tiles)
        {
            if (preloadedTiles.TryGetValue(tile.tileName, out var obj))
            {
                preloadedTiles.Remove(tile.tileName);
                visibleTiles[tile.tileName] = obj;
                obj.SetActive(true);
            }
        }

        if (zoneParent._connectedCollider != null)
            zoneParent._connectedCollider.SetActive(true);

        if (zoneParent._zoneObjects != null)
        {
            foreach (var obj in zoneParent._zoneObjects)
                obj.SetActive(true);
        }
    }

    public void HideConnectedZone(MapZoneData zone, MapZoneData _currentZone)
    {
        if (!mapParentLookup.TryGetValue(zone.zoneName, out var zoneParent)) return;

        currentZone = _currentZone;

        foreach (var tile in zone.tileData.tiles)
        {
            if (visibleTiles.TryGetValue(tile.tileName, out var obj))
            {
                visibleTiles.Remove(tile.tileName);
                obj.SetActive(false);
                preloadedTiles[tile.tileName] = obj;
            }
        }

        if (zoneParent._connectedCollider != null)
            zoneParent._connectedCollider.SetActive(false);

        if (zoneParent._zoneObjects != null)
        {
            foreach (var obj in zoneParent._zoneObjects)
                obj.SetActive(false);
        }
    }

    private IEnumerator LoadTile(TileInfo tile, MapParent zoneParent, bool setActive, int sessionId)
    {
        var handle = Addressables.LoadAssetAsync<Sprite>(tile.addressKey);
        yield return handle;

        if (sessionId != currentLoadSessionId) yield break;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject obj = zoneParent.GetTile(tile.localOffset, handle.Result);

            obj.GetComponent<SpriteRenderer>().enabled = false;
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
    MapZoneData teleportZone;
    bool isTeleportPending = false;
    public void PreloadTeleportZone(MapZoneData zone)
    {
        currentLoadSessionId++;
        int sessionId = currentLoadSessionId;
        teleportZone = zone;
        isTeleportPending = true;

        StartCoroutine(PreloadOnlyZone(zone, sessionId));
    }

    private IEnumerator PreloadOnlyZone(MapZoneData zone, int sessionId)
    {
        if (!mapParentLookup.TryGetValue(zone.zoneName, out var zoneParent))
            yield break;

        var preloadOnlyTiles = new HashSet<string>();
        int preloadCount = 0;

        foreach (var tile in zone.tileData.tiles)
        {
            preloadOnlyTiles.Add(tile.tileName);
            if (!visibleTiles.ContainsKey(tile.tileName) && !preloadedTiles.ContainsKey(tile.tileName))
            {
                StartCoroutine(LoadTile(tile, zoneParent, false, sessionId));
                preloadCount++;
                if (preloadCount >= tilesPerFrame)
                {
                    preloadCount = 0;
                    yield return null;
                }
            }
        }

        // 預熱但不啟用 zone 物件
        if (zoneParent._zoneObjects != null)
        {
            foreach (var obj in zoneParent._zoneObjects)
                obj.SetActive(false);
        }

        if (zoneParent._connectedCollider != null)
            zoneParent._connectedCollider.SetActive(false);
    }

    public void ActivateTeleportZone(MapZoneData zone, Vector3 targetPosition)
    {
        if (!isTeleportPending || zone != teleportZone) return;

        isTeleportPending = false;
        teleportZone = null;

        // 手動切換區塊，不同於 LoadAndActivateZone 的立即切換
        currentLoadSessionId++;
        int sessionId = currentLoadSessionId;

        // 關閉前一個區塊
        if (currentZone != null && mapParentLookup.TryGetValue(currentZone.zoneName, out var prevParent))
        {
            if (prevParent._connectedCollider != null)
                prevParent._connectedCollider.SetActive(false);
            if (prevParent._zoneObjects != null)
            {
                foreach (var obj in prevParent._zoneObjects)
                    obj.SetActive(false);
            }
        }

        // 正式切換區塊
        currentZone = zone;
        StartCoroutine(ActivateZone(zone, sessionId));

        // 傳送玩家
        if (player != null)
            player.position = targetPosition;
    }

    public void TemporarilyHideCurrentZone()
    {
        if (currentZone == null || !mapParentLookup.TryGetValue(currentZone.zoneName, out var zoneParent))
            return;

        // 將所有 tile 設為非活躍，並放回 preload 清單
        foreach (var tile in currentZone.tileData.tiles)
        {
            if (visibleTiles.TryGetValue(tile.tileName, out var obj))
            {
                visibleTiles.Remove(tile.tileName);
                obj.SetActive(false);
                preloadedTiles[tile.tileName] = obj;
            }
        }

        // 關閉 collider 和 zone objects
        if (zoneParent._connectedCollider != null)
            zoneParent._connectedCollider.SetActive(false);

        if (zoneParent._zoneObjects != null)
        {
            foreach (var obj in zoneParent._zoneObjects)
                obj.SetActive(false);
        }
    }
    public void ReactivateCurrentZone()
    {
        if (currentZone == null || !mapParentLookup.TryGetValue(currentZone.zoneName, out var zoneParent))
            return;

        foreach (var tile in currentZone.tileData.tiles)
        {
            if (preloadedTiles.TryGetValue(tile.tileName, out var obj))
            {
                preloadedTiles.Remove(tile.tileName);
                visibleTiles[tile.tileName] = obj;
                obj.SetActive(true);
            }
        }

        if (zoneParent._connectedCollider != null)
            zoneParent._connectedCollider.SetActive(true);

        if (zoneParent._zoneObjects != null)
        {
            foreach (var obj in zoneParent._zoneObjects)
                obj.SetActive(true);
        }
    }

}