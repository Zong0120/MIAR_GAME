using UnityEngine;

public class PlayerSoundEnter : StateMachineBehaviour
{
    [SerializeField]private SoundType sound;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SoundManager.PlaySound(sound);
    }
}
