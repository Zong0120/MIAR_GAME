using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class EquipManager : MonoBehaviour
{
    public static EquipManager Instance { get; private set; }

    public CurrentEquipped[] currentEquipped= new CurrentEquipped[2];
    private int currentItemIndex=0;
    private int previousItemIndex=1;

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    void Update()
    {
        if(currentEquipped[currentItemIndex]._item != null)
        {
            currentEquipped[currentItemIndex].Cooldown();
        }
    }

    public void Equip(InventoryItem item)
    {
        if (item == null || item.itemData == null) return;
        // 檢查是否已經裝備
        if (item.isEquipped)return;
        
        if(currentEquipped[previousItemIndex]._item==null)
        {
            currentEquipped[previousItemIndex]._item = item;
            item.isEquipped = true;
        }
        else
        {
            currentEquipped[previousItemIndex]._item.isEquipped = false;
            currentEquipped[previousItemIndex]._item = item;
            item.isEquipped = true;
        }
        currentEquipped[previousItemIndex].SetItem(item);
        currentItemIndex = previousItemIndex;
        previousItemIndex = (previousItemIndex==1)?0:1 ;
        
        // 根據裝備產生道具控制器
        Instantiate(item.itemData.itemPrefab);
    }

    public void UseEuip()
    {
        if(currentEquipped[currentItemIndex]._item != null)
        {
            if(currentEquipped[currentItemIndex]._item.IsOnCooldown) return;

            Debug.Log("使用道具01: " + currentEquipped[currentItemIndex]._item.itemData.itemName);

            CooldownManager.Instance.StartCooldown(currentEquipped[currentItemIndex]._item);

            Debug.Log("使用道具02: " + currentEquipped[currentItemIndex]._item.itemData.itemName);
        }
    }
}

[System.Serializable]
public class CurrentEquipped
{
    public InventoryItem _item;
    [SerializeField]private UnityEngine.UI.Image itemImage;
    [SerializeField]private TextMeshProUGUI itemNumberText;
    [SerializeField]private UnityEngine.UI.Image cooldownMaskImage;
    [SerializeField]private GameObject HilightImage;

    public void SetItem(InventoryItem item)
    {
        _item = item;
        if (_item != null)
        {
            itemImage.sprite = _item.itemData.itemImage;
            itemImage.gameObject.SetActive(true);
            if(_item.itemData.restrictedItem)
                itemNumberText.text = _item.currentCount.ToString();
            else
                itemNumberText.text = "";
            HilightImage.SetActive(true);
        }
        else
        {
            itemImage.gameObject.SetActive(false);
            itemImage.sprite = null;
            itemNumberText.text = "";
            HilightImage.SetActive(false);
        }
    }

    public void Cooldown()
    {
        if(_item.IsOnCooldown)
        {
            //1-0
            cooldownMaskImage.fillAmount = _item.cooldownRemaining;
        }
    }
}
