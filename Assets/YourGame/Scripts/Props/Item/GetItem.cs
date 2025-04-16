using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Data;
using UnityEngine;

public class GetItem : MonoBehaviour
{
    public ItemData thisItem; // 物品數據

    public float floatingSpeed = 10f; // 浮现速度
    public float scalingSpeed = 3f; // 放大速度

    [SerializeField] private Vector3 maxsale;
    [SerializeField] public int ItemNum = 1;
    private bool isInteractable = true;
    
    private void OnEnable() {
        GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);    
    }
    private void OnDisable() {
        GetComponent<SpriteRenderer>().color = new Color(0.6f,0.6f,0.6f,1);
    }
    void Start()
    {
        transform.GetComponent<GetItem>().enabled = false;
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

        Vector3 centerPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0f));

        // 移動到目標螢幕位置
        while (Vector3.Distance(itemTransform.position, centerPosition) > 0.001f)
        {
            itemTransform.position = Vector3.MoveTowards(itemTransform.position, centerPosition, Time.deltaTime * floatingSpeed);
            yield return null;
        }

        // 放大到目標大小
        while (Mathf.Abs(itemTransform.localScale.x - maxsale.x) > 0.001f)
        {
            if (itemTransform.localScale.x < maxsale.x)
            {
                itemTransform.localScale += Vector3.one * Time.deltaTime * scalingSpeed;
            }
            else
            {
                // 如果已經達到 maxsale.x，則直接設置為 maxsale.x 並跳出迴圈
                itemTransform.localScale = new Vector3(maxsale.x, maxsale.y, maxsale.z);
                break;
            }
            yield return null;
        }

        AddNewItem();

        yield return new WaitForSeconds(1f);

        Destroy(itemTransform.gameObject);
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
