using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Burst.CompilerServices;

public class EquipManager : MonoBehaviour
{
    public static EquipManager Instance { get; private set; }

    public CurrentEquipped[] currentEquipped= new CurrentEquipped[2];
    [SerializeField]private Transform itemSwapPos;
    private int currentItemIndex=0;
    private int previousItemIndex=1;

    public event System.Action HintItemUse;
    public event System.Action HintItemSwitch;

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
        if(currentEquipped[previousItemIndex]._item != null)
        {
            currentEquipped[previousItemIndex].Cooldown();
        }
    }

    public void Equip(InventoryItem item)
    {
        if (item == null || item.itemData == null) return;
        // 檢查是否已經裝備
        if (item.isEquipped) return;

        if (currentEquipped[previousItemIndex]._item == null)
        {
            currentEquipped[previousItemIndex]._item = item;
            item.isEquipped = true;

        }
        else
        {
            currentEquipped[previousItemIndex]._item.isEquipped = false;
            currentEquipped[previousItemIndex]._item = item;
            item.isEquipped = true;
            HintItemSwitch?.Invoke();
        }
        currentItemIndex = previousItemIndex;
        previousItemIndex = (previousItemIndex == 1) ? 0 : 1;

        // 將 itemPrefab 作為子物件創建在 itemSwapPos 節點下
        GameObject instantiatedObject = Instantiate(item.itemData.itemPrefab, itemSwapPos);

        currentEquipped[currentItemIndex].SetItem(item, instantiatedObject.GetComponent<MonoBehaviour>());
        currentEquipped[previousItemIndex].HilightItemClose();
        InventoryItemManager.Instance.CloseBag();

        HintItemUse?.Invoke();
    }

    public void UpdateEquipCount(InventoryItem item)
    {
        if(currentEquipped[0]._item == item)
        {
            currentEquipped[0].UpdateItemCount();
        }
        else if(currentEquipped[1]._item == item)
        {
            currentEquipped[1].UpdateItemCount();
        }
    }

    public void RemoveEquip(InventoryItem item)
    {
        if (item == null || item.itemData == null) return;
        // 檢查是否已經裝備
        if (!item.isEquipped) return;

        item.isEquipped = false;
        if(currentEquipped[currentItemIndex]._item == item)
        {
            previousItemIndex = currentItemIndex;
            currentItemIndex = (currentItemIndex == 1) ? 0 : 1;
        }
        currentEquipped[previousItemIndex]._item = null;
        currentEquipped[previousItemIndex].ClearItem();
        currentEquipped[currentItemIndex].HilightItemOpen();
    }

    public void SwitchEquipIndex(int index)
    {
        if(currentItemIndex == index) return;
        if(currentEquipped[index]._item == null) return;

        SwitchEquipIndex();
    }

    public void SwitchEquipIndex()
    {
        if(currentEquipped[previousItemIndex]._item == null) return;

        currentEquipped[currentItemIndex].HilightItemClose();
        currentEquipped[previousItemIndex].HilightItemOpen();

        currentItemIndex = previousItemIndex;
        previousItemIndex = (previousItemIndex==1)?0:1 ;
    }

    public void UseEquip()
    {
        if(currentEquipped[currentItemIndex]._item != null)
        {
            currentEquipped[currentItemIndex].UseItem();
        }
    }

    public void EquipStartCooldown()
    {
        if(currentEquipped[currentItemIndex]._item != null)
        {
            currentEquipped[currentItemIndex].ItemStartCooldown();
        }
    }
}

[System.Serializable]

public class CurrentEquipped
{
    public InventoryItem _item;
    private MonoBehaviour _itemObject;
    [SerializeField]private UnityEngine.UI.Image itemImage;
    [SerializeField]private TextMeshProUGUI itemNumberText;
    [SerializeField]private UnityEngine.UI.Image cooldownMaskImage;
    [SerializeField]private GameObject HilightImage;

    public void SetItem(InventoryItem item,MonoBehaviour itemObject)
    {
        _item = item;
        itemImage.sprite = _item.itemData.itemImage;
        itemImage.gameObject.SetActive(true);
        if(_item.itemData.restrictedItem)
            itemNumberText.text = _item.currentCount.ToString();
        else
            itemNumberText.text = "";
        HilightImage.SetActive(true);
        cooldownMaskImage.fillAmount = 0;

        if(_itemObject != null)
            UnityEngine.Object.Destroy(_itemObject.gameObject);
        _itemObject = itemObject;
        if(_item.itemData as WeaponData)
        {
            _itemObject.GetComponent<WeaponItem>().SetWeaponData((_item.itemData as WeaponData));
        }
    }

    public void UseItem()
    {
        if(_item.IsOnCooldown) return;

        (_itemObject as IUseable).Use();
    }

    public void UpdateItemCount()
    {
        if(_item.itemData.restrictedItem)
        {
            itemNumberText.text = _item.currentCount.ToString();
        }
    }

    public void ItemStartCooldown()
    {
        if(_item.IsOnCooldown) return;

        CooldownManager.Instance.StartCooldown(_item);
    }

    public void ClearItem()
    {
        _item = null;
        itemImage.sprite = null;
        itemNumberText.text = "";
        cooldownMaskImage.fillAmount = 0;
        itemImage.gameObject.SetActive(false);
        HilightImage.SetActive(false);
        if(_itemObject != null)
            UnityEngine.Object.Destroy(_itemObject.gameObject);
    }

    public void HilightItemOpen()
    {
        if (_item != null)
        {
            HilightImage.SetActive(true);
            itemImage.gameObject.SetActive(true);
            _itemObject.gameObject.SetActive(true);
        }
    }

    public void HilightItemClose()
    {
        HilightImage.SetActive(false);
        if (_item == null)
            itemImage.gameObject.SetActive(false);
        else _itemObject.gameObject.SetActive(false);
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
