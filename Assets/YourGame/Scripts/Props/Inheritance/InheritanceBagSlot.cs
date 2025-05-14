using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InheritanceBagSlot : MonoBehaviour
{
    private InventoryItem item;
    private ItemData itemData;
    private InheritanceManager _inheritanceManager => InheritanceManager.Instance;
    [SerializeField]private Image itemImage;
    [SerializeField]private TextMeshProUGUI itemCountText;
    [SerializeField]private GameObject itemInfoPanel;
    [SerializeField]private Image cooldownImage;
    public void InitSlot()
    {
        itemData = null;
        itemImage.sprite = null;
        itemCountText.text = "";
        itemInfoPanel.SetActive(false);
    }
    void OnEnable()
    {
        if(item != null)
            cooldownImage.fillAmount = item.cooldownRemaining;
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

    public void OnBtnClicked()
    {
        if (item != null && itemData != null)
        {
            if(itemData as WeaponData)
            {
                if(_inheritanceManager.InheritanceWeapon.CanAddNewInheritanceItem(item))
                {
                    InitSlot();
                    InventoryItemManager.Instance.weaponBag.RemoveInventoryItem(item);
                }
            }
            else if(itemData as PropData)
            {
                if(_inheritanceManager.InheritanceProp.CanAddNewInheritanceItem(item))
                {
                    InitSlot();
                    InventoryItemManager.Instance.propBag.RemoveInventoryItem(item);
                }
            }
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
        if(_item.IsOnCooldown)cooldownImage.gameObject.SetActive(true);
        else cooldownImage.gameObject.SetActive(false);
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

}