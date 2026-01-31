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

        // 적 스폰
        SpawnEnemy();

        m_isBattleEnded = false;
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (m_currentEnemy != null)
        {
            m_currentEnemy.OnDeath -= OnEnemyDeath;
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
                m_currentEnemy.SetHealth(YaCht_GameManager.enemyHealth, YaCht_GameManager.enemyMaxHealth);

                // 이벤트 구독
                m_currentEnemy.OnDeath += OnEnemyDeath;

                Debug.Log($"[WWEMainGame] 적 스폰 완료: {currentEnemyData.m_name} (스테이지 {currentEnemyData.m_stageNumber})");
            }
            else
            {
                Debug.LogError("[WWEMainGame] Enemy Prefab에 YaCht_Enemy 컴포넌트가 없습니다!");
            }
        }
        else
        {
            Debug.LogError("[WWEMainGame] Enemy Prefab 또는 Spawn Position이 설정되지 않았습니다!");
        }
    }
    
    // 적 사망 시 호출
    private void OnEnemyDeath()
    {
        if (m_currentEnemy == null) return;

        bool isBoss = m_currentEnemy.IsBoss;
        
        if (isBoss)
        {
            OnBossDefeated();
        }
        else
        {
            OnNormalEnemyDefeated();
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
        // YaCht_StageDatabase를 통해 보스전 확인
        bool isBoss = YaCht_GameManager.IsCurrentStageBoss();
        
        if (isBoss)
        {
            Debug.Log("[WWEMainGame] 보스 처치! 유물 선택 씬으로 이동");
            YaCht_GameManager.SetRelicSceneFromBossDefeat();
            SceneManager.LoadScene("YaCht_RelicScene");
        }
        else
        {
            bool success = YaCht_GameManager.MoveToNextStage();
            if (success)
            {
                Debug.Log("[WWEMainGame] 다음 스테이지 로드");
                SceneManager.LoadScene("YaCht_GameScene");
            }
            else
            {
                Debug.LogError("[WWEMainGame] 다음 스테이지로 이동 실패!");
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
        YaCht_ComboType combo = YaCht_ComboChecker.CheckCombo(setupCardData, wrestlerType);
        
        YaCht_GameManager.RelicManager.OnCardsUsed(setupCardData);
        float baseDamage = YaCht_ComboChecker.CalculateComboDamage(setupCardData, wrestlerType, combo);
        float finalDamage = YaCht_GameManager.RelicManager.CalculateFinalDamage(baseDamage, setupCardData);
        
        float enemyHealthPercent = (YaCht_GameManager.enemyHealth / YaCht_GameManager.enemyMaxHealth) * 100f;
        bool instantKill = YaCht_GameManager.RelicManager.CheckRestTombstoneInstantKill(setupCardData, enemyHealthPercent);
        
        if (instantKill)
        {
            if (m_currentEnemy != null)
            {
                m_currentEnemy.TakeDamage(m_currentEnemy.CurrentHealth, true);
            }
            YaCht_GameManager.DamageEnemy(YaCht_GameManager.enemyHealth);
            Debug.Log("[유물] 안식의 비석 발동! 즉시 처치!");
        }
        else
        {
            if (m_currentEnemy != null)
            {
                m_currentEnemy.TakeDamage(finalDamage, true);
            }
            YaCht_GameManager.DamageEnemy(finalDamage);
        }
        
        Debug.Log($"=== 전투 결과 ===");
        Debug.Log($"기본 데미지: {baseDamage}");
        Debug.Log($"최종 데미지: {finalDamage}");

        if (combo != YaCht_ComboType.None)
        {
            YaCht_GameManager.RelicManager.OnComboAchieved();
        }
        
        YaCht_ComboData comboData = YaCht_ComboDatabase.GetComboData(wrestlerType, combo);
        if (comboData.comboLevel == YaCht_ComboLevel.Combo3 || 
            comboData.comboLevel == YaCht_ComboLevel.Combo4)
        {
            YaCht_GameManager.RelicManager.OnEasyNormalComboSuccess();
        }

        YaCht_GameManager.AddScore(comboData.scoreMultiplier);

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
