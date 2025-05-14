using UnityEngine;

public class WeaponItem : MonoBehaviour,IUseable
{
    private Transform _parentTransform => transform.parent;
    private Animator animator=>
        GetComponent<Animator>();
    private PolygonCollider2D weaponCollider=>
        GetComponent<PolygonCollider2D>();
    [SerializeField]private string weaponSoundName;
    [SerializeField]private float validAngleRange = 40f; // 有效角度範圍，例如 60 度
    private int damageAmount;

    public void SetWeaponData(WeaponData weaponData)
    {
        damageAmount = weaponData.damage;
    }

    public virtual void Use()
    {
        animator.SetTrigger("Attack");
        weaponCollider.enabled = true;
        EquipManager.Instance.EquipStartCooldown();
    }

    public void PlayAudio()
    {
        SoundManager.PlaySoundItemAudio(SoundType.Weapon, weaponSoundName);
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

        // 計算滑鼠方向與水平線（右方）的夾角
        float angleFromHorizontal = Vector2.Angle(Vector2.right, direction);

        // 若方向在左邊（即x為負），以 Vector2.left 計算角度
        if (direction.x < 0)
            angleFromHorizontal = Vector2.Angle(Vector2.left, direction);
        // 如果角度在設定範圍內才更新方向
        if (angleFromHorizontal <= validAngleRange)
        {
            //Debug.Log("angleFromHorizontal:" + angleFromHorizontal);
            _parentTransform.right = -direction;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<IDamageable>()!=null)
            collision.gameObject.GetComponent<IDamageable>().TakeDamage(damageAmount, null);
    }
}
