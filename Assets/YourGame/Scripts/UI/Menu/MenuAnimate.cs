using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MenuAnimate : MonoBehaviour
{
    public List<RototingGear> imagelint;

    private Coroutine rotationCoroutine;

    private void OnEnable() 
    {
        // 啟動旋轉協程
        rotationCoroutine = StartCoroutine(AnimateMenu());
    }

    private void OnDisable()
    {
        // 停止旋轉協程，避免物件被停用後協程仍在執行
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
            rotationCoroutine = null;
        }
    }

    IEnumerator AnimateMenu()
    {
        while (true) // 持續執行
        {
            for (int i = 0; i < imagelint.Count; i++)
            {
                if (imagelint[i] != null && imagelint[i].image != null)
                {
                    imagelint[i].image.transform.Rotate(0, 0, imagelint[i].rotationSpeed * Time.deltaTime);
                }
            }
            yield return null; // 每幀執行一次
        }
    }
}

[System.Serializable]
public class RototingGear
{
    public Image image;
    public float rotationSpeed;
}