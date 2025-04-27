using UnityEngine;

[CreateAssetMenu(fileName = "InheritanceInventoryItem", menuName = "Inventory/InheritanceInventoryItem", order = 0)]
public class InheritanceInventoryItem : ScriptableObject
{
    public InheritanceItemData[] _itemData = new InheritanceItemData[4]; // 物品數據

    public void InitAllInheritanceInventoryItem()
    {
        for (int i = 0; i < _itemData.Length; i++)
        {
            _itemData[i] = new InheritanceItemData();
        }
    }
}

[System.Serializable]
public class InheritanceItemData
{
    public ItemData _itemData; // 物品數據
    private int itemCount; // 物品數量

    public void InitInheritanceItemData(ItemData itemData = null, int itemCount = 0)
    {
        if(itemData == null)
        {
            _itemData = null;
            itemCount = 0;
        }
        else
        {
            _itemData = itemData;
            this.itemCount = itemCount;
        }
    }

    public int AddItemCount(int count)
    {
        itemCount += count;
        return itemCount;
    }

    public int GetItemCount()
    {
        return itemCount;
    }
}