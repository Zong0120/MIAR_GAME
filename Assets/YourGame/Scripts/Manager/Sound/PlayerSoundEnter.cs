using UnityEngine;

public class PlayerSoundEnter : StateMachineBehaviour
{
    [SerializeField]private SoundType sound;
    [SerializeField,Range(0,1)]private float volume = 1f;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SoundManager.PlaySound(sound, volume);
    }
}
