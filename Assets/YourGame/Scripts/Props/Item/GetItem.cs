using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Data;
using UnityEngine;

public class GetItem : MonoBehaviour
{
    public ItemData thisItem; // 物品數據

    public float floatingSpeed = 30f; // 浮现速度
    public float scalingSpeed = 5f; // 放大速度

    [SerializeField] private Vector3 maxsale = new Vector3(2.5f,2.5f,2.5f); // 最大缩放比例
    [SerializeField] public int ItemNum = 1;
    private bool isInteractable = true;
    
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
    
        // Disable collider
        gameObject.GetComponent<Collider2D>().enabled = false;
    
        Vector3 centerPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane + 1f));
    
        // 啟動移動和放大的協程
        IEnumerator moveCoroutine = MoveToPosition(itemTransform, centerPosition);
        IEnumerator scaleCoroutine = ScaleToSize(itemTransform, maxsale);
    
        // 同時執行兩個協程
        yield return StartCoroutine(RunCoroutines(moveCoroutine, scaleCoroutine));
    
        AddNewItem();
    
        yield return new WaitForSeconds(1f);
    

        Destroy(itemTransform.gameObject);
    }

    private IEnumerator RunCoroutines(params IEnumerator[] coroutines)
    {
        foreach (var coroutine in coroutines)
        {
            StartCoroutine(coroutine);
        }
    
        foreach (var coroutine in coroutines)
        {
            yield return coroutine;
        }
    }

    private IEnumerator MoveToPosition(Transform itemTransform, Vector3 targetPosition)
    {
        while (Vector3.Distance(itemTransform.position, targetPosition) > 0.001f)
        {
            itemTransform.position = Vector3.MoveTowards(itemTransform.position, targetPosition, Time.deltaTime * floatingSpeed);
            yield return null;
        }
    }

    private IEnumerator ScaleToSize(Transform itemTransform, Vector3 targetScale)
    {
        while (Mathf.Abs(itemTransform.localScale.x - targetScale.x) > 0.001f)
        {
            if (itemTransform.localScale.x < targetScale.x)
            {
                itemTransform.localScale += Vector3.one * Time.deltaTime * scalingSpeed;
            }
            else
            {
                itemTransform.localScale = targetScale;
                break;
            }
            yield return null;
        }

    }

    private void AddNewItem()
    {
        // 這裡可以添加你想要的物品獲取邏輯
        // 例如，將物品添加到玩家的背包中
        Debug.Log("獲得物品: " + thisItem.itemName);
        if(thisItem as WeaponData)
            InventoryItemManager.Instance.weaponBag.Add(thisItem, ItemNum);
        else if(thisItem as PropData)
            InventoryItemManager.Instance.propBag.Add(thisItem, ItemNum);
    }
}
