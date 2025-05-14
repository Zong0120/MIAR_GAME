using System.Collections;
using System.Collections.Generic;
using AirFishLab.ScrollingList.Demo;
using UnityEngine;
namespace AirFishLab.ScrollingList.Demo
{
public class DoorLock : MonoBehaviour
{
    private bool canTrigger = true;
    private bool playerInside = false;
    private float elapsedTime = 0f;
    private float doorCloseTime = 1.5f; // 關門動畫啟動時間
    private Animator DoorAnimator => GetComponentInChildren<Animator>();
    private string DoorOpenAnimator = "OpenDoor";

    private bool Locked = true;
    private LockManager _lockManager;
    [SerializeField] private Sprite question;
    [SerializeField] private string questPassword;

    [SerializeField] private GameObject targetObject;
    private Material targetMaterial;
    private float initialOpacity;
    private float targetOpacity = 0f;
    private float fadeDuration = 2f;

    private void Start()
    {
        _lockManager = LockManager.Instance.GetComponent<LockManager>();
        Renderer renderer = targetObject.GetComponent<Renderer>();
        targetMaterial = renderer.material;
        initialOpacity = targetMaterial.color.a;
    }

    public void StartFadeOut()
    {
        if (targetObject != null)
        {
            StartCoroutine(FadeOut());
        }
        else
        {
            Debug.LogError("Target object is not assigned.");
        }
    }

    // 淡去效果
    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float newOpacity = Mathf.Lerp(initialOpacity, targetOpacity, elapsedTime / fadeDuration);
            Color newColor = targetMaterial.color;
            newColor.a = newOpacity;
            targetMaterial.color = newColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Color finalColor = targetMaterial.color;
        finalColor.a = targetOpacity;
        targetMaterial.color = finalColor;

        this.enabled = false;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canTrigger) // 如果進入區域的是玩家
        {
            playerInside = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 如果離開區域的是玩家
        {
            canTrigger = true;
            playerInside = false;
            elapsedTime = 0f; // 重置計時器

            _lockManager.CloseLock();
        }
    }

    private void Update()
    {
        if (playerInside)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= doorCloseTime)
            {
                if(Locked)
                    _lockManager.OpenLock(question, questPassword,this);
                playerInside = false;
            }
        }
    }

    public void UnlockDoor()
    {
        Locked = false;
        targetObject.SetActive(false);
        StartFadeOut();
        DoorAnimator.SetBool(DoorOpenAnimator, true);
    }
}

}