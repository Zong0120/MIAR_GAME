using UnityEngine;

public class ItemData : ScriptableObject
{
    public string itemName;
    public string itemNameCh;
    public Sprite itemImage;
    [TextArea] public string itemDescription;
    [TextArea] public string itemCode;

    public GameObject itemPrefab;
    public float cooldownTime;
    public bool restrictedItem = false;
    public bool cooldownDisappear = false;
}
