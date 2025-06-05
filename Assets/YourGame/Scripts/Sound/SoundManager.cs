using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public enum SoundType
{
    FOOTSTEP,
    BackgroundMusic,
    Enemy,
    UI,
    Lock,
    Weapon
}

[ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [Header("Sound List")]
    [SerializeField] private SoundList[] soundList;
    [SerializeField] private AudioSource _backgroundMusicSource;
    [SerializeField] private AudioSource _soundEffectSource;

    public float spookyPitch = 0.6f;// 詭異音樂音高（原本為 1）
    public float pitchChangeSpeed = 1f; // 音高變化速度
    private Coroutine _backgroundWeirdToneCoroutine;

    private static SoundManager _instance;
    public static SoundManager Instance => _instance;

    private Dictionary<SoundType, Dictionary<string, AudioClip>> _soundDictionaries;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            BuildSoundDictionaries();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBackgroundMusic("gaming-1");
    }

    private void BuildSoundDictionaries()
    {
        _soundDictionaries = new Dictionary<SoundType, Dictionary<string, AudioClip>>();

        for (int i = 0; i < soundList.Length; i++)
        {
            var type = (SoundType)i;
            var dict = new Dictionary<string, AudioClip>();

            foreach (var clip in soundList[i].Sounds)
            {
                if (clip != null && !dict.ContainsKey(clip.name))
                {
                    dict.Add(clip.name, clip);
                }
            }

            _soundDictionaries[type] = dict;
        }
    }

    public static void BackgroundWeirdTone()
    {
        if (_instance == null) return;

        if (_instance._backgroundMusicSource != null)
        {
            _instance._backgroundMusicSource.pitch = Mathf.Lerp(_instance._backgroundMusicSource.pitch, _instance.spookyPitch, Time.deltaTime * _instance.pitchChangeSpeed);
            _instance._backgroundWeirdToneCoroutine =_instance.StartCoroutine(_instance.BackgroundWeirdToneCoroutine());
        }
    }
    private IEnumerator BackgroundWeirdToneCoroutine()
    {
        while (_instance._backgroundMusicSource.pitch > _instance.spookyPitch)
        {
            _instance._backgroundMusicSource.pitch = Mathf.Lerp(_instance._backgroundMusicSource.pitch, _instance.spookyPitch, Time.deltaTime * _instance.pitchChangeSpeed);
            yield return null;
        }
    }
    public static void BackgroundNormalTone()
    {
        if (_instance == null) return;

        if (_instance._backgroundMusicSource != null)
        {
            if (_instance._backgroundWeirdToneCoroutine != null)
            {
                _instance.StopCoroutine(_instance._backgroundWeirdToneCoroutine);
                _instance._backgroundWeirdToneCoroutine = null;
            }
            _instance._backgroundMusicSource.pitch = 1f;
        }
    }

    public static void ReStartBackgroundMusic()
    {
        if (_instance == null) return;

        if (_instance._backgroundMusicSource != null)
        {
            _instance._backgroundMusicSource.Stop();
            _instance._backgroundMusicSource.Play();
        }
    }

    public static void PlayBackgroundMusic(string name, bool loop = true, SoundType soundType = SoundType.BackgroundMusic)
    {
        if (_instance == null || !_instance._soundDictionaries.TryGetValue(soundType, out var dict)) return;

        if (dict.TryGetValue(name, out var clip))
        {
            _instance._backgroundMusicSource.clip = clip;
            _instance._backgroundMusicSource.loop = loop;
            _instance._backgroundMusicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Background music '{name}' not found in '{soundType}'");
        }
    }
    public static void PauseBackgroundMusic()
    {
        if (_instance != null)
        {
            _instance._backgroundMusicSource.Pause();
        }
    }
    public static void ResumeBackgroundMusic()
    {
        if (_instance != null)
        {
            _instance._backgroundMusicSource.UnPause();
        }
    }

    public static void PlaySound(SoundType soundType)
    {
        if (_instance == null || !_instance._soundDictionaries.TryGetValue(soundType, out var dict)) return;

        if (dict.Count == 0)
        {
            Debug.LogWarning($"No clips found for '{soundType}'");
            return;
        }

        var values = new List<AudioClip>(dict.Values);
        var randomClip = values[UnityEngine.Random.Range(0, values.Count)];
        _instance._soundEffectSource.PlayOneShot(randomClip);
    }

    public static void PlaySoundItemAudio(SoundType soundType, string soundName)
    {
        if (_instance == null || !_instance._soundDictionaries.TryGetValue(soundType, out var dict)) return;

        if (dict.TryGetValue(soundName, out var clip))
        {
            _instance._soundEffectSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found in '{soundType}'");
        }
    }

    public static AudioSource GetBackgroundMusicSource()
    {
        return _instance != null ? _instance._backgroundMusicSource : null;
    }
    public static AudioSource GetSoundEffectSource()
    {
        return _instance != null ? _instance._soundEffectSource : null;
    }

    public static void ToggleBackgroundMusic()
    {
        _instance._backgroundMusicSource.mute = !_instance._backgroundMusicSource.mute;
    }
    public static void ToggleSoundEffect()
    {
        _instance._soundEffectSource.mute = !_instance._soundEffectSource.mute;
    }

    public static void SetBackgroundMusicVolume(float volume)
    {
        
        _instance._backgroundMusicSource.volume = volume;
    }
    public static void SetSoundEffectVolume(float volume)
    {
        _instance._soundEffectSource.volume = volume;
    }
}

[Serializable]
public class SoundList
{
    public AudioClip[] Sounds => sounds;
    [SerializeField] public string name;
    [SerializeField] private AudioClip[] sounds;
}
