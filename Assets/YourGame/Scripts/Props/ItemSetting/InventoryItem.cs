public class InventoryItem
{
    public ItemData itemData;
    public int currentCount;
    public float cooldownRemaining;
    public bool isEquipped;

    public InventoryItem(ItemData data, int count = 1)
    {
        itemData = data;
        currentCount = count;
        cooldownRemaining = 0;
        isEquipped = false;
    }

    public bool IsOnCooldown => cooldownRemaining > 0;

    public void StartCooldown()
    {
        cooldownRemaining = 1;
    }

    public void UpdateCooldown(float deltaTime)
    {
        if (cooldownRemaining > 0)
        {
            cooldownRemaining -= deltaTime/itemData.cooldownTime;
            if (cooldownRemaining < 0) cooldownRemaining = 0;
        }
    }
}