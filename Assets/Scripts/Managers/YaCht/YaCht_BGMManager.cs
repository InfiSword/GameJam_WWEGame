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
    private string m_currentBGMPath = "";
    private string m_currentSelectBGMPath = "";

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
        m_selectAudioSource.volume = 0.7f;
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
        if (m_audioSource != null && !m_audioSource.isPlaying)
        {
            m_audioSource.UnPause();
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
}
