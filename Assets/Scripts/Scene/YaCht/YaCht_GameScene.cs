using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class YaCht_GameScene : MonoBehaviour
{    
    [SerializeField] private YaCht_WWEMainGame wwe;
    
    [SerializeField] private Image backgroundImage;
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private Transform specialEffectSpawnPoint;
    
    [Header("Debug Settings")]
    [SerializeField] private bool m_enableDebugKeys = true;
    
    private GameObject currentSpecialEffect;
    
    void Start()
    {
        // 씬 로드 시 모든 공격 이펙트 정리 (WWEMainGame이 있으면 호출)
        YaCht_WWEMainGame wweMainGame = FindFirstObjectByType<YaCht_WWEMainGame>();
        if (wweMainGame != null)
        {
            wweMainGame.ClearAllAttackEffects();
        }
        
        CheckAudioListener();
        
        if (YaCht_GameManager.StageManager != null)
        {
            if (YaCht_GameManager.StageManager.CurrentStageNumber == 1 && 
                YaCht_GameManager.StageManager.CurrentEnemyHealth <= 0)
            {
                YaCht_GameManager.StartNewStage(1);
            }
                                   
            YaCht_GameManager.StageManager.OnPhaseChanged += OnPhaseChanged;
        }
        
        LoadStageResources();
        
        YaCht_GameManager.CardManager.StartGame();
        wwe.Init();
        
        int currentStage = YaCht_GameManager.StageManager.CurrentStageNumber;
        string stageDesc = YaCht_GameManager.StageManager.GetCurrentStageDescription();
    }
    
    void Update()
    {
        // 디버그 키 입력 처리
        if (m_enableDebugKeys)
        {
            HandleDebugKeys();
        }
    }
    
    /// <summary>
    /// 디버그 키 입력 처리
    /// </summary>
    private void HandleDebugKeys()
    {
        // Q키: 적 처치
        if (Input.GetKeyDown(KeyCode.Q))
        {
            KillCurrentEnemy();
        }
        
        // W키: 다음 스테이지로 이동
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveToNextStageDebug();
        }
        
        // R키: 스테이지를 1로 초기화
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetToStage1();
        }
    }
    
    /// <summary>
    /// [디버그] 현재 적 즉시 처치
    /// </summary>
    private void KillCurrentEnemy()
    {
        if (wwe != null && wwe.CurrentEnemy != null)
        {
            float currentHealth = wwe.CurrentEnemy.CurrentHealth;
            wwe.CurrentEnemy.TakeDamage(currentHealth, true);
            Debug.Log($"[DEBUG] Q키 입력: 적 즉시 처치 ({currentHealth} 데미지)");
        }
        else
        {
            Debug.LogWarning("[DEBUG] 현재 적이 존재하지 않습니다!");
        }
    }
    
    /// <summary>
    /// [디버그] 다음 스테이지로 강제 이동
    /// </summary>
    private void MoveToNextStageDebug()
    {
        if (YaCht_GameManager.StageManager == null)
        {
            Debug.LogWarning("[DEBUG] StageManager가 없습니다!");
            return;
        }
        
        int currentStage = YaCht_GameManager.StageManager.CurrentStageNumber;
        int totalStages = YaCht_GameManager.GetTotalStageCount();
        
        if (currentStage >= totalStages)
        {
            Debug.LogWarning($"[DEBUG] 이미 최종 스테이지입니다! (현재: {currentStage}/{totalStages})");
            return;
        }
        
        Debug.Log($"[DEBUG] W키 입력: 스테이지 {currentStage} 에서 {currentStage + 1} 이동");
        
        // 현재 스테이지 보스 확인
        bool isBoss = YaCht_GameManager.IsCurrentStageBoss();
        
        if (isBoss)
        {
            // 보스 스테이지에서 왔을 때 -> 유물 씬으로 이동
            YaCht_GameManager.SetRelicSceneFromBossDefeat();
            SceneManager.LoadScene("YaCht_RelicScene");
        }
        else
        {
            // 일반 스테이지에서 왔을 때 -> 바로 다음 스테이지로
            bool success = YaCht_GameManager.MoveToNextStage();
            if (success)
            {
                SceneManager.LoadScene("YaCht_GameScene");
            }
            else
            {
                Debug.LogError("[DEBUG] 다음 스테이지로 이동 실패!");
            }
        }
    }
    
    /// <summary>
    /// [디버그] 스테이지를 1로 초기화
    /// </summary>
    private void ResetToStage1()
    {
        Debug.Log("[DEBUG] R키 입력: 스테이지를 1로 초기화");
        YaCht_GameManager.Clear();
        YaCht_GameManager.StartNewStage(1);
        SceneManager.LoadScene("YaCht_GameScene");
    }
    
    /// <summary>
    /// 씬에 AudioListener가 있는지 확인
    /// </summary>
    private void CheckAudioListener()
    {
        AudioListener listener = FindFirstObjectByType<AudioListener>();
        if (listener == null)
        {
            Debug.LogError("[GameScene] AudioListener를 찾을 수 없습니다! 오디오가 재생되지 않습니다.");
            Debug.LogWarning("[GameScene] Main Camera에 AudioListener 컴포넌트를 추가해주세요.");
            
            // 코드로 Camera에 AudioListener 추가 (임시 처리)
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                listener = mainCamera.gameObject.AddComponent<AudioListener>();
                Debug.Log("[GameScene] Main Camera에 AudioListener를 코드로 추가했습니다.");
            }
            else
            {
                Debug.LogError("[GameScene] Main Camera를 찾을 수 없습니다!");
            }
        }
        else
        {
            Debug.Log($"[GameScene] AudioListener 발견: {listener.gameObject.name}");
        }
    }
    
    /// <summary>
    /// 스테이지에 맞게 배경, BGM, 특수효과 로드
    /// </summary>
    private void LoadStageResources()
    {
        if (YaCht_GameManager.StageManager == null)
        {
            Debug.LogWarning("[GameScene] StageManager가 없습니다!");
            return;
        }
        
        LoadBackground();
        
        if (YaCht_GameManager.StageManager.IsCurrentEnemyBoss())
        {
            SpawnSpecialEffect();
        }
    }
    
    /// <summary>
    /// 배경 이미지 로드
    /// </summary>
    private void LoadBackground()
    {
        if (backgroundImage == null)
        {
            Debug.LogWarning("[GameScene] backgroundImage가 할당되지 않았습니다!");
            return;
        }
        
        string backgroundPath = YaCht_GameManager.StageManager.GetCurrentBackgroundPath();
        LoadBackgroundFromPath(backgroundPath);
    }
    
    /// <summary>
    /// 경로에 맞게 배경 이미지 로드 (public으로 만들어서 외부에서 호출 가능)
    /// </summary>
    public void LoadBackgroundFromPath(string backgroundPath)
    {
        if (backgroundImage == null)
        {
            Debug.LogWarning("[GameScene] backgroundImage가 할당되지 않았습니다!");
            return;
        }
        
        if (string.IsNullOrEmpty(backgroundPath))
        {
            Debug.LogWarning("[GameScene] 배경 경로가 비어있습니다!");
            return;
        }
        
        Sprite backgroundSprite = Resources.Load<Sprite>(backgroundPath);
        
        if (backgroundSprite != null)
        {
            backgroundImage.sprite = backgroundSprite;
            Debug.Log($"[GameScene] 배경 로드 완료: {backgroundPath}");
        }
        else
        {
            Debug.LogWarning($"[GameScene] 배경을 찾을 수 없습니다: {backgroundPath}");
        }
    }
    
    /// <summary>
    /// 스테이지에 맞게 BGM 재생
    /// </summary>
    private void PlayBGMFromPath(string bgmPath)
    {
        if (bgmAudioSource == null)
        {
            Debug.LogWarning("[GameScene] bgmAudioSource가 할당되지 않았습니다!");
            return;
        }
        
        if (string.IsNullOrEmpty(bgmPath))
        {
            Debug.LogWarning("[GameScene] BGM 경로가 비어있습니다!");
            return;
        }
        
        AudioClip bgmClip = Resources.Load<AudioClip>(bgmPath);
        
        if (bgmClip != null)
        {
            bgmAudioSource.clip = bgmClip;
            bgmAudioSource.loop = true;
            bgmAudioSource.volume = 0.7f; // 기본 볼륨 (70%)
            bgmAudioSource.Play();
            
            Debug.Log($"[GameScene] BGM 재생 시작: {bgmPath}");
            Debug.Log($"[GameScene] AudioSource 상태 - isPlaying: {bgmAudioSource.isPlaying}, volume: {bgmAudioSource.volume}, mute: {bgmAudioSource.mute}");
        }
        else
        {
            Debug.LogWarning($"[GameScene] BGM을 찾을 수 없습니다: {bgmPath}");
        }
    }
    
    /// <summary>
    /// 특수 효과 생성 (보스전)
    /// </summary>
    private void SpawnSpecialEffect()
    {
        string effectPath = YaCht_GameManager.StageManager.GetCurrentSpecialEffectPath();
        SpawnSpecialEffectFromPath(effectPath);
    }
    
    /// <summary>
    /// 경로에 맞게 특수 효과 생성
    /// </summary>
    private void SpawnSpecialEffectFromPath(string effectPath)
    {
        if (string.IsNullOrEmpty(effectPath))
        {
            Debug.Log("[GameScene] 특수 효과 경로가 비어있습니다. (의미: 특수 효과가 없는 스테이지)");
            return;
        }
        
        GameObject effectPrefab = Resources.Load<GameObject>(effectPath);
        
        if (effectPrefab != null)
        {
            if (currentSpecialEffect != null)
            {
                Destroy(currentSpecialEffect);
            }
            
            if (specialEffectSpawnPoint != null)
            {
                currentSpecialEffect = Instantiate(effectPrefab, specialEffectSpawnPoint.position, Quaternion.identity);
                currentSpecialEffect.transform.SetParent(specialEffectSpawnPoint);
            }
            else
            {
                currentSpecialEffect = Instantiate(effectPrefab);
            }
            
            Debug.Log($"[GameScene] 특수 효과 생성: {effectPath}");
        }
        else
        {
            Debug.LogWarning($"[GameScene] 특수 효과를 찾을 수 없습니다: {effectPath}");
        }
    }
    
    private void OnDestroy()
    {
        if (currentSpecialEffect != null)
        {
            Destroy(currentSpecialEffect);
        }
        
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
        }
        
        if (YaCht_GameManager.StageManager != null)
        {
            YaCht_GameManager.StageManager.OnPhaseChanged -= OnPhaseChanged;
        }
    }
    
    /// <summary>
    /// 스테이지 페이즈 변경 이벤트 핸들러
    /// </summary>
    private void OnPhaseChanged(int phaseNumber, YaCht_PhaseData phaseData)
    {
        Debug.Log($"[GameScene] 스테이지 {phaseNumber} 페이즈 변경: {phaseData.m_phaseDescription}");
        
        if (!string.IsNullOrEmpty(phaseData.m_backgroundResourcePath))
        {
            LoadBackgroundFromPath(phaseData.m_backgroundResourcePath);
        }
        
        if (!string.IsNullOrEmpty(phaseData.m_bgmResourcePath))
        {
            PlayBGMFromPath(phaseData.m_bgmResourcePath);
        }
        
        if (!string.IsNullOrEmpty(phaseData.m_specialEffectResourcePath))
        {
            SpawnSpecialEffectFromPath(phaseData.m_specialEffectResourcePath);
        }
    }
}
