using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TargetManager : MonoBehaviour
{
    public static TargetManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private GameObject targetPanel;
    [SerializeField] private RectTransform _ScrollViewContent;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }
    public void ClearTargetRecord()
    {
        targetText.text = "";
    }

    public void AddTargetRecord(string target)
    {
        targetText.text = targetText.text +"\n"+ target;
        // 獲取文字內容的高度
        float textHeight = targetText.preferredHeight;
        if(textHeight <= 500)
        {
            textHeight = 500;
        }

        // 設置 ScrollView Content 的高度
        Vector2 newSize = _ScrollViewContent.sizeDelta;
        newSize.y = textHeight;
        _ScrollViewContent.sizeDelta = new Vector2(_ScrollViewContent.sizeDelta.x, 505.28f + (textHeight - 500f));
    }

    public void OpenTarget()
    {
        if (targetPanel.activeSelf)
        {
            targetPanel.SetActive(false);
            return;
        }
        targetPanel.SetActive(true);
    }
}
