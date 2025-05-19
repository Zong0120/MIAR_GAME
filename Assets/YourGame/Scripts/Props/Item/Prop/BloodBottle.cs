using UnityEngine;
using PlayerInputAction;
public class BloodBottle : PropItem
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
        Debug.Log("BloodBottle effect used!");
        HealthManager.Instance.Heal(1);
    }
}
