using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryProgressData", menuName = "Story/ProgressData")]
public class StoryProgressData : ScriptableObject
{
    [Header("主線章節順序")]
    public List<StoryChapter> chapters;


    [Header("已解鎖的章節與區域")]
    public int currentChapterIndex = 0;
    public List<string> usedChapterSpawnPoints;
    public List<StoryZone> unlockedZones;
    public List<string> usedZoneSpawnPoints;

    /// <summary>
    /// 重置所有進度（重新開始遊戲）
    /// </summary>
    public void ResetProgress()
    {
        currentChapterIndex = 0;
        unlockedZones.Clear();
        usedChapterSpawnPoints.Clear();
    }

    /// <summary>
    /// 檢查是否可以生成觸發器
    /// </summary>
    public bool CanSpawnChapterTrigger(string spawnID)
    {
        return !usedChapterSpawnPoints.Contains(spawnID) && HasNextChapter();
    }
    public bool CanSpawnZoneTrigger(string spawnID)
    {
        return !usedZoneSpawnPoints.Contains(spawnID);
    }


    /// <summary>
    /// 解鎖下一個主線章節（順序性）
    /// </summary>
    public void UnlockNextChapter()
    {
        if (currentChapterIndex < chapters.Count)
        {
            currentChapterIndex++;
        }
    }

    /// <summary>
    /// 解鎖特定區域的敘述
    /// </summary>
    public void UnlockZone(StoryZone zone)
    {
        unlockedZones.Add(zone);
        usedZoneSpawnPoints.Add(zone.zoneID);
    }

    /// <summary>
    /// 是否還有尚未解鎖的主線章節
    /// </summary>
    public bool HasNextChapter() => currentChapterIndex < chapters.Count;

    /// <summary>
    /// 解鎖下一個主線章節
    /// </summary>
    public StoryChapter PeekNextChapter(string spawnID)
    {
        if (!HasNextChapter()) return null;
        currentChapterIndex++;
        usedChapterSpawnPoints.Add(spawnID);
        return chapters[currentChapterIndex-1];
    }
}