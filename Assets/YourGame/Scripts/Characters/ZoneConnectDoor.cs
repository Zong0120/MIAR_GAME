using System.Collections;
using UnityEngine;

public class ZoneConnectDoor : MonoBehaviour
{
    [SerializeField] private MapZoneData connectedZoneUp,connectedZoneDown;
    [SerializeField] private GameObject UpMask, DownMask;
    [SerializeField] private float Y_threshold;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Animator animator;
    private bool isOpen = false;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            OpenGate();
        }
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            CloseGate();
        }
    }

    public void OpenGate()
    {
        if (isOpen) return;
        isOpen = true;

        animator.SetBool("Close", false);
        animator.CrossFade("Open",0.1f);

        if(playerTransform.position.y > Y_threshold)
        {
            MapZoneManager.Instance.ShowConnectedZone(connectedZoneDown);
            FadeOut(UpMask);
        }
        else
        {
            MapZoneManager.Instance.ShowConnectedZone(connectedZoneUp);
            FadeOut(DownMask);
        }
    }

    public void CloseGate()
    {
        if (!isOpen) return;
        isOpen = false;

        animator.SetBool("Close", true);

        // 根據玩家位置判斷是否需要隱藏
        if (playerTransform.position.y > Y_threshold)
        {
            MapZoneManager.Instance.HideConnectedZone(connectedZoneDown,connectedZoneUp);
        }
        else
        {
            MapZoneManager.Instance.HideConnectedZone(connectedZoneUp,connectedZoneDown);
        }
    }

    IEnumerator FadeOut(GameObject mask)
    {
        mask.SetActive(true);
        float duration = 3f;
        float elapsedTime = 0f;

        Color color = mask.GetComponent<SpriteRenderer>().color;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            color.a = alpha;
            mask.GetComponent<SpriteRenderer>().color = color;
            yield return null;
        }
        mask.SetActive(false);
    }
}
