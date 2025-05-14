using UnityEngine;

public class PropItem : MonoBehaviour,IUseable
{
    private Animator animator=>
        GetComponent<Animator>();
    private AudioClip audioSmash=>
        GetComponent<AudioClip>();
    [SerializeField] private bool canAngle = false;
    [SerializeField] private Vector2 maxAngle= new Vector2(45f, 45f);

    public virtual void Use()
    {
        if (!CanUseProp()) return;
        animator.CrossFade("PropUse", 0.1f);
        EquipManager.Instance.EquipStartCooldown();
        UseEffect();
    }
    public virtual bool CanUseProp()
    {
        return true;
    }
    public virtual void UseEffect()
    {
        Debug.Log("PropItem effect used!");
    }

    public void PlayAudio()
    {
        //SoundManager.PlaySoundItemAudio(audioSmash);
    }

    private void Update()
    {
        FaceMouse();
    }

    private void FaceMouse()
    {
        if (!canAngle) return;
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
    
        Vector2 direction = mousePos - transform.position;
    
        // 計算目標角度（以度數表示）
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    
        // 限制角度範圍（例如 -45 到 45 度）
        float clampedAngle = Mathf.Clamp(targetAngle, -maxAngle.x, maxAngle.y);
    
        // 設定旋轉角度
        transform.rotation = Quaternion.Euler(0f, 0f, clampedAngle);
    
        Debug.Log($"WeaponItem rotation: {clampedAngle}");
    }
}
