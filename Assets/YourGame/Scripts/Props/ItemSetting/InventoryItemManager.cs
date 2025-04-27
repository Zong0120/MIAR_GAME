using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class InventoryItemManager : MonoBehaviour
{
    public static InventoryItemManager Instance { get; private set; }

    public BagInventoryItem weaponBag,propBag;
    public StoryTextInventory storyZoneBag;
    public StoryTextInventory storyChapterBag;

    [SerializeField]private GameObject BagUIRoot;
    private Canvas _canvas => GetComponent<Canvas>();

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    void Start()
    {
        weaponBag.UseButton.GetComponent<Button>().onClick.AddListener(weaponBag.EquipItem);
        propBag.UseButton.GetComponent<Button>().onClick.AddListener(propBag.EquipItem);
        BagUIRoot.SetActive(false);   
    }

    public bool BagIsOpen()
    {
        return BagUIRoot.activeSelf;
    }

    public void OpenBag()
    {
        BagUIRoot.SetActive(true);
    }

    public void CloseBag()
    {
        BagUIRoot.SetActive(false);
    }
}

[System.Serializable]
public class BagInventoryItem
{
    public List<InventoryItem> items= new();
    public List<GameObject> itemSlots;
    [SerializeField]private Image itemImage;
    [SerializeField]private TextMeshProUGUI itemInfoText;
    [SerializeField]private TextMeshProUGUI itemDescribeCodeText;
    public GameObject UseButton;
    private InventoryItem currentItem;
    public void Add(ItemData item,int count =1)
    {
        for(int indext = 0;indext<items.Count;indext++)
        {
            if (items[indext].itemData.itemName == item.itemName)
            {
                items[indext].currentCount += count;
                itemSlots[indext].GetComponent<InventoryItemSlot>().SetItem(items[indext]);
                return;
            }
        }
        items.Add(new InventoryItem(item, count));
        itemSlots[items.Count-1].GetComponent<InventoryItemSlot>().SetItem(items[items.Count-1]);

        Debug.Log("Add New Item: " + item.itemName);
    }

    public void Remove(ItemData item, int count = 1)
    {
        for (int indext = 0; indext < items.Count; indext++)
        {
            if (items[indext].itemData.itemName == item.itemName)
            {
                items[indext].currentCount -= count;
                if (items[indext].currentCount <= 0)
                {
                    items.RemoveAt(indext);
                    itemSlots[indext].GetComponent<InventoryItemSlot>().InitSlot();
                }
                else
                {
                    itemSlots[indext].GetComponent<InventoryItemSlot>().SetItem(items[indext]);
                }
                return;
            }
        }
    }

    public void RemoveInventoryItem(InventoryItem item)
    {
        for (int indext = 0; indext < items.Count; indext++)
        {
            if (items[indext].itemData.itemName == item.itemData.itemName)
            {
                itemSlots[indext].GetComponent<InventoryItemSlot>().InitSlot();
                EquipManager.Instance.RemoveEquip(item);
                items.RemoveAt(indext);
                ClearItemDisplayInfo();
                return;
            }
        }
    }

    public void UpdateItemDisplayInfo(ItemData item)
    {
        itemInfoText.text = item.itemDescription;
        itemDescribeCodeText.text = item.itemCode;
        itemImage.sprite = item.itemImage;
        itemImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150f*(item.itemImage.rect.width/item.itemImage.rect.height));
        itemImage.enabled = true;

        currentItem = items.Find(x => x.itemData.itemName == item.itemName);
        if(currentItem.IsOnCooldown || currentItem.isEquipped)
        {
            UseButton.SetActive(false);
            currentItem = null;
        }
        else
            UseButton.SetActive(true);
    }

    public void EquipItem()
    {
        EquipManager.Instance.Equip(currentItem);
        UseButton.SetActive(false);
        currentItem = null;
    }
    public void ClearItemDisplayInfo()
    {
        itemInfoText.text = "";
        itemDescribeCodeText.text = "";
        itemImage.sprite = null;
        itemImage.enabled = false;
        currentItem = null;
        UseButton.SetActive(false);
    }
}

[System.Serializable]
public class StoryTextInventory
{
    public List<StoryButtonSlot> storySlots;
    private int currentIndex = 0;

    public void Add(StoryText story)
    {
        storySlots[currentIndex].gameObject.SetActive(true);
        storySlots[currentIndex].SetStory(story);
        currentIndex++;
        Debug.Log("Add New Inventory Story: " + story.title);
    }
}