using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        HeartAnimator.SetTrigger("fullblood");
    }

    public void SetEmpty()
    {
        IsFull = false;
        HeartAnimator.SetTrigger("emptyblood");
    }
}
public class HealthManager : MonoBehaviour
{
    [SerializeField] private GameObject playerDeathVFXPrefab;
    [SerializeField] private GameObject HealthBar;
    [SerializeField] private GameObject HeartPrefab;

    //private Flash flash;
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;
    private bool canTakeDamage = true;
    private float damageRecoveryTime = 0.5f;
    public bool IsDead { get; private set; }
    //private bool IsDeadEnd = false;

    private float DeathtimeScale=0.2f;
    private HealthHeart[] HeartAnimations;
    //audiomanager Audiomanager;
    [SerializeField] private GameObject gameoverCanvas;

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
        

        /*
        Hearts = new Animator[maxHealth];
        for (int i = 0; i < maxHealth; i++)
        {
            Hearts[i] = Instantiate(HeartPrefab, HealthBar.transform).GetComponent<Animator>();
        }
        */
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
        //base.Awake();

        //flash = transform.GetComponent<Flash>();
        //Audiomanager = GameObject.FindGameObjectWithTag("Audio").GetComponent<audiomanager>();


        

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
    /*
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (canTakeDamage)
        {
            TakeDamage(1, collision.transform);
        }
    }
    */
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

        for(int i = 0; i < damageAmount; i++)//扣血
        {
            if(currentHealth != 0)//血量索引
            {
                currentHealth -=1;
                HeartAnimations[currentHealth].SetEmpty();
            }
        }
    }

    public void TakeDamage(int damageAmount, Transform hitTransform)
    {
        if (!canTakeDamage)
        {
            return;
        }

        //StartCoroutine(flash.FlashRoutine());
        canTakeDamage = false;
        //StartCoroutine(DamageRecoveryRoutine());
        Damage(damageAmount);
        CheckPlayerDeath();
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
            PlayerDeath();
        }
    }

    public void PlayerDeath()
    {
        /*
        if(InheritanceBox.Instance != null)
        {
            InheritanceBox.Instance.SetCanInherit(true);
        }
        */
        //Player.Instance.GetComponent<Animator>().SetBool(PlayDeath,true);
        //Player.Instance.CanvasCanOpen = false;
        //UIMainController._ins.switchUIPage("gaming");
        /*
        if (!gameOver.activeSelf)
        {
            gameOver.SetActive(true);
        }
        */
    }

    public void PlayerDeathOnEnd()
    {
        //Player.Instance.isFreezed = true;
        Time.timeScale = DeathtimeScale;
        //Player.Instance.GetComponent<Animator>().SetBool(PlayDeath,false);
        //Instantiate(playerDeathVFXPrefab, transform.position, Quaternion.identity);
        //Audiomanager.StopSFX();
        //直接異步加載場景
        //StartCoroutine(LoadSceneAsync());
        //Invoke("LoadScene", 2f);
        gameoverCanvas.SetActive(true);
        //StartCoroutine(gameOverC(gameoverCanvas.GetComponent<CanvasGroup>(),0f, 1f, 4f));
        //IsDeadEnd = true;
    }

    IEnumerator gameOverC(CanvasGroup canvasGroup,float startAlpha, float endAlpha, float duration)
    {

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }
    
    

}

