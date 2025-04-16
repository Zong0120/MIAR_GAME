using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.Rendering.PostProcessing;
using TMPro;

public class Health :MonoBehaviour//Singleton<Health>
{
    /*
    [SerializeField] private float knockBackThrust = 10f;
    [SerializeField] private GameObject playerDeathVFXPrefab;
    [SerializeField] private GameObject HeartPrefab;

    private KnockBack knockBack;
    private Flash flash;
    public int maxHealth = 5;
    public int currentHealth;
    private bool canTakeDamage = true;
    private float damageRecoveryTime = 0.5f;
    public bool IsDead { get; private set; }
    private string Heart_Redure = "HeartRedure";
    private string PlayDeath ="Death";
    private bool IsDeadEnd = false;
    
    [SerializeField] private GameObject gameOver;

    [Range(0,2)] public float DeathtimeScale=0.2f;


    [SerializeField] private GameObject HealthPointBar;
    private GameObject[] Hearts;
    audiomanager Audiomanager;

    [SerializeField] private GameObject gameoverCanvas;

    void ClearHeartBarChildNodes()
    {
        foreach (Transform child in HealthPointBar.transform)
        {
            Destroy(child.gameObject);
        }
        Hearts = null;
    }

    void CreateHeartPrefabs()
    {
        Hearts = new GameObject[maxHealth];
        for (int i = 0; i < maxHealth; i++)
        {
            Hearts[i] = Instantiate(HeartPrefab, HealthPointBar.transform);
        }
    }

    public void expansionMax(int num)
    {
        ClearHeartBarChildNodes();
        maxHealth += num;
        currentHealth += num;
        Hearts = new GameObject[maxHealth];
        for (int i = 0; i < maxHealth; i++)
        {
            Hearts[i] = Instantiate(HeartPrefab, HealthPointBar.transform);
        }
        for(int i = currentHealth; i < maxHealth; i ++)
        {
            Hearts[i].GetComponent<Animator>().Play("heart_empty", -1, 0f);
            Hearts[i].GetComponent<Animator>().SetBool(Heart_Redure,true);
        }
    }

    public void reduceMax(int num)
    {
        ClearHeartBarChildNodes();
        maxHealth -= num;
        currentHealth -= num;
        if(currentHealth<1)currentHealth =1;
        Hearts = new GameObject[maxHealth];
        for (int i = 0; i < maxHealth; i++)
        {
            Hearts[i] = Instantiate(HeartPrefab, HealthPointBar.transform);
        }
        for(int i = currentHealth; i < maxHealth; i ++)
        {
            Hearts[i].GetComponent<Animator>().Play("heart_empty", -1, 0f);
            Hearts[i].GetComponent<Animator>().SetBool(Heart_Redure,true);
        }
    }

    public void LockBoold(int num)
    {
        ClearHeartBarChildNodes();
        maxHealth = num;
        currentHealth = num;
        Hearts = new GameObject[maxHealth];
        for (int i = 0; i < maxHealth; i++)
        {
            Hearts[i] = Instantiate(HeartPrefab, HealthPointBar.transform);
        }
    }


    protected override void Awake()
    {
        base.Awake();

        knockBack = transform.GetComponent<KnockBack>();
        flash = transform.GetComponent<Flash>();
        Audiomanager = GameObject.FindGameObjectWithTag("Audio").GetComponent<audiomanager>();


        ClearHeartBarChildNodes();
        CreateHeartPrefabs();

    }

    private void Start()
    {
        IsDead = false;
        currentHealth = maxHealth;
        gameOver.SetActive(false);
        gameoverCanvas.SetActive(false);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        EnemyAI enemyAI = collision.transform.GetComponent<EnemyAI>();

        if (enemyAI != null && canTakeDamage)
        {
            TakeDamage(1, collision.transform);
        }
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
                Hearts[currentHealth].GetComponent<Animator>().SetBool(Heart_Redure,false);
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
                Hearts[currentHealth].GetComponent<Animator>().SetBool(Heart_Redure,true);//將血量索引的愛心播放扣減動畫
            }
        }
    }

    public void TakeDamage(int damageAmount, Transform hitTransform)
    {
        if (!canTakeDamage)
        {
            return;
        }

        knockBack.GetKnockedBack(hitTransform, knockBackThrust);
        StartCoroutine(flash.FlashRoutine());
        canTakeDamage = false;
        StartCoroutine(DamageRecoveryRoutine());
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
            Destroy(ActiveWeapon.Instance.gameObject);
            currentHealth = 0;
            Debug.Log("Player Death");
            PlayerDeath();
        }
    }

    public void PlayerDeath()
    {
        if(InheritanceBox.Instance != null)
        {
            InheritanceBox.Instance.SetCanInherit(true);
        }

        Player.Instance.GetComponent<Animator>().SetBool(PlayDeath,true);
        Player.Instance.CanvasCanOpen = false;
        UIMainController._ins.switchUIPage("gaming");

        if (!gameOver.activeSelf)
        {
            gameOver.SetActive(true);
        }
    }

    public void PlayerDeathOnEnd()
    { 
        //ApplyGrayScaleEffect();
        Player.Instance.isFreezed = true;
        Time.timeScale = DeathtimeScale;
        Player.Instance.GetComponent<Animator>().SetBool(PlayDeath,false);
        Instantiate(playerDeathVFXPrefab, transform.position, Quaternion.identity);
        Audiomanager.StopSFX();
        gameoverCanvas.SetActive(true);
        StartCoroutine(gameOverC(gameoverCanvas.GetComponent<CanvasGroup>(),0f, 1f, 4f));
        IsDeadEnd = true;
    }
    
    void Update()
    {
        if(IsDeadEnd && Input.GetKeyDown(KeyCode.M))
        {
            LoadScene();
            HintBoxControll.Instance.init_();
        }
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
    private void LoadScene()
    {
        Player.Instance.gameObject.SetActive(false);
        Destroy(gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void FullLoadScene()
    {
        Terminal_Canvas.Instance.FullyInitialize();
        Player.Instance.gameObject.SetActive(false);
        Destroy(gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }
    */
}
