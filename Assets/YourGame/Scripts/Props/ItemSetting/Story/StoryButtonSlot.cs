using TMPro;
using UnityEngine;

public class StoryButtonSlot : MonoBehaviour
{
    private StoryText _storyText;
    [SerializeField]private TextMeshProUGUI _storyTextTitle;
    [SerializeField]private GameObject _scrollView;
    [SerializeField]private TextMeshProUGUI _storyTitleText;
    [SerializeField]private TextMeshProUGUI _storyDescribeText;
    [SerializeField]private RectTransform _ScrollViewContent;
    
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

            _scrollView.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Story text is not set.");
            
        }
    }

     private void AdjustContentHeight()
    {
        // 獲取文字內容的高度
        float textHeight = _storyDescribeText.preferredHeight;
        if(textHeight <= 900)
        {
            textHeight = 900;
        }

        // 設置 ScrollView Content 的高度
        Vector2 newSize = _ScrollViewContent.sizeDelta;
        newSize.y = textHeight;
        _ScrollViewContent.sizeDelta = new Vector2(_ScrollViewContent.sizeDelta.x, 1153 + (textHeight - 900));
    }
}
