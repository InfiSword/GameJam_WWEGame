using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; 

public class TestCardGame : MonoBehaviour
{
    [SerializeField] private Button m_setupButton;
    [SerializeField] private Button m_drawButton;

    [Header("Setup Slots")]
    [SerializeField] private GameObject m_cardSetupObj_1;
    [SerializeField] private GameObject m_cardSetupObj_2;
    [SerializeField] private GameObject m_cardSetupObj_3;

    [SerializeField] private CardManager m_cardManager;

    private List<WWECard>[] m_setupCardStacks = new List<WWECard>[3];    
    [SerializeField] private Vector3 m_stackOffset = new Vector3(0.2f, 0.2f, 0.1f);

    private void Start()
    {
        // 슬롯별 카드 스택 초기화
        for (int i = 0; i < m_setupCardStacks.Length; i++)
        {
            m_setupCardStacks[i] = new List<WWECard>();
        }
        
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

        int targetSlotIndex = GetNextSlotIndex();
        
        int originalHandIndex = m_cardManager.GetCardIndexInHand(card);
    
        GameObject setupSlot = GetSetupSlot(targetSlotIndex);
        if (setupSlot != null)
        {
            int stackCount = m_setupCardStacks[targetSlotIndex].Count;
            Vector3 offset = m_stackOffset * stackCount;
            
            card.SetupCard(setupSlot.transform, targetSlotIndex, originalHandIndex, offset);
            m_setupCardStacks[targetSlotIndex].Add(card);
            m_cardManager.SetupCard(card, targetSlotIndex);

            Debug.Log($"카드를 슬롯 {targetSlotIndex + 1}에 배치했습니다. (스택: {stackCount + 1}장)");
        }
    }
    
    // 다음 슬롯 인덱스를 라운드 로빈으로 반환
    private int GetNextSlotIndex()
    {
        int minCount = int.MaxValue;
        int targetIndex = 0;
        
        for (int i = 0; i < m_setupCardStacks.Length; i++)
        {
            if (m_setupCardStacks[i].Count < minCount)
            {
                minCount = m_setupCardStacks[i].Count;
                targetIndex = i;
            }
        }
        
        return targetIndex;
    }

    // 셋업 슬롯에서 카드 제거 (내부용)
    private void RemoveCardFromSetup(WWECard card)
    {
        if (card == null) return;

        int slotIndex = card.SetupSlotIndex;
        if (slotIndex >= 0 && slotIndex < m_setupCardStacks.Length)
        {
            if (m_setupCardStacks[slotIndex].Contains(card))
            {
                m_setupCardStacks[slotIndex].Remove(card);
                
                // 남은 카드들의 위치 재조정
                RepositionStackCards(slotIndex);

                m_cardManager.ReleaseCardFromSetup(card);
                card.ReleaseSetup();
            }
        }
    }
    
    // 특정 슬롯의 가장 위(처음) 카드 제거 - 스택의 맨 위 카드 제거
    public void RemoveTopCardFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= m_setupCardStacks.Length) return;
        
        if (m_setupCardStacks[slotIndex].Count > 0)
        {
            // 스택의 첫 번째 카드(가장 위에 보이는 카드) 제거
            WWECard topCard = m_setupCardStacks[slotIndex][0];
            RemoveCardFromSetup(topCard);
        }
    }

    // 특정 슬롯의 카드들 위치 재조정
    private void RepositionStackCards(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= m_setupCardStacks.Length) return;
        
        for (int i = 0; i < m_setupCardStacks[slotIndex].Count; i++)
        {
            WWECard card = m_setupCardStacks[slotIndex][i];
            if (card != null)
            {
                Vector3 offset = m_stackOffset * i;
                card.transform.localPosition = offset;
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

    // 셋업된 카드들을 사용하는 메서드 (추후 구현용)
    public void ExecuteSetupCards()
    {
        for (int i = 0; i < m_setupCardStacks.Length; i++)
        {
            foreach (var card in m_setupCardStacks[i])
            {
                if (card != null)
                {
                    // 여기서 카드 사용 로직 구현
                }
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
