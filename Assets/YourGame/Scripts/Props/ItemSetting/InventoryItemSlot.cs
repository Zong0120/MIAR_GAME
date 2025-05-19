using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryItemSlot : MonoBehaviour
{
    private InventoryItem item;
    public ItemData itemData{ get; private set; }
    [SerializeField]private Image itemImage;
    [SerializeField]private TextMeshProUGUI itemCountText;
    [SerializeField]private GameObject itemInfoPanel;
    [SerializeField]private Image cooldownImage;
    public void InitSlot()
    {
        item = null;
        itemData = null;
        itemImage.sprite = null;
        itemCountText.text = "";
        itemInfoPanel.SetActive(false);
    }
    void OnEnable()
    {
        if(item != null)
        {
            if(item.IsOnCooldown)
                cooldownImage.gameObject.SetActive(true);
            else
                cooldownImage.gameObject.SetActive(false);
        }
        else
            InitSlot();
    }

    void Update()
    {
        if(item!=null && item.IsOnCooldown)
        {
            cooldownImage.fillAmount = item.cooldownRemaining;
        }
    }

    public void SetItem(InventoryItem _item)
    {
        item = _item;
        itemData = _item.itemData;
        itemImage.sprite = itemData.itemImage;
        ReplaceImage();
        if(_item.itemData.restrictedItem)
            itemCountText.text = item.currentCount.ToString();
        else
            itemCountText.text = "";
        itemInfoPanel.SetActive(true);
    }
    void ReplaceImage()
    {
        Sprite newSprite = itemData.itemImage;
        float aspectRatio = itemImage.sprite.rect.width / newSprite.rect.height;

        itemImage.sprite = newSprite;

        RectTransform rectTransform = itemImage.rectTransform;

        // 根据宽高比例调整大小
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 70f * aspectRatio);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 70f);
    }

    public void OnClickSlot()
    {
        if(itemData != null)
        {
            if(itemData as WeaponData)
                InventoryItemManager.Instance.weaponBag.UpdateItemDisplayInfo(itemData);
            else if(itemData as PropData)
                InventoryItemManager.Instance.propBag.UpdateItemDisplayInfo(itemData);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemData != null)
        {
            itemInfoPanel.SetActive(true);
            
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        itemInfoPanel.SetActive(false);
    }
}