using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using PlayerInputAction;

[Serializable]
public class GuidanceNodeData
{
    public string nodeId;
    public string guidanceType;
    public List<string> guidanceLines;
    public string highlightTargetId;
    public string[] nextNodeIds;
    public bool isOptional;
}

[Serializable]
public class GuidanceNodeDataList
{
    public List<GuidanceNodeData> nodes;
}

[Serializable]
public class highlightTargetId
{
    public string targetId;
    public string targetName;
    public Vector2 position;
    public GameObject targetObject;
    public bool is1Floor;
}

[Serializable]
public class highlightStoryId
{
    public string targetId;
    public Vector2 position;
    public bool is1Floor;
}

public class GuidanceSystem : MonoBehaviour
{
    public static GuidanceSystem Instance;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private CanvasGroup panel;
    public Image systemImage;
    [SerializeField] private float typeSpeed = 0.03f;
    [SerializeField] private GameObject TargetPoint;

    [Header("JSON File")]
    [SerializeField] private TextAsset jsonData;

    [Header("TargetPoint")]
    [SerializeField] private List<highlightTargetId> highlightTargets = new();
    [SerializeField] private List<highlightStoryId> highlightStoryId = new();
    [Header("Phase 2")]
    [SerializeField] private Material _GlitchMaterial;
    [SerializeField] private SpriteRenderer _SmallMap1, _SmallMap2;
    [SerializeField] private Image OverEffectImage;
    public bool _isPhase2 { get; private set; } = false;


    private Dictionary<string, GuidanceNodeData> nodeMap = new();
    private HashSet<string> completedMainNodes = new();
    private GuidanceNodeData currentNode;

    private Queue<string> guidanceQueue = new();
    private bool isShowing = false;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    #region Settings
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadGuidanceNodes();
    }
    void Start()
    {
        MainNodeInit();
        TriggerNode("GameStart");
    }

    private void MainNodeInit()
    {
        List<string> InheritanceData = InheritanceManager.Instance.LoadInheritanceData();
        foreach (var mainNode in highlightTargets)
        {
            if (mainNode.targetObject == null)
            {
                Debug.LogWarning($"[主節點初始化] 找不到目標物件: {mainNode.targetId}");
                completedMainNodes.Add(mainNode.targetId);
                continue;
            }

            // 檢查該主節點是否已完成
            if (completedMainNodes.Contains(mainNode.targetId))
            {
                Debug.Log($"[主節點初始化] 節點已完成: {mainNode.targetId}");
                Destroy(mainNode.targetObject);
                continue;
            }

            if (InheritanceData.Contains(mainNode.targetName))
            {
                Debug.Log($"[主節點初始化] 節點已完成: {mainNode.targetId}");
                completedMainNodes.Add(mainNode.targetId);
                Destroy(mainNode.targetObject);
                continue;
            }

            // 訂閱事件：當目標物件被銷毀時觸發
            var targetObject = mainNode.targetObject;
            var targetId = mainNode.targetId;

            targetObject.AddComponent<DestroyEventListener>().OnDestroyed += () =>
            {
                Debug.Log($"[主節點觸發] 目標物件被銷毀: {targetId}");
                CompletedMainNodes(targetId);
            };

            Debug.Log($"[主節點初始化] 已訂閱目標物件事件: {mainNode.targetId}");
        }
    }

    void LoadGuidanceNodes()
    {
        nodeMap.Clear();
        var dataList = JsonUtility.FromJson<GuidanceNodeDataList>(jsonData.text);
        foreach (var node in dataList.nodes)
            nodeMap[node.nodeId] = node;
    }

    public void TriggerNode(string nodeId)
    {
        if (completedMainNodes.Contains(nodeId)) return;

        if (!nodeMap.TryGetValue(nodeId, out var node)) return;

        if (node.guidanceType == "Main" || node.guidanceType == "Main-2")
            SetCurrentNode(nodeId);
        else
            SetGuidance(node.guidanceLines);
    }

    public void SetCurrentNode(string nodeId)
    {
        if (!nodeMap.TryGetValue(nodeId, out var node)) return;
        Debug.Log($"[導引觸發] 節點: {node.nodeId}");
        currentNode = node;
        HighlightMapTarget(nodeId);
        if (node.guidanceLines.Count > 0)
            SetGuidance(node.guidanceLines);
    }

    public void SetGuidance(List<string> lines)
    {
        guidanceQueue.Clear();
        foreach (var line in lines) guidanceQueue.Enqueue(line);
        if (!isShowing) StartCoroutine(ShowGuidanceRoutine());
    }

    private IEnumerator ShowGuidanceRoutine()
    {
        isShowing = true;
        while (guidanceQueue.Count > 0)
        {
            string current = guidanceQueue.Dequeue();
            yield return ShowText(current);

            float waitTime = Mathf.Max(2f, current.Length * typeSpeed);
            float timer = 0f;
            while (!Input.GetKeyDown(KeyCode.Space) && timer < waitTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            TargetManager.Instance.AddTargetRecord(current);
        }
        Hide();
        isShowing = false;
    }

    public IEnumerator NextMain(float waitTime = 1f)
    {
        // 若還有下一節點，自動觸發
        if (currentNode != null && currentNode.nextNodeIds.Length > 0)
        {
            foreach (string nextId in currentNode.nextNodeIds)
            {
                if (!IsNodeCompleted(nextId))
                {
                    Debug.Log($"[導引自動接續] → {nextId}");
                    yield return new WaitForSeconds(waitTime);
                    TargetManager.Instance.ClearTargetRecord();
                    // 觸發下一節點
                    SetCurrentNode(nextId);
                    break;
                }
            }
        }
    }
    public void CompletedMainNodes(string nodeId)
    {
        if (nodeMap.TryGetValue(nodeId, out var node))
        {
            completedMainNodes.Add(node.nodeId);
            // 觸發下一節點
            if (node.nextNodeIds.Length > 0)
            {
                foreach (string nextId in node.nextNodeIds)
                {
                    if (!IsNodeCompleted(nextId))
                    {
                        Debug.Log($"[導引自動接續] → {nextId}");
                        TargetManager.Instance.ClearTargetRecord();
                        // 觸發下一節點
                        SetCurrentNode(nextId);
                        return;
                    }
                }
                //檢查completedMainNodes數量是否為8
                if (completedMainNodes.Count >= 8 && completedMainNodes.Count < 16)
                {
                    Debug.Log($"[導引完成] ****一階段導引已完成****");
                    SystemPhase2();
                }
                else if (completedMainNodes.Count >= 16)
                {
                    TriggerNode("ClearnRoom");
                    TargetPoint.SetActive(false);
                }
            }
            else
            {
                Debug.Log($"[導引完成] 節點已完成: {node.nodeId}");
            }
        }
        else
        {
            Debug.LogWarning($"[導引完成] 找不到節點: {nodeId}");
        }
    }

    private IEnumerator ShowText(string content)
    {
        textComponent.text = "";
        panel.gameObject.SetActive(true);
        // 緩入效果
        yield return FadeCanvasGroup(panel, 0f, 1f, 0.5f);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(content));
        yield return typingCoroutine;
    }

    private IEnumerator TypeText(string content)
    {
        content = ReplaceVariables(content);
        textComponent.text = "";
        isTyping = true;
        foreach (char c in content)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                textComponent.text = content;
                isTyping = false;
                yield break;
            }
            textComponent.text += c;
            //if (typingSound && audioSource) audioSource.PlayOneShot(typingSound);
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
    }

    private void Hide()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        // 啟動緩出效果
        StartCoroutine(FadeOutAndDisable());
    }

    private IEnumerator FadeOutAndDisable()
    {
        // 緩出效果
        yield return FadeCanvasGroup(panel, 1f, 0f, 0.5f);

        panel.gameObject.SetActive(false);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
    }

    public void HighlightMapTarget(string nodeId)
    {
        var targetH = _isPhase2
            ? null
            : highlightTargets.Find(t => t.targetId == nodeId);
        if (targetH != null)
        {
            TargetPoint.SetActive(true);
            TargetPoint.transform.position = targetH.position;
            TargetPoint.GetComponent<SpriteRenderer>().color = targetH.is1Floor ? new Color(1, 1, 1, 1) : new Color(0, 0, 0, 1);

            SmallCameraFollowing.Instance.MoveSmallCamera(targetH.position, 3f);
        }
        else if (_isPhase2)
        {
            var targetH2 = highlightStoryId.Find(t => t.targetId == nodeId);
            if (targetH2 != null)
            {
                TargetPoint.SetActive(true);
                TargetPoint.transform.position = targetH2.position;
                TargetPoint.GetComponent<SpriteRenderer>().color = targetH2.is1Floor ? new Color(1, 1, 1, 1) : new Color(0, 0, 0, 1);

                SmallCameraFollowing.Instance.MoveSmallCamera(targetH2.position, 3f);
            }
        }
        else
        {
            Debug.Log($"[小地圖提示] 找不到目標: {nodeId} highlightTargetId");
        }
    }

    private string ReplaceVariables(string text)
    {
        return Regex.Replace(text, @"\[#(\w+)\]", match =>
        {
            string key = match.Groups[1].Value;
            return key switch
            {
                "DeathCount" => PlayerPrefs.GetInt("DeathCount", 0).ToString(),
                _ => match.Value
            };
        });
    }

    public bool IsNodeCompleted(string nodeId) => completedMainNodes.Contains(nodeId);
    #endregion

    #region Custom Functions
    public void ShowRandomDeathMessage()
    {
        List<GuidanceNodeData> deathNodes = new();
        foreach (var node in nodeMap.Values)
        {
            if (node.guidanceType == "Death")
                deathNodes.Add(node);
        }

        if (deathNodes.Count == 0)
        {
            Debug.LogWarning("找不到任何 guidanceType = Death 的節點！");
            return;
        }

        var chosen = deathNodes[UnityEngine.Random.Range(0, deathNodes.Count)];
        Debug.Log($"[死亡提示] 隨機顯示節點：{chosen.nodeId}");

        SetGuidance(chosen.guidanceLines);
    }

    public void SystemPhase2()
    {
        _isPhase2 = true;
        systemImage.material = _GlitchMaterial;
        _SmallMap1.material = _GlitchMaterial;
        _SmallMap2.material = _GlitchMaterial;
        TriggerNode("Secound_MainStart");
        StartCoroutine(WaitForCompletedMainNodes(10f, "Secound_MainStart"));
    }
    public void AddCompletedChapter(string chapterId)
    {
        if (!completedMainNodes.Contains(chapterId))
        {
            completedMainNodes.Add(chapterId);
            Debug.Log($"[導引初始化] 節點已完成: {chapterId}");
        }
    }

    IEnumerator WaitForCompletedMainNodes(float seconds, string nodeId)
    {
        yield return new WaitForSeconds(seconds);
        CompletedMainNodes("Secound_MainStart");
    }

    public void WrongEnding()
    {
        PlayerController.Instance.FreezePlayer();
        PlayerController.Instance.AnimationChagneIdle();
        systemImage.material = _GlitchMaterial;

        StartCoroutine(OverEffectImgFadIn());
    }
    IEnumerator OverEffectImgFadIn()
    {
        OverEffectImage.gameObject.SetActive(true);
        Color fadeColor = OverEffectImage.color;
        fadeColor.a = 0;
        OverEffectImage.color = fadeColor;
        // 淡入過程
        while (OverEffectImage.color.a < 1)
        {
            fadeColor = OverEffectImage.color;
            fadeColor.a += Time.deltaTime / 1f;
            OverEffectImage.color = fadeColor;
            yield return null;
        }
        SetGuidance(nodeMap["WrongEnding"].guidanceLines);
        yield return new WaitForSeconds(5f);
        //FadeOut
        while (OverEffectImage.color.a > 0)
        {
            fadeColor = OverEffectImage.color;
            fadeColor.a -= Time.deltaTime / 1f;
            OverEffectImage.color = fadeColor;
            yield return null;
        }
        OverEffectImage.gameObject.SetActive(false);

        HealthManager.Instance.DirectDeath();
    }
    #endregion
}
