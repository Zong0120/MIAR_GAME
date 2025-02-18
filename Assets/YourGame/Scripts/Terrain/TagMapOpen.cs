using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagMapOpen : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MapTile")) // 進入範圍，開啟地圖塊
        {
            other.gameObject.GetComponent<SpriteRenderer>().enabled = true; 
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("MapTile")) // 離開範圍，關閉地圖塊
        {
            other.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
