using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public enum SoundType
{
    FOOTSTEP
}

[RequireComponent(typeof(AudioSource)),ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [SerializeField]private SoundList[] soundList;
    private static SoundManager Instance;
    private AudioSource _audioSource;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType sound,float volume=1)
    {
        AudioClip[] clips = Instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        Instance._audioSource.PlayOneShot(randomClip, volume);
    }
#if UNITY_EDITOR
    private void Onable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));   
        Array.Resize(ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }    
    }
#endif
}

[Serializable]
public class SoundList
{
    public AudioClip[] Sounds{get => sounds;}
    [SerializeField] public string name;
    [SerializeField] private AudioClip[] sounds;
}