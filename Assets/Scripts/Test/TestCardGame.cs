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

    [Header("Background Panel")]
    [SerializeField] private GameObject m_backgroundPanel;
    [SerializeField] private Button m_backgroundButton;

    [SerializeField] private CardManager m_cardManager;

    private List<WWECard>[] m_setupCardStacks = new List<WWECard>[3];    
    [SerializeField] private Vector3 m_stackOffset = new Vector3(0.2f, 0.2f, 0.1f);

    private WWECard m_currentPreviewOriginalCard;

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

        // 백그라운드 버튼 이벤트 연결
        if (m_backgroundButton != null)
        {
            m_backgroundButton.onClick.AddListener(OnBackgroundClicked);
        }

        // 백그라운드 패널 초기 비활성화
        if (m_backgroundPanel != null)
        {
            m_backgroundPanel.SetActive(false);
        }
    }

    private void OnBackgroundClicked()
    {
        CloseCardPreview();
    }

    // Setup 버튼 클릭 이벤트
    private void OnSetupButtonClicked()
    {
        m_cardManager.Init();
        m_cardManager.StartGame();
    }

    // Draw 버튼 클릭 이벤트
    private void OnDrawButtonClicked()
    {
        m_cardManager.DrawCard();
    }

    // 손패 카드가 클릭되었을 때
    public void OnHandCardClicked(WWECard card)
    {
        if (card == null) return;

        WWECard previewCard = m_cardManager.GetPreviewCard();

        // 미리보기가 이미 활성화되어 있는 경우
        if (m_currentPreviewOriginalCard != null && previewCard != null && previewCard.gameObject.activeSelf)
        {
            // 같은 카드를 클릭한 경우 - 셋업으로 이동
            if (m_currentPreviewOriginalCard == card)
            {
                OnPreviewCardClicked();
            }
            // 다른 카드를 클릭한 경우 - 새 카드 미리보기 표시
            else
            {
                ShowCardPreview(card);
            }
            return;
        }

        // 미리보기가 없는 상태에서 카드 클릭 - 미리보기 표시
        ShowCardPreview(card);
    }

    // 카드 미리보기 표시
    private void ShowCardPreview(WWECard originalCard)
    {
        WWECard previewCard = m_cardManager.GetPreviewCard();
        if (previewCard == null) return;

        previewCard.UpdateCardData(originalCard.GetCardData);
        previewCard.gameObject.SetActive(true);

        if (m_backgroundPanel != null)
        {
            m_backgroundPanel.SetActive(true);
        }

        m_currentPreviewOriginalCard = originalCard;        
    }

    // 미리보기 카드가 클릭되었을 때 - 셋업 진행
    public void OnPreviewCardClicked()
    {
        if (m_currentPreviewOriginalCard == null) return;

        int targetSlotIndex = GetNextSlotIndex();
        int originalHandIndex = m_cardManager.GetCardIndexInHand(m_currentPreviewOriginalCard);
    
        GameObject setupSlot = GetSetupSlot(targetSlotIndex);
        if (setupSlot != null)
        {
            int stackCount = m_setupCardStacks[targetSlotIndex].Count;
            Vector3 offset = m_stackOffset * stackCount;
            
            m_currentPreviewOriginalCard.SetupCard(setupSlot.transform, targetSlotIndex, originalHandIndex, offset);
            m_setupCardStacks[targetSlotIndex].Add(m_currentPreviewOriginalCard);
            m_cardManager.SetupCard(m_currentPreviewOriginalCard, targetSlotIndex);
        }

        CloseCardPreview();
    }

    // 카드 미리보기 닫기
    private void CloseCardPreview()
    {
        WWECard previewCard = m_cardManager.GetPreviewCard();
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
    
    // 특정 슬롯의 가장 위(마지막) 카드 제거 - 스택의 맨 위 카드 제거
    public void RemoveTopCardFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= m_setupCardStacks.Length) return;
        
        if (m_setupCardStacks[slotIndex].Count > 0)
        {
            // 스택의 마지막 카드(가장 위에 보이는 카드) 제거
            int lastIndex = m_setupCardStacks[slotIndex].Count - 1;
            WWECard topCard = m_setupCardStacks[slotIndex][lastIndex];
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
                card.transform.localPosition = m_stackOffset * i;
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

        if (m_backgroundButton != null)
        {
            m_backgroundButton.onClick.RemoveListener(OnBackgroundClicked);
        }
    }
}
