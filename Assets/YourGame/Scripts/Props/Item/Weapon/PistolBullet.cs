using UnityEngine;

public class PistolBullet : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 20f;
    private Vector2 startPos;
    private int damage;
    private float projectileRange;

    public void SetData(int damage, float projectileRange)
    {
        this.damage = damage;
        this.projectileRange = projectileRange;
    }

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        MoveProjectile();
        DetectFireDistance();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage, null);
        Destroy(gameObject);
    }

    private void MoveProjectile()
    {
        transform.Translate(Vector2.right * (moveSpeed * Time.deltaTime));
    }

    public void UpdateProjectileRange(float projectileRange)
    {
        this.projectileRange = projectileRange;
    }

    public void UpdateMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    private void DetectFireDistance()
    {
        if (Vector2.Distance(transform.position, startPos) > projectileRange)
        {
            Destroy(gameObject);
        }
    }
}
