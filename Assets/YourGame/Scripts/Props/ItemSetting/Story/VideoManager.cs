using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class VideoManager : MonoBehaviour
{
    private VideoPlayer _videoPlayer;
    [SerializeField]private GameObject _videoImage;
    [SerializeField] private GameObject _videoRoot;
    [SerializeField] private Image _videoFadin;
    private float _waitTime;
    public static VideoManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }
    private void Start()
    {
        _videoRoot.SetActive(false); // 隱藏 VideoPlayer 物件
        _videoPlayer = _videoRoot.GetComponent<VideoPlayer>();
    }

    public void PlayVideo(VideoClip videoClip, float waitTime = 0)
    {
        _videoRoot.SetActive(true); // 顯示 VideoPlayer 物件
        //_videoPlayer.prepareCompleted += OnVideoPrepared; // 訂閱準備完成事件
        _videoPlayer.loopPointReached += OnVideoEnd; // 訂閱影片結束事件
        _videoPlayer.clip = videoClip;
        _waitTime = waitTime;
        StartCoroutine(FadeIn(2)); // 開始淡入
    }
    private IEnumerator FadeIn(float fadeTime)
    {
        _videoImage.SetActive(false); // 隱藏 RawImage
        _videoFadin.gameObject.SetActive(true);
        Color fadeColor = _videoFadin.color;
        fadeColor.a = 0;
        _videoFadin.color = fadeColor;

        // 開始預載入影片
        _videoPlayer.Prepare();

        // 淡入過程
        while (_videoFadin.color.a < 1)
        {
            fadeColor = _videoFadin.color;
            fadeColor.a += Time.deltaTime / fadeTime;
            _videoFadin.color = fadeColor;
            yield return null;
        }

        // 等待影片準備完成
        while (!_videoPlayer.isPrepared)
        {
            yield return null; // 等待下一幀
        }

        // 等待指定的時間
        yield return new WaitForSeconds(_waitTime);

        // 播放影片
        Time.timeScale = 0; // 暫停遊戲時間
        SoundManager.PauseBackgroundMusic(); // 暫停背景音樂
        _videoPlayer.gameObject.SetActive(true); // 啟用 VideoPlayer 物件
        _videoImage.SetActive(true); // 顯示 RawImage
        _videoPlayer.Play(); // 開始播放影片
        _videoFadin.gameObject.SetActive(false); // 隱藏淡入畫面
    }
    
    // video end close _videoImage and _videoPlayer
    private void OnVideoEnd(VideoPlayer source)
    {
        Debug.Log("Video Ended");
        _videoPlayer.loopPointReached -= OnVideoEnd; // 取消訂閱影片結束事件
        //_videoPlayer.prepareCompleted -= OnVideoPrepared; // 取消訂閱準備完成事件
        _videoPlayer.gameObject.SetActive(false); // 關閉 VideoPlayer 物件
        SoundManager.ResumeBackgroundMusic();
        Time.timeScale = 1; // 恢復遊戲時間
    }
}
