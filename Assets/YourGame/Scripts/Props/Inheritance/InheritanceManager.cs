using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class InheritanceManager : MonoBehaviour
{
    public static InheritanceManager Instance { get; private set; }
    public InheritanceItem InheritanceWeapon, InheritanceProp;

    private InventoryItem[] WeaponBagInventoryItems,
        PropBagInventoryItems;
    [SerializeField] private GameObject InherUIRoot;
    private InventoryItemManager _inventoryItemManager => InventoryItemManager.Instance;
    private Canvas canvas => GetComponent<Canvas>();
    private CanvasGroup canvasGroup => GetComponent<CanvasGroup>();


    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    public void InventoryRefresh()
    {
        WeaponBagInventoryItems = _inventoryItemManager.weaponBag.items.ToArray();
        PropBagInventoryItems = _inventoryItemManager.propBag.items.ToArray();
    }

    public void InheritanceWeaponRefresh()
    {
        WeaponBagInventoryItems = _inventoryItemManager.weaponBag.items.ToArray();
        InheritanceWeapon.SetBagItem(WeaponBagInventoryItems);
    }
    public void InheritancePropRefresh()
    {
        PropBagInventoryItems = _inventoryItemManager.propBag.items.ToArray();
        InheritanceProp.SetBagItem(PropBagInventoryItems);
    }

    public void ShowCanvas()
    {
        Debug.Log("InheritanceManager ShowCanvas");
        InventoryRefresh();
        OpenWeaponInheritance();
        canvas.transform.localScale = new UnityEngine.Vector3(0.5f, 0.5f, 0.5f);
        canvasGroup.alpha = 0.4f;
        InherUIRoot.SetActive(true);
        StartCoroutine(ScaleAndFadeCanvas());
    }

    private IEnumerator ScaleAndFadeCanvas()
    {
        float timer = 0f;
        float duration = 0.3f; // 動畫持續時間

        while (timer < duration)
        {
            timer += Time.deltaTime;

            // 計算 scale 的插值
            float scaleProgress = timer / duration;
            float newScale = Mathf.Lerp(0.5f, 1f, scaleProgress);
            canvas.transform.localScale = new UnityEngine.Vector3(newScale, newScale, 1f);

            // 計算 alpha 的插值
            float alphaProgress = timer / duration;
            float newAlpha = Mathf.Lerp(0.4f, 1f, alphaProgress);
            canvasGroup.alpha = newAlpha;

            yield return null;
        }
    }

    public void CloseCanvas()
    {
        StartCoroutine(ScaleAndFadeCanvasClose());
    }

    private IEnumerator ScaleAndFadeCanvasClose()
    {
        float timer = 0f;
        float duration = 0.5f; // 動畫持續時間

        while (timer < duration)
        {
            timer += Time.deltaTime;

            // 計算 scale 的插值
            float scaleProgress = timer / duration;
            float newScale = Mathf.Lerp(1f, 0.5f, scaleProgress);
            canvas.transform.localScale = new UnityEngine.Vector3(newScale, newScale, 1f);

            // 計算 alpha 的插值
            float alphaProgress = timer / duration;
            float newAlpha = Mathf.Lerp(1f, 0.4f, alphaProgress);
            canvasGroup.alpha = newAlpha;

            yield return null;
        }
        InherUIRoot.SetActive(false);
    }

    public void OpenWeaponInheritance()
    {
        WeaponBagInventoryItems = _inventoryItemManager.weaponBag.items.ToArray();
        InheritanceWeapon.InheritanceItemPanel.SetActive(true);
        InheritanceProp.InheritanceItemPanel.SetActive(false);
        InheritanceWeapon.SetInherBoxItem();
        InheritanceWeapon.SetBagItem(WeaponBagInventoryItems);
    }

    public void OpenPropInheritance()
    {
        PropBagInventoryItems = _inventoryItemManager.propBag.items.ToArray();
        InheritanceProp.InheritanceItemPanel.SetActive(true);
        InheritanceWeapon.InheritanceItemPanel.SetActive(false);
        InheritanceProp.SetInherBoxItem();
        InheritanceProp.SetBagItem(PropBagInventoryItems);
    }

    public List<string> LoadInheritanceData()
    {
        List<string> data = new List<string>();
        for (int i = 0; i < InheritanceWeapon._InventoryItemDatas._itemData.Length; i++)
        {
            if (InheritanceWeapon._InventoryItemDatas._itemData[i]._itemData != null)
            {
                data.Add(InheritanceWeapon._InventoryItemDatas._itemData[i]._itemData.itemName);
            }
        }
        for (int i = 0; i < InheritanceProp._InventoryItemDatas._itemData.Length; i++)
        {
            if (InheritanceProp._InventoryItemDatas._itemData[i]._itemData != null)
            {
                data.Add(InheritanceProp._InventoryItemDatas._itemData[i]._itemData.itemName);
            }
        }
        return data;
    }
}

[System.Serializable]
public class InheritanceItem
{
    public InheritanceInventoryItem _InventoryItemDatas; // 物品數據

    public GameObject InheritanceItemPanel; // 遺傳物品面板

    [SerializeField]private InheritanceInherBoxSlot[] _InheritanceItemBox = new InheritanceInherBoxSlot[4]; // 物品框

    [SerializeField]private InheritanceBagSlot[] _BagItemBox = new InheritanceBagSlot[9]; // 背包框

    public bool CanAddNewInheritanceItem(InventoryItem item)//bag To Inheritance
    {
        // 嘗試找到相同名稱的物品並增加數量

        for (int i = 0; i < _InventoryItemDatas._itemData.Length; i++)
        {
            if (_InventoryItemDatas._itemData[i]._itemData == item.itemData)
            {
                _InventoryItemDatas._itemData[i].AddItemCount(item.currentCount);
                _InheritanceItemBox[i].SetItem(_InventoryItemDatas._itemData[i]);
                return true;
            }
        }
    
        // 嘗試找到空位並新增物品
        for (int i = 0; i < _InventoryItemDatas._itemData.Length; i++)
        {
            if (_InventoryItemDatas._itemData[i]._itemData == null)
            {
                _InventoryItemDatas._itemData[i].InitInheritanceItemData(item.itemData,item.currentCount);
                _InheritanceItemBox[i].SetItem(_InventoryItemDatas._itemData[i]);
                return true;
            }
        }
    
        // 無法新增物品
        return false;
    }

    public void RemoveInheritanceItem(InheritanceItemData Inheritem)
    {
        for(int i = 0; i < _InventoryItemDatas._itemData.Length; i++)
        {
            if (_InventoryItemDatas._itemData[i]._itemData == Inheritem._itemData)
            {
                if(Inheritem._itemData as WeaponData)
                    InventoryItemManager.Instance.weaponBag.Add(Inheritem._itemData,Inheritem.GetItemCount());
                else
                    InventoryItemManager.Instance.propBag.Add(Inheritem._itemData,Inheritem.GetItemCount());

                Inheritem.InitInheritanceItemData();
                break;
            }
        }
    }

    public void SetInherBoxItem()
    {
        for (int i = 0; i < _InventoryItemDatas._itemData.Length; i++)
        {
            if (_InventoryItemDatas._itemData[i]._itemData != null)
            {
                _InheritanceItemBox[i].SetItem(_InventoryItemDatas._itemData[i]);
            }
            else
            {
                _InheritanceItemBox[i].InitSlot();
            }
        }
    }
    
    public bool HaveIem(string name)
    {
        for (int i = 0; i < _InventoryItemDatas._itemData.Length; i++)
        {
            if (_InventoryItemDatas._itemData[i]._itemData != null)
            {
                if (_InventoryItemDatas._itemData[i]._itemData.itemName == name)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void SetBagItem(InventoryItem[] items)
    {
        for (int i = 0; i < _BagItemBox.Length; i++)
        {
            if (i < items.Length && items[i] != null)
            {
                _BagItemBox[i].SetItem(items[i]);
            }
            else
            {
                _BagItemBox[i].InitSlot();
            }
        }
    }
}