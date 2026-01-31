using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class YaCht_WWEMainGame : MonoBehaviour
{
    [Header("Setup Area")]
    [SerializeField] private Transform m_setupArea;
    [SerializeField] private int m_maxSetupCards = 6;
    [SerializeField] private float m_setupCardSpacing = 1.5f;

    [Header("Enemy System")]
    [SerializeField] private Transform m_enemySpawnPosition;
    [SerializeField] private GameObject m_enemyPrefab;
    private YaCht_Enemy m_currentEnemy;
    public YaCht_Enemy CurrentEnemy => m_currentEnemy;

    [Header("UI Elements")]
    [SerializeField] private GameObject m_backgroundPanel;
    [SerializeField] private Button m_backgroundButton;
    [SerializeField] private Button m_fightButton;
    [SerializeField] private Button m_rerollButton;

    [SerializeField] private TextMeshProUGUI m_rerollCountText;
    [SerializeField] private TextMeshProUGUI m_stageInfoText;
    [SerializeField] private TextMeshProUGUI m_scoreText;

    [Header("Reroll Settings")]
    [SerializeField] private int m_maxRerollCount = 3;

    [Header("Victory/Defeat Settings")]
    [SerializeField] private float m_victoryPanelDelay = 1.5f; 

    [Header("Victory Panel")]
    [SerializeField] private GameObject m_victoryPanel;
    [SerializeField] private TextMeshProUGUI m_victoryText;
    [SerializeField] private TextMeshProUGUI m_victoryCurrentScoreText;
    [SerializeField] private TextMeshProUGUI m_victoryHighScoreText;
    [SerializeField] private Button m_nextStageButton;

    [Header("Defeat Panel")]
    [SerializeField] private GameObject m_defeatPanel;
    [SerializeField] private TextMeshProUGUI m_defeatText;
    [SerializeField] private TextMeshProUGUI m_defeatCurrentScoreText;
    [SerializeField] private TextMeshProUGUI m_defeatHighScoreText;
    [SerializeField] private Button m_titleButton;

    [Header("Ending Panel")]
    [SerializeField] private GameObject m_endingPanel;
    [SerializeField] private TextMeshProUGUI m_endingText;
    [SerializeField] private TextMeshProUGUI m_endingCurrentScoreText;
    [SerializeField] private TextMeshProUGUI m_endingHighScoreText;
    [SerializeField] private Button m_endingTitleButton;

    [SerializeField] private ScrollRect m_relicsInfoPanel;
    [SerializeField] private GameObject m_relicIconButtonPrefab;
    [SerializeField] private GameObject m_relicItemPrefab;
    [SerializeField] private Transform m_relicDetailContainer;

    [SerializeField] private YaCht_ComboGuideUI m_comboGuideUI;
    private YaCht_CardManager m_cardManager;

    private List<YaCht_WWECard> m_setupCards = new List<YaCht_WWECard>();
    private List<Transform> m_setupSlots = new List<Transform>();
    private YaCht_WWECard m_currentPreviewOriginalCard;
    private int m_currentRerollCount;
    private bool m_isBattleEnded = false;
    private bool m_isAttacking = false; // 공격 중인지 여부
    private int m_currentBossPhase = 1;
    private int m_maxBossPhase = 1;

    // 유물 아이콘 버튼 관리
    private List<YaCht_RelicIconButton> m_relicIconButtons = new List<YaCht_RelicIconButton>();
    // 유물 아이템 관리 (RelicType -> RelicItem)
    private Dictionary<YaCht_RelicType, YaCht_RelicItem> m_relicItems = new Dictionary<YaCht_RelicType, YaCht_RelicItem>();
    private YaCht_RelicItem m_currentActiveRelicItem = null;

    // 고점수 저장 키
    private const string HIGHSCORE_KEY = "YaCht_HighScore";

    public void Init()
    {
        m_setupCards.Clear();
        CreateSetupSlots();
        m_currentRerollCount = m_maxRerollCount;

        m_backgroundButton.onClick.AddListener(OnBackgroundClicked);
        m_fightButton.onClick.AddListener(OnFightButtonClicked);
        m_rerollButton.onClick.AddListener(OnRerollButtonClicked);
        m_backgroundPanel.SetActive(false);

        if (m_victoryPanel != null)
        {
            m_victoryPanel.SetActive(false);
        }

        if (m_defeatPanel != null)
        {
            m_defeatPanel.SetActive(false);
        }

        if (m_endingPanel != null)
        {
            m_endingPanel.SetActive(false);
        }

        if (m_nextStageButton != null)
        {
            m_nextStageButton.onClick.AddListener(OnNextStageButtonClicked);
        }

        if (m_titleButton != null)
        {
            m_titleButton.onClick.AddListener(OnTitleButtonClicked);
        }

        if (m_endingTitleButton != null)
        {
            m_endingTitleButton.onClick.AddListener(OnTitleButtonClicked);
        }

        m_cardManager = YaCht_GameManager.CardManager;
        m_comboGuideUI.Initialize(YaCht_GameManager.nowPlayerData.GetWrestlerType());

        if (YaCht_GameManager.StageManager != null)
        {
            YaCht_GameManager.StageManager.OnBossDefeated += OnBossDefeated;
            YaCht_GameManager.StageManager.OnEnemyDefeatedNormal += OnNormalEnemyDefeated;
            YaCht_GameManager.StageManager.OnPhaseChanged += OnStagePhaseChanged;
        }

        PlayStageBGM();
        SpawnEnemy();

        m_isBattleEnded = false;
        m_isAttacking = false;
        UpdateRelicIcons();
        UpdateUI();
    }

    private void PlayStageBGM()
    {
        if (YaCht_GameManager.StageManager == null) return;

        YaCht_WrestlerType currentWrestler = YaCht_GameManager.nowPlayerData.wrestlerType;
        int currentStage = YaCht_GameManager.StageManager.CurrentStageNumber;

        bool isBossStage = YaCht_GameManager.IsCurrentStageBoss();

        Debug.Log($"[PlayStageBGM] 스테이지: {currentStage}, 보스 스테이지: {isBossStage}");

        // 보스 스테이지이고 여러 페이즈인 경우 1번 BGM 재생
        if (isBossStage && YaCht_GameManager.StageManager.CurrentStageData.HasMultiplePhases)
        {
            YaCht_BGMManager.Instance.PlayBossPhaseBGM(currentWrestler, currentStage, 1);
            Debug.Log($"[PlayStageBGM] 보스 스테이지 - 보스 1번 BGM 재생");
        }
        else if (isBossStage)
        {
            // 보스 스테이지: enemyIndex = 4 재생
            YaCht_BGMManager.Instance.PlayStageBGM(currentWrestler, currentStage, 4);
            Debug.Log($"[PlayStageBGM] 보스 스테이지 BGM 재생");
        }
        else
        {
            YaCht_BGMManager.Instance.PlayStageBGM(currentWrestler, currentStage, 1);
            Debug.Log($"[PlayStageBGM] 일반 스테이지 BGM 재생");
        }
    }

    private void OnDestroy()
    {
        if (YaCht_GameManager.StageManager != null)
        {
            YaCht_GameManager.StageManager.OnBossDefeated -= OnBossDefeated;
            YaCht_GameManager.StageManager.OnEnemyDefeatedNormal -= OnNormalEnemyDefeated;
            YaCht_GameManager.StageManager.OnPhaseChanged -= OnStagePhaseChanged;
            YaCht_GameManager.StageManager.UnregisterEnemy();
        }

        if (m_nextStageButton != null)
        {
            m_nextStageButton.onClick.RemoveListener(OnNextStageButtonClicked);
        }

        if (m_titleButton != null)
        {
            m_titleButton.onClick.RemoveListener(OnTitleButtonClicked);
        }

        if (m_endingTitleButton != null)
        {
            m_endingTitleButton.onClick.RemoveListener(OnTitleButtonClicked);
        }

        // 유물 아이콘 버튼들 정리
        ClearRelicIcons();
    }

    private void SpawnEnemy()
    {
        if (m_currentEnemy != null)
        {
            if (YaCht_GameManager.StageManager != null)
            {
                YaCht_GameManager.StageManager.UnregisterEnemy();
            }
            Destroy(m_currentEnemy.gameObject);
            m_currentEnemy = null;
        }

        if (m_enemyPrefab != null && m_enemySpawnPosition != null && YaCht_GameManager.StageManager != null)
        {
            GameObject enemyObj = Instantiate(m_enemyPrefab, m_enemySpawnPosition);
            m_currentEnemy = enemyObj.GetComponent<YaCht_Enemy>();

            if (m_currentEnemy != null)
            {
                YaCht_EnemyData currentEnemyData = YaCht_GameManager.StageManager.CurrentEnemy;
                m_currentEnemy.Initialize(currentEnemyData, m_enemySpawnPosition);
                YaCht_GameManager.StageManager.RegisterEnemy(m_currentEnemy);

                m_currentBossPhase = 1;

                if (m_currentEnemy.IsBoss && YaCht_GameManager.StageManager.CurrentStageData.HasMultiplePhases)
                {
                    m_maxBossPhase = YaCht_GameManager.StageManager.CurrentStageData.m_phases.Length;
                    Debug.Log($"[SpawnEnemy] 최대 보스 페이즈: {m_maxBossPhase}");
                }
                else
                {
                    m_maxBossPhase = 1;
                }
            }
        }
    }

    private void OnBossDefeated()
    {
        Debug.Log($"[OnBossDefeated] 보스 {m_currentBossPhase} 페이즈 클리어!");

        // SoulBell 효과: 적 처치 시 HP 저장
        if (m_currentEnemy != null)
        {
            YaCht_GameManager.RelicManager.OnEnemyDefeated(m_currentEnemy.MaxHealth);
        }

        if (m_currentBossPhase < m_maxBossPhase)
        {
            StartCoroutine(TransitionToNextBossPhase());
        }
        else
        {
            m_isBattleEnded = true;
            YaCht_BGMManager.Instance.StopBGM();
            
            // 최종 보스인지 확인
            bool isFinalBoss = YaCht_GameManager.StageManager != null && 
                              YaCht_GameManager.StageManager.CurrentStageNumber >= YaCht_GameManager.GetTotalStageCount();

            if (isFinalBoss)
            {
                // 최종 보스 클리어: End BGM 재생
                if (YaCht_GameManager.nowPlayerData.wrestlerType == YaCht_WrestlerType.JohnCena)
                    YaCht_BGMManager.Instance.PlayerJohnCenaEnd();
                else
                    YaCht_BGMManager.Instance.PlayerUnderTakerEnd();

                UpdateAndSaveHighScore();
                StartCoroutine(ShowEndingPanelDelayed("게임 클리어! 축하합니다!"));
            }
            else
            {
                // 일반 보스 클리어: Victory BGM 재생
                if (YaCht_GameManager.nowPlayerData.wrestlerType == YaCht_WrestlerType.JohnCena)
                    YaCht_BGMManager.Instance.PlayerJohnCenaVictory();
                else
                    YaCht_BGMManager.Instance.PlayerUnderTakerVictory();

                UpdateAndSaveHighScore();
                StartCoroutine(ShowVictoryPanelDelayed("보스 클리어! 다음으로 이동합니다!"));
            }
        }
    }

    /// <summary>
    /// 보스 페이즈 전환
    /// </summary>
    private System.Collections.IEnumerator TransitionToNextBossPhase()
    {
        m_currentBossPhase++;
        Debug.Log($"[TransitionToNextBossPhase] 보스 {m_currentBossPhase} 페이즈 전환!");

        // 라운드 초기화
        YaCht_GameManager.currentRound = 1;

        // 리롤 초기화
        m_currentRerollCount = m_maxRerollCount;
        Debug.Log($"[TransitionToNextBossPhase] 보스 페이즈 전환 - 리롤 초기화! (리롤: {m_currentRerollCount}/{m_maxRerollCount})");

        // 다음 보스 페이즈 데이터 가져오기
        YaCht_PhaseData? nextPhase = YaCht_GameManager.StageManager.CurrentStageData.GetPhaseByNumber(m_currentBossPhase);

        if (!nextPhase.HasValue)
        {
            Debug.LogError($"[TransitionToNextBossPhase] 보스 {m_currentBossPhase} 페이즈 데이터 없음!");
            yield break;
        }

        // 보스 체력 초기화
        YaCht_EnemyData enemyData = YaCht_GameManager.StageManager.CurrentEnemy;
        float nextPhaseMaxHealth = enemyData.m_maxHealth;
        
        // 2페이지 전환 시 추가 체력 적용
        int currentStage = YaCht_GameManager.StageManager.CurrentStageNumber;
        if (m_currentBossPhase == 2)
        {
            if (currentStage == 8) // 2 챕터 보스
            {
                // 2페이지시 체력 3000 추가 -> 총 8000
                nextPhaseMaxHealth = enemyData.m_maxHealth + 3000;
                Debug.Log($"[TransitionToNextBossPhase] 2 챕터 보스 2페이지: 체력 {enemyData.m_maxHealth} + 3000 = {nextPhaseMaxHealth}");
            }
            else if (currentStage == 12) // 3 챕터 마지막 보스
            {
                // 2페이지시 체력 2배 상승 -> 8만
                nextPhaseMaxHealth = enemyData.m_maxHealth * 2;
                Debug.Log($"[TransitionToNextBossPhase] 3 챕터 마지막 보스 2페이지: 체력 {enemyData.m_maxHealth} * 2 = {nextPhaseMaxHealth}");
            }
        }

        // 보스 리스폰
        if (m_currentEnemy != null)
        {
            yield return StartCoroutine(m_currentEnemy.RespawnBossCoroutine(nextPhaseMaxHealth));
        }

        // 배경 변경
        YaCht_GameScene gameScene = FindFirstObjectByType<YaCht_GameScene>();
        if (gameScene != null && !string.IsNullOrEmpty(nextPhase.Value.m_backgroundResourcePath))
        {
            gameScene.LoadBackgroundFromPath(nextPhase.Value.m_backgroundResourcePath);
            Debug.Log($"[TransitionToNextBossPhase] 배경 변경: {nextPhase.Value.m_backgroundResourcePath}");
        }

        // BGM 재생
        if (!string.IsNullOrEmpty(nextPhase.Value.m_bgmResourcePath))
        {
            YaCht_WrestlerType currentWrestler = YaCht_GameManager.nowPlayerData.wrestlerType;
            YaCht_BGMManager.Instance.PlayBossPhaseBGM(currentWrestler, currentStage, m_currentBossPhase);
            Debug.Log($"[TransitionToNextBossPhase] BGM 재생: 보스 {m_currentBossPhase}");
        }

        // 셋업 카드 초기화
        m_cardManager.ClearSetupCards();
        // 고정 카드가 아닌 카드만 제거
        m_setupCards.RemoveAll(card => card == null || !card.IsFixedCard);

        yield return new WaitForSeconds(0.5f);

        // 새 라운드 시작
        yield return StartCoroutine(m_cardManager.StartNewRound());

        // 고정 카드 설정
        YaCht_CardData? fixedCard = YaCht_GameManager.RelicManager.GetFixedMaskCard(
            YaCht_GameManager.nowPlayerData.playerDeck
        );

        if (fixedCard.HasValue && m_setupCards.Count < m_maxSetupCards && m_setupSlots.Count > 0)
        {
            yield return StartCoroutine(AutoSetupFixedCard(fixedCard.Value));
        }

        UpdateUI();
    }

    private void OnStagePhaseChanged(int phaseNumber, YaCht_PhaseData phaseData)
    {
        Debug.Log($"[OnStagePhaseChanged] HP 초기화: Phase {phaseNumber}");
        Debug.Log($"[OnStagePhaseChanged] {phaseData.m_phaseDescription}");

        m_currentBossPhase = phaseNumber;

        if (!string.IsNullOrEmpty(phaseData.m_bgmResourcePath))
        {
            YaCht_WrestlerType currentWrestler = YaCht_GameManager.nowPlayerData.wrestlerType;
            int currentStage = YaCht_GameManager.StageManager.CurrentStageNumber;
            YaCht_BGMManager.Instance.PlayBossPhaseBGM(currentWrestler, currentStage, phaseNumber);
        }
    }

    private void OnNormalEnemyDefeated()
    {
        // SoulBell 효과: 적 처치 시 HP 저장
        if (m_currentEnemy != null)
        {
            YaCht_GameManager.RelicManager.OnEnemyDefeated(m_currentEnemy.MaxHealth);
        }

        m_isBattleEnded = true;
        YaCht_BGMManager.Instance.StopBGM();
        
        if(YaCht_GameManager.nowPlayerData.wrestlerType == YaCht_WrestlerType.JohnCena) 
            YaCht_BGMManager.Instance.PlayerJohnCenaVictory();
        else
            YaCht_BGMManager.Instance.PlayerUnderTakerVictory();

        UpdateAndSaveHighScore();
        StartCoroutine(ShowVictoryPanelDelayed("클리어! 다음으로 이동합니다!"));
    }

    /// <summary>
    /// 고점수 저장
    /// </summary>
    private void UpdateAndSaveHighScore()
    {
        int currentScore = YaCht_GameManager.totalScore;
        int highScore = PlayerPrefs.GetInt(HIGHSCORE_KEY, 0);

        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt(HIGHSCORE_KEY, currentScore);
            PlayerPrefs.Save();
            Debug.Log($"[UpdateAndSaveHighScore] 고점수 갱신! {highScore} -> {currentScore}");
        }
        else
        {
            Debug.Log($"[UpdateAndSaveHighScore] 고점수: {currentScore}, 최고점수: {highScore}");
        }
    }

    /// <summary>
    /// 고점수 조회
    /// </summary>
    private int GetHighScore()
    {
        return PlayerPrefs.GetInt(HIGHSCORE_KEY, 0);
    }

    /// <summary>
    /// 패배 패널 표시
    /// </summary>
    private void ShowDefeatPanel()
    {
        m_isBattleEnded = true;

        YaCht_BGMManager.Instance.StopBGM();
        UpdateAndSaveHighScore();

        if (m_defeatPanel != null)
        {
            m_defeatPanel.SetActive(true);
        }

        if (m_defeatText != null)
        {
            m_defeatText.text = "패배!\n4턴 동안 데미지를 입히지 못했습니다.";
        }

        // 현재 점수 표시
        if (m_defeatCurrentScoreText != null)
        {
            m_defeatCurrentScoreText.text = $"현재 점수: {YaCht_GameManager.totalScore:N0}";
        }

        if (m_defeatHighScoreText != null)
        {
            m_defeatHighScoreText.text = $"최고 점수: {GetHighScore():N0}";
        }

        if (m_fightButton != null)
        {
            m_fightButton.interactable = false;
        }

        if (m_rerollButton != null)
        {
            m_rerollButton.interactable = false;
        }

        Debug.Log("[ShowDefeatPanel] 4턴 동안 데미지를 입히지 못했습니다!");
    }

    /// <summary>
    /// 승리 패널 표시
    /// </summary>
    private System.Collections.IEnumerator ShowVictoryPanelDelayed(string message, bool isEnd = false)
    {
        Debug.Log($"[ShowVictoryPanelDelayed] {m_victoryPanelDelay}초 후 승리 패널 표시");
        yield return new WaitForSeconds(m_victoryPanelDelay);

        if (m_victoryPanel != null)
        {
            m_victoryPanel.SetActive(true);
        }

        if (m_victoryText != null)
        {
            m_victoryText.text = message;
        }

        // 현재 점수 표시
        if (m_victoryCurrentScoreText != null)
        {
            m_victoryCurrentScoreText.text = $"현재 점수: {YaCht_GameManager.totalScore:N0}";
        }

        if (m_victoryHighScoreText != null)
        {
            m_victoryHighScoreText.text = $"최고 점수: {GetHighScore():N0}";
        }

        if (m_fightButton != null)
        {
            m_fightButton.interactable = false;
        }

        if (m_rerollButton != null)
        {
            m_rerollButton.interactable = false;
        }

        Debug.Log("[ShowVictoryPanelDelayed] 승리 패널 표시");
    }

    /// <summary>
    /// 엔딩 패널 표시
    /// </summary>
    private System.Collections.IEnumerator ShowEndingPanelDelayed(string message)
    {
        Debug.Log($"[ShowEndingPanelDelayed] {m_victoryPanelDelay}초 후 엔딩 패널 표시");
        yield return new WaitForSeconds(m_victoryPanelDelay);

        if (m_endingPanel != null)
        {
            m_endingPanel.SetActive(true);
        }

        if (m_endingText != null)
        {
            m_endingText.text = message;
        }

        // 현재 점수 표시
        if (m_endingCurrentScoreText != null)
        {
            m_endingCurrentScoreText.text = $"현재 점수: {YaCht_GameManager.totalScore:N0}";
        }

        if (m_endingHighScoreText != null)
        {
            m_endingHighScoreText.text = $"최고 점수: {GetHighScore():N0}";
        }

        if (m_fightButton != null)
        {
            m_fightButton.interactable = false;
        }

        if (m_rerollButton != null)
        {
            m_rerollButton.interactable = false;
        }

        Debug.Log("[ShowEndingPanelDelayed] 엔딩 패널 표시");
    }

    private void OnNextStageButtonClicked()
    {
        bool isBoss = YaCht_GameManager.IsCurrentStageBoss();

        if (isBoss)
        {
            YaCht_GameManager.SetRelicSceneFromBossDefeat();
            SceneManager.LoadScene("YaCht_RelicScene");
        }
        else
        {
            bool success = YaCht_GameManager.MoveToNextStage();
            if (success)
            {
                SceneManager.LoadScene("YaCht_GameScene");
            }
        }
    }

    /// <summary>
    /// 타이틀 버튼 클릭
    /// </summary>
    private void OnTitleButtonClicked()
    {
        Debug.Log("[OnTitleButtonClicked] 타이틀 버튼 클릭");

        // 게임 초기화
        YaCht_GameManager.Clear();

        SceneManager.LoadScene("YaCht_TitleScene");
    }

    private void CreateSetupSlots()
    {
        foreach (var slot in m_setupSlots)
        {
            if (slot != null)
            {
                Destroy(slot.gameObject);
            }
        }
        m_setupSlots.Clear();

        if (m_setupArea == null) return;

        float totalWidth = (m_maxSetupCards - 1) * m_setupCardSpacing;
        float startX = -totalWidth * 0.5f;

        for (int i = 0; i < m_maxSetupCards; i++)
        {
            GameObject slotObj = new GameObject($"SetupSlot_{i}");
            Transform slotTransform = slotObj.transform;
            slotTransform.SetParent(m_setupArea);

            float x = startX + i * m_setupCardSpacing;
            slotTransform.localPosition = new Vector3(x, 0f, 0f);
            slotTransform.localRotation = Quaternion.identity;
            slotTransform.localScale = Vector3.one;

            m_setupSlots.Add(slotTransform);
        }
    }

    private void UpdateUI()
    {
        string phaseText = "";
        if (m_currentEnemy != null && m_currentEnemy.IsBoss && m_maxBossPhase > 1)
        {
            phaseText = $" (보스 {m_currentBossPhase}/{m_maxBossPhase})";
        }
       
        if (m_stageInfoText != null && YaCht_GameManager.StageManager != null)
        {
            m_stageInfoText.text = YaCht_GameManager.StageManager.GetStageInfoString();
            m_stageInfoText.text += $"라운드: {YaCht_GameManager.currentRound} / 4{phaseText}\n";            
        }

        // 유물 아이콘은 UpdateRelicIcons()에서 별도로 관리

        if (m_rerollCountText != null)
        {
            m_rerollCountText.text = $"리롤: {m_currentRerollCount} / {m_maxRerollCount}";
        }

        // 점수 UI 표시
        if (m_scoreText != null)
        {
            m_scoreText.text = $"점수: {YaCht_GameManager.totalScore:N0}";
        }

        if (m_rerollButton != null)
        {
            m_rerollButton.interactable = m_currentRerollCount > 0 && !m_isBattleEnded && !m_cardManager.IsProcessing && !m_isAttacking;
        }

        if (m_fightButton != null)
        {
            m_fightButton.interactable = !m_isBattleEnded && !m_isAttacking;
        }
    }

    private void OnBackgroundClicked()
    {
        CloseCardPreview();
        HideRelicDetail();
    }

    public void OnHandCardClicked(YaCht_WWECard card)
    {
        if (card == null) return;

        if (m_isBattleEnded)
        {
            return;
        }

        if (m_cardManager.IsProcessing)
        {
            return;
        }

        YaCht_WWECard previewCard = m_cardManager.GetPreviewCard();

        if (m_currentPreviewOriginalCard != null && previewCard != null && previewCard.gameObject.activeSelf)
        {
            if (m_currentPreviewOriginalCard == card)
            {
                OnPreviewCardClicked();
            }
            else
            {
                ShowCardPreview(card);
            }
            return;
        }

        ShowCardPreview(card);
    }

    private void ShowCardPreview(YaCht_WWECard originalCard)
    {
        YaCht_WWECard previewCard = m_cardManager.GetPreviewCard();
        if (previewCard == null) return;

        previewCard.UpdateCardData(originalCard.GetCardData);
        previewCard.gameObject.SetActive(true);

        if (m_backgroundPanel != null)
        {
            m_backgroundPanel.SetActive(true);
        }

        m_currentPreviewOriginalCard = originalCard;
    }

    public void OnPreviewCardClicked()
    {
        if (m_currentPreviewOriginalCard == null) return;

        if (m_setupCards.Count >= m_maxSetupCards)
        {
            CloseCardPreview();
            return;
        }

        if (m_setupArea != null && m_setupSlots.Count > 0)
        {
            int cardIndex = m_setupCards.Count;
            Transform targetSlot = m_setupSlots[cardIndex];

            m_currentPreviewOriginalCard.SetupCard(targetSlot, cardIndex, Vector3.zero);
            m_setupCards.Add(m_currentPreviewOriginalCard);
            m_cardManager.SetupCard(m_currentPreviewOriginalCard, cardIndex);

            // 콤보 가이드 업데이트
            UpdateComboGuide();
        }

        CloseCardPreview();
    }

    private void CloseCardPreview()
    {
        YaCht_WWECard previewCard = m_cardManager.GetPreviewCard();
        if (previewCard != null)
        {
            previewCard.gameObject.SetActive(false);
        }

        if (m_backgroundPanel != null)
        {
            m_backgroundPanel.SetActive(false);
        }

        m_currentPreviewOriginalCard = null;
    }

    public void RemoveTopCardFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= m_setupCards.Count) return;

        YaCht_WWECard cardToRemove = m_setupCards[slotIndex];
        
        // 고정 카드는 제거할 수 없음
        if (cardToRemove.IsFixedCard)
        {
            Debug.Log($"[RemoveTopCardFromSlot] 고정 카드는 제거할 수 없습니다: {cardToRemove.GetCardData.m_name}");
            return;
        }

        m_setupCards.RemoveAt(slotIndex);

        m_cardManager.ReleaseCardFromSetup(cardToRemove);
        cardToRemove.ReleaseSetup();

        RepositionSetupCards();

        // 콤보 가이드 업데이트
        UpdateComboGuide();
    }

    private void RepositionSetupCards()
    {
        for (int i = 0; i < m_setupCards.Count; i++)
        {
            if (m_setupCards[i] != null && i < m_setupSlots.Count)
            {
                m_setupCards[i].SetupCard(m_setupSlots[i], i, Vector3.zero);
            }
        }
    }

    /// <summary>
    /// 콤보 가이드 업데이트
    /// </summary>
    private void UpdateComboGuide()
    {
        if (m_comboGuideUI == null) return;

        List<YaCht_CardData> setupCardData = new List<YaCht_CardData>();
        foreach (var card in m_setupCards)
        {
            if (card != null)
            {
                setupCardData.Add(card.GetCardData);
            }
        }

        m_comboGuideUI.OnSetupChanged(setupCardData);
    }

    /// <summary>
    /// 셋업 카드 데이터 가져오기
    /// </summary>
    public List<YaCht_CardData> GetSetupCards()
    {
        List<YaCht_CardData> setupCardData = new List<YaCht_CardData>();
        foreach (var card in m_setupCards)
        {
            if (card != null)
            {
                setupCardData.Add(card.GetCardData);
            }
        }
        return setupCardData;
    }

    private void OnRerollButtonClicked()
    {
        if (m_currentRerollCount <= 0)
        {
            Debug.Log("리롤 보너스 소모 완료!");
            return;
        }

        if (m_isBattleEnded)
        {
            return;
        }

        if (m_cardManager.IsProcessing)
        {
            Debug.LogWarning("리롤 처리 중 오류!");
            return;
        }

        if (m_isAttacking)
        {
            Debug.LogWarning("공격 중에는 리롤할 수 없습니다!");
            return;
        }

        m_currentRerollCount--;
        StartCoroutine(m_cardManager.RerollHand());
        UpdateUI();

        Debug.Log($"리롤 완료! 리롤 횟수: {m_currentRerollCount}");
    }

    private void OnFightButtonClicked()
    {
        if (m_setupCards.Count == 0)
        {
            Debug.Log("셋업 카드가 없습니다!");
            return;
        }

        if (m_isBattleEnded)
        {
            return;
        }

        if (m_isAttacking)
        {
            Debug.LogWarning("공격 중에는 전투를 시작할 수 없습니다!");
            return;
        }

        CloseCardPreview();

        // 콤보 가이드 초기화
        if (m_comboGuideUI != null)
        {
            m_comboGuideUI.ResetAllHighlights();
        }

        List<YaCht_CardData> setupCardData = new List<YaCht_CardData>();
        foreach (var card in m_setupCards)
        {
            setupCardData.Add(card.GetCardData);
        }

        YaCht_WrestlerType wrestlerType = YaCht_GameManager.nowPlayerData.wrestlerType;

        Debug.Log("=== 전투 시작 ===");
        Debug.Log($"셋업 카드 개수: {setupCardData.Count}");
        foreach (var card in setupCardData)
        {
            Debug.Log($"  - {card.m_name} ({card.m_rarity}, 데미지: {card.m_baseDamage})");
        }

        Debug.Log("\n[1턴] 카드 사용 이벤트 (OnCardsUsed)");
        YaCht_GameManager.RelicManager.OnCardsUsed(setupCardData);

        Debug.Log("\n[2턴] 콤보 데미지 계산");
        float baseDamage = YaCht_ComboChecker.CalculateComboDamage(setupCardData, wrestlerType, YaCht_ComboType.None);
        Debug.Log($"콤보 데미지 기본 데미지: {baseDamage:F1}");

        Debug.Log("\n[3턴] 최종 데미지 계산 (데미지 증가 효과 적용)");
        float finalDamage = YaCht_GameManager.RelicManager.CalculateFinalDamage(baseDamage, setupCardData);
        Debug.Log($"최종 데미지: {finalDamage:F1}");

        float enemyHealthPercent = 0f;
        if (m_currentEnemy != null && m_currentEnemy.MaxHealth > 0)
        {
            enemyHealthPercent = (m_currentEnemy.CurrentHealth / m_currentEnemy.MaxHealth) * 100f;
        }
        bool instantKill = YaCht_GameManager.RelicManager.CheckRestTombstoneInstantKill(setupCardData, enemyHealthPercent);

        StartCoroutine(ExecuteSequentialCardAttacks(instantKill, finalDamage, setupCardData, wrestlerType, baseDamage));
    }

    /// <summary>
    /// 순차적으로 카드 공격 실행
    /// </summary>
    private System.Collections.IEnumerator ExecuteSequentialCardAttacks(bool instantKill, float finalDamage,
        List<YaCht_CardData> setupCardData, YaCht_WrestlerType wrestlerType, float baseDamage)
    {
        m_isAttacking = true; // 공격 시작
        UpdateUI(); // UI 업데이트하여 리롤 버튼 비활성화

        // YouCantSeeMe 효과: 공격 횟수 계산
        // 4턴일 때 2턴(2턴, 3턴) 동안 입힌 총 데미지에 따라 카드당 공격 횟수 결정
        // 3회 = 200 이상, 4회 = 300 이상, 5회 = 400 이상
        int attackCountPerCard = YaCht_GameManager.RelicManager.GetYouCantSeeMeAttackCount(m_setupCards.Count);
        bool hasYouCantSeeMe = YaCht_GameManager.RelicManager.HasRelic(YaCht_RelicType.YouCantSeeMe);

        for (int i = 0; i < m_setupCards.Count; i++)
        {
            YaCht_WWECard card = m_setupCards[i];

            if (card != null && m_currentEnemy != null)
            {
                // YouCantSeeMe 효과: 각 카드당 3~5번 공격 (조건에 따라)
                // 유물이 없으면 기본 1회 공격
                int currentCardAttackCount = hasYouCantSeeMe ? attackCountPerCard : 1;
                
                for (int attackIndex = 0; attackIndex < currentCardAttackCount; attackIndex++)
                {
                    yield return StartCoroutine(card.AttackEnemyCoroutine(m_currentEnemy.transform));
                    yield return new WaitForSeconds(0.1f);
                    
                    // GamblerMask2 효과: 랭크별 확률로 한 번 더 공격
                    if (YaCht_GameManager.RelicManager.HasRelic(YaCht_RelicType.GamblerMask2))
                    {
                        float extraAttackChance = YaCht_GameManager.RelicManager.GetGamblerMask2AttackChance(card.GetCardData.m_rarity);
                        float roll = Random.Range(0f, 100f);
                        
                        if (roll < extraAttackChance)
                        {
                            Debug.Log($"[GamblerMask2] 추가 공격 발동! (랭크: {card.GetCardData.m_rarity}, 확률: {extraAttackChance}%, 결과: {roll:F1})");
                            yield return StartCoroutine(card.AttackEnemyCoroutine(m_currentEnemy.transform));
                            yield return new WaitForSeconds(0.1f);
                        }
                    }
                }
            }
        }

        if (instantKill)
        {
            if (m_currentEnemy != null)
            {
                m_currentEnemy.TakeDamage(m_currentEnemy.CurrentHealth, true);
            }
            Debug.Log("[ExecuteSequentialCardAttacks] 툼스톤 즉사!");
        }
        else
        {
            Debug.Log($"\n[4턴] 최종 데미지 적용: {finalDamage:F1}");
            if (m_currentEnemy != null)
            {
                m_currentEnemy.TakeDamage(finalDamage, true);
            }
        }

        Debug.Log($"\n=== 전투 종료 ===");
        Debug.Log($"기본 데미지: {baseDamage:F1}");
        Debug.Log($"최종 데미지: {finalDamage:F1}");
        Debug.Log($"최종 데미지 비율: {(finalDamage / baseDamage):P0}");

        List<YaCht_ComboType> allCombos = YaCht_ComboChecker.CheckAllCombos(setupCardData, wrestlerType);

        if (allCombos.Count > 0)
        {
            YaCht_GameManager.RelicManager.OnComboAchieved();

            YaCht_ComboData firstComboData = YaCht_ComboDatabase.GetComboData(wrestlerType, allCombos[0]);

            int totalScore = 0;
            foreach (var combo in allCombos)
            {
                YaCht_ComboData comboData = YaCht_ComboDatabase.GetComboData(wrestlerType, combo);
                totalScore += comboData.scoreMultiplier;
            }
            YaCht_GameManager.AddScore(totalScore);
            Debug.Log($"[ExecuteSequentialCardAttacks] 콤보 점수 추가: +{totalScore} (점수: {YaCht_GameManager.totalScore})");
        }

        // 셋업 카드 초기화 (고정 카드는 유지)
        m_cardManager.ClearSetupCards();
        // 고정 카드가 아닌 카드만 제거
        m_setupCards.RemoveAll(card => card == null || !card.IsFixedCard);

        m_isAttacking = false; // 공격 종료
        UpdateUI();

        if (m_currentEnemy != null && m_currentEnemy.IsDead)
        {
            yield break;
        }

        if (YaCht_GameManager.IsGameOver())
        {
            ShowDefeatPanel();
            yield break;
        }

        YaCht_GameManager.NextRound();
        yield return StartCoroutine(StartNewRoundCoroutine());
    }

    private System.Collections.IEnumerator StartNewRoundCoroutine()
    {
        CloseCardPreview();

        m_currentRerollCount = m_maxRerollCount;

        yield return StartCoroutine(m_cardManager.StartNewRound());

        YaCht_CardData? fixedCard = YaCht_GameManager.RelicManager.GetFixedMaskCard(
            YaCht_GameManager.nowPlayerData.playerDeck
        );

        if (fixedCard.HasValue && m_setupCards.Count < m_maxSetupCards && m_setupSlots.Count > 0)
        {
            yield return StartCoroutine(AutoSetupFixedCard(fixedCard.Value));
        }

        UpdateUI();
        Debug.Log($"=== 라운드 {YaCht_GameManager.currentRound} 시작 ===");
    }

    private System.Collections.IEnumerator AutoSetupFixedCard(YaCht_CardData cardData)
    {
        // FixedMask: 덱에서 카드를 직접 생성하여 셋업
        YaCht_WWECard fixedCard = m_cardManager.CreateFixedMaskCard(cardData);

        if (fixedCard != null)
        {
            int cardIndex = m_setupCards.Count;
            Transform targetSlot = m_setupSlots[cardIndex];

            fixedCard.SetupCard(targetSlot, cardIndex, Vector3.zero);
            m_setupCards.Add(fixedCard);
            m_cardManager.SetupCard(fixedCard, cardIndex);

            Debug.Log($"[AutoSetupFixedCard] 고정 카드 설정: {cardData.m_name} 셋업 (고정 카드)");

            // 콤보 가이드 업데이트
            UpdateComboGuide();

            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            Debug.LogWarning($"[AutoSetupFixedCard] 고정 카드 생성 실패: {cardData.m_name}");
        }
    }

    /// <summary>
    /// 유물 아이콘 버튼들 업데이트
    /// </summary>
    private void UpdateRelicIcons()
    {
        if (m_relicsInfoPanel == null || m_relicsInfoPanel.content == null)
        {
            Debug.LogWarning("[UpdateRelicIcons] RelicsInfoPanel 또는 Content가 없습니다!");
            return;
        }

        if (m_relicDetailContainer == null)
        {
            Debug.LogWarning("[UpdateRelicIcons] RelicDetailContainer가 없습니다!");
            return;
        }

        // 기존 아이콘 버튼들 및 RelicItem들 제거
        ClearRelicIcons();

        // PlayerData에서 소유한 유물 목록 가져오기
        if (YaCht_GameManager.nowPlayerData == null)
        {
            Debug.LogWarning("[UpdateRelicIcons] PlayerData가 없습니다!");
            return;
        }

        List<YaCht_RelicType> playerRelics = YaCht_GameManager.nowPlayerData.playerRelics;

        if (playerRelics == null || playerRelics.Count == 0)
        {
            Debug.Log("[UpdateRelicIcons] 소유한 유물이 없습니다.");
            return;
        }

        // 유물 아이콘 버튼 및 RelicItem 생성
        foreach (var relicType in playerRelics)
        {
            YaCht_RelicData relicData = YaCht_RelicDatabase.GetRelicData(relicType);                       

            CreateRelicIconButton(relicData);
            CreateRelicItem(relicData);
        }

        Debug.Log($"[UpdateRelicIcons] 유물 아이콘 버튼 {m_relicIconButtons.Count}개, RelicItem {m_relicItems.Count}개 생성 완료");
    }

    /// <summary>
    /// 유물 아이콘 버튼 생성
    /// </summary>
    private void CreateRelicIconButton(YaCht_RelicData relicData)
    {
        if (m_relicIconButtonPrefab == null)
        {
            Debug.LogWarning("[CreateRelicIconButton] RelicIconButtonPrefab이 없습니다!");
            return;
        }

        if (m_relicsInfoPanel == null || m_relicsInfoPanel.content == null)
        {
            Debug.LogWarning("[CreateRelicIconButton] RelicsInfoPanel 또는 Content가 없습니다!");
            return;
        }

        // 프리팹 인스턴스화
        GameObject iconButtonObj = Instantiate(m_relicIconButtonPrefab, m_relicsInfoPanel.content);
        YaCht_RelicIconButton iconButton = iconButtonObj.GetComponent<YaCht_RelicIconButton>();

        if (iconButton == null)
        {
            Debug.LogWarning("[CreateRelicIconButton] YaCht_RelicIconButton 컴포넌트가 없습니다!");
            Destroy(iconButtonObj);
            return;
        }

        // 아이콘 버튼 초기화
        iconButton.Init(relicData, OnRelicIconClicked);
        m_relicIconButtons.Add(iconButton);

        Debug.Log($"[CreateRelicIconButton] 유물 아이콘 버튼 생성: {relicData.name}");
    }

    /// <summary>
    /// RelicItem 생성
    /// </summary>
    private void CreateRelicItem(YaCht_RelicData relicData)
    {
        if (m_relicItemPrefab == null)
        {
            Debug.LogWarning("[CreateRelicItem] RelicItemPrefab이 없습니다!");
            return;
        }

        if (m_relicDetailContainer == null)
        {
            Debug.LogWarning("[CreateRelicItem] RelicDetailContainer가 없습니다!");
            return;
        }

        // RelicItem 생성
        GameObject relicItemObj = Instantiate(m_relicItemPrefab, m_relicDetailContainer);
        YaCht_RelicItem relicItem = relicItemObj.GetComponent<YaCht_RelicItem>();
        
        if (relicItem == null)
        {
            Debug.LogWarning("[CreateRelicItem] YaCht_RelicItem 컴포넌트가 없습니다!");
            Destroy(relicItemObj);
            return;
        }

        // RelicItem 초기화 (클릭 시 비활성화)
        relicItem.Init(relicData, () => {
            Debug.Log($"[CreateRelicItem] RelicItem 클릭: {relicData.name}");
            HideRelicDetail();
        });

        // 초기 상태는 비활성화
        relicItemObj.SetActive(false);

        // 딕셔너리에 저장
        m_relicItems[relicData.relicType] = relicItem;

        Debug.Log($"[CreateRelicItem] RelicItem 생성: {relicData.name}");
    }

    /// <summary>
    /// 유물 아이콘 버튼 클릭 이벤트
    /// </summary>
    private void OnRelicIconClicked(YaCht_RelicData relicData)
    {
        Debug.Log($"[OnRelicIconClicked] 유물 아이콘 클릭: {relicData.name}");

        // 이미 활성화된 RelicItem이 있으면 비활성화
        if (m_currentActiveRelicItem != null)
        {
            // 같은 RelicItem이면 비활성화
            if (m_relicItems.ContainsKey(relicData.relicType) && 
                m_relicItems[relicData.relicType] == m_currentActiveRelicItem)
            {
                HideRelicDetail();
                return;
            }
            // 다른 RelicItem이면 기존 것 비활성화
            else
            {
                HideRelicDetail();
            }
        }

        // 해당 RelicItem 활성화
        ShowRelicDetail(relicData);
    }

    /// <summary>
    /// 유물 상세 정보 표시
    /// </summary>
    private void ShowRelicDetail(YaCht_RelicData relicData)
    {
        if (!m_relicItems.ContainsKey(relicData.relicType))
        {
            Debug.LogWarning($"[ShowRelicDetail] RelicItem을 찾을 수 없습니다: {relicData.name}");
            return;
        }

        YaCht_RelicItem relicItem = m_relicItems[relicData.relicType];
        if (relicItem != null && relicItem.gameObject != null)
        {
            relicItem.gameObject.SetActive(true);
            m_currentActiveRelicItem = relicItem;
            Debug.Log($"[ShowRelicDetail] RelicItem 활성화: {relicData.name}");
        }
    }

    /// <summary>
    /// 유물 상세 정보 숨기기
    /// </summary>
    private void HideRelicDetail()
    {
        if (m_currentActiveRelicItem != null && m_currentActiveRelicItem.gameObject != null)
        {
            m_currentActiveRelicItem.gameObject.SetActive(false);
            Debug.Log($"[HideRelicDetail] RelicItem 비활성화");
        }
        m_currentActiveRelicItem = null;
    }

    /// <summary>
    /// 유물 아이콘 버튼들 및 RelicItem들 제거
    /// </summary>
    private void ClearRelicIcons()
    {
        // 아이콘 버튼들 제거
        foreach (var iconButton in m_relicIconButtons)
        {
            if (iconButton != null && iconButton.gameObject != null)
            {
                Destroy(iconButton.gameObject);
            }
        }
        m_relicIconButtons.Clear();

        // RelicItem들 제거
        foreach (var relicItem in m_relicItems.Values)
        {
            if (relicItem != null && relicItem.gameObject != null)
            {
                Destroy(relicItem.gameObject);
            }
        }
        m_relicItems.Clear();
        m_currentActiveRelicItem = null;
    }
}
