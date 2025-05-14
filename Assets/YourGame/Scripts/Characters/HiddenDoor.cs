using UnityEngine;
using System.Collections;


public class HiddenDoor : MonoBehaviour
{
    [SerializeField]private Animator animator;
    [SerializeField]private SpriteRenderer Mask;
    [SerializeField]private bool Horizontal;
    [SerializeField]private Transform playerTransform;
    private float threshold;
    private Coroutine DoorOpenCoroutine;

    private void Start() 
    {
        if(Horizontal)
        {
            threshold = transform.position.x;
        }
        else
        {
            threshold = transform.position.y;
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            DoorOpenCoroutine = StartCoroutine(OpenGate());
        }
    }
    void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            if(Horizontal)
            {
                if(playerTransform.position.x > threshold)
                {
                    if(DoorOpenCoroutine != null)
                        StopCoroutine(DoorOpenCoroutine);
                    StartCoroutine(CloseGate());
                }
                else
                {
                    if(DoorOpenCoroutine != null)
                        StopCoroutine(DoorOpenCoroutine);
                    StartCoroutine(CloseGate());
                    StartCoroutine(MaskFadeIn());
                }
            }
            else
            {
                if(playerTransform.position.y < threshold)
                {
                    if(DoorOpenCoroutine != null)
                        StopCoroutine(DoorOpenCoroutine);
                    StartCoroutine(CloseGate());
                    StartCoroutine(MaskFadeIn());
                }
                else
                {
                    if(DoorOpenCoroutine != null)
                        StopCoroutine(DoorOpenCoroutine);
                    StartCoroutine(CloseGate());
                }
            }
        }
    }

    private IEnumerator MaskFadeOut()
    {
        float fadeDuration = 0.5f;
        float elapsedTime = 0f;

        Color startColor = Mask.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            Mask.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            yield return null;
        }
        Mask.gameObject.SetActive(false);
    }
    private IEnumerator MaskFadeIn()
    {
        Mask.gameObject.SetActive(true);
        float fadeDuration = 0.5f;
        float elapsedTime = 0f;

        Color startColor = Mask.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            Mask.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            yield return null;
        }
    }

    private IEnumerator OpenGate()
    {
        yield return new WaitForSeconds(2f);
        animator.CrossFade("HDoorOpen",0.1f);
        if(Horizontal)
        {
            if(playerTransform.position.x < threshold)
            {
                StartCoroutine(MaskFadeOut());
            }
        }
        else
        {
            if(playerTransform.position.y < threshold)
            {
                StartCoroutine(MaskFadeOut());
            }
        }
        DoorOpenCoroutine = null;
    }
    private IEnumerator CloseGate()
    {
        yield return new WaitForSeconds(0.5f);
        animator.CrossFade("HDoorClose",0.1f);
    }
}
