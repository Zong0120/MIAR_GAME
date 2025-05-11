using UnityEngine;

public class PlaySoundAudioChlip : MonoBehaviour
{
    [SerializeField] private SoundType soundType;
    [SerializeField] private string soundName;
    public void PlaySound()
    {
        if (string.IsNullOrEmpty(soundName))
        {
            Debug.LogWarning("Sound name is not set.");
            return;
        }

        SoundManager.PlaySoundItemAudio(soundType, soundName);
    }
}
