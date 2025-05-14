using System.Collections;
using UnityEngine;

public class ZoneConnectDoor : MonoBehaviour
{
    [SerializeField] private MapZoneData connectedZoneUp,connectedZoneDown;
    [SerializeField] private GameObject UpMask, DownMask;
    [SerializeField] private float Y_threshold;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Animator animator;
    private Coroutine OpenGateCoroutine;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            OpenGateCoroutine = StartCoroutine(OpenGate());
        }
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            CloseGate();
        }
    }

    private IEnumerator OpenGate()
    {
        yield return new WaitForSeconds(2f);
        animator.CrossFade("Open",0.1f);

        if(playerTransform.position.y > Y_threshold)
        {
            DownMask.SetActive(false);
            MapZoneManager.Instance.ShowConnectedZone(connectedZoneDown);
            StartCoroutine(FadeOut(UpMask));
        }
        else
        {
            UpMask.SetActive(false);
            MapZoneManager.Instance.ShowConnectedZone(connectedZoneUp);
            StartCoroutine(FadeOut(DownMask));
        }
        OpenGateCoroutine = null;
    }


    public void CloseGate()
    {
        animator.CrossFade("Close",0.1f);
        if (OpenGateCoroutine != null)
        {
            StopCoroutine(OpenGateCoroutine);
            OpenGateCoroutine = null;
        }
        // 根據玩家位置判斷是否需要隱藏
        if (playerTransform.position.y > Y_threshold)
        {
            MapZoneManager.Instance.HideConnectedZone(connectedZoneDown,connectedZoneUp);
            UpMask.SetActive(true);
            UpMask.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.9f);
            DownMask.SetActive(true);
        }
        else
        {
            MapZoneManager.Instance.HideConnectedZone(connectedZoneUp,connectedZoneDown);
            DownMask.SetActive(true);
            DownMask.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.9f);
            UpMask.SetActive(true);
        }
    }

    IEnumerator FadeOut(GameObject mask)
    {
        float duration = 1f;
        float elapsedTime = 0f;

        Color color = mask.GetComponent<SpriteRenderer>().color;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0.9f, 0f, elapsedTime / duration);
            color.a = alpha;
            mask.GetComponent<SpriteRenderer>().color = color;
            yield return null;
        }
        mask.SetActive(false);
    }
}
