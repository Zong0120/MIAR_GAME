using UnityEngine;

namespace AirFishLab.ScrollingList.Demo
{
public class Decoder : PropItem
{
    private Collider2D propCollider=>
        GetComponent<Collider2D>();
    private bool canUse = false;
    
    private void Onable()
    {
        Debug.Log("Decoder is enabled");
    }
    private void OnDisable()
    {
        // Disable the item
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("LockDoor"))
        {
            canUse = true;
            Debug.Log("Can use decoder on lock door");
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("LockDoor"))
        {
            canUse = false;
            Debug.Log("Cannot use decoder on lock door");
        }
    }

    public override bool CanUseProp()
    {
        if(!canUse)return false;
        return true;
    }

    public override void UseEffect()
    {
        LockManager.Instance.LockToBingo();
    }
}
}