using UnityEngine;
using System.Collections;

public class GhostEnemy : MonoBehaviour, IDamageable
{
    public static GhostEnemy Instance;
    public float spawnOffset = 2f;
    public float moveSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float triggerDistance = 10f;
    public float destroyMargin = 1f;
    public float pursuitDistance = 200f;
    public int maxHealth = 4;
    private int currentHealth;

    private SpriteRenderer Ghostsprite => GetComponent<SpriteRenderer>();
    private Collider2D GhostCollider => GetComponent<Collider2D>();
    private Transform player;
    private Vector2 lastPlayerPos;
    private bool isChasing = false;
    private bool hasEnteredView = false;
    private bool faceIsRight = true;
    private bool isWeirdTone = false;

    private Camera cam;
    private Vector2 moveDir;
    private Coroutine behaviorRoutine;
    private Coroutine flickerRoutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        cam = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Activate()
    {
        Ghostsprite.enabled = true;
        GhostCollider.enabled = true;
        currentHealth = maxHealth;
        isWeirdTone = false;
        lastPlayerPos = player.position;
        isChasing = false;
        hasEnteredView = false;
        SpawnFromRandomEdge();

        if (behaviorRoutine != null)
            StopCoroutine(behaviorRoutine);

        behaviorRoutine = StartCoroutine(GhostBehavior());
        flickerRoutine = StartCoroutine(Flickering());
    }

    IEnumerator GhostBehavior()
    {
        while (!hasEnteredView)
        {
            transform.position += (Vector3)(moveDir * moveSpeed * Time.deltaTime);
            hasEnteredView = IsInsideCameraBounds();
            yield return null;
        }

        while (!isChasing)
        {
            transform.position += (Vector3)(moveDir * moveSpeed * Time.deltaTime);

            if (Vector2.Distance(player.position, lastPlayerPos) > triggerDistance)
            {
                isChasing = true;
                break;
            }

            if (IsOutOfBounds())
            {
                Debug.Log("幽靈離開畫面未追擊，玩家平安");
                Deactivate();
                yield break;
            }

            yield return null;
        }

        while (isChasing)
        {
            Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(dir * chaseSpeed * Time.deltaTime);

            if (Vector2.Distance(player.position, transform.position) > pursuitDistance)
            {
                Debug.Log("幽靈追擊失敗，玩家逃脫");
                Deactivate();
                yield break;
            }

            // Flip sprite
            if (faceIsRight && dir.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                faceIsRight = false;
            }
            else if (!faceIsRight && dir.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                faceIsRight = true;
            }
            if (!isWeirdTone)
            {
                // 音樂變詭異
                SoundManager.BackgroundWeirdTone();
                isWeirdTone = true;
            }

            yield return null;
        }
    }

    IEnumerator Flickering()
    {
        float minAlpha = 0.5f;
        float maxAlpha = 1;
        float duration = Random.Range(1f, 3f); // 呼吸燈的完整周期時間
        float t = 0;

        while (true)
        {
            // 從 minAlpha 到 maxAlpha
            while (t < duration)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Lerp(minAlpha, maxAlpha, t / duration);
                Ghostsprite.color = new Color(1, 1, 1, alpha);
                yield return null;
            }

            // 重置時間並反向從 maxAlpha 到 minAlpha
            t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Lerp(maxAlpha, minAlpha, t / duration);
                Ghostsprite.color = new Color(1, 1, 1, alpha);
                yield return null;
            }

            // 重置時間以進行下一個周期
            t = 0;
            duration = Random.Range(1f, 3f); // 隨機化下一個周期的時間
        }
    }

    void SpawnFromRandomEdge()
    {
        Vector3 camPos = new Vector3(cam.transform.position.x, cam.transform.position.y, 0);
        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        float side = Random.value;
        Vector3 spawnPos;

        if (side < 0.25f) // 上
        {
            float x = Random.Range(-width, width);
            spawnPos = camPos + new Vector3(x, height + spawnOffset, 0);
        }
        else if (side < 0.5f) // 下
        {
            float x = Random.Range(-width, width);
            spawnPos = camPos + new Vector3(x, -height - spawnOffset, 0);
        }
        else if (side < 0.75f) // 左
        {
            float y = Random.Range(-height, height);
            spawnPos = camPos + new Vector3(-width - spawnOffset, y, 0);
        }
        else // 右
        {
            float y = Random.Range(-height, height);
            spawnPos = camPos + new Vector3(width + spawnOffset, y, 0);
        }

        transform.position = spawnPos;

        Vector2 toCenter = ((Vector2)camPos - (Vector2)spawnPos).normalized;
        float angleOffset = Random.Range(-30f, 30f);
        moveDir = Quaternion.Euler(0, 0, angleOffset) * toCenter;

        transform.rotation = (moveDir.x > 0) ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        faceIsRight = moveDir.x <= 0;
    }

    bool IsInsideCameraBounds()
    {
        Vector3 camPos = cam.transform.position;
        float height = cam.orthographicSize;
        float width = height * cam.aspect;
        Vector2 pos = transform.position;

        return (pos.x >= camPos.x - width && pos.x <= camPos.x + width &&
                pos.y >= camPos.y - height && pos.y <= camPos.y + height);
    }

    bool IsOutOfBounds()
    {
        Vector3 camPos = cam.transform.position;
        float height = cam.orthographicSize;
        float width = height * cam.aspect;
        Vector2 offset = (Vector2)transform.position - (Vector2)camPos;

        return (Mathf.Abs(offset.x) > width + destroyMargin || Mathf.Abs(offset.y) > height + destroyMargin);
    }

    public void Deactivate()
    {
        if (behaviorRoutine != null)
            StopCoroutine(behaviorRoutine);
        StopCoroutine(flickerRoutine);
        SoundManager.BackgroundNormalTone();
        Ghostsprite.enabled = false;
        GhostCollider.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isChasing && other.CompareTag("Player"))
        {
            other.GetComponent<IDamageable>()?.TakeDamage(2, transform, "GhostEnemy");
            Deactivate();
        }
    }
    public void TakeDamage(int damageAmount, Transform hitTransform, string name = "")
    {
        currentHealth -= damageAmount;
        Debug.Log($"GhostEnemy took {damageAmount} damage from {name}");
        if (currentHealth <= 0)
        {
            Debug.Log("GhostEnemy is dead");
            // Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
            // GetComponent<PickUpSpawners>().DropItems();
            Deactivate();
        }
    }
}
