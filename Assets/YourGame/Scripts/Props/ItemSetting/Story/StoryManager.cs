using UnityEngine;
using TMPro;
using System.Collections;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;
    public StoryProgressData storyProgressData;
    public GameObject storyTriggerPrefab;
    public GameObject storyZoneTriggerPrefab;
    public StoryChapterSwapPoint[] chapterSpawnPoints;
    public StoryZoneSwapPoint[] zoneSpawnPoints;
    public bool ChaptersCollectionComplete() => !storyProgressData.HasNextChapter();

    [Header("Canvas")]
    public GameObject UIRoot;
    private CanvasGroup _canvasGroup => GetComponent<CanvasGroup>();
    private RectTransform _rectTransform => GetComponent<RectTransform>();
    public RectTransform test1;
    [SerializeField]private TextMeshProUGUI storyTitleText;
    [SerializeField]private TextMeshProUGUI storyDescribeText;

    private Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    private Vector3 endPosition = new Vector3(Screen.width -Screen.width/20f,Screen.height/9.6f, 0);

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
    private void Start()
    {
        InitStoryPoints();
        InitInventoryStory();
    }

    private void InitStoryPoints()
    {
        for (int i = 0; i < chapterSpawnPoints.Length; i++)
        {
            if (!storyProgressData.CanSpawnChapterTrigger(chapterSpawnPoints[i].spawnID))
            {
                GuidanceSystem.Instance.AddCompletedChapter(chapterSpawnPoints[i].spawnID);
                Destroy(chapterSpawnPoints[i].storyChapter.gameObject);
                continue;

            }
            else chapterSpawnPoints[i].storyChapter._storySpawnID = chapterSpawnPoints[i].spawnID;
        }
        for(int i = 0; i < zoneSpawnPoints.Length; i++)
        {
            if (!storyProgressData.CanSpawnZoneTrigger(zoneSpawnPoints[i].storyZone.zoneID))
            {
                Destroy(zoneSpawnPoints[i].storyZoneTriggerPrefab);
            }
        }
    }
    public void InitInventoryStory()
    {
        foreach (var story in storyProgressData.unlockedZones)
        {
            InventoryItemManager.Instance.storyZoneBag.Add(story);
        }
        for(int i = 0;i<storyProgressData.usedChapterSpawnPoints.Count;i++)
        {
            InventoryItemManager.Instance.storyChapterBag.Add(storyProgressData.chapters[i]);
        }
    }

    public void UnlockNextChapter(string spawnID)
    {
        if (!storyProgressData.HasNextChapter())
            return;
        StoryChapter currentStory = storyProgressData.PeekNextChapter(spawnID);
        VideoManager.Instance.PlayVideo(currentStory.videoClip, 1,spawnID);
        InventoryItemManager.Instance.storyChapterBag.Add(currentStory);
    }

    public void UnlockZone(StoryZone zone)
    {
        if (storyProgressData.CanSpawnZoneTrigger(zone.zoneID))
        {
            storyProgressData.UnlockZone( zone);
            ShowStory(zone.title, zone.content);
            InventoryItemManager.Instance.storyZoneBag.Add(zone);
            Debug.Log("獲得物品: " + zone.title);
        }
    }

    public void ShowStory(string title, string description)
    {
        storyTitleText.text = title;
        storyDescribeText.text = description;
        StartCoroutine(ShowStoryUIFloatingEffect());
    }
    //canvas graup 0 to 1,scale 0.5 to 1
    IEnumerator ShowStoryUIFloatingEffect()
    {
        float duration = 0.5f; // 動畫持續時間
        float elapsedTime = 0f;
    
        Vector3 startScale = new Vector3(0.5f, 0.5f, 1);
        Vector3 endScale = new Vector3(1f, 1f, 1);
        
        _rectTransform.position = screenCenter;
        UIRoot.SetActive(true);
    
        float startAlpha = 0.5f;
        float endAlpha = 1f;
    
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
    
            // 線性插值縮放
            _rectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
    
            // 線性插值透明度
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
    
            yield return null;
        }
    
        // 確保最終值正確
        _rectTransform.localScale = endScale;
        _canvasGroup.alpha = endAlpha;
    }

    public void CloseWindow()
    {
        StartCoroutine(CloseCanvasEffect());
    }

    IEnumerator CloseCanvasEffect()
    {
        float duration = 0.5f; // 動畫持續時間
        float elapsedTime = 0f;
    
        Vector3 startScale = new Vector3(1f, 1f, 1);
        Vector3 endScale = new Vector3(0.1f, 0.1f, 1);
    
        // 獲取目標位置（假設目標 UI 元素有 RectTransform）
        RectTransform targetUI = test1;
        if (targetUI == null)
        {
            Debug.LogError("Target UI not assigned!");
            yield break;
        }
    
        float startAlpha = 1f;
        float endAlpha = 0.7f;
    
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
    
            // 線性插值縮放
            _rectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
    
            // 線性插值位置
            _rectTransform.position = Vector3.Lerp(screenCenter, endPosition, t);
    
            // 線性插值透明度
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
    
            yield return null;
        }
    
        // 確保最終值正確
        _rectTransform.localScale = endScale;
        _rectTransform.position = endPosition;
        _canvasGroup.alpha = endAlpha;
    
        // 隱藏 Canvas
        UIRoot.SetActive(false);
    }

}
[System.Serializable]
public class StoryChapterSwapPoint
{
    public string spawnID;
    public GetStoryChapter storyChapter;
}
[System.Serializable]
public class StoryZoneSwapPoint
{
    public string spawnID;
    public StoryZone storyZone;
    public GameObject storyZoneTriggerPrefab;
}