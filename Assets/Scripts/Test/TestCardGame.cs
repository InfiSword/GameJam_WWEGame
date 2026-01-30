using UnityEngine;
using UnityEngine.UI;

public class TestCardGame : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button m_setupButton;
    [SerializeField] private Button m_drawButton;

    [Header("Setup Slots")]
    [SerializeField] private GameObject m_cardSetupObj_1;
    [SerializeField] private GameObject m_cardSetupObj_2;
    [SerializeField] private GameObject m_cardSetupObj_3;

    [Header("Manager Reference")]
    [SerializeField] private CardManager m_cardManager;

    private WWECard[] m_setupCards = new WWECard[3];

    private void Start()
    {
        // 버튼 이벤트 연결
        if (m_setupButton != null)
        {
            m_setupButton.onClick.AddListener(OnSetupButtonClicked);
        }

        if (m_drawButton != null)
        {
            m_drawButton.onClick.AddListener(OnDrawButtonClicked);
        }
    }

    // Setup 버튼 클릭 이벤트
    private void OnSetupButtonClicked()
    {
        m_cardManager.Init();
        m_cardManager.StartGame();

        Debug.Log("초기 손패 생성 중");
    }

    // Draw 버튼 클릭 이벤트
    private void OnDrawButtonClicked()
    {
        m_cardManager.DrawCard();
    }

    // 카드가 클릭되었을 때 호출되는 메서드
    public void OnCardClicked(WWECard card)
    {
        if (card == null) return;

        // 빈 슬롯 찾기
        int emptySlotIndex = -1;
        for (int i = 0; i < m_setupCards.Length; i++)
        {
            if (m_setupCards[i] == null)
            {
                emptySlotIndex = i;
                break;
            }
        }

        if (emptySlotIndex == -1)
        {
            Debug.Log("모든 셋업 슬롯이 가득 찼습니다!");
            return;
        }

        // 카드를 셋업 슬롯에 배치
        GameObject setupSlot = GetSetupSlot(emptySlotIndex);
        if (setupSlot != null)
        {
            card.SetupCard(setupSlot.transform, emptySlotIndex);
            m_setupCards[emptySlotIndex] = card;
            m_cardManager.SetupCard(card, emptySlotIndex);

            Debug.Log($"카드를 슬롯 {emptySlotIndex + 1}에 배치했습니다. ({GetSetupCount()}/3)");
        }
    }

    // 셋업 슬롯에서 카드 제거
    public void RemoveCardFromSetup(WWECard card)
    {
        if (card == null) return;

        int slotIndex = card.SetupSlotIndex;
        if (slotIndex >= 0 && slotIndex < m_setupCards.Length)
        {
            if (m_setupCards[slotIndex] == card)
            {
                m_setupCards[slotIndex] = null;

                m_cardManager.ReleaseCardFromSetup(card);
                card.ReleaseSetup();
            }
        }
    }

    // 슬롯 인덱스로 GameObject 반환
    private GameObject GetSetupSlot(int index)
    {
        switch (index)
        {
            case 0: return m_cardSetupObj_1;
            case 1: return m_cardSetupObj_2;
            case 2: return m_cardSetupObj_3;
            default: return null;
        }
    }

    // 현재 셋업된 카드 개수
    private int GetSetupCount()
    {
        int count = 0;
        foreach (var card in m_setupCards)
        {
            if (card != null) count++;
        }
        return count;
    }

    // 셋업된 카드들을 사용하는 메서드 (추후 구현용)
    public void ExecuteSetupCards()
    {
        for (int i = 0; i < m_setupCards.Length; i++)
        {
            if (m_setupCards[i] != null)
            {
                // 여기서 카드 사용 로직 구현
            }
        }
    }

    private void OnDestroy()
    {
        // 버튼 이벤트 해제
        if (m_setupButton != null)
        {
            m_setupButton.onClick.RemoveListener(OnSetupButtonClicked);
        }

        if (m_drawButton != null)
        {
            m_drawButton.onClick.RemoveListener(OnDrawButtonClicked);
        }
    }
}
