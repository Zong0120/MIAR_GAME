using System.Collections;
using UnityEngine;

public class GetStoryZone : MonoBehaviour
{
    public StoryZone thisStory; // 物品數據
    public float floatingSpeed = 30f; // 浮现速度
    private bool isInteractable = true;
    private GameObject player => GameObject.FindGameObjectWithTag("Player");

    public void SetStory(StoryZone story)
    {
        thisStory = story;
    }
    
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
    
    
        AddNewItem();

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

    private void AddNewItem()
    {
        //Debug.Log("獲得物品: " + thisStory.title);
        StoryManager.Instance.UnlockZone(thisStory);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
