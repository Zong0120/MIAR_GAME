using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField] private Material whiteFlashMat;
    [SerializeField] private float restoreDefaultMatTime = 0.2f;

    private Material defaultMat;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMat = spriteRenderer.material;
    }

    public IEnumerator FlashRoutine()
    {
        spriteRenderer.material = whiteFlashMat;
        yield return new WaitForSeconds(restoreDefaultMatTime);
        spriteRenderer.material = defaultMat;
    }

    public IEnumerator FlashRoutine(float time)
    {
        spriteRenderer.material = whiteFlashMat;
        yield return new WaitForSeconds(time);
        spriteRenderer.material = defaultMat;
    }

    public float GetRestoreDefaultMatTime()
    {
        return restoreDefaultMatTime;
    }
}
