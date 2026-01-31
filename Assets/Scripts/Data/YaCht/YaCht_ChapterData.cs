using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 챕터와 스테이지 정보를 담는 데이터 클래스
/// </summary>
public class YaCht_ChapterData
{
    public int ChapterNumber { get; private set; }
    public string ChapterName { get; private set; }
    public List<YaCht_EnemyData> Stages { get; private set; }

    public YaCht_ChapterData(int chapterNumber, string chapterName)
    {
        ChapterNumber = chapterNumber;
        ChapterName = chapterName;
        Stages = new List<YaCht_EnemyData>();
    }

    public void AddStage(YaCht_EnemyData enemyData)
    {
        Stages.Add(enemyData);
    }

    public YaCht_EnemyData GetStage(int stageIndex)
    {
        if (stageIndex >= 0 && stageIndex < Stages.Count)
        {
            return Stages[stageIndex];
        }
        Debug.LogError($"잘못된 스테이지 인덱스: {stageIndex}");
        return default(YaCht_EnemyData);
    }

    public int GetStageCount()
    {
        return Stages.Count;
    }
}
