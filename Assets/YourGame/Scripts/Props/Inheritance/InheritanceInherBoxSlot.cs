using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InheritanceInherBoxSlot : MonoBehaviour
{
    private InheritanceItemData item;
    private ItemData itemData;
    private InheritanceManager _inheritanceManager => InheritanceManager.Instance;
    [SerializeField]private TextMeshProUGUI itemCountText;
    [SerializeField]private Image itemImage;
    [SerializeField]private GameObject itemInfoPanel;
    public void InitSlot()
    {
        itemData = null;
        itemImage.sprite = null;
        itemInfoPanel.SetActive(false);
    }
    void OnEnable()
    {
        if(itemData == null)
            InitSlot();
    }

    public void OnBtnClicked()
    {
        if (item != null && itemData != null)
        {
            if(itemData as WeaponData)
            {
                _inheritanceManager.InheritanceWeapon.RemoveInheritanceItem(item);
                InitSlot();
                _inheritanceManager.InheritanceWeaponRefresh();
            }
            else if(itemData as PropData)
            {
                _inheritanceManager.InheritanceProp.RemoveInheritanceItem(item);
                InitSlot();
                _inheritanceManager.InheritancePropRefresh();
            }
        }
    }
    

    public void SetItem(InheritanceItemData _item)
    {
        item = _item;
        itemData = _item._itemData;
        itemImage.sprite = itemData.itemImage;
        if(itemData.restrictedItem)
            itemCountText.text = item.GetItemCount().ToString();
        else
            itemCountText.text = "";
        ReplaceImage();
        
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

}