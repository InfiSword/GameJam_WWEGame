using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 챕터와 적 정보를 저장하는 데이터베이스
/// </summary>
public static class YaCht_EnemyDatabase
{
    private static List<YaCht_ChapterData> s_chapters = null;

    /// <summary>
    /// 데이터베이스 초기화
    /// </summary>
    public static void Initialize()
    {
        if (s_chapters != null)
            return;

        s_chapters = new List<YaCht_ChapterData>();

        // ====================================
        // 1챕터 - 한국 (스테이지 1~4)
        // ====================================
        YaCht_ChapterData chapter1 = new YaCht_ChapterData(1, "한국");
        chapter1.AddStage(new YaCht_EnemyData("골목 싸움꾼", 100, 1, 1, false, "Sprites/Enemies/Chapter1/1"));
        chapter1.AddStage(new YaCht_EnemyData("배달 라이더", 200, 2, 1, false, "Sprites/Enemies/Chapter1/2"));
        chapter1.AddStage(new YaCht_EnemyData("체육관 관장 제자", 300, 3, 1, false, "Sprites/Enemies/Chapter1/3"));
        chapter1.AddStage(new YaCht_EnemyData("한국 WWE 챔피언", 800, 4, 1, true, "Sprites/Enemies/Chapter1/4"));
        s_chapters.Add(chapter1);

        // ====================================
        // 2챕터 - 전세계 (스테이지 5~8)
        // ====================================
        YaCht_ChapterData chapter2 = new YaCht_ChapterData(2, "전세계");
        chapter2.AddStage(new YaCht_EnemyData("무에타이 선수", 1000, 5, 2, false, "Sprites/Enemies/Chapter2/5"));
        chapter2.AddStage(new YaCht_EnemyData("닌자 ", 1500, 6, 2, false, "Sprites/Enemies/Chapter2/6"));
        chapter2.AddStage(new YaCht_EnemyData("중세기사 ", 2000, 7, 2, false, "Sprites/Enemies/Chapter2/7"));
        chapter2.AddStage(new YaCht_EnemyData("WWE 챔피언", 5000, 8, 2, true, "Sprites/Enemies/Chapter2/8"));
        s_chapters.Add(chapter2);

        // ====================================
        // 3챕터 - 우주 (스테이지 9~12)
        // ====================================
        YaCht_ChapterData chapter3 = new YaCht_ChapterData(3, "우주");
        chapter3.AddStage(new YaCht_EnemyData("우주 해적", 10000, 9, 3, false, "Sprites/Enemies/Chapter3/9"));
        chapter3.AddStage(new YaCht_EnemyData("사이보그 용병", 12000, 10, 3, false, "Sprites/Enemies/Chapter3/10"));
        chapter3.AddStage(new YaCht_EnemyData("군용 안드로이드", 20000, 11, 3, false, "Sprites/Enemies/Chapter3/11"));
        chapter3.AddStage(new YaCht_EnemyData("은하 챔피언", 40000, 12, 3, true, "Sprites/Enemies/Chapter3/12"));
        s_chapters.Add(chapter3);

        Debug.Log("YaCht_EnemyDatabase 초기화 완료: 총 3개 챕터, 12개 스테이지");
    }

    /// <summary>
    /// 특정 챕터 데이터 가져오기
    /// </summary>
    public static YaCht_ChapterData GetChapter(int chapterNumber)
    {
        Initialize();

        if (chapterNumber >= 1 && chapterNumber <= s_chapters.Count)
        {
            return s_chapters[chapterNumber - 1];
        }

        Debug.LogError($"잘못된 챕터 번호: {chapterNumber}");
        return null;
    }

    /// <summary>
    /// 스테이지 번호로 적 데이터 가져오기 (1~12)
    /// </summary>
    public static YaCht_EnemyData GetEnemyByStage(int stageNumber)
    {
        Initialize();

        if (stageNumber < 1 || stageNumber > 12)
        {
            Debug.LogError($"잘못된 스테이지 번호: {stageNumber}. 1~12 범위만 지원합니다.");
            return default(YaCht_EnemyData);
        }

        // 스테이지 번호로부터 챕터와 챕터 내 스테이지 계산
        int chapterNumber = ((stageNumber - 1) / 4) + 1;
        int stageInChapter = (stageNumber - 1) % 4;

        YaCht_ChapterData chapter = GetChapter(chapterNumber);
        if (chapter != null)
        {
            return chapter.GetStage(stageInChapter);
        }

        return default(YaCht_EnemyData);
    }

    /// <summary>
    /// 모든 챕터 리스트 가져오기
    /// </summary>
    public static List<YaCht_ChapterData> GetAllChapters()
    {
        Initialize();
        return s_chapters;
    }

    /// <summary>
    /// 총 챕터 수
    /// </summary>
    public static int GetTotalChapterCount()
    {
        Initialize();
        return s_chapters.Count;
    }

    /// <summary>
    /// 총 스테이지 수 (YaCht_StageDatabase와 동기화)
    /// </summary>
    public static int GetTotalStageCount()
    {
        return YaCht_StageDatabase.GetTotalStageCount();
    }
}
