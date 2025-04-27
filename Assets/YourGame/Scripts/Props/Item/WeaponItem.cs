using UnityEngine;

public class WeaponItem : MonoBehaviour,IUseable
{
    private Animator animator=>
        GetComponent<Animator>();
    private AudioClip audioSmash=>
        GetComponent<AudioClip>();
    private PolygonCollider2D weaponCollider=>
        GetComponent<PolygonCollider2D>();
    [SerializeField] private Vector2 maxAngle= new Vector2(45f, 45f);

    private int damageAmount;

    public void SetDamage(int damage)
    {
        damageAmount = damage;
    }

    public void Use()
    {
        Debug.Log("WeaponItem used!");
        animator.SetTrigger("Attack");
        weaponCollider.enabled = true;
    }

    public void PlayAudio()
    {
        SoundManager.PlaySoundItemAudio(audioSmash);
    }

    public void AfterAttackAnimEvent()
    {
        weaponCollider.enabled = false;
    }

    private void Update()
    {
        FaceMouse();
    }

    private void FaceMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
    
        Vector2 direction = mousePos - transform.position;
    
        // 計算目標角度（以度數表示）
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    
        // 限制角度範圍（例如 -45 到 45 度）
        float clampedAngle = Mathf.Clamp(targetAngle, -maxAngle.x, maxAngle.y);
    
        // 設定旋轉角度
        transform.rotation = Quaternion.Euler(0f, 0f, clampedAngle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {

            /*
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
            }
            */
        }
    }
}
