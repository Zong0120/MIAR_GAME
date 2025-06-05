using UnityEngine;

public class PropItem : MonoBehaviour,IUseable
{
    private Animator animator=>
        GetComponent<Animator>();

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

}
