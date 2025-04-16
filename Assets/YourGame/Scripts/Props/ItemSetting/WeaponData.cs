using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class WeaponData : ItemData
{
    [Header("Weapon Settings")]
    public int damage;
    public float attackRate;
    public float maxDistance;
    public int magSize;
    public float reloadTime;
}