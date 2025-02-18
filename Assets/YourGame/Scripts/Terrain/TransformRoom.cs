using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformRoom : MonoBehaviour
{
    [SerializeField]private SpriteRenderer spriteRenderer;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            spriteRenderer.enabled = true;
        
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            spriteRenderer.enabled = false;
    }
}
