using UnityEngine;
using System;

public class YaCht_StageManager : MonoBehaviour
{
    private static YaCht_StageManager s_instance;
    public static YaCht_StageManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                GameObject go = new GameObject("@StageManager");
                s_instance = go.AddComponent<YaCht_StageManager>();
                DontDestroyOnLoad(go);
            }
            return s_instance;
        }
    }

    // 현재 스테이지 번호 관리
    public int CurrentStageNumber { get; private set; } = 1;
    public YaCht_StageData CurrentStageData { get; private set; }
    public int CurrentPhase { get; private set; } = 1;
    
    // 현재 스테이지 적 인스턴스 관리
    private YaCht_Enemy m_currentEnemyInstance;
    public YaCht_Enemy CurrentEnemyInstance => m_currentEnemyInstance;
    
    // 현재 스테이지 적 데이터 관리
    public YaCht_EnemyData CurrentEnemy
    {
        get
        {
            return YaCht_EnemyDatabase.GetEnemyByStage(CurrentStageNumber);
        }
    }
    
    // 현재 스테이지 적 체력 관리
    public float CurrentEnemyHealth => m_currentEnemyInstance != null ? m_currentEnemyInstance.CurrentHealth : 0f;

    // 이벤트: 보스 적 처치 이벤트
    public event Action OnBossDefeated;
    
    // 이벤트: 일반 적 처치 이벤트
    public event Action OnEnemyDefeatedNormal;
    
    // 이벤트: 페이즈 변경 이벤트
    public event Action<int, YaCht_PhaseData> OnPhaseChanged;

    private void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        s_instance = this;
        DontDestroyOnLoad(gameObject);
        
        YaCht_EnemyDatabase.Initialize();
        YaCht_StageDatabase.Initialize();
    }

    /// <summary>
    /// 현재 스테이지 적 인스턴스 등록
    /// </summary>
    public void RegisterEnemy(YaCht_Enemy enemy)
    {
        // 현재 스테이지 적 인스턴스 등록 제거
        if (m_currentEnemyInstance != null)
        {
            m_currentEnemyInstance.OnHealthChanged -= OnEnemyHealthChanged;
            m_currentEnemyInstance.OnDeath -= OnEnemyDeath;
        }
        
        m_currentEnemyInstance = enemy;
        
        if (enemy != null)
        {
            // 현재 스테이지 적 인스턴스 등록
            m_currentEnemyInstance.OnHealthChanged += OnEnemyHealthChanged;
            m_currentEnemyInstance.OnDeath += OnEnemyDeath;
            
            Debug.Log($"[StageManager] 현재 스테이지 적: {enemy.EnemyName}");
        }
    }
    
    /// <summary>
    /// 현재 스테이지 적 인스턴스 등록 제거
    /// </summary>
    public void UnregisterEnemy()
    {
        // 현재 스테이지 적 인스턴스 등록 제거
        if (m_currentEnemyInstance != null)
        {
            m_currentEnemyInstance.OnHealthChanged -= OnEnemyHealthChanged;
            m_currentEnemyInstance.OnDeath -= OnEnemyDeath;
        }
        
        m_currentEnemyInstance = null;
        Debug.Log("[StageManager] 현재 스테이지 적 등록 제거");
    }
    
    /// <summary>
    /// 현재 스테이지 적 체력 변경 이벤트
    /// </summary>
    private void OnEnemyHealthChanged(float currentHealth, float maxHealth)
    {
        if (!CurrentStageData.HasMultiplePhases)
            return;
        
        float currentPercent = currentHealth / maxHealth;
        YaCht_PhaseData? currentPhase = CurrentStageData.GetPhaseByHealthPercent(currentPercent);
        
        // 현재 스테이지 적 체력 변경 이벤트 처리
        if (currentPhase.HasValue && currentPhase.Value.m_phaseNumber != CurrentPhase)
        {
            CurrentPhase = currentPhase.Value.m_phaseNumber;
            Debug.Log($"[StageManager] 현재 스테이지 적 체력 변경! Phase {CurrentPhase}");
            Debug.Log($"[StageManager] {currentPhase.Value.m_phaseDescription}");
            
            // 현재 스테이지 적 체력 변경 이벤트 발생
            OnPhaseChanged?.Invoke(CurrentPhase, currentPhase.Value);
        }
    }
    
    /// <summary>
    /// 현재 스테이지 적 처치 이벤트
    /// </summary>
    private void OnEnemyDeath()
    {
        Debug.Log($"[StageManager] {CurrentEnemy.m_name} 처치! 스테이지 {CurrentStageNumber} 보스!");

        // YaCht_StageDatabase에서 보스 스테이지 여부 확인
        if (YaCht_StageDatabase.IsBossStage(CurrentStageNumber))
        {
            Debug.Log($"보스 스테이지 처치! 챕터 {CurrentEnemy.m_chapterNumber} 보스 처치");
            
            // 보스 스테이지 처치 이벤트 발생
            OnBossDefeated?.Invoke();
        }
        else
        {
            // 일반 스테이지 처치 이벤트 발생
            OnEnemyDefeatedNormal?.Invoke();
        }
    }

    /// <summary>
    /// 게임 시작
    /// </summary>
    public void StartGame()
    {
        CurrentStageNumber = 1;
        LoadStage(CurrentStageNumber);
    }

    /// <summary>
    /// 스테이지 로드
    /// </summary>
    public void LoadStage(int stageNumber)
    {
        CurrentStageNumber = stageNumber;
        CurrentStageData = YaCht_StageDatabase.GetStageData(stageNumber);
        CurrentPhase = 1;         
    }

    /// <summary>
    /// 스테이지 이동
    /// </summary>
    public bool MoveToNextStage()
    {
        if (CurrentEnemyHealth > 0)
        {
            Debug.LogWarning("스테이지 이동 실패! 현재 스테이지 적 체력이 0 이상입니다.");
            return false;
        }

        if (CurrentStageNumber >= YaCht_StageDatabase.GetTotalStageCount())
        {
            Debug.LogWarning("스테이지 이동 실패! 마지막 스테이지입니다.");
            return false;
        }

        LoadStage(CurrentStageNumber + 1);
        return true;
    }

    /// <summary>
    /// 현재 챕터 번호 반환
    /// </summary>
    public int GetCurrentChapterNumber()
    {
        return CurrentEnemy.m_chapterNumber;
    }

    /// <summary>
    /// 현재 스테이지 적 보스 여부 확인
    /// </summary>
    public bool IsCurrentEnemyBoss()
    {
        return YaCht_StageDatabase.IsBossStage(CurrentStageNumber);
    }

    /// <summary>
    /// 현재 스테이지 적 체력 퍼센트 반환
    /// </summary>
    public float GetEnemyHealthPercent()
    {
        if (CurrentEnemy.m_maxHealth <= 0)
            return 0f;

        return CurrentEnemyHealth / CurrentEnemy.m_maxHealth;
    }

    /// <summary>
    /// 게임 초기화
    /// </summary>
    public void ResetGame()
    {
        CurrentStageNumber = 1;
        LoadStage(1);
        Debug.Log("[StageManager] 게임 초기화");
    }

    /// <summary>
    /// 스테이지 정보 문자열 반환
    /// </summary>
    public string GetStageInfoString()
    {
        return $"챕터   \t{CurrentEnemy.m_chapterNumber}\n"+
               $"스테이지  {CurrentStageNumber}\n";
    }

    /// <summary>
    /// 현재 스테이지 설명 문자열 반환
    /// </summary>
    public string GetCurrentStageDescription()
    {
        return CurrentStageData.m_stageDescription;
    }

    /// <summary>
    /// 현재 스테이지 배경 경로 반환
    /// </summary>
    public string GetCurrentBackgroundPath()
    {
        return CurrentStageData.m_backgroundResourcePath;
    }

    /// <summary>
    /// 현재 스테이지 BGM 경로 반환
    /// </summary>
    public string GetCurrentBGMPath()
    {
        return CurrentStageData.m_bgmResourcePath;
    }

    /// <summary>
    /// 현재 스테이지 특수 효과 경로 반환
    /// </summary>
    public string GetCurrentSpecialEffectPath()
    {
        return CurrentStageData.m_specialEffectResourcePath;
    }

    /// <summary>
    /// 현재 페이즈 데이터 반환
    /// </summary>
    public YaCht_PhaseData? GetCurrentPhaseData()
    {
        if (!CurrentStageData.HasMultiplePhases)
            return null;
        
        float healthPercent = GetEnemyHealthPercent();
        return CurrentStageData.GetPhaseByHealthPercent(healthPercent);
    }
    
    public bool IsMultiPhaseStage()
    {
        return CurrentStageData.HasMultiplePhases;
    }
}
