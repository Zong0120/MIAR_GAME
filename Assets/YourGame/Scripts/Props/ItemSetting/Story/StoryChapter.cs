using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "NewStoryChapter", menuName = "Story/StoryChapter")]
public class StoryChapter : StoryText
{
    public VideoClip videoClip;
}
