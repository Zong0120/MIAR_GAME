using UnityEngine;
namespace PlayerInputAction
{
public class EnergyDrink : PropItem
{
    private Animator animator => GetComponent<Animator>();
    public override void Use()
    {
        if (!CanUseProp()) return;
        animator.CrossFade("PropUse", 0.1f);
        EquipManager.Instance.EquipStartCooldown();
    }
    public override void UseEffect()
    {
        HealthManager.Instance.InvincibleTime(10f);
        PlayerController.Instance.Accelerate_Player(5f, 10f);
    }
}
}