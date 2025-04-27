using UnityEngine;

public class ButtonCloseOpen : MonoBehaviour
{
    [SerializeField] private GameObject targetWindow; // 目標視窗
    [SerializeField] private GameObject otherWindow; // 其他視窗

    public void ToggleWindow()
    {
        if (!targetWindow.activeSelf)
        {
            targetWindow.SetActive(true);
            otherWindow.SetActive(false);
        }
    }
}
