using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class YaCht_WWEMainGame : MonoBehaviour
{
    [Header("Setup Area")]
    [SerializeField] private Transform m_setupArea;
    [SerializeField] private int m_maxSetupCards = 6;
    [SerializeField] private float m_setupCardSpacing = 1.5f;

    [Header("UI Elements")]
    [SerializeField] private GameObject m_backgroundPanel;
    [SerializeField] private Button m_backgroundButton;
    [SerializeField] private Button m_fightButton;
    [SerializeField] private Button m_rerollButton;
    [SerializeField] private TextMeshProUGUI m_comboInfoText;
    [SerializeField] private TextMeshProUGUI m_roundText;
    [SerializeField] private TextMeshProUGUI m_enemyHealthText;
    [SerializeField] private TextMeshProUGUI m_rerollCountText;

    [Header("Reroll Settings")]
    [SerializeField] private int m_maxRerollCount = 3;

    [SerializeField] private YaCht_ComboGuideUI m_comboGuideUI;
    private YaCht_CardManager m_cardManager;

    private List<YaCht_WWECard> m_setupCards = new List<YaCht_WWECard>();
    private List<Transform> m_setupSlots = new List<Transform>();

    private YaCht_WWECard m_currentPreviewOriginalCard;

    private int m_currentRerollCount;

    public void Init()
    {
        // 셋업 카드 리스트 초기화
        m_setupCards.Clear();

        // 셋업 슬롯 Transform 생성
        CreateSetupSlots();

        // 리롤 카운트 초기화
        m_currentRerollCount = m_maxRerollCount;

        m_backgroundButton.onClick.AddListener(OnBackgroundClicked);

        m_fightButton.onClick.AddListener(OnFightButtonClicked);

        m_rerollButton.onClick.AddListener(OnRerollButtonClicked);

        m_backgroundPanel.SetActive(false);

        m_cardManager = YaCht_GameManager.CardManager;
        m_comboGuideUI.Initialize(YaCht_GameManager.nowPlayerData.GetWrestlerType());

        UpdateUI();
    }

    // 셋업 슬롯 Transform 생성
    private void CreateSetupSlots()
    {
        // 기존 슬롯 정리
        foreach (var slot in m_setupSlots)
        {
            if (slot != null)
            {
                Destroy(slot.gameObject);
            }
        }
        m_setupSlots.Clear();

        if (m_setupArea == null) return;

        // m_maxSetupCards 개수만큼 슬롯 생성
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
        // 실시간 조합 정보 업데이트
        if (m_comboInfoText != null)
        {
            string comboInfo = m_cardManager.GetCurrentComboInfo();
            m_comboInfoText.text = comboInfo;
        }
    }

    private void UpdateUI()
    {
        // 라운드 정보 업데이트
        if (m_roundText != null)
        {
            m_roundText.text = $"라운드: {YaCht_GameManager.currentRound} / 4";
        }

        // 적 체력 업데이트
        if (m_enemyHealthText != null)
        {
            m_enemyHealthText.text = $"적 체력: {YaCht_GameManager.enemyHealth:F0} / {YaCht_GameManager.enemyMaxHealth:F0}";
        }

        // 리롤 카운트 업데이트
        if (m_rerollCountText != null)
        {
            m_rerollCountText.text = $"리롤: {m_currentRerollCount} / {m_maxRerollCount}";
        }

        // 리롤 버튼 활성화/비활성화
        if (m_rerollButton != null)
        {
            m_rerollButton.interactable = m_currentRerollCount > 0;
        }
    }

    private void OnBackgroundClicked()
    {
        CloseCardPreview();
    }

    // 손패 카드가 클릭되었을 때
    public void OnHandCardClicked(YaCht_WWECard card)
    {
        if (card == null) return;

        YaCht_WWECard previewCard = m_cardManager.GetPreviewCard();

        // 프리뷰가 이미 활성화되어 있는 경우
        if (m_currentPreviewOriginalCard != null && previewCard != null && previewCard.gameObject.activeSelf)
        {
            // 같은 카드를 클릭한 경우 - 셋업으로 이동
            if (m_currentPreviewOriginalCard == card)
            {
                OnPreviewCardClicked();
            }
            // 다른 카드를 클릭한 경우 - 새 카드 프리뷰 표시
            else
            {
                ShowCardPreview(card);
            }
            return;
        }

        // 프리뷰가 없는 상태에서 카드 클릭 - 프리뷰 표시
        ShowCardPreview(card);
    }

    // 카드 프리뷰 표시
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

    // 프리뷰 카드가 클릭되었을 때 - 셋업 진행
    public void OnPreviewCardClicked()
    {
        if (m_currentPreviewOriginalCard == null) return;

        // 셋업 카드 최대치 체크
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

    // 카드 프리뷰 닫기
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

    // 셋업 슬롯에서 카드 제거
    public void RemoveTopCardFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= m_setupCards.Count) return;

        YaCht_WWECard cardToRemove = m_setupCards[slotIndex];
        m_setupCards.RemoveAt(slotIndex);

        m_cardManager.ReleaseCardFromSetup(cardToRemove);
        cardToRemove.ReleaseSetup();

        // 남은 카드들을 다음 슬롯으로 이동
        RepositionSetupCards();
    }

    // 셋업 카드들을 슬롯 순서대로 재정렬
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

    // 리롤 버튼 클릭 시
    private void OnRerollButtonClicked()
    {
        if (m_currentRerollCount <= 0)
        {
            Debug.Log("리롤 횟수가 부족합니다!");
            return;
        }

        m_currentRerollCount--;
        StartCoroutine(m_cardManager.RerollHand());
        UpdateUI();

        Debug.Log($"리롤 사용! 남은 횟수: {m_currentRerollCount}");
    }

    // 싸우기 버튼 클릭 시
    private void OnFightButtonClicked()
    {
        if (m_setupCards.Count == 0)
        {
            Debug.Log("셋업된 카드가 없습니다!");
            return;
        }

        // 카드 프리뷰 닫기
        CloseCardPreview();

        List<YaCht_CardData> setupCardData = new List<YaCht_CardData>();
        foreach (var card in m_setupCards)
        {
            setupCardData.Add(card.GetCardData);
        }

        YaCht_WrestlerType wrestlerType = YaCht_GameManager.nowPlayerData.wrestlerType;
        YaCht_ComboType combo = YaCht_ComboChecker.CheckCombo(setupCardData, wrestlerType);
        float totalDamage = YaCht_ComboChecker.CalculateComboDamage(setupCardData, wrestlerType, combo);

        Debug.Log($"=== 전투 시작 ===");
        Debug.Log(m_cardManager.GetCurrentComboInfo());
        Debug.Log($"총 데미지: {totalDamage}");

        // 적에게 데미지 적용
        YaCht_GameManager.DamageEnemy(totalDamage);

        // 점수 추가
        YaCht_ComboData comboData = YaCht_ComboDatabase.GetComboData(wrestlerType, combo);
        YaCht_GameManager.AddScore(comboData.scoreMultiplier);

        // 셋업 카드 제거
        m_cardManager.ClearSetupCards();
        m_setupCards.Clear();

        // UI 업데이트
        UpdateUI();

        // 게임 종료 체크
        if (YaCht_GameManager.IsGameOver())
        {
            if (YaCht_GameManager.enemyHealth <= 0)
            {
                Debug.Log("승리!");
            }
            else
            {
                Debug.Log("4라운드 종료!");
            }
            return;
        }

        // 다음 라운드로
        YaCht_GameManager.NextRound();

        // 새 라운드 시작 - 모든 패 버리고 10장 다시 뽑기
        StartCoroutine(StartNewRoundCoroutine());
    }

    // 새 라운드 시작 코루틴
    private System.Collections.IEnumerator StartNewRoundCoroutine()
    {
        // 카드 프리뷰 닫기
        CloseCardPreview();

        // 리롤 카운트 리셋
        m_currentRerollCount = m_maxRerollCount;

        yield return StartCoroutine(m_cardManager.StartNewRound());

        UpdateUI();
        Debug.Log($"=== 라운드 {YaCht_GameManager.currentRound} 시작 ===");
    }

    private void OnDestroy()
    {
        if (m_backgroundButton != null)
        {
            m_backgroundButton.onClick.RemoveListener(OnBackgroundClicked);
        }

        if (m_fightButton != null)
        {
            m_fightButton.onClick.RemoveListener(OnFightButtonClicked);
        }

        if (m_rerollButton != null)
        {
            m_rerollButton.onClick.RemoveListener(OnRerollButtonClicked);
        }
    }
}
