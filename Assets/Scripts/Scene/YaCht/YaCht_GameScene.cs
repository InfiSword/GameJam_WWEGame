using UnityEngine;
using UnityEngine.UI;

public class YaCht_GameScene : MonoBehaviour
{    
    [SerializeField] private YaCht_WWEMainGame wwe;
    
    [SerializeField] private Image backgroundImage;
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private Transform specialEffectSpawnPoint;
    
    private GameObject currentSpecialEffect;
    
    void Start()
    {
        if (YaCht_GameManager.StageManager != null)
        {
            if (YaCht_GameManager.StageManager.CurrentStageNumber == 1 && 
                YaCht_GameManager.StageManager.CurrentEnemyHealth <= 0)
            {
                YaCht_GameManager.StartNewStage(1);
            }
                                   
            // 페이즈 전환 이벤트 구독
            YaCht_GameManager.StageManager.OnPhaseChanged += OnPhaseChanged;
        }
        
        // 스테이지 리소스 로드
        LoadStageResources();
        
        // 게임 시작
        YaCht_GameManager.CardManager.StartGame();
        wwe.Init();
        
        int currentStage = YaCht_GameManager.StageManager.CurrentStageNumber;
        string stageDesc = YaCht_GameManager.StageManager.GetCurrentStageDescription();
        Debug.Log($"[GameScene] 스테이지 {currentStage} 시작 - {stageDesc}");
    }
    
    /// <summary>
    /// 현재 스테이지의 배경, BGM, 특수효과 로드
    /// </summary>
    private void LoadStageResources()
    {
        if (YaCht_GameManager.StageManager == null)
        {
            Debug.LogWarning("[GameScene] StageManager가 없습니다!");
            return;
        }
        
        // 배경 이미지 로드
        LoadBackground();
        
        // BGM 재생
        PlayBGM();
        
        // 보스 스테이지면 특수 효과 생성
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
    /// 지정된 경로로 배경 이미지 로드
    /// </summary>
    private void LoadBackgroundFromPath(string backgroundPath)
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
    /// BGM 재생
    /// </summary>
    private void PlayBGM()
    {
        if (bgmAudioSource == null)
        {
            Debug.LogWarning("[GameScene] bgmAudioSource가 할당되지 않았습니다!");
            return;
        }
        
        string bgmPath = YaCht_GameManager.StageManager.GetCurrentBGMPath();
        PlayBGMFromPath(bgmPath);
    }
    
    /// <summary>
    /// 지정된 경로로 BGM 재생
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
            bgmAudioSource.Play();
            Debug.Log($"[GameScene] BGM 재생 시작: {bgmPath}");
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
    /// 지정된 경로로 특수 효과 생성
    /// </summary>
    private void SpawnSpecialEffectFromPath(string effectPath)
    {
        if (string.IsNullOrEmpty(effectPath))
        {
            Debug.Log("[GameScene] 특수 효과 경로가 비어있습니다. (정상: 특수 효과가 없는 스테이지)");
            return;
        }
        
        GameObject effectPrefab = Resources.Load<GameObject>(effectPath);
        
        if (effectPrefab != null)
        {
            // 기존 특수 효과가 있으면 제거
            if (currentSpecialEffect != null)
            {
                Destroy(currentSpecialEffect);
            }
            
            // 새 특수 효과 생성
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
        // 씬 종료 시 특수 효과 정리
        if (currentSpecialEffect != null)
        {
            Destroy(currentSpecialEffect);
        }
        
        // BGM 정지
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
        }
        
        // 페이즈 전환 이벤트 구독 해제
        if (YaCht_GameManager.StageManager != null)
        {
            YaCht_GameManager.StageManager.OnPhaseChanged -= OnPhaseChanged;
        }
    }
    
    /// <summary>
    /// 페이즈 전환 이벤트 핸들러
    /// </summary>
    private void OnPhaseChanged(int phaseNumber, YaCht_PhaseData phaseData)
    {
        Debug.Log($"[GameScene] 페이즈 {phaseNumber} 전환: {phaseData.m_phaseDescription}");
        
        // 배경 변경
        if (!string.IsNullOrEmpty(phaseData.m_backgroundResourcePath))
        {
            LoadBackgroundFromPath(phaseData.m_backgroundResourcePath);
        }
        
        // BGM 변경 (지정된 경우)
        if (!string.IsNullOrEmpty(phaseData.m_bgmResourcePath))
        {
            PlayBGMFromPath(phaseData.m_bgmResourcePath);
        }
        
        // 특수 효과 변경 (지정된 경우)
        if (!string.IsNullOrEmpty(phaseData.m_specialEffectResourcePath))
        {
            SpawnSpecialEffectFromPath(phaseData.m_specialEffectResourcePath);
        }
    }
}
