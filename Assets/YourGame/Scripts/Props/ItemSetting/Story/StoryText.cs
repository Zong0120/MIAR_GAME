using UnityEngine;

public class StoryText: ScriptableObject
{
    public string title;
    [TextArea(4,15)]public string content;
}