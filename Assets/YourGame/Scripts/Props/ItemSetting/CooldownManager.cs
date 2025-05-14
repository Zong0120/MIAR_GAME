using System.Collections.Generic;
using UnityEngine;

public class CooldownManager : MonoBehaviour
{
    public static CooldownManager Instance { get; private set; }
    private readonly List<InventoryItem> cooldownDict = new();
    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    public void StartCooldown(InventoryItem item)
    {
        if (!cooldownDict.Contains(item))
        {
            cooldownDict.Add(item);
            item.StartCooldown();
        }
    }

    void Update()
    {
        for (int i = cooldownDict.Count - 1; i >= 0; i--)
        {
            if (cooldownDict[i].IsOnCooldown)
            {
                cooldownDict[i].UpdateCooldown(Time.deltaTime);
            }
            else
            {
                if(cooldownDict[i].itemData.restrictedItem)
                {
                    InventoryItemManager.Instance.RemoveItemCount(cooldownDict[i].itemData);
                }
                cooldownDict.RemoveAt(i);
            }
        }
    }
}
