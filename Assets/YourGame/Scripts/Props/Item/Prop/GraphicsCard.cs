using UnityEngine;
using PlayerInputAction;
using System.Collections;

public class GraphicsCard : PropItem
{
    private Animator animator =>
        GetComponent<Animator>();
    private float ani_speed;
    private float UseTime = 2f;
    private bool isCooldown = false;
    private float usetimes;
    private bool canUse = true;

    private void OnEnable()
    {
        ani_speed = 1f;
        UseTime = 2f;
        usetimes = 0f;
        canUse = true;
    }
    private void OnDisable()
    {
        animator.speed = 1f;
        PlayerController.Instance.ReduceSpeed((usetimes / 100) * 2);
    }

    public override void Use()
    {
        if (isCooldown) return;
        animator.CrossFade("PropUse", 0.1f);
        EquipManager.Instance.EquipStartCooldown();
        StartCoroutine(UseCooldown());
        ComboEffect();
    }
    public IEnumerator UseCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(UseTime);
        isCooldown = false;
    }
    public void ComboEffect()
    {
        if (!canUse) return;
        if (UseTime > 0.2f)
        {
            UseTime -= 0.3f;
            ani_speed += 0.3f;
            animator.speed = ani_speed;
        }
        usetimes += 1f;
        if (usetimes % 100 == 0 && canUse)
        {
            PlayerController.Instance.AddSpeed(2);
        }
        if (usetimes >= 1500)
        {
            canUse = false;
        }
    }
}
