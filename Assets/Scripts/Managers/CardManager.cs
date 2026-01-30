using UnityEngine;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    [Header("Card Prefab")]
    private GameObject m_cardPrefab;
    
    [Header("Card Positions")]
    [SerializeField] private Transform m_handTransform;
    [SerializeField] private float m_cardSpacing = 150f;
    [SerializeField] private float m_cardYOffset = -300f;
    
    [Header("Card Collections")]
    private List<CardData> m_deck = new List<CardData>();
    private List<WWECard> m_hand = new List<WWECard>();
    private List<CardData> m_discardPile = new List<CardData>();
    
    [Header("Settings")]
    private int m_initialHandSize = 5;
    
    public void Init()
    { 
        m_cardPrefab = Resources.Load<GameObject>("Prefabs/Card");  
    }

    public void StartGame()
    {
        InitializeDeck();
        ShuffleDeck();
        DrawInitialHand();
    }
    
    // 초기 덱 구성
    private void InitializeDeck()
    {
        m_deck.Clear();
        
        // 기본 카드들로 덱 구성 (각 카드 2장씩)
        m_deck.Add(CardDatabase.Chop);
        m_deck.Add(CardDatabase.Chop);
        m_deck.Add(CardDatabase.LowKick);
        m_deck.Add(CardDatabase.LowKick);
        m_deck.Add(CardDatabase.Jab);
        m_deck.Add(CardDatabase.Jab);
        m_deck.Add(CardDatabase.Headbutt);
        m_deck.Add(CardDatabase.Headbutt);
        m_deck.Add(CardDatabase.RearNakedChoke);
        m_deck.Add(CardDatabase.HeartPunch);
        
        Debug.Log($"덱 초기화 완료: {m_deck.Count}장");
    }
    
    // 덱 셔플
    private void ShuffleDeck()
    {
        for (int i = 0; i < m_deck.Count; i++)
        {
            CardData temp = m_deck[i];
            int randomIndex = Random.Range(i, m_deck.Count);
            m_deck[i] = m_deck[randomIndex];
            m_deck[randomIndex] = temp;
        }
        
        Debug.Log("덱 셔플 완료");
    }
    
    // 초기 핸드 드로우
    private void DrawInitialHand()
    {
        for (int i = 0; i < m_initialHandSize; i++)
        {
            DrawCard();
        }
        
        Debug.Log($"초기 {m_initialHandSize}장 드로우 완료");
    }
    
    // 카드 1장 드로우
    public void DrawCard()
    {
        // 덱이 비어있으면 버린 카드 더미를 섞어서 덱으로
        if (m_deck.Count == 0)
        {
            if (m_discardPile.Count == 0)
            {
                Debug.Log("드로우할 카드가 없습니다!");
                return;
            }
            
            ReshuffleDiscardPile();
        }
        
        // 덱의 맨 위 카드 드로우
        CardData drawnCard = m_deck[0];
        m_deck.RemoveAt(0);
        
        // 카드 오브젝트 생성
        CreateCardObject(drawnCard);
        
        Debug.Log($"{drawnCard.m_name} 드로우! (남은 덱: {m_deck.Count}장)");
    }
    
    // 버린 카드 더미를 섞어서 덱으로 만들기
    private void ReshuffleDiscardPile()
    {
        Debug.Log("버린 카드 더미를 섞어 덱으로 만듭니다.");
        m_deck.AddRange(m_discardPile);
        m_discardPile.Clear();
        ShuffleDeck();
    }
    
    // 카드 오브젝트 생성 및 배치
    private void CreateCardObject(CardData cardData)
    {
        if (m_cardPrefab == null)
        {
            Debug.LogWarning("카드 프리팹이 설정되지 않았습니다!");
            return;
        }
        
        // 카드 생성
        GameObject cardObj = Instantiate(m_cardPrefab, m_handTransform);
        WWECard wweCard = cardObj.GetComponent<WWECard>();
        
        if (wweCard != null)
        {
            wweCard.Init(cardData);
            m_hand.Add(wweCard);
            
            // 카드 위치 재배치
            RepositionCards();
        }
        else
        {
            Debug.LogError("WWECard 컴포넌트를 찾을 수 없습니다!");
            Destroy(cardObj);
        }
    }
    
    // 손패의 카드들을 재배치
    private void RepositionCards()
    {
        int cardCount = m_hand.Count;
        float totalWidth = (cardCount - 1) * m_cardSpacing;
        float startX = -totalWidth / 2f;
        
        for (int i = 0; i < cardCount; i++)
        {
            RectTransform cardRect = m_hand[i].GetComponent<RectTransform>();
            if (cardRect != null)
            {
                Vector2 targetPos = new Vector2(startX + (i * m_cardSpacing), m_cardYOffset);
                cardRect.anchoredPosition = targetPos;
            }
        }
    }
    
    // 카드 사용 (손패에서 버린 카드 더미로)
    public void PlayCard(WWECard card, TargetState target)
    {
        if (!m_hand.Contains(card))
        {
            Debug.LogWarning("손패에 없는 카드입니다!");
            return;
        }
        
        // 카드 효과 발동
        card.UseCard(target);
        
        // 손패에서 제거
        m_hand.Remove(card);
        
        // 버린 카드 더미에 추가
        m_discardPile.Add(card.GetCardData);
        
        // 카드 오브젝트 제거
        Destroy(card.gameObject);
        
        // 남은 카드들 재배치
        RepositionCards();
        
        Debug.Log($"{card.GetCardData.m_name} 사용 완료! (버린 카드: {m_discardPile.Count}장)");
    }
    
    // 턴 종료 시 손패 버리기
    public void DiscardHand()
    {
        foreach (WWECard card in m_hand)
        {
            m_discardPile.Add(card.GetCardData);
            Destroy(card.gameObject);
        }
        
        m_hand.Clear();
        Debug.Log("손패 모두 버림");
    }
    
    // 새 턴 시작 (손패 버리고 새로 드로우)
    public void StartNewTurn()
    {
        DiscardHand();
        DrawInitialHand();
        Debug.Log("새 턴 시작!");
    }
    
    // 현재 덱 정보
    public int GetDeckCount() => m_deck.Count;
    public int GetHandCount() => m_hand.Count;
    public int GetDiscardCount() => m_discardPile.Count;
}
