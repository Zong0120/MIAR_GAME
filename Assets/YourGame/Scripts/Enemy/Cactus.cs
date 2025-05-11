using System.Collections;
using PlayerInputAction;
using UnityEngine;

namespace PlayerInputAction
{
public class Cactus : MonoBehaviour,IDamageable
{
    [SerializeField]private float damageRecoveryTime = 0.5f;    
    [SerializeField]private int maxHealth = 8;
    private int currentHealth;
    private Animator animator => GetComponent<Animator>();
    private Flash flash => GetComponentInChildren<Flash>();

    private float _cactusNoiseRandomTime = 0.5f;
    private bool canTakeDamage = true;

    public void TakeDamage(int damageAmount, Transform hitTransform)
    {
        if (!canTakeDamage)return;
        canTakeDamage = false;
        currentHealth -= damageAmount;
        StartCoroutine(flash.FlashRoutine());
        StartCoroutine(DamageRecoveryRoutine());
        StartCoroutine(CheckDetectDeathRoutine());

        Debug.Log("currentHealth:" + currentHealth);
    }
    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnEnable()
    {
        // 啟動持續播放的協程
        StartCoroutine(CactusNoiseLoop());
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private IEnumerator CactusNoiseLoop()
    {
        while (true)
        {
            // 播放 Noise 動畫
            _cactusNoiseRandomTime = Random.Range(0.5f, 2f); // 隨機時間
            animator.CrossFade("Cactus Noise", 0.1f);
            yield return new WaitForSeconds(_cactusNoiseRandomTime);

            _cactusNoiseRandomTime = Random.Range(1f, 3f); // 隨機時間
            // 播放 Idle 動畫
            animator.CrossFade("Cactus Idle", 0.1f);
            yield return new WaitForSeconds(_cactusNoiseRandomTime); // Idle 持續時間
        }
    }

    private IEnumerator CheckDetectDeathRoutine()
    {
        yield return new WaitForSeconds(flash.GetRestoreDefaultMatTime());

        DetectDeath();
    }

    private void DetectDeath()
    {
        if(currentHealth <= 0)
        {
            //Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);

            //GetComponent<PickUpSpawners>().DropItems();

            //Player.Instance.AtkMonster+=1;
            //Debug.Log("Monster:"+Player.Instance.AtkMonster);
            HealthManager.Instance.SetGoldHeart();
            Debug.Log("Cactus Death");
            Destroy(transform.parent.gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            // 造成傷害
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(1, transform);
        }
    }
}
}