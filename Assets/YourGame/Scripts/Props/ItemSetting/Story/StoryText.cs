using UnityEngine;

public class StoryText: ScriptableObject
{
    public string title;
    [TextArea(3, 10)]
    public string content;
}