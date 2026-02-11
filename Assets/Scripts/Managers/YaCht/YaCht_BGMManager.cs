using UnityEngine;

/// <summary>
/// 선택한 레슬러에 따라 MainGame에서 BGM을 재생하는 매니저
/// </summary>
public class YaCht_BGMManager : MonoBehaviour
{
    private static YaCht_BGMManager m_instance;
    public static YaCht_BGMManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject go = new GameObject("@BGMManager");
                m_instance = go.AddComponent<YaCht_BGMManager>();
                DontDestroyOnLoad(go);
            }
            return m_instance;
        }
    }

    private AudioSource m_audioSource; // 배경 BGM용
    private AudioSource m_selectAudioSource; // 선택 BGM용
    private AudioSource m_effectAudioSource; // 이펙트 사운드용 (카드 사운드, 콤보 사운드)
    private AudioSource m_sskillAudioSource; // S급 기술 사운드 전용 (겹침 방지)
    private string m_currentBGMPath = "";
    private string m_currentSelectBGMPath = "";
    private float m_originalBGMVolume = 0.7f; // 원래 BGM 볼륨 저장
    private Coroutine m_bgmResumeCoroutine = null; // BGM 복원 코루틴

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSource();
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// AudioSource 초기화
    /// </summary>
    private void InitializeAudioSource()
    {
        // 배경 BGM용 AudioSource
        m_audioSource = gameObject.GetComponent<AudioSource>();
        if (m_audioSource == null)
        {
            m_audioSource = gameObject.AddComponent<AudioSource>();
        }

        m_audioSource.loop = true;
        m_audioSource.playOnAwake = false;
        m_audioSource.volume = 0.7f;

        // 선택 BGM용 AudioSource (별도 오브젝트로 생성)
        GameObject selectBGMObject = new GameObject("SelectBGM");
        selectBGMObject.transform.SetParent(transform);
        m_selectAudioSource = selectBGMObject.AddComponent<AudioSource>();
        m_selectAudioSource.loop = false; // 선택 BGM은 1회 재생
        m_selectAudioSource.playOnAwake = false;
        m_selectAudioSource.volume = 0.6f;

        // 이펙트 사운드용 AudioSource (별도 오브젝트로 생성)
        GameObject effectSoundObject = new GameObject("EffectSound");
        effectSoundObject.transform.SetParent(transform);
        m_effectAudioSource = effectSoundObject.AddComponent<AudioSource>();
        m_effectAudioSource.loop = false; // 이펙트 사운드는 1회 재생
        m_effectAudioSource.playOnAwake = false;
        m_effectAudioSource.volume = 0.5f;

        // S급 기술 사운드 전용 AudioSource (별도 오브젝트로 생성)
        GameObject sskillSoundObject = new GameObject("SSkillSound");
        sskillSoundObject.transform.SetParent(transform);
        m_sskillAudioSource = sskillSoundObject.AddComponent<AudioSource>();
        m_sskillAudioSource.loop = false; // S급 기술 사운드는 1회 재생
        m_sskillAudioSource.playOnAwake = false;
        m_sskillAudioSource.volume = 0.7f;
    }

    /// <summary>
    /// 타이틀 씬 BGM 재생
    /// 파일명 형식: BGM/BG/Title
    /// </summary>
    public void PlayTitleBGM()
    {
        string bgmPath = "BGM/BG/Robby";
        PlayBGM(bgmPath);
        Debug.Log($"[BGM] 타이틀 BGM 재생: {bgmPath}");
    }

    public void PlayerJohnCenaVictory()
    {
        string bgmPath = "BGM/JohnCena_Victory";
        PlayBGM(bgmPath);
    }

    public void PlayerJohnCenaEnd()
    {
        string bgmPath = "BGM/JohnCena_End";
        PlayBGM(bgmPath);
    }

    public void PlayerUnderTakerVictory()
    {
        string bgmPath = "BGM/UnderTaker_Victory";
        PlayBGM(bgmPath);
    }

    public void PlayerUnderTakerEnd()
    {
        string bgmPath = "BGM/UnderTaker_Ending";
        PlayBGM(bgmPath);
    }



    /// <summary>
    /// 레슬러 선택 시 BGM 재생 (1회 재생, 배경 BGM과 독립적으로 재생)
    /// 파일명 형식: {레슬러폴더}/1-0
    /// 예: JohnCena/1-0, Undertaker/1-0
    /// </summary>
    public void PlayWrestlerSelectBGM(YaCht_WrestlerType wrestlerType)
    {
        string wrestlerFolder = GetWrestlerFolder(wrestlerType);
        string bgmPath = $"BGM/BG/{wrestlerFolder}/1-0";
        PlaySelectBGM(bgmPath);
    }

    /// <summary>
    /// 스테이지 BGM 재생
    /// 파일명 형식: {레슬러폴더}/{챕터번호}-{스테이지번호}
    /// 예: 1-1 (1스테이지 1Enemy), 1-2 (1스테이지 2Enemy), 2-1 (2스테이지 1Enemy) 등
    /// </summary>
    public void PlayStageBGM(YaCht_WrestlerType wrestlerType, int stageNumber, int enemyIndex)
    {
        string wrestlerFolder = GetWrestlerFolder(wrestlerType);

        // 챕터 번호 계산 (4스테이지마다 1챕터)
        int chapterNumber = ((stageNumber - 1) / 4) + 1;

        // 챕터 내 스테이지 번호 (1~4)
        int stageInChapter = ((stageNumber - 1) % 4) + 1;

        string bgmPath = $"BGM/BG/{wrestlerFolder}/{chapterNumber}-{stageInChapter}";

        Debug.Log($"[BGM DEBUG] 스테이지 번호: {stageNumber}, 챕터: {chapterNumber}, 챕터내스테이지: {stageInChapter}, 적 인덱스: {enemyIndex}");
        Debug.Log($"[BGM DEBUG] 재생 경로: {bgmPath}");

        PlayBGM(bgmPath);
    }

    /// <summary>
    /// 보스 페이즈 BGM 재생 (3-41, 3-42 형식)
    /// 파일명 형식: {레슬러폴더}/{챕터번호}-4{페이즈번호}
    /// 예: 3-41 (3챕터 보스 1페이즈), 3-42 (3챕터 보스 2페이즈)
    /// </summary>
    public void PlayBossPhaseBGM(YaCht_WrestlerType wrestlerType, int stageNumber, int phase)
    {
        string wrestlerFolder = GetWrestlerFolder(wrestlerType);

        // 챕터 번호 계산
        int chapterNumber = ((stageNumber - 1) / 4) + 1;

        string bgmPath = $"BGM/BG/{wrestlerFolder}/{chapterNumber}-4{phase}";

        Debug.Log($"[BGM DEBUG] 보스 페이즈 BGM - 스테이지 번호: {stageNumber}, 챕터: {chapterNumber}, 페이즈: {phase}");
        Debug.Log($"[BGM DEBUG] 재생 경로: {bgmPath}");

        PlayBGM(bgmPath);
    }

    /// <summary>
    /// BGM 재생 (반복)
    /// </summary>
    private void PlayBGM(string path)
    {
        // 같은 BGM이면 재생하지 않음
        if (m_currentBGMPath == path && m_audioSource != null && m_audioSource.isPlaying)
        {
            Debug.Log($"[BGM] 이미 재생 중: {path}");
            return;
        }

        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip != null)
        {
            m_currentBGMPath = path;
            m_audioSource.loop = true;
            m_audioSource.clip = clip;
            m_audioSource.Play();
            Debug.Log($"[BGM] 재생 (반복): {path}");
        }
        else
        {
            Debug.LogWarning($"[BGM] 찾을 수 없음: {path}");
        }
    }

    /// <summary>
    /// 선택 BGM 재생 (1회, 배경 BGM과 독립적으로 재생)
    /// </summary>
    private void PlaySelectBGM(string path)
    {
        if (m_selectAudioSource == null)
        {
            Debug.LogError("[BGM] 선택 BGM AudioSource가 초기화되지 않았습니다!");
            return;
        }

        // 같은 선택 BGM이면 재생하지 않음
        if (m_currentSelectBGMPath == path && m_selectAudioSource.isPlaying)
        {
            Debug.Log($"[BGM] 선택 BGM 이미 재생 중: {path}");
            return;
        }

        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip != null)
        {
            m_currentSelectBGMPath = path;
            m_selectAudioSource.loop = false; // 반복 비활성화
            m_selectAudioSource.clip = clip;
            m_selectAudioSource.Play();
            Debug.Log($"[BGM] 선택 BGM 재생 (1회): {path}");
        }
        else
        {
            Debug.LogWarning($"[BGM] 선택 BGM 찾을 수 없음: {path}");
        }
    }

    /// <summary>
    /// 선택 BGM 정지
    /// </summary>
    public void StopSelectBGM()
    {
        if (m_selectAudioSource != null && m_selectAudioSource.isPlaying)
        {
            m_selectAudioSource.Stop();
            m_selectAudioSource.clip = null;
            m_currentSelectBGMPath = "";
            Debug.Log("[BGM] 선택 BGM 정지");
        }
    }

    /// <summary>
    /// 선택 BGM 재생 중인지 확인
    /// </summary>
    public bool IsSelectBGMPlaying()
    {
        return m_selectAudioSource != null && m_selectAudioSource.isPlaying;
    }

    /// <summary>
    /// 레슬러 타입에 따라 폴더명 반환
    /// </summary>
    private string GetWrestlerFolder(YaCht_WrestlerType wrestlerType)
    {
        switch (wrestlerType)
        {
            case YaCht_WrestlerType.JohnCena:
                return "JohnCena";
            case YaCht_WrestlerType.Undertaker:
                return "Undertaker";
            default:
                return "Default";
        }
    }

    /// <summary>
    /// 배경 BGM 정지 (선택 BGM은 계속 재생)
    /// </summary>
    public void StopBGM()
    {
        if (m_audioSource != null && m_audioSource.isPlaying)
        {
            m_audioSource.Stop();
            m_audioSource.clip = null;
            m_currentBGMPath = "";
        }
    }

    /// <summary>
    /// 모든 BGM 정지 (배경 BGM + 선택 BGM)
    /// </summary>
    public void StopAllBGM()
    {
        StopBGM();
        StopSelectBGM();
    }

    /// <summary>
    /// BGM 일시정지
    /// </summary>
    public void PauseBGM()
    {
        if (m_audioSource != null && m_audioSource.isPlaying)
        {
            m_audioSource.Pause();
        }
    }

    /// <summary>
    /// BGM 재개
    /// </summary>
    public void ResumeBGM()
    {
        if (m_audioSource != null && m_audioSource.clip != null)
        {
            // 일시정지 상태이거나 재생 중이 아닌 경우 재개
            if (!m_audioSource.isPlaying)
            {
                m_audioSource.UnPause();
            }
        }
    }

    /// <summary>
    /// S급 기술 사운드 중단 (적 처치 또는 스테이지 전환 시 호출)
    /// </summary>
    public void StopSSkillSound()
    {
        if (m_sskillAudioSource != null && m_sskillAudioSource.isPlaying)
        {
            m_sskillAudioSource.Stop();
            m_sskillAudioSource.clip = null;
            Debug.Log("[BGM] S급 기술 사운드 중단");
        }

        // BGM 복원 코루틴 중지
        if (m_bgmResumeCoroutine != null)
        {
            StopCoroutine(m_bgmResumeCoroutine);
            m_bgmResumeCoroutine = null;
        }

        // BGM 즉시 복원 (일시정지 상태라면)
        if (m_audioSource != null && m_audioSource.clip != null)
        {
            ResumeBGM();
            if (!m_audioSource.isPlaying)
            {
                m_audioSource.Play();
            }
            // 원래 볼륨으로 복원
            m_audioSource.volume = m_originalBGMVolume;
            Debug.Log($"[BGM] BGM 즉시 복원 (볼륨: {m_originalBGMVolume})");
        }
    }

    /// <summary>
    /// 배경 BGM 볼륨 설정
    /// </summary>
    public void SetVolume(float volume)
    {
        if (m_audioSource != null)
        {
            m_audioSource.volume = Mathf.Clamp01(volume);
        }
    }

    /// <summary>
    /// 선택 BGM 볼륨 설정
    /// </summary>
    public void SetSelectBGMVolume(float volume)
    {
        if (m_selectAudioSource != null)
        {
            m_selectAudioSource.volume = Mathf.Clamp01(volume);
        }
    }

    /// <summary>
    /// 모든 BGM 볼륨 설정 (배경 BGM + 선택 BGM)
    /// </summary>
    public void SetAllBGMVolume(float volume)
    {
        SetVolume(volume);
        SetSelectBGMVolume(volume);
    }

    /// <summary>
    /// 재생 중인지 확인
    /// </summary>
    public bool IsPlaying()
    {
        return m_audioSource != null && m_audioSource.isPlaying;
    }

    /// <summary>
    /// 카드 공격 사운드 재생 (레어도별, S급은 카드 이름으로 특수 기술 사운드 확인)
    /// 경로: BGM/CardSound/{레어도} 또는 BGM/CardSound/{카드이름}
    /// S급 기술 사용 시 BGM을 일시정지하고, 사운드가 90% 진행되면 점진적으로 BGM 복원
    /// </summary>
    public void PlayCardSound(YaCht_CardRarity rarity, string cardName = "")
    {
        if (m_effectAudioSource == null)
        {
            Debug.LogError("[BGM] 이펙트 사운드 AudioSource가 초기화되지 않았습니다!");
            return;
        }

        string soundPath = "";      

        // S급 카드는 카드 이름으로 특수 기술 사운드 확인
        if (rarity == YaCht_CardRarity.S && !string.IsNullOrEmpty(cardName))
        {
            soundPath = $"BGM/CardSound/{cardName}";
            AudioClip clip = Resources.Load<AudioClip>(soundPath);
            
            // 특수 기술 사운드가 있으면 재생
            if (clip != null)
            {                               
                // 현재 재생 중인 S급 기술 사운드가 있으면 정지
                if (m_sskillAudioSource != null && m_sskillAudioSource.isPlaying)
                {
                    m_sskillAudioSource.Stop();
                    Debug.Log("[BGM] 기존 S급 기술 사운드 정지 (새로운 S급 기술 재생)");
                    
                    // 기존 BGM 복원 코루틴도 중지
                    if (m_bgmResumeCoroutine != null)
                    {
                        StopCoroutine(m_bgmResumeCoroutine);
                        m_bgmResumeCoroutine = null;
                    }
                    
                    // BGM 즉시 복원 (기존 S급 기술이 중단되었으므로)
                    if (m_audioSource != null && m_audioSource.clip != null)
                    {
                        ResumeBGM();
                        m_audioSource.volume = m_originalBGMVolume;
                    }
                }
                
                // 원래 BGM 볼륨 저장
                if (m_audioSource != null && m_audioSource.isPlaying)
                {
                    m_originalBGMVolume = m_audioSource.volume;
                    // BGM 일시정지
                    PauseBGM();
                    Debug.Log($"[BGM] S급 기술 사용 - BGM 일시정지 (원래 볼륨: {m_originalBGMVolume})");
                }

                // S급 기술 사운드 재생 (전용 AudioSource 사용)
                if (m_sskillAudioSource == null)
                {
                    Debug.LogError("[BGM] S급 기술 사운드 AudioSource가 초기화되지 않았습니다!");
                    return;
                }
                
                m_sskillAudioSource.clip = clip;
                m_sskillAudioSource.Play();
                Debug.Log($"[BGM] 특수 기술 사운드 재생: {soundPath}");

                // 사운드가 90% 진행되면 BGM 점진적 복원 시작
                m_bgmResumeCoroutine = StartCoroutine(ResumeBGMAfterSSkill(clip.length));
                return;
            }
            // 특수 기술 사운드가 없으면 일반 A.mp3 재생
            else
            {
                soundPath = "BGM/CardSound/A";
                Debug.Log($"[BGM] 특수 기술 사운드를 찾을 수 없어 일반 사운드로 대체: {cardName} -> A");
            }
        }
        else
        {
            // D, C, B, A는 기존처럼 레어도별 사운드 재생
            string rarityString = GetRarityString(rarity);
            soundPath = $"BGM/CardSound/{rarityString}";
        }

        AudioClip finalClip = Resources.Load<AudioClip>(soundPath);
        if (finalClip != null)
        {
            m_effectAudioSource.PlayOneShot(finalClip);
            Debug.Log($"[BGM] 카드 사운드 재생: {soundPath}");
        }
        else
        {
            Debug.LogWarning($"[BGM] 카드 사운드를 찾을 수 없음: {soundPath}");
        }
    }

    /// <summary>
    /// S급 기술 사운드 재생 후 BGM 점진적 복원 코루틴
    /// 사운드가 90% 진행되면 작은 소리로 시작하여 점진적으로 원래 볼륨으로 복원
    /// </summary>
    private System.Collections.IEnumerator ResumeBGMAfterSSkill(float soundDuration)
    {
        // 사운드가 90% 진행될 때까지 대기
        float waitTime = soundDuration * 0.9f;
        yield return new WaitForSeconds(waitTime);

        // 기존 복원 코루틴이 있으면 중지 (이미 이 코루틴이므로 중지할 필요 없음)
        // 하지만 안전을 위해 확인

        // BGM 복원 시작
        if (m_audioSource != null && m_audioSource.clip != null)
        {
            // BGM 재개 (일시정지 상태에서 재개)
            ResumeBGM();

            // BGM이 재생 중인지 확인 (재개 실패 시 재생)
            if (!m_audioSource.isPlaying)
            {
                m_audioSource.Play();
            }

            // 작은 소리로 시작 (원래 볼륨의 10%)
            float startVolume = m_originalBGMVolume * 0.1f;
            m_audioSource.volume = startVolume;

            Debug.Log($"[BGM] BGM 복원 시작 - 시작 볼륨: {startVolume}, 목표 볼륨: {m_originalBGMVolume}");

            // 점진적으로 원래 볼륨으로 복원 (나머지 10% 시간 동안)
            float fadeDuration = soundDuration * 0.1f;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                // 부드러운 페이드인 (ease-in)
                float smoothT = t * t;
                m_audioSource.volume = Mathf.Lerp(startVolume, m_originalBGMVolume, smoothT);
                yield return null;
            }

            // 최종 볼륨 설정 (확실하게 복원)
            m_audioSource.volume = m_originalBGMVolume;
            Debug.Log($"[BGM] S급 기술 사운드 종료 - BGM 복원 완료 (볼륨: {m_originalBGMVolume})");
        }
        else
        {
            Debug.LogWarning("[BGM] BGM AudioSource가 없거나 클립이 없어 복원할 수 없습니다!");
        }

        m_bgmResumeCoroutine = null;
    }

    /// <summary>
    /// 콤보 사운드 재생 (콤보 레벨별 랜덤 선택)
    /// 해당 레벨 폴더의 모든 사운드 + 루트 폴더 사운드들을 포함하여 랜덤 선택
    /// </summary>
    public void PlayComboSound(int comboLevel)
    {
        if (m_effectAudioSource == null)
        {
            Debug.LogError("[BGM] 이펙트 사운드 AudioSource가 초기화되지 않았습니다!");
            return;
        }

        if (comboLevel < 3 || comboLevel > 6)
        {
            Debug.LogWarning($"[BGM] 유효하지 않은 콤보 레벨: {comboLevel}");
            return;
        }

        // 콤보 레벨에 맞는 폴더 경로
        string comboFolderPath = $"BGM/ComboSound/{comboLevel}Combo";
        
        // 각 콤보 레벨별 알려진 사운드 파일명들 (확장자 제외)
        System.Collections.Generic.Dictionary<int, string[]> comboSoundFiles = new System.Collections.Generic.Dictionary<int, string[]>
        {
            { 3, new string[] { "3com", "CCC", "DDD" } },
            { 4, new string[] { "4com", "AAAA", "BBBC", "BBCC" } },
            { 5, new string[] { "5comb", "AAABB", "SSSAB" } },
            { 6, new string[] { "6Combo", "SSAABB", "SSSAAB" } }
        };

        // 루트 폴더의 사운드 파일들 (확장자 제외)
        string[] rootSoundNames = { "HisNot", "OHMYGOD", "Insane", "WACombo" };

        // 사용 가능한 사운드 경로 리스트
        System.Collections.Generic.List<string> availableSounds = new System.Collections.Generic.List<string>();

        // 콤보 폴더의 사운드들 추가
        if (comboSoundFiles.ContainsKey(comboLevel))
        {
            foreach (var soundName in comboSoundFiles[comboLevel])
            {
                string comboPath = $"{comboFolderPath}/{soundName}";
                AudioClip comboClip = Resources.Load<AudioClip>(comboPath);
                if (comboClip != null)
                {
                    availableSounds.Add(comboPath);
                }
            }
        }

        // 루트 폴더의 사운드들 추가
        foreach (var soundName in rootSoundNames)
        {
            string rootPath = $"BGM/ComboSound/{soundName}";
            AudioClip rootClip = Resources.Load<AudioClip>(rootPath);
            if (rootClip != null)
            {
                availableSounds.Add(rootPath);
            }
        }

        // 사용 가능한 사운드가 없으면 경고
        if (availableSounds.Count == 0)
        {
            Debug.LogWarning($"[BGM] 콤보 레벨 {comboLevel}에 대한 사운드를 찾을 수 없습니다!");
            return;
        }

        // 랜덤으로 하나 선택
        int randomIndex = Random.Range(0, availableSounds.Count);
        string selectedSoundPath = availableSounds[randomIndex];

        AudioClip clip = Resources.Load<AudioClip>(selectedSoundPath);
        if (clip != null)
        {
            m_effectAudioSource.PlayOneShot(clip);
            Debug.Log($"[BGM] 콤보 사운드 재생 (레벨 {comboLevel}): {selectedSoundPath}");
        }
        else
        {
            Debug.LogWarning($"[BGM] 선택된 콤보 사운드를 로드할 수 없음: {selectedSoundPath}");
        }
    }

    /// <summary>
    /// 레어도를 문자열로 변환
    /// </summary>
    private string GetRarityString(YaCht_CardRarity rarity)
    {
        switch (rarity)
        {
            case YaCht_CardRarity.S:
                return "S";
            case YaCht_CardRarity.A:
                return "A";
            case YaCht_CardRarity.B:
                return "B";
            case YaCht_CardRarity.C:
                return "C";
            case YaCht_CardRarity.D:
                return "D";
            default:
                return "D";
        }
    }
}
