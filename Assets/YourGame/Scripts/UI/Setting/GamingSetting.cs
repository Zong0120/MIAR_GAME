using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayerInputAction;
using System.Collections.Generic;

public class GameingSetting : MonoBehaviour
{
    [SerializeField] private Slider BackgroundMusicvolumeSlider;
    [SerializeField] private Slider SoundEffectvolumeSlider;
    [SerializeField] private Toggle BackgroundMusicToggle;
    [SerializeField] private Toggle SoundEffectToggle;

    [Header("Canvas")]
    [SerializeField] private Canvas SettingMenu;
    [SerializeField] private Canvas TitleMenu;

    //public StoryProgressData _storyProgressData;

    private AudioSource _backgroundMusicSource;
    private AudioSource _soundEffectSource;
    void Start()
    {
        _backgroundMusicSource = SoundManager.GetBackgroundMusicSource();
        _soundEffectSource = SoundManager.GetSoundEffectSource();

        BackgroundMusicvolumeSlider.value = _backgroundMusicSource.volume;
        SoundEffectvolumeSlider.value = _soundEffectSource.volume;
        BackgroundMusicToggle.isOn = !_backgroundMusicSource.mute;
        SoundEffectToggle.isOn = !_soundEffectSource.mute;
    }

    void OnEnable()
    {
        Time.timeScale = 0;
    }
    void OnDisable()
    {
        Time.timeScale = 1;
    }
    public void ContinueGame()
    {
        gameObject.SetActive(false);
    }

    public void ReStartGame()
    {
        HealthManager.Instance.ReSetProgress();
        gameObject.SetActive(false);
    }
    public void StartGame()
    {
        HealthManager.Instance.DirectDeath();
        gameObject.SetActive(false);
    }

    public void BackToMainMenu()
    {
        gameObject.SetActive(false);
        SceneManager.LoadSceneAsync("Menu");
    }
    public void OpenSettingMenu()
    {
        SettingMenu.enabled = true;
        TitleMenu.enabled = false;
    }
    public void CloseSettingMenu()
    {
        SettingMenu.enabled = false;
        TitleMenu.enabled = true;
    }

    public void SetBackgroundMusicVolume()
    {
        SoundManager.SetBackgroundMusicVolume(BackgroundMusicvolumeSlider.value);
    }
    public void SetSoundEffectVolume()
    {
        SoundManager.SetSoundEffectVolume(SoundEffectvolumeSlider.value);
    }
    public void ToggleBackgroundMusic()
    {
        SoundManager.ToggleBackgroundMusic();
    }
    public void ToggleSoundEffect()
    {
        SoundManager.ToggleSoundEffect();
    }
    /*
    public void Step1()
    {
        List<string> propDatas = new List<string> { "Pistol","Oboe","AlarmClock","Brick","GraphicsCard"};
        for (int i = 0; i < propDatas.Count; i++)
        {
            GuidanceSystem.Instance.AddCompletedChapter(propDatas[i]);
        }

        GuidanceSystem.Instance.CompletedMainNodes("Decoder");
    }

    public void Step2()
    {
        List<string> arr = new List<string> { "DR", "AR", "SR", "CC", "BGR-1", "CC" };
        for (int i = 0; i < arr.Count; i++)
        {
            _storyProgressData.usedChapterSpawnPoints.Add(arr[i]);
        }
        HealthManager.Instance.DirectDeath();
    }
    */
}