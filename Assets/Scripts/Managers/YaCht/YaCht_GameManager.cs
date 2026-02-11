using Unity.VisualScripting;
using UnityEngine;

public class YaCht_GameManager : MonoBehaviour
{
    #region Manager
    static YaCht_GameManager m_instance;
    public static YaCht_GameManager Instance { get { Init(); return m_instance; } }

    public YaCht_CardManager m_cardManager;
    public static YaCht_CardManager CardManager { get { return Instance.m_cardManager; } }

    public YaCht_RelicManager m_relicManager;
    public static YaCht_RelicManager RelicManager { get { return Instance.m_relicManager; } }

    public YaCht_StageManager m_stageManager;
    public static YaCht_StageManager StageManager { get { return Instance.m_stageManager; } }

    #endregion

    public static YaCht_PlayerData nowPlayerData = new YaCht_PlayerData();
    
    // 현재 라운드 (0부터 시작, 표시는 +1)
    public static int currentRound = 0;
    public static int totalScore = 0;
    
    // 현재 덱 선택 여부
    private static bool m_isFromDeckSelection = true;
    
    static void Init()
    {
        if (m_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            GameObject cardObj = GameObject.Find("@CardManager");
            GameObject relicObj = GameObject.Find("@RelicManager");
            GameObject stageObj = GameObject.Find("@StageManager");
            
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<YaCht_GameManager>();

                if (cardObj == null)
                {
                    cardObj = new GameObject { name = "@CardManager" };
                    cardObj.AddComponent<YaCht_CardManager>();
                }

                if (relicObj == null)
                {
                    relicObj = new GameObject { name = "@RelicManager" };
                    relicObj.AddComponent<YaCht_RelicManager>();
                }

                if (stageObj == null)
                {
                    stageObj = new GameObject { name = "@StageManager" };
                    stageObj.AddComponent<YaCht_StageManager>();
                }
            }            
            
            DontDestroyOnLoad(go);
            m_instance = go.GetComponent<YaCht_GameManager>();
            m_instance.m_cardManager = cardObj.GetComponent<YaCht_CardManager>();
            m_instance.m_relicManager = relicObj.GetComponent<YaCht_RelicManager>();
            m_instance.m_stageManager = stageObj.GetComponent<YaCht_StageManager>();
            cardObj.transform.SetParent(m_instance.transform);
            relicObj.transform.SetParent(m_instance.transform);
            stageObj.transform.SetParent(m_instance.transform);
            m_instance.m_cardManager.Init();
            m_instance.m_relicManager.Init();
            
            YaCht_EnemyDatabase.Initialize();
            YaCht_StageDatabase.Initialize();
        }
    }

    public static void Clear()
    {
        currentRound = 0; // 0라운드로 초기화 (표시는 1)
        totalScore = 0;
        RelicManager.ResetGameEffects();  // 엠블럼 게임 효과 초기화
        
        // 플레이어 유물 전체 제거
        nowPlayerData.ClearRelics();
        
        // 모든 공격 이펙트 정리는 WWEMainGame에서 관리
        
        // StageManager 스테이지 초기화
        if (StageManager != null)
        {
            StageManager.ResetGame();            
        }
    }

    public static void NextRound()
    {
        currentRound++;
        // 엠블럼 턴 효과 초기화 (RKO 효과)
        RelicManager.ResetTurnEffects();
    }
    
    public static bool IsGameOver()
    {
        return currentRound > 4;
    }
    
    public static void AddScore(int score)
    {
        totalScore += score;
    }
    
    public static void StartNewStage(int stageNumber)
    {
        if (StageManager != null)
        {
            StageManager.LoadStage(stageNumber);           
            currentRound = 0; // 0라운드로 시작 (표시는 1)
            totalScore = 0;
            
            // 엠블럼 스테이지 효과 초기화 (RKO 효과)
            RelicManager.ResetStageEffects();
        }
    }
    
    public static bool MoveToNextStage()
    {
        if (StageManager != null)
        {
            bool success = StageManager.MoveToNextStage();
            if (success)
            {               
                currentRound = 0; // 0라운드로 시작 (표시는 1)
                totalScore = 0;
                
                // 엠블럼 스테이지 효과 초기화 (RKO 효과)
                RelicManager.ResetStageEffects();
            }
            return success;
        }
        return false;
    }
    
    #region 엠블럼 덱 선택 여부 체크
    
    /// <summary>
    /// 엠블럼 덱 선택 여부 설정
    /// </summary>
    public static void SetRelicSceneFromDeckSelection()
    {
        m_isFromDeckSelection = true;
        Debug.Log("[GameManager] 엠블럼 덱 선택 여부: 엠블럼 덱 선택");
    }
    
    /// <summary>
    /// 엠블럼 보스 처치 여부 설정
    /// </summary>
    public static void SetRelicSceneFromBossDefeat()
    {
        m_isFromDeckSelection = false;
        Debug.Log("[GameManager] 엠블럼 덱 선택 여부: 엠블럼 보스 처치");
    }
    
    /// <summary>
    /// 엠블럼 덱 선택 여부 체크
    /// </summary>
    public static bool IsRelicSceneFromDeckSelection()
    {
        return m_isFromDeckSelection;
    }
    
    #endregion

    #region 엠블럼 스테이지 초기화

    /// <summary>
    /// 엠블럼 스테이지 초기화
    /// </summary>
    public static int GetTotalStageCount()
    {
        return YaCht_StageDatabase.GetTotalStageCount();
    }
    
    /// <summary>
    /// 엠블럼 스테이지 보스 여부 체크
    /// </summary>
    public static bool IsCurrentStageBoss()
    {
        if (StageManager != null)
        {
            return YaCht_StageDatabase.IsBossStage(StageManager.CurrentStageNumber);
        }
        return false;
    }
    
    #endregion
}
