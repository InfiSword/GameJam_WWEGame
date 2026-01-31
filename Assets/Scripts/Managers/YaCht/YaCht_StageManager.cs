using UnityEngine;
using System;

/// <summary>
/// 스테이지 진행과 적 정보를 관리하는 매니저
/// </summary>
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

    // 현재 스테이지 정보
    public int CurrentStageNumber { get; private set; } = 1;
    public YaCht_EnemyData CurrentEnemy { get; private set; }
    public YaCht_StageData CurrentStageData { get; private set; }
    public float CurrentEnemyHealth { get; private set; }
    public int CurrentPhase { get; private set; } = 1;  // 현재 페이즈

    // 이벤트: 보스 처치 시 호출
    public event Action OnBossDefeated;
    
    // 이벤트: 일반 적 처치 시 호출
    public event Action OnEnemyDefeatedNormal;
    
    // 이벤트: 페이즈 전환 시 호출
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
    /// 게임 시작 - 첫 스테이지로 초기화
    /// </summary>
    public void StartGame()
    {
        CurrentStageNumber = 1;
        LoadStage(CurrentStageNumber);
    }

    /// <summary>
    /// 특정 스테이지 로드
    /// </summary>
    public void LoadStage(int stageNumber)
    {
        CurrentStageNumber = stageNumber;
        CurrentEnemy = YaCht_EnemyDatabase.GetEnemyByStage(stageNumber);
        CurrentStageData = YaCht_StageDatabase.GetStageData(stageNumber);
        CurrentEnemyHealth = CurrentEnemy.m_maxHealth;
        CurrentPhase = 1;  // 페이즈 초기화

        Debug.Log($"[StageManager] 스테이지 {stageNumber} 로드: {CurrentEnemy.m_name} (체력: {CurrentEnemyHealth})");
        Debug.Log($"[StageManager] 배경: {CurrentStageData.m_backgroundResourcePath}");
        Debug.Log($"[StageManager] BGM: {CurrentStageData.m_bgmResourcePath}");
        
        if (CurrentStageData.HasMultiplePhases)
        {
            Debug.Log($"[StageManager] 다중 페이즈 스테이지: {CurrentStageData.m_phases.Length}개 페이즈");
        }

        // GameManager에서 적 정보 동기화
        YaCht_GameManager.enemyHealth = CurrentEnemyHealth;
        YaCht_GameManager.enemyMaxHealth = CurrentEnemy.m_maxHealth;
    }

    /// <summary>
    /// 적에게 데미지 입히기
    /// </summary>
    public void DamageEnemy(float damage)
    {
        float previousHealth = CurrentEnemyHealth;
        CurrentEnemyHealth -= damage;
        if (CurrentEnemyHealth < 0)
            CurrentEnemyHealth = 0;

        YaCht_GameManager.enemyHealth = CurrentEnemyHealth;

        Debug.Log($"[StageManager] {CurrentEnemy.m_name}에게 {damage} 데미지! 남은 체력: {CurrentEnemyHealth}/{CurrentEnemy.m_maxHealth}");

        // 페이즈 전환 체크 (다중 페이즈 스테이지인 경우)
        CheckPhaseTransition(previousHealth, CurrentEnemyHealth);

        if (CurrentEnemyHealth <= 0)
        {
            OnEnemyDefeated();
        }
    }
    
    /// <summary>
    /// 페이즈 전환 체크
    /// </summary>
    private void CheckPhaseTransition(float previousHealth, float currentHealth)
    {
        if (!CurrentStageData.HasMultiplePhases)
            return;
        
        float previousPercent = previousHealth / CurrentEnemy.m_maxHealth;
        float currentPercent = currentHealth / CurrentEnemy.m_maxHealth;
        
        YaCht_PhaseData? previousPhase = CurrentStageData.GetPhaseByHealthPercent(previousPercent);
        YaCht_PhaseData? currentPhase = CurrentStageData.GetPhaseByHealthPercent(currentPercent);
        
        // 페이즈가 변경되었는지 확인
        if (previousPhase.HasValue && currentPhase.HasValue)
        {
            if (previousPhase.Value.m_phaseNumber != currentPhase.Value.m_phaseNumber)
            {
                CurrentPhase = currentPhase.Value.m_phaseNumber;
                Debug.Log($"[StageManager] ★ 페이즈 전환! Phase {CurrentPhase} ★");
                Debug.Log($"[StageManager] {currentPhase.Value.m_phaseDescription}");
                
                // 페이즈 전환 이벤트 발생
                OnPhaseChanged?.Invoke(CurrentPhase, currentPhase.Value);
            }
        }
    }

    /// <summary>
    /// 적 처치 시 호출
    /// </summary>
    private void OnEnemyDefeated()
    {
        Debug.Log($"[StageManager] {CurrentEnemy.m_name} 처치! 스테이지 {CurrentStageNumber} 클리어!");

        // YaCht_StageDatabase를 사용하여 보스전 확인
        if (YaCht_StageDatabase.IsBossStage(CurrentStageNumber))
        {
            Debug.Log($"보스 스테이지 처치! 챕터 {CurrentEnemy.m_chapterNumber} 클리어 완료");
            
            // 보스 처치 이벤트 발생
            OnBossDefeated?.Invoke();
        }
        else
        {
            // 일반 적 처치 이벤트 발생
            OnEnemyDefeatedNormal?.Invoke();
        }

        // 다음 스테이지가 있는지 확인
        if (CurrentStageNumber < YaCht_StageDatabase.GetTotalStageCount())
        {
            Debug.Log("다음 스테이지로 진행 가능합니다.");
        }
        else
        {
            Debug.Log("★★★ 모든 스테이지 클리어! 게임 완료! ★★★");
        }
    }

    /// <summary>
    /// 다음 스테이지로 이동
    /// </summary>
    public bool MoveToNextStage()
    {
        if (CurrentEnemyHealth > 0)
        {
            Debug.LogWarning("현재 적을 처치하지 않았습니다!");
            return false;
        }

        if (CurrentStageNumber >= YaCht_StageDatabase.GetTotalStageCount())
        {
            Debug.LogWarning("마지막 스테이지입니다!");
            return false;
        }

        LoadStage(CurrentStageNumber + 1);
        return true;
    }

    /// <summary>
    /// 현재 챕터 번호 가져오기
    /// </summary>
    public int GetCurrentChapterNumber()
    {
        return CurrentEnemy.m_chapterNumber;
    }

    /// <summary>
    /// 현재 보스 스테이지인지 확인 (YaCht_StageDatabase 활용)
    /// </summary>
    public bool IsCurrentEnemyBoss()
    {
        return YaCht_StageDatabase.IsBossStage(CurrentStageNumber);
    }

    /// <summary>
    /// 현재 적 체력 퍼센트 (0~1)
    /// </summary>
    public float GetEnemyHealthPercent()
    {
        if (CurrentEnemy.m_maxHealth <= 0)
            return 0f;

        return CurrentEnemyHealth / CurrentEnemy.m_maxHealth;
    }

    /// <summary>
    /// 게임 리셋
    /// </summary>
    public void ResetGame()
    {
        CurrentStageNumber = 1;
        LoadStage(1);
        Debug.Log("[StageManager] 게임 리셋 완료");
    }

    /// <summary>
    /// 스테이지 정보 UI 표시용 문자열
    /// </summary>
    public string GetStageInfoString()
    {
        return $"챕터 {CurrentEnemy.m_chapterNumber} - 스테이지 {CurrentStageNumber}\n" +
               $"{CurrentEnemy.m_name}\n" +
               $"체력: {CurrentEnemyHealth:F0} / {CurrentEnemy.m_maxHealth}";
    }

    /// <summary>
    /// 현재 스테이지 설명 가져오기
    /// </summary>
    public string GetCurrentStageDescription()
    {
        return CurrentStageData.m_stageDescription;
    }

    /// <summary>
    /// 현재 스테이지 배경 경로 가져오기
    /// </summary>
    public string GetCurrentBackgroundPath()
    {
        return CurrentStageData.m_backgroundResourcePath;
    }

    /// <summary>
    /// 현재 스테이지 BGM 경로 가져오기
    /// </summary>
    public string GetCurrentBGMPath()
    {
        return CurrentStageData.m_bgmResourcePath;
    }

    /// <summary>
    /// 현재 스테이지 특수 효과 경로 가져오기
    /// </summary>
    public string GetCurrentSpecialEffectPath()
    {
        return CurrentStageData.m_specialEffectResourcePath;
    }

    /// <summary>
    /// 현재 페이즈 데이터 가져오기
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
