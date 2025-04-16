using UnityEngine;

[CreateAssetMenu(fileName = "New Prop", menuName = "Inventory/Prop")]
public class PropData : ItemData
{
    [Header("Prop Settings")]
    public int maxStack;
    
}
