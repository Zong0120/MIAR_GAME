using UnityEngine;

public class PlayFootStep : MonoBehaviour
{
    public void PlayFootStepSound()
    {
        SoundManager.PlaySound(SoundType.FOOTSTEP);
    }
}
