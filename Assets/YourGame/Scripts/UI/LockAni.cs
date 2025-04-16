using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockAni : MonoBehaviour
{
    [SerializeField] private Canvas canvas_parent;
    [SerializeField]private Canvas canvas;
    public float scalingSpeed = 1000f;
    public float fadingSpeed = 1000f;
    [SerializeField] private AudioSource chain;
    [SerializeField] private AudioSource icon_lock;

    private void lock_audio(){
        chain.Play();
        /*
        if(!HintBoxControll.Instance.isHintmeetLock)
        {
            HintBoxControll.Instance.HintmeetLock();
            HintBoxControll.Instance.isHintmeetLock = true;
        }
        */
    }


    private void Start()
    {
        if (canvas == null)
        {
            canvas = GetComponent<Canvas>();
        }

        canvas.enabled = false;
    }

    private void Update()
    {
        if (canvas.enabled)
        {
            if (canvas.transform.localScale.x > 1)
            {
                float newScale = Mathf.Lerp(canvas.transform.localScale.x, 1.8f, Time.deltaTime * scalingSpeed);
                canvas.transform.localScale = new Vector3(newScale, newScale, 1.5f);
            }

            if (canvas.GetComponent<CanvasGroup>().alpha > 0)
            {
                float newAlpha = Mathf.Lerp(canvas.GetComponent<CanvasGroup>().alpha, 1f, Time.deltaTime * fadingSpeed);
                canvas.GetComponent<CanvasGroup>().alpha = newAlpha;
            }
            else
            {
                canvas.enabled = false;
            }
        }
    }

    public void ShowCanvas()
    {
        canvas.transform.localScale = new Vector3(2.55f, 2.55f, 2.55f);

        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0.4f;

        canvas.enabled = true;
        //icon_lock.Play();
    }

    public void ParentCanvasClosed(){
        canvas_parent.enabled = false;
    }
}
