using System.Collections;
using UnityEngine;

public class Oboe : WeaponItem
{
    private Animator animator => GetComponent<Animator>();
    private Collider2D weaponCollider => GetComponent<Collider2D>();
    private float ani_speed;
    private float attackTime = 2f;
    private bool isCooldown = false;

    private void OnEnable()
    {
        ani_speed = 1f;
        attackTime = 2f;
    }

    public override void Use()
    {
        if (isCooldown) return;
        animator.SetTrigger("Attack");
        weaponCollider.enabled = true;
        ComboEffect();
        StartCoroutine(AttackCooldown());
    }
    public IEnumerator AttackCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(attackTime);
        isCooldown = false;
    }

    public void ComboEffect()
    {
        if(attackTime > 0.2f)
        {
            attackTime -= 0.3f;
            ani_speed += 0.3f;
            animator.speed = ani_speed;
        }
    }
}
