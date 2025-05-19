using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayerInputAction;

public class GameingSetting : MonoBehaviour
{
    [SerializeField] private Slider BackgroundMusicvolumeSlider;
    [SerializeField] private Slider SoundEffectvolumeSlider;
    [SerializeField] private Toggle BackgroundMusicToggle;
    [SerializeField] private Toggle SoundEffectToggle;

    [Header("Canvas")]
    [SerializeField] private Canvas SettingMenu;
    [SerializeField] private Canvas TitleMenu;

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

    public void BackToMainMenu()
    {
        gameObject.SetActive(false);
        MenuManager.Instance.OpenMenu("Menu");
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
}