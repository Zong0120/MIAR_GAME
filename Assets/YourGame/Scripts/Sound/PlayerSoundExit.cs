using UnityEngine;

public class PlayerSoundExit : StateMachineBehaviour
{
    [SerializeField]private SoundType sound;
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SoundManager.PlaySound(sound);
    }
}
