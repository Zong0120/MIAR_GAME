using UnityEngine;
using System.Collections.Generic;

public enum GuidanceType { Main, Side }

[CreateAssetMenu(fileName = "NewGuidanceNode", menuName = "Guidance/Guidance Node")]
public class GuidanceNode : ScriptableObject
{
    public string nodeId;
    public GuidanceType guidanceType;

    [Header("導引文字（支援多段落）")]
    [TextArea(2, 4)]
    public List<string> guidanceLines = new();

    [Header("小地圖目標（若有）")]
    public string highlightTargetId;

    [Header("下一步導引節點")]
    public string[] nextNodeIds;

    public bool isOptional;
}
