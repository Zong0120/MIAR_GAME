using UnityEngine;
using UnityEngine.UI;

public class AlphaHitTest : MonoBehaviour
{
    [Range(0f, 1f)]
    public float alphaThreshold = 0.05f; // 透明度閾值（0.1 表示 10% 透明度以上才可點擊）
    void Start()
    {
        Image img = GetComponent<Image>();
        if (img != null && img.sprite != null)
        {
            img.alphaHitTestMinimumThreshold = alphaThreshold;
        }
    } 

    public void TestClick()
    {
        Debug.Log("tset");
    }
}
