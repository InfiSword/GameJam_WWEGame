using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 엠블럼 스테이지 데이터 관리
/// </summary>
public static class YaCht_StageDatabase
{
    private static Dictionary<int, YaCht_StageData> s_stageDataMap = null;

    /// <summary>
    /// 엠블럼 스테이지 데이터 초기화
    /// </summary>
    public static void Initialize()
    {
        if (s_stageDataMap != null)
            return;

        s_stageDataMap = new Dictionary<int, YaCht_StageData>();

        s_stageDataMap.Add(1, new YaCht_StageData(
            stageNumber: 1,
            chapterNumber: 1,
            backgroundPath: "Sprites/BG/Chapter1/1_1",
            bgmPath: "Audio/BGM/Chapter1/Normal",
            isBoss: false,
            description: "초기 스테이지"
        ));

        s_stageDataMap.Add(2, new YaCht_StageData(
            stageNumber: 2,
            chapterNumber: 1,
            backgroundPath: "Sprites/BG/Chapter1/1_2",
            bgmPath: "Audio/BGM/Chapter1/Normal",
            isBoss: false,
            description: "초기 스테이지"
        ));

        s_stageDataMap.Add(3, new YaCht_StageData(
            stageNumber: 3,
            chapterNumber: 1,
            backgroundPath: "Sprites/BG/Chapter1/1_3",
            bgmPath: "Audio/BGM/Chapter1/Normal",
            isBoss: false,
            description: "초기 스테이지"
        ));

        s_stageDataMap.Add(4, new YaCht_StageData(
            stageNumber: 4,
            chapterNumber: 1,
            backgroundPath: "Sprites/BG/Chapter1/1_4",
            bgmPath: "Audio/BGM/Chapter1/Boss",
            isBoss: true,
            description: "초기 스테이지",
            effectPath: "Effects/Chapter1/BossAura"
        ));

        s_stageDataMap.Add(5, new YaCht_StageData(
            stageNumber: 5,
            chapterNumber: 2,
            backgroundPath: "Sprites/BG/Chapter2/2_1",
            bgmPath: "Audio/BGM/Chapter2/Normal",
            isBoss: false,
            description: "중간 스테이지"
        ));

        s_stageDataMap.Add(6, new YaCht_StageData(
            stageNumber: 6,
            chapterNumber: 2,
            backgroundPath: "Sprites/BG/Chapter2/2_2",
            bgmPath: "Audio/BGM/Chapter2/Normal",
            isBoss: false,
            description: "중간 스테이지"
        ));

        s_stageDataMap.Add(7, new YaCht_StageData(
            stageNumber: 7,
            chapterNumber: 2,
            backgroundPath: "Sprites/BG/Chapter2/2_3",
            bgmPath: "Audio/BGM/Chapter2/Normal",
            isBoss: false,
            description: "중간 스테이지"
        ));

        s_stageDataMap.Add(8, new YaCht_StageData(
            stageNumber: 8,
            chapterNumber: 2,
            backgroundPath: "Sprites/BG/Chapter2/2_4_1",  // 1중간 스테이지
            bgmPath: "Audio/BGM/Chapter2/Boss",
            isBoss: true,
            description: "중간 스테이지",
            effectPath: "Effects/Chapter2/BossAura",
            phases: new YaCht_PhaseData[]
            {
                // 중간 스테이지 1: 중간 스테이지 100% ~ 50%
                new YaCht_PhaseData(
                    phaseNumber: 1,
                    healthThreshold: 1.0f,
                    backgroundPath: "Sprites/BG/Chapter2/2_4_1",
                    description: "중간 스테이지 1",
                    bgmPath: "",  // BGM 중간 스테이지
                    effectPath: "Effects/Chapter2/BossAura"
                ),
                // 중간 스테이지 2: 중간 스테이지 50% 중간 스테이지
                new YaCht_PhaseData(
                    phaseNumber: 2,
                    healthThreshold: 0.5f,
                    backgroundPath: "Sprites/BG/Chapter2/2_4_2",  // 2중간 스테이지
                    description: "중간 스테이지 2",
                    bgmPath: "Audio/BGM/Chapter2/BossPhase2",  // 2중간 스테이지 BGM (중간 스테이지)
                    effectPath: "Effects/Chapter2/BossAuraPhase2"  // 2중간 스테이지 특수 효과 (중간 스테이지)
                )
            }
        ));

        s_stageDataMap.Add(9, new YaCht_StageData(
            stageNumber: 9,
            chapterNumber: 3,
            backgroundPath: "Sprites/BG/Chapter3/3_1",
            bgmPath: "Audio/BGM/Chapter3/Normal",
            isBoss: false,
            description: "종료 스테이지"
        ));

        s_stageDataMap.Add(10, new YaCht_StageData(
            stageNumber: 10,
            chapterNumber: 3,
            backgroundPath: "Sprites/BG/Chapter3/3_2",
            bgmPath: "Audio/BGM/Chapter3/Normal",
            isBoss: false,
            description: "종료 스테이지"
        ));

        s_stageDataMap.Add(11, new YaCht_StageData(
            stageNumber: 11,
            chapterNumber: 3,
            backgroundPath: "Sprites/BG/Chapter3/3_3",
            bgmPath: "Audio/BGM/Chapter3/Normal",
            isBoss: false,
            description: "종료 스테이지"
        ));

        s_stageDataMap.Add(12, new YaCht_StageData(
            stageNumber: 12,
            chapterNumber: 3,
            backgroundPath: "Sprites/BG/Chapter3/3_4_1",
            bgmPath: "Audio/BGM/Chapter3/Boss",
            isBoss: true,
            description: "종료 스테이지",
            effectPath: "Effects/Chapter3/BossAura",
            phases: new YaCht_PhaseData[]
            {
                new YaCht_PhaseData(
                    phaseNumber: 1,
                    healthThreshold: 1.0f,
                    backgroundPath: "Sprites/BG/Chapter3/3_4_1",
                    description: "종료 스테이지 1",
                    bgmPath: "",
                    effectPath: "Effects/Chapter3/BossAura"
                ),
                new YaCht_PhaseData(
                    phaseNumber: 2,
                    healthThreshold: 0.5f,
                    backgroundPath: "Sprites/BG/Chapter3/3_4_2",
                    description: "종료 스테이지 2",
                    bgmPath: "Audio/BGM/Chapter3/BossPhase2",
                    effectPath: "Effects/Chapter3/BossAuraPhase2"
                )
            }
        ));

        Debug.Log("YaCht_StageDatabase 초기화 완료: 12개 엠블럼 스테이지 데이터 로드");
    }

    /// <summary>
    /// 엠블럼 스테이지 번호에 해당하는 스테이지 데이터 반환
    /// </summary>
    public static YaCht_StageData GetStageData(int stageNumber)
    {
        Initialize();

        if (s_stageDataMap.ContainsKey(stageNumber))
        {
            return s_stageDataMap[stageNumber];
        }

        Debug.LogError($"엠블럼 스테이지 번호: {stageNumber}");
        return default(YaCht_StageData);
    }

    /// <summary>
    /// 챕터 번호에 해당하는 스테이지 데이터 목록 반환
    /// </summary>
    public static List<YaCht_StageData> GetStagesByChapter(int chapterNumber)
    {
        Initialize();

        List<YaCht_StageData> chapterStages = new List<YaCht_StageData>();

        foreach (var kvp in s_stageDataMap)
        {
            if (kvp.Value.m_chapterNumber == chapterNumber)
            {
                chapterStages.Add(kvp.Value);
            }
        }

        // 스테이지 번호 순서로 정렬
        chapterStages.Sort((a, b) => a.m_stageNumber.CompareTo(b.m_stageNumber));

        return chapterStages;
    }

    /// <summary>
    /// 보스 스테이지 여부 체크
    /// </summary>
    public static bool IsBossStage(int stageNumber)
    {
        YaCht_StageData stageData = GetStageData(stageNumber);
        return stageData.m_isBossStage;
    }

    /// <summary>
    /// 배경 경로 반환
    /// </summary>
    public static string GetBackgroundPath(int stageNumber)
    {
        YaCht_StageData stageData = GetStageData(stageNumber);
        return stageData.m_backgroundResourcePath;
    }

    /// <summary>
    /// BGM 경로 반환
    /// </summary>
    public static string GetBGMPath(int stageNumber)
    {
        YaCht_StageData stageData = GetStageData(stageNumber);
        return stageData.m_bgmResourcePath;
    }

    /// <summary>
    /// 특수 효과 경로 반환
    /// </summary>
    public static string GetSpecialEffectPath(int stageNumber)
    {
        YaCht_StageData stageData = GetStageData(stageNumber);
        return stageData.m_specialEffectResourcePath;
    }

    /// <summary>
    /// 총 스테이지 개수 반환
    /// </summary>
    public static int GetTotalStageCount()
    {
        Initialize();
        return s_stageDataMap.Count;
    }

    /// <summary>
    /// 스테이지 설명 문자열 반환
    /// </summary>
    public static string GetStageDescription(int stageNumber)
    {
        YaCht_StageData stageData = GetStageData(stageNumber);
        return stageData.m_stageDescription;
    }
}
