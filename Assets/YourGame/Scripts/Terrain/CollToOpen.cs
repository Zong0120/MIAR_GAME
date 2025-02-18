using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollToOpen : MonoBehaviour
{
    public List<_Opens> OpenList = new List<_Opens>();
    public List<_Closes> CloseList = new List<_Closes>();
    public GameObject InEndCloseObj; // 結束後要關閉的物件
    public float delay = 0.1f; // 每次操作的延遲時間

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(OpenObjects(() => StartCoroutine(CloseObjects(() => {
                if (InEndCloseObj != null)
                {
                    InEndCloseObj.SetActive(false);
                }
            }))));
        }
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(OpenObjects(() => StartCoroutine(CloseObjects(() => {
                if (InEndCloseObj != null)
                {
                    InEndCloseObj.SetActive(false);
                }
            }))));
        }
    }

    private IEnumerator OpenObjects(System.Action callback)
    {
        foreach (var item in OpenList)
        {
            item.OpenObj.SetActive(true);
            yield return new WaitForSeconds(delay);
        }
        callback?.Invoke();
    }

    private IEnumerator CloseObjects(System.Action callback)
    {
        foreach (var item in CloseList)
        {
            item.CloseObj.SetActive(false);
            yield return new WaitForSeconds(delay);
        }
        callback?.Invoke();
    }
}

[System.Serializable]
public class _Opens
{
    public GameObject OpenObj;
}

[System.Serializable]
public class _Closes
{
    public GameObject CloseObj;
}
