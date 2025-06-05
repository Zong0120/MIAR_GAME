using UnityEngine;
using System.Collections;

public class Pistol : WeaponItem
{
    [SerializeField] private string _shootSound = "Weapon_PistolShoot";
    private Animator animator => GetComponent<Animator>();
    private Collider2D weaponCollider => GetComponent<Collider2D>();
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    private int damageAmount;
    private float attackRange;
    public override void Use()
    {
        if (InventoryItemManager.Instance._bullet() == null)
        {
            animator.CrossFade("WeaponAtk", 0.1f);
            weaponCollider.enabled = true;
            EquipManager.Instance.EquipStartCooldown();
        }
        else
        {
            WeaponData bulletData = InventoryItemManager.Instance._bullet().itemData as WeaponData;
            damageAmount = bulletData.damage;
            attackRange = bulletData.attackRange;
            InventoryItemManager.Instance.weaponBag.Remove(InventoryItemManager.Instance._bullet().itemData);
            EquipManager.Instance.EquipStartCooldown();
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        animator.CrossFade("WeaponShoot", 1f);
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        PistolBullet bulletScript = bullet.GetComponent<PistolBullet>();
        bulletScript.SetData(damageAmount, attackRange);
        HealthManager.Instance.GetKnockedBack(bullet.transform, 90f);
        AnimatorStateInfo stateInfo;
        do
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return null; // 等待下一幀
        } while (stateInfo.IsName("WeaponShoot") && stateInfo.normalizedTime < 1f);
    }
    
    public void ShootSound()
    {
        SoundManager.PlaySoundItemAudio(SoundType.Weapon, _shootSound);
    }
}
