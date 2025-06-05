using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using AirFishLab.ScrollingList.Demo;


public class InventoryItemManager : MonoBehaviour
{
    public static InventoryItemManager Instance { get; private set; }

    public BagInventoryItem weaponBag,propBag;
    public StoryTextInventory storyZoneBag;
    public StoryTextInventory storyChapterBag;

    [SerializeField] private GameObject BagUIRoot;
    [SerializeField]private GameObject BigMapCamera;
    
    //special item
    public InventoryItem _decoder()=>propBag.GetInventoryItem("decoder");
    public InventoryItem _bullet()=>weaponBag.GetInventoryItem("bullet");

    public bool BagIsEmpty()
    {
        if (weaponBag.items.Count == 0 && propBag.items.Count == 0)
            return true;
        else
            return false;
    }
    public bool haveItem(string name)
    {
        for (int i = 0; i < propBag.items.Count; i++)
        {
            if (propBag.items[i].itemData.itemName == name)
                return true;
        }
        for (int i = 0; i < weaponBag.items.Count; i++)
        {
            if (weaponBag.items[i].itemData.itemName == name)
                return true;
        }
        return false;
    }


    public event Action HintBackpackOpen;
    public event Action HintItemEquip;

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
        SoundManager.PlaySoundItemAudio(SoundType.UI, "UI_Button");
        StartCoroutine(UpdateBigMap());
        TimerManager.Instance.BagTimePause = true;
        if (HintItemEquip != null)
        {
            if (weaponBag.items.Count == 1 && propBag.items.Count == 0 && haveItem("bullet")) return;
            HintItemEquip.Invoke();
        }
    }

    public void CloseBag()
    {
        BagUIRoot.SetActive(false);
        SoundManager.PlaySoundItemAudio(SoundType.UI, "UI_Button");
        TimerManager.Instance.BagTimePause = false;
    }

    public void OpenMap()
    {
        OpenBag();
        BagPageManager.Instance.ToPage(2);
    }

    private IEnumerator UpdateBigMap()
    {
        BigMapCamera.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        BigMapCamera.SetActive(false);
    }

    public void AddItem(ItemData item, int count = 1)
    {
        if (item as WeaponData)
        {
            weaponBag.Add(item, count);
        }
        else if (item as PropData)
        {
            propBag.Add(item, count);
        }
        
        HintBackpackOpen?.Invoke();
    }

    public void RemoveItemCount(ItemData item, int count = 1)
    {
        if (item as WeaponData)
        {
            weaponBag.Remove(item, count);
        }
        else if (item as PropData)
        {
            propBag.Remove(item, count);
        }

    }

    public void RemoveRandomItem()
    {
        // 隨機選擇背包
        int randomBag = UnityEngine.Random.Range(0, 2);
    
        // 根據隨機選擇處理武器背包或道具背包
        if (randomBag == 0 && weaponBag.items.Count > 0)
        {
            RemoveRandomItemFromBag(weaponBag);
        }
        else if (propBag.items.Count > 0)
        {
            RemoveRandomItemFromPropBag();
        }
        else if(weaponBag.items.Count > 0) RemoveRandomItemFromBag(weaponBag);
    }
    
    private void RemoveRandomItemFromBag(BagInventoryItem bag)
    {
        int randomIndex = UnityEngine.Random.Range(0, bag.items.Count);
        InventoryItem item = bag.items[randomIndex];
        bag.RemoveInventoryItem(item);
    }
    
    private void RemoveRandomItemFromPropBag()
    {
        int randomIndex = UnityEngine.Random.Range(0, propBag.items.Count);
        InventoryItem item = propBag.items[randomIndex];
        // 如果是 "decoder"，檢查是否需要跳過或重新選擇
        if (item.itemData.name == "decoder")
        {
            if (propBag.items.Count == 1)
            {
                // 唯一的道具是 "decoder"，且武器背包也空，則不移除
                if(weaponBag.items.Count == 0)
                    return;
                else RemoveRandomItemFromBag(weaponBag);
            }
            else
            {
                // 隨機選擇另一個道具
                int newRandomIndex;
                do
                {
                    newRandomIndex = UnityEngine.Random.Range(0, propBag.items.Count);
                } while (newRandomIndex == randomIndex);
                item = propBag.items[newRandomIndex];
                propBag.RemoveInventoryItem(item);
                return;
            }
        }
        // 移除非 "decoder" 的道具
        propBag.RemoveInventoryItem(item);
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
        for(int index = 0; index < itemSlots.Count; index++)
        {
            if (itemSlots[index].GetComponent<InventoryItemSlot>().itemData==null)
            {
                itemSlots[index].GetComponent<InventoryItemSlot>().SetItem(items[items.Count-1]);
                Debug.Log("Add New Item: " + item.itemName);
                break;
            }
        }
        //itemSlots[items.Count-1].GetComponent<InventoryItemSlot>().SetItem(items[items.Count-1]);
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
                    itemSlots[indext].GetComponent<InventoryItemSlot>().InitSlot();
                    if(items[indext].isEquipped)
                    {
                        EquipManager.Instance.RemoveEquip(items[indext]);
                    }
                    items.RemoveAt(indext);
                }
                else
                {
                    itemSlots[indext].GetComponent<InventoryItemSlot>().SetItem(items[indext]);
                    if (items[indext].isEquipped)
                    {
                        EquipManager.Instance.UpdateEquipCount(items[indext]);
                    }
                }
                return;
            }
        }
    }

    public InventoryItem GetInventoryItem(string name)
    {
        for (int indext = 0; indext < items.Count; indext++)
        {
            if (items[indext].itemData.itemName == name)
            {
                return items[indext];
            }
        }
        return null;
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
        if(currentItem.IsOnCooldown || currentItem.isEquipped||item.itemName == "bullet")
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
        //Debug.Log("Add New Inventory Story: " + story.title);
    }
}