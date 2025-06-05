using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryButtonSlot : MonoBehaviour
{
    private StoryText _storyText;
    [SerializeField]private TextMeshProUGUI _storyTextTitle;
    [SerializeField]private GameObject _scrollView;
    [SerializeField]private TextMeshProUGUI _storyTitleText;
    [SerializeField]private TextMeshProUGUI _storyDescribeText;
    [SerializeField]private RectTransform _ScrollViewContent;
    [SerializeField]private Button _chapterButton;
    
    public void SetStory(StoryText text)
    {
        _storyText = text;
        _storyTextTitle.text = text.title;
    }

    public void OnClick()
    {
        if (_storyText != null)
        {
            _storyTitleText.text = _storyText.title;
            _storyDescribeText.text = _storyText.content;

            AdjustContentHeight();
            if(_storyText as StoryChapter)
            {
                _chapterButton.gameObject.SetActive(true);
                _chapterButton.onClick.AddListener(() =>
                {
                    VideoManager.Instance.PlayVideo((_storyText as StoryChapter).videoClip);
                });
            }
            else
            {
                _chapterButton.gameObject.SetActive(false);
            }
            _scrollView.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Story text is not set.");
            
        }
    }

    private void AdjustContentHeight()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_ScrollViewContent);
    }

}
