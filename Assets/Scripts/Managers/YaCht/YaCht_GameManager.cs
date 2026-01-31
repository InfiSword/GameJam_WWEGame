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
    
    // 게임 상태
    public static int currentRound = 1;
    public static int totalScore = 0;
    public static float enemyHealth = 400f;
    public static float enemyMaxHealth = 400f;
    
    // 유물 씬 진입 - 덱 선택 후인지 여부
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

    private void Update()
    {

    }

    public static void Clear()
    {
        currentRound = 1;
        totalScore = 0;
        RelicManager.ResetAllEffects();
        
        // StageManager로 적 정보 초기화
        if (StageManager != null)
        {
            StageManager.ResetGame();
            enemyHealth = StageManager.CurrentEnemyHealth;
            enemyMaxHealth = StageManager.CurrentEnemy.m_maxHealth;
        }
    }
    
    public static void NextRound()
    {
        currentRound++;
        // 턴별 효과 초기화
        RelicManager.ResetTurnEffects();
    }
    
    public static bool IsGameOver()
    {
        return currentRound > 4 || enemyHealth <= 0;
    }
    
    public static void AddScore(int score)
    {
        totalScore += score;
    }
    
    public static void DamageEnemy(float damage)
    {
        if (StageManager != null)
        {
            StageManager.DamageEnemy(damage);
            enemyHealth = StageManager.CurrentEnemyHealth;
        }
        else
        {
            enemyHealth -= damage;
            if (enemyHealth < 0)
                enemyHealth = 0;
        }
    }
    
    public static void StartNewStage(int stageNumber)
    {
        if (StageManager != null)
        {
            StageManager.LoadStage(stageNumber);
            enemyHealth = StageManager.CurrentEnemyHealth;
            enemyMaxHealth = StageManager.CurrentEnemy.m_maxHealth;
            currentRound = 1;
            totalScore = 0;
        }
    }
    
    public static bool MoveToNextStage()
    {
        if (StageManager != null)
        {
            bool success = StageManager.MoveToNextStage();
            if (success)
            {
                enemyHealth = StageManager.CurrentEnemyHealth;
                enemyMaxHealth = StageManager.CurrentEnemy.m_maxHealth;
                currentRound = 1;
                totalScore = 0;
            }
            return success;
        }
        return false;
    }
    
    #region 유물 씬 컨텍스트 관리
    
    /// <summary>
    /// 덱 선택에서 유물 씬으로 진입
    /// </summary>
    public static void SetRelicSceneFromDeckSelection()
    {
        m_isFromDeckSelection = true;
        Debug.Log("[GameManager] 유물 씬: 덱 선택 후");
    }
    
    /// <summary>
    /// 보스 처치 후 유물 씬으로 진입
    /// </summary>
    public static void SetRelicSceneFromBossDefeat()
    {
        m_isFromDeckSelection = false;
        Debug.Log("[GameManager] 유물 씬: 보스 처치 후");
    }
    
    /// <summary>
    /// 덱 선택에서 온 것인지 확인
    /// </summary>
    public static bool IsRelicSceneFromDeckSelection()
    {
        return m_isFromDeckSelection;
    }
    
    #endregion

    #region 스테이지 헬퍼

    /// <summary>
    /// 전체 스테이지 수 가져오기
    /// </summary>
    public static int GetTotalStageCount()
    {
        return YaCht_StageDatabase.GetTotalStageCount();
    }
    
    /// <summary>
    /// 현재 스테이지가 보스전인지 확인
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
