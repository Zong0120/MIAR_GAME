using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;
using System.Text.RegularExpressions;

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
    public Vector2 position;
    public bool is1Floor;
}

public class GuidanceSystem : MonoBehaviour
{
    public static GuidanceSystem Instance;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private CanvasGroup panel;
    //[SerializeField] private AudioSource audioSource;
    //[SerializeField] private AudioClip typingSound;
    [SerializeField] private float typeSpeed = 0.03f;
    [SerializeField] private GameObject TargetPoint;

    [Header("JSON File")]
    [SerializeField] private TextAsset jsonData;

    [Header("TargetPoint")]
    [SerializeField] private List<highlightTargetId> highlightTargets = new();

    private Dictionary<string, GuidanceNodeData> nodeMap = new();
    private HashSet<string> completedNodes = new();
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
    void LoadGuidanceNodes()
    {
        nodeMap.Clear();
        var dataList = JsonUtility.FromJson<GuidanceNodeDataList>(jsonData.text);
        foreach (var node in dataList.nodes)
            nodeMap[node.nodeId] = node;
    }

    public void TriggerNode(string nodeId)
    {
        if (completedNodes.Contains(nodeId)) return;

        if (!nodeMap.TryGetValue(nodeId, out var node)) return;

        completedNodes.Add(nodeId);
        Debug.Log($"[導引觸發] {nodeId}");

        if (node.guidanceType == "Main")
            SetCurrentNode(nodeId);
        else
            SetGuidance(node.guidanceLines);
    }

    public void SetCurrentNode(string nodeId)
    {
        if (!nodeMap.TryGetValue(nodeId, out var node)) return;

        currentNode = node;

        if (node.guidanceLines.Count > 0)
            SetGuidance(node.guidanceLines);

        if (!string.IsNullOrEmpty(node.highlightTargetId))
            HighlightMapTarget(node.highlightTargetId);
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
        }
        Hide();
        isShowing = false;
        // 若還有下一節點，自動觸發
        if (currentNode != null && currentNode.nextNodeIds.Length > 0)
        {
            string nextId = currentNode.nextNodeIds[0];
            if (!IsNodeCompleted(nextId))
            {
                Debug.Log($"[導引自動接續] → {nextId}");
                yield return new WaitForSeconds(0.5f);
                SetCurrentNode(nextId);
            }
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
        TargetManager.Instance.AddTargetRecord(textComponent.text);
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
        Debug.Log($"[小地圖提示] 高亮目標: {nodeId}");
        var target = highlightTargets.Find(t => t.targetId == nodeId);
        if (target != null)
        {
            TargetPoint.SetActive(true);
            TargetPoint.transform.position = target.position;
            TargetPoint.GetComponent<SpriteRenderer>().color = target.is1Floor ? new Color(1, 1, 1, 1) : new Color(1, 0, 0, 1);
        }
        else
        {
            Debug.LogWarning($"[小地圖提示] 找不到目標: {nodeId}");
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

    public bool IsNodeCompleted(string nodeId) => completedNodes.Contains(nodeId);
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

    #endregion
}
