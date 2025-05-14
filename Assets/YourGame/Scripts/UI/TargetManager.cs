using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TargetManager : MonoBehaviour
{
    public static TargetManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private GameObject targetPanel;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    public void AddTargetRecord(string target)
    {
        targetText.text = targetText.text +"\n"+ target;
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
