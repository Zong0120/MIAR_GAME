using System.Collections;
using System.Collections.Generic;
using AirFishLab.ScrollingList.Demo;
using UnityEngine;

public class LockAni : MonoBehaviour
{
    [SerializeField] private LockManager _lockmanager;
    [SerializeField]private Canvas canvas;
    public float scalingSpeed = 1000f;
    public float fadingSpeed = 1000f;

    private Animator _animator => GetComponent<Animator>();

    public void lock_chain_sound(){
        SoundManager.PlaySoundItemAudio(SoundType.Lock,"Lock_Chain");
    }

    private void OnEnable()
    {
        _animator.SetBool("unclocked",false);
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
        canvasGroup.alpha = 0.4f;

        canvas.enabled = true;
        SoundManager.PlaySoundItemAudio(SoundType.Lock, "Lock_Iron");
    }

    public void Unclocked()
    {
        _animator.SetBool("unclocked", true);
    }

    public void ParentCanvasClosed(){
        _lockmanager.enabled = false;
    }
}
