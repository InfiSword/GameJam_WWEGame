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

    [Header("UI Elements")]
    [SerializeField] private GameObject m_backgroundPanel;
    [SerializeField] private Button m_backgroundButton;
    [SerializeField] private Button m_fightButton;
    [SerializeField] private Button m_rerollButton;
    [SerializeField] private TextMeshProUGUI m_comboInfoText;
    [SerializeField] private TextMeshProUGUI m_roundText;
    [SerializeField] private TextMeshProUGUI m_rerollCountText;
    [SerializeField] private TextMeshProUGUI m_stageInfoText;

    [Header("Reroll Settings")]
    [SerializeField] private int m_maxRerollCount = 3;

    [Header("Victory Panel")]
    [SerializeField] private GameObject m_victoryPanel;
    [SerializeField] private TextMeshProUGUI m_victoryText;
    [SerializeField] private Button m_nextStageButton;

    [SerializeField] private YaCht_ComboGuideUI m_comboGuideUI;
    private YaCht_CardManager m_cardManager;

    private List<YaCht_WWECard> m_setupCards = new List<YaCht_WWECard>();
    private List<Transform> m_setupSlots = new List<Transform>();
    private YaCht_WWECard m_currentPreviewOriginalCard;
    private int m_currentRerollCount;
    private bool m_isBattleEnded = false;

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

        if (m_nextStageButton != null)
        {
            m_nextStageButton.onClick.AddListener(OnNextStageButtonClicked);
        }

        m_cardManager = YaCht_GameManager.CardManager;
        m_comboGuideUI.Initialize(YaCht_GameManager.nowPlayerData.GetWrestlerType());

        // StageManager 이벤트 구독
        if (YaCht_GameManager.StageManager != null)
        {
            YaCht_GameManager.StageManager.OnBossDefeated += OnBossDefeated;
            YaCht_GameManager.StageManager.OnEnemyDefeatedNormal += OnNormalEnemyDefeated;
        }

        // 적 스폰
        SpawnEnemy();

        m_isBattleEnded = false;
        UpdateUI();
    }

    private void OnDestroy()
    {
        // StageManager 이벤트 구독 해제
        if (YaCht_GameManager.StageManager != null)
        {
            YaCht_GameManager.StageManager.OnBossDefeated -= OnBossDefeated;
            YaCht_GameManager.StageManager.OnEnemyDefeatedNormal -= OnNormalEnemyDefeated;
            YaCht_GameManager.StageManager.UnregisterEnemy();
        }

        if (m_nextStageButton != null)
        {
            m_nextStageButton.onClick.RemoveListener(OnNextStageButtonClicked);
        }
    }

    // 적 스폰
    private void SpawnEnemy()
    {
        // 기존 적 제거
        if (m_currentEnemy != null)
        {
            if (YaCht_GameManager.StageManager != null)
            {
                YaCht_GameManager.StageManager.UnregisterEnemy();
            }
            Destroy(m_currentEnemy.gameObject);
            m_currentEnemy = null;
        }

        // 새 적 생성
        if (m_enemyPrefab != null && m_enemySpawnPosition != null && YaCht_GameManager.StageManager != null)
        {
            GameObject enemyObj = Instantiate(m_enemyPrefab, m_enemySpawnPosition);
            m_currentEnemy = enemyObj.GetComponent<YaCht_Enemy>();

            if (m_currentEnemy != null)
            {
                YaCht_EnemyData currentEnemyData = YaCht_GameManager.StageManager.CurrentEnemy;
                m_currentEnemy.Initialize(currentEnemyData, m_enemySpawnPosition);

                YaCht_GameManager.StageManager.RegisterEnemy(m_currentEnemy);
            }
        }
    }

    private void OnBossDefeated()
    {
        m_isBattleEnded = true;
        ShowVictoryPanel("보스 처치! 유물을 선택하세요!");
    }

    private void OnNormalEnemyDefeated()
    {
        m_isBattleEnded = true;
        ShowVictoryPanel("승리! 다음 스테이지로 진행하세요!");
    }

    private void ShowVictoryPanel(string message)
    {
        if (m_victoryPanel != null)
        {
            m_victoryPanel.SetActive(true);
        }

        if (m_victoryText != null)
        {
            m_victoryText.text = message;
        }

        if (m_fightButton != null)
        {
            m_fightButton.interactable = false;
        }

        if (m_rerollButton != null)
        {
            m_rerollButton.interactable = false;
        }
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

    private void Update()
    {
        if (m_comboInfoText != null)
        {
            string comboInfo = m_cardManager.GetCurrentComboInfo();
            m_comboInfoText.text = comboInfo;
        }
    }

    private void UpdateUI()
    {
        if (m_roundText != null)
        {
            m_roundText.text = $"라운드: {YaCht_GameManager.currentRound} / 4";
        }

        if (m_stageInfoText != null && YaCht_GameManager.StageManager != null)
        {
            m_stageInfoText.text = YaCht_GameManager.StageManager.GetStageInfoString();
        }

        if (m_rerollCountText != null)
        {
            m_rerollCountText.text = $"리롤: {m_currentRerollCount} / {m_maxRerollCount}";
        }

        if (m_rerollButton != null)
        {
            m_rerollButton.interactable = m_currentRerollCount > 0 && !m_isBattleEnded;
        }

        if (m_fightButton != null)
        {
            m_fightButton.interactable = !m_isBattleEnded;
        }
    }

    private void OnBackgroundClicked()
    {
        CloseCardPreview();
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
        m_setupCards.RemoveAt(slotIndex);

        m_cardManager.ReleaseCardFromSetup(cardToRemove);
        cardToRemove.ReleaseSetup();

        RepositionSetupCards();
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

    private void OnRerollButtonClicked()
    {
        if (m_currentRerollCount <= 0)
        {
            Debug.Log("리롤 횟수가 부족합니다!");
            return;
        }

        if (m_isBattleEnded)
        {
            return;
        }

        m_currentRerollCount--;
        StartCoroutine(m_cardManager.RerollHand());
        UpdateUI();

        Debug.Log($"리롤 사용! 남은 횟수: {m_currentRerollCount}");
    }

    private void OnFightButtonClicked()
    {
        if (m_setupCards.Count == 0)
        {
            Debug.Log("셋업된 카드가 없습니다!");
            return;
        }

        if (m_isBattleEnded)
        {
            return;
        }

        CloseCardPreview();

        List<YaCht_CardData> setupCardData = new List<YaCht_CardData>();
        foreach (var card in m_setupCards)
        {
            setupCardData.Add(card.GetCardData);
        }

        YaCht_WrestlerType wrestlerType = YaCht_GameManager.nowPlayerData.wrestlerType;

        Debug.Log("=== 전투 시작 ===");
        Debug.Log($"셋업 카드 수: {setupCardData.Count}");
        foreach (var card in setupCardData)
        {
            Debug.Log($"  - {card.m_name} ({card.m_rarity}급, 데미지: {card.m_baseDamage})");
        }

        // 유물 효과 먼저 적용
        Debug.Log("\n[1단계] 유물 효과 적용 (OnCardsUsed)");
        YaCht_GameManager.RelicManager.OnCardsUsed(setupCardData);

        // 모든 콤보 데미지 계산
        Debug.Log("\n[2단계] 콤보 데미지 계산");
        float baseDamage = YaCht_ComboChecker.CalculateComboDamage(setupCardData, wrestlerType, YaCht_ComboType.None);
        Debug.Log($"콤보 적용 후 기본 데미지: {baseDamage:F1}");
        
        Debug.Log("\n[3단계] 최종 데미지 계산 (유물 배수 적용)");
        float finalDamage = YaCht_GameManager.RelicManager.CalculateFinalDamage(baseDamage, setupCardData);
        Debug.Log($"유물 적용 후 최종 데미지: {finalDamage:F1}");

        // 안식의 비석 즉사 체크
        float enemyHealthPercent = 0f;
        if (m_currentEnemy != null && m_currentEnemy.MaxHealth > 0)
        {
            enemyHealthPercent = (m_currentEnemy.CurrentHealth / m_currentEnemy.MaxHealth) * 100f;
        }
        bool instantKill = YaCht_GameManager.RelicManager.CheckRestTombstoneInstantKill(setupCardData, enemyHealthPercent);

        if (instantKill)
        {
            if (m_currentEnemy != null)
            {
                m_currentEnemy.TakeDamage(m_currentEnemy.CurrentHealth, true);
            }
            Debug.Log("[유물] 안식의 비석 발동! 즉시 처치!");
        }
        else
        {
            Debug.Log($"\n[4단계] 적에게 데미지 적용: {finalDamage:F1}");
            if (m_currentEnemy != null)
            {
                m_currentEnemy.TakeDamage(finalDamage, true);
            }
        }

        Debug.Log($"\n=== 전투 결과 ===");
        Debug.Log($"기본 데미지: {baseDamage:F1}");
        Debug.Log($"최종 데미지: {finalDamage:F1}");
        Debug.Log($"데미지 증가율: {(finalDamage / baseDamage):P0}");

        // 모든 콤보 가져오기
        List<YaCht_ComboType> allCombos = YaCht_ComboChecker.CheckAllCombos(setupCardData, wrestlerType);

        // 콤보가 있으면 유물 효과 발동
        if (allCombos.Count > 0)
        {
            YaCht_GameManager.RelicManager.OnComboAchieved();

            // 첫 번째 콤보 정보로 레벨 체크 (Easy/Normal 판정용)
            YaCht_ComboData firstComboData = YaCht_ComboDatabase.GetComboData(wrestlerType, allCombos[0]);
            if (firstComboData.comboLevel == YaCht_ComboLevel.Combo3 ||
                firstComboData.comboLevel == YaCht_ComboLevel.Combo4)
            {
                YaCht_GameManager.RelicManager.OnEasyNormalComboSuccess();
            }

            // 점수는 모든 콤보 합산
            int totalScore = 0;
            foreach (var combo in allCombos)
            {
                YaCht_ComboData comboData = YaCht_ComboDatabase.GetComboData(wrestlerType, combo);
                totalScore += comboData.scoreMultiplier;
            }
            YaCht_GameManager.AddScore(totalScore);
        }

        m_cardManager.ClearSetupCards();
        m_setupCards.Clear();

        UpdateUI();

        if (m_currentEnemy != null && m_currentEnemy.IsDead)
        {
            return;
        }

        if (YaCht_GameManager.IsGameOver())
        {
            Debug.Log("4라운드 종료! 스테이지 실패!");
            return;
        }

        YaCht_GameManager.NextRound();
        StartCoroutine(StartNewRoundCoroutine());
    }

    private System.Collections.IEnumerator StartNewRoundCoroutine()
    {
        CloseCardPreview();

        int bonusReroll = YaCht_GameManager.RelicManager.ConsumeMercyMaskBonus();
        m_currentRerollCount = m_maxRerollCount + bonusReroll;

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
        YaCht_WWECard targetCard = m_cardManager.FindCardInHand(cardData);

        if (targetCard != null)
        {
            int cardIndex = m_setupCards.Count;
            Transform targetSlot = m_setupSlots[cardIndex];

            targetCard.SetupCard(targetSlot, cardIndex, Vector3.zero);
            m_setupCards.Add(targetCard);
            m_cardManager.SetupCard(targetCard, cardIndex);

            Debug.Log($"[유물] 고정의 가면: {cardData.m_name} 자동 셋업");

            yield return new WaitForSeconds(0.3f);
        }
    }
}
