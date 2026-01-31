using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스테이지별 배경, BGM, 특수효과를 관리하는 데이터베이스
/// </summary>
public static class YaCht_StageDatabase
{
    private static Dictionary<int, YaCht_StageData> s_stageDataMap = null;

    /// <summary>
    /// 데이터베이스 초기화
    /// </summary>
    public static void Initialize()
    {
        if (s_stageDataMap != null)
            return;

        s_stageDataMap = new Dictionary<int, YaCht_StageData>();

        // ====================================
        // 1챕터 - 초급 (스테이지 1~4)
        // ====================================
        
        // 스테이지 1: 약한 상대 - 기본 배경
        s_stageDataMap.Add(1, new YaCht_StageData(
            stageNumber: 1,
            chapterNumber: 1,
            backgroundPath: "Sprites/BG/Chapter1/1_1",
            bgmPath: "Audio/BGM/Chapter1/Normal",
            isBoss: false,
            description: "첫 번째 도전"
        ));

        // 스테이지 2: 약한 라이벌 - 기본 배경
        s_stageDataMap.Add(2, new YaCht_StageData(
            stageNumber: 2,
            chapterNumber: 1,
            backgroundPath: "Sprites/BG/Chapter1/1_2",
            bgmPath: "Audio/BGM/Chapter1/Normal",
            isBoss: false,
            description: "라이벌과의 대결"
        ));

        // 스테이지 3: 체급을 뛰어넘는 도전 - 기본 배경
        s_stageDataMap.Add(3, new YaCht_StageData(
            stageNumber: 3,
            chapterNumber: 1,
            backgroundPath: "Sprites/BG/Chapter1/1_3",
            bgmPath: "Audio/BGM/Chapter1/Normal",
            isBoss: false,
            description: "체급을 넘어서"
        ));

        // 스테이지 4: 초급 챔피언십 - 보스전 (특수 배경 + 특수 BGM + 특수 효과)
        s_stageDataMap.Add(4, new YaCht_StageData(
            stageNumber: 4,
            chapterNumber: 1,
            backgroundPath: "Sprites/BG/Chapter1/1_4",
            bgmPath: "Audio/BGM/Chapter1/Boss",
            isBoss: true,
            description: "초급 챔피언십 결전",
            effectPath: "Effects/Chapter1/BossAura"
        ));

        // ====================================
        // 2챕터 - 중급전 (스테이지 5~8)
        // ====================================
        
        // 스테이지 5: 전국대회 진출
        s_stageDataMap.Add(5, new YaCht_StageData(
            stageNumber: 5,
            chapterNumber: 2,
            backgroundPath: "Sprites/BG/Chapter2/2_1",
            bgmPath: "Audio/BGM/Chapter2/Normal",
            isBoss: false,
            description: "전국대회 시작"
        ));

        // 스테이지 6: 랭커 
        s_stageDataMap.Add(6, new YaCht_StageData(
            stageNumber: 6,
            chapterNumber: 2,
            backgroundPath: "Sprites/BG/Chapter2/2_2",
            bgmPath: "Audio/BGM/Chapter2/Normal",
            isBoss: false,
            description: "랭커와의 대결"
        ));

        // 스테이지 7: 준결승전 
        s_stageDataMap.Add(7, new YaCht_StageData(
            stageNumber: 7,
            chapterNumber: 2,
            backgroundPath: "Sprites/BG/Chapter2/2_3",
            bgmPath: "Audio/BGM/Chapter2/Normal",
            isBoss: false,
            description: "준결승 진출"
        ));

        // 스테이지 8: WWE 챔피언 - 보스전 (2페이즈)
        s_stageDataMap.Add(8, new YaCht_StageData(
            stageNumber: 8,
            chapterNumber: 2,
            backgroundPath: "Sprites/BG/Chapter2/2_4_1",  // 1페이즈 배경
            bgmPath: "Audio/BGM/Chapter2/Boss",
            isBoss: true,
            description: "WWE 챔피언 결전",
            effectPath: "Effects/Chapter2/BossAura",
            phases: new YaCht_PhaseData[]
            {
                // 페이즈 1: 체력 100% ~ 50%
                new YaCht_PhaseData(
                    phaseNumber: 1,
                    healthThreshold: 1.0f,
                    backgroundPath: "Sprites/BG/Chapter2/2_4_1",
                    description: "WWE 챔피언의 첫 번째 형태",
                    bgmPath: "",  // BGM 유지
                    effectPath: "Effects/Chapter2/BossAura"
                ),
                // 페이즈 2: 체력 50% 이하
                new YaCht_PhaseData(
                    phaseNumber: 2,
                    healthThreshold: 0.5f,
                    backgroundPath: "Sprites/BG/Chapter2/2_4_2",  // 2페이즈 배경
                    description: "WWE 챔피언의 각성 형태!",
                    bgmPath: "Audio/BGM/Chapter2/BossPhase2",  // 2페이즈 전용 BGM (선택사항)
                    effectPath: "Effects/Chapter2/BossAuraPhase2"  // 2페이즈 전용 효과 (선택사항)
                )
            }
        ));

        // ====================================
        // 3챕터 - 최고 (스테이지 9~12)
        // ====================================
        
        // 스테이지 9: 전설 도전
        s_stageDataMap.Add(9, new YaCht_StageData(
            stageNumber: 9,
            chapterNumber: 3,
            backgroundPath: "Sprites/BG/Chapter3/3_1",
            bgmPath: "Audio/BGM/Chapter3/Normal",
            isBoss: false,
            description: "전설과의 대결"
        ));

        // 스테이지 10: 하이브리드 병기
        s_stageDataMap.Add(10, new YaCht_StageData(
            stageNumber: 10,
            chapterNumber: 3,
            backgroundPath: "Sprites/BG/Chapter3/3_2",
            bgmPath: "Audio/BGM/Chapter3/Normal",
            isBoss: false,
            description: "최강의 도전자"
        ));

        // 스테이지 11: 진정 언더테이크
        s_stageDataMap.Add(11, new YaCht_StageData(
            stageNumber: 11,
            chapterNumber: 3,
            backgroundPath: "Sprites/BG/Chapter3/3_3",
            bgmPath: "Audio/BGM/Chapter3/Normal",
            isBoss: false,
            description: "최후의 관문"
        ));

        // 스테이지 12: 최종 챔피언 - 보스전
        s_stageDataMap.Add(12, new YaCht_StageData(
            stageNumber: 12,
            chapterNumber: 3,
            backgroundPath: "Sprites/BG/Chapter3/3_4",
            bgmPath: "Audio/BGM/Chapter3/Boss",
            isBoss: true,
            description: "최종 챔피언 결전",
            effectPath: "Effects/Chapter3/BossAura"
        ));

        Debug.Log("YaCht_StageDatabase 초기화 완료: 총 12개 스테이지 리소스 설정");
    }

    /// <summary>
    /// 스테이지 번호로 스테이지 데이터 가져오기
    /// </summary>
    public static YaCht_StageData GetStageData(int stageNumber)
    {
        Initialize();

        if (s_stageDataMap.ContainsKey(stageNumber))
        {
            return s_stageDataMap[stageNumber];
        }

        Debug.LogError($"잘못된 스테이지 번호: {stageNumber}");
        return default(YaCht_StageData);
    }

    /// <summary>
    /// 챕터별 모든 스테이지 데이터 가져오기
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

        // 스테이지 번호로 정렬
        chapterStages.Sort((a, b) => a.m_stageNumber.CompareTo(b.m_stageNumber));

        return chapterStages;
    }

    /// <summary>
    /// 보스 스테이지인지 확인
    /// </summary>
    public static bool IsBossStage(int stageNumber)
    {
        YaCht_StageData stageData = GetStageData(stageNumber);
        return stageData.m_isBossStage;
    }

    /// <summary>
    /// 배경 리소스 경로 가져오기
    /// </summary>
    public static string GetBackgroundPath(int stageNumber)
    {
        YaCht_StageData stageData = GetStageData(stageNumber);
        return stageData.m_backgroundResourcePath;
    }

    /// <summary>
    /// BGM 리소스 경로 가져오기
    /// </summary>
    public static string GetBGMPath(int stageNumber)
    {
        YaCht_StageData stageData = GetStageData(stageNumber);
        return stageData.m_bgmResourcePath;
    }

    /// <summary>
    /// 특수 효과 리소스 경로 가져오기
    /// </summary>
    public static string GetSpecialEffectPath(int stageNumber)
    {
        YaCht_StageData stageData = GetStageData(stageNumber);
        return stageData.m_specialEffectResourcePath;
    }

    /// <summary>
    /// 전체 스테이지 수
    /// </summary>
    public static int GetTotalStageCount()
    {
        Initialize();
        return s_stageDataMap.Count;
    }

    /// <summary>
    /// 스테이지 설명 가져오기
    /// </summary>
    public static string GetStageDescription(int stageNumber)
    {
        YaCht_StageData stageData = GetStageData(stageNumber);
        return stageData.m_stageDescription;
    }
}
