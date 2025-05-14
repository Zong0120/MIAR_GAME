using System.Collections;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class HealthHeart
{
    public Animator HeartAnimator;
    public bool IsFull;
    public HealthHeart(Animator animator,bool isFull=true)
    {
        HeartAnimator = animator;
        IsFull = isFull;
        if(!IsFull)SetEmpty();
    }

    public void SetFull()
    {
        IsFull = true;
        HeartAnimator.CrossFade("fullblood",0.1f);
    }

    public void SetEmpty()
    {
        IsFull = false;
        HeartAnimator.CrossFade("emptyblood",0.1f);
    }

    public void SetGold(Material HeartGoldMaterial)
    {
        HeartAnimator.gameObject.GetComponent<UnityEngine.UI.Image>().material = HeartGoldMaterial;
    }

    public void SetNormal()
    {
        HeartAnimator.gameObject.GetComponent<UnityEngine.UI.Image>().material = null;
    }
}
namespace PlayerInputAction
{
public class HealthManager : MonoBehaviour,IDamageable
{
    public static HealthManager Instance { get; private set; }
    //[SerializeField] private GameObject playerDeathVFXPrefab;
    [SerializeField] private GameObject HealthBar;
    [SerializeField] private GameObject HeartPrefab;
    [SerializeField] private Material HeartGoldMaterial;

    private Flash flash =>GetComponent<Flash>();
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;
    public int currentFreeDamage{ get; private set; } = -1;
    private bool canTakeDamage = true;
    public bool IsDead { get; private set; }
    private HealthHeart[] HeartAnimations;
    //audiomanager Audiomanager;
    //[SerializeField] private GameObject gameoverCanvas;

    [SerializeField] private float knockBackTime = 0.2f;
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    [SerializeField] private float knockBackThrust = 20f;
    [SerializeField] private float damageRecoveryTime = 3f;


    void ClearHeartBarChildNodes()
    {
        foreach (Transform child in HealthBar.transform)
        {
            Destroy(child.gameObject);
        }
        //Hearts = null;
        HeartAnimations = null;
    }

    void CreateHeartPrefabs()
    {
        HeartAnimations = new HealthHeart[maxHealth];
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heartObject = Instantiate(HeartPrefab, HealthBar.transform);
            HeartAnimations[i] = new HealthHeart(heartObject.GetComponent<Animator>());
        }
    }

    public void expansionMax(int num)
    {
        //ClearHeartBarChildNodes();
        // 更新 maxHealth
        maxHealth += num;

        for(int i = currentHealth; i < maxHealth; i ++)
        {
            if(num > 0)
            {
                if(HeartAnimations[i] != null)
                    HeartAnimations[i].SetFull();
                else
                {
                    GameObject heartObject = Instantiate(HeartPrefab, HealthBar.transform);
                    HeartAnimations[i] = new HealthHeart(heartObject.GetComponent<Animator>());
                }
                num -- ;
            }
            else
            {
                HeartAnimations[i].SetEmpty();
            }
        }
    }

    public void reduceMax(int num)
    {
        // 更新 maxHealth

        for (int i = currentHealth-1; i > 0; i--)
        {
            if (num > 0)
            {
                HeartAnimations[i].SetEmpty();
                num--;
            }
        }
        currentHealth -= num;
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            IsDead = true;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ClearHeartBarChildNodes();
        CreateHeartPrefabs();
        IsDead = false;
        currentHealth = maxHealth;
        //gameOver.SetActive(false);
        //gameoverCanvas.SetActive(false);
    }
    public bool IsFullHealth()
    {
        if(currentHealth == maxHealth)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void Heal(int healingAmount)
    {
        if (IsDead)
        {
            return;
        }

        for(int i = 0; i < healingAmount; i++)
        {
            if(currentHealth < maxHealth)
            {
                Debug.Log("Heal"+currentHealth);
                HeartAnimations[currentHealth].SetFull();
                currentHealth +=1;
            }
            else
            {
                return;
            }
        }
    }
    public void Damage(int damageAmount)
    {
        if (IsDead)//沒血了就不用扣血了
        {
            return;
        }

        bool isInjuried = false;
        for(int i = 0; i < damageAmount; i++)//扣血
        {
            if(currentFreeDamage > -1)
            {
                HeartAnimations[currentFreeDamage].SetNormal();
                currentFreeDamage -= 1;
                //play audio
            }
            else
            {
                currentHealth -=1;
                HeartAnimations[currentHealth].SetEmpty();
                isInjuried = true;
            }
        }
        if(isInjuried)
            InventoryItemManager.Instance.RemoveRandomItem();
    }

    public void SetGoldHeart(int Amount = 1)
    {
        for(int i = 0; i < Amount; i++)
        {
            if(currentHealth > 0 && currentFreeDamage < currentHealth)
            {
                currentFreeDamage += 1;
                HeartAnimations[currentFreeDamage].SetGold(HeartGoldMaterial);
            }
        }
    }

    public void InvincibleTime(float time)
    {
        if (IsDead)
        {
            return;
        }
        canTakeDamage = false;
        StartCoroutine(flash.FlashRoutine(time));
        StartCoroutine(InvincibleTimeRoutine(time));
    }
    private IEnumerator InvincibleTimeRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        canTakeDamage = true;
    }

    public void TakeDamage(int damageAmount, Transform hitTransform)
    {
        if (!canTakeDamage)
        {
            return;
        }
        canTakeDamage = false;
        SoundManager.PlaySoundItemAudio(SoundType.Cactus, "CactusATK");
        GetKnockedBack(hitTransform, knockBackThrust);
        StartCoroutine(flash.FlashRoutine(damageRecoveryTime));
        StartCoroutine(DamageRecoveryRoutine());
        Damage(damageAmount);
        CheckPlayerDeath();
    }

    public void GetKnockedBack(Transform damageSource, float knockBackThrust)
    {
        Vector2 difference = knockBackThrust * rb.mass *  (transform.position - damageSource.position).normalized;
        rb.AddForce(difference, ForceMode2D.Impulse);
        StartCoroutine(KnockBackRoutine());
    }

    private IEnumerator KnockBackRoutine()
    {
        yield return new WaitForSeconds(knockBackTime);
        rb.linearVelocity = Vector2.zero;
    }
    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private void CheckPlayerDeath()
    {
        if(currentHealth <= 0 && !IsDead)
        {
            IsDead = true;
            //Destroy(ActiveWeapon.Instance.gameObject);
            currentHealth = 0;
            Debug.Log("Player Death");
            PlayerController.Instance.PlayerDeath();
        }
    }

}
}