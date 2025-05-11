using System.Collections;
using UnityEngine;

public class CactusAera : MonoBehaviour
{
    [SerializeField]private GameObject _cactus;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _cactus.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _cactus.SetActive(false);
        }
    }
}