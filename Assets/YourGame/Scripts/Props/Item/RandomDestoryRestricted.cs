using UnityEngine;
using System.Collections;

public class RandomDestoryRestricted : MonoBehaviour
{
    public float percent = 0.5f; // 50% 機率
    void Start()
    {
        float randomValue = Random.Range(0f, 1f);
        // 如果隨機值小於設定的百分比，則銷毀物件
        if (randomValue < percent)
        {
            Destroy(gameObject);
        }
    }

}
