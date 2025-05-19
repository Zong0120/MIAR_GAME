using System.Collections;
using UnityEngine;
using PlayerInputAction;

public class GetStoryChapter : MonoBehaviour
{
    public string _storySpawnID;
    public float floatingSpeed = 30f; // 浮现速度
    private bool isInteractable = true;
    private GameObject player => GameObject.FindGameObjectWithTag("Player");
    
    private void OnEnable() {
        GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);    
    }
    private void OnDisable() {
        GetComponent<SpriteRenderer>().color = new Color(0.6f,0.6f,0.6f,1);
    }
    void Update()
    {
        if (isInteractable)
        {
            StartCoroutine(FloatingEffect(transform));
        }
    }

    IEnumerator FloatingEffect(Transform itemTransform)
    {
        isInteractable = false;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 99;
    
        // 啟動移動和放大的協程
        IEnumerator moveCoroutine = MoveToPosition(itemTransform, player.transform.position);
    
        StartCoroutine(moveCoroutine);
    
    
        yield return new WaitForSeconds(1f);

        PlayerController.Instance.ReadChapter(_storySpawnID);
        Destroy(itemTransform.gameObject);
    }

    private IEnumerator MoveToPosition(Transform itemTransform, Vector3 targetPosition)
    {
        while (Vector3.Distance(itemTransform.position, targetPosition) > 0.001f)
        {
            itemTransform.position = Vector3.MoveTowards(itemTransform.position, targetPosition, Time.deltaTime * floatingSpeed);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}