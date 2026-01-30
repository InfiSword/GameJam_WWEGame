using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YaCht_CardSelectScene : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button m_startButton;
    [SerializeField] private GameObject m_selectCardPrefab;
    [SerializeField] private Transform m_ownedCardsParent;
    [SerializeField] private Transform m_swapCardsParent;
    
    [Header("Card Layout Settings")]
    [SerializeField] private float m_ownedCardSpacing = 150f;
    [SerializeField] private float m_swapCardSpacing = 200f;
    [SerializeField] private int m_swapCardCount = 2;
    
    private List<YaCht_WWESelectCard> m_ownedCardObjects = new List<YaCht_WWESelectCard>();
    private List<YaCht_WWESelectCard> m_swapCardObjects = new List<YaCht_WWESelectCard>();
    
    private YaCht_WWESelectCard m_selectedOwnedCard = null;
    private YaCht_WWESelectCard m_selectedSwapCard = null;
    private int m_selectedOwnedIndex = -1;
    
    void Start()
    {
        DisplayOwnedCards();
        GenerateSwapCards();
        
        if (m_startButton != null)
        {
            m_startButton.onClick.AddListener(OnStartButtonClicked);
        }
    }  
    
    private void DisplayOwnedCards()
    {
        ClearCardList(m_ownedCardObjects);
        
        int deckCount = YaCht_GameManager.nowPlayerData.playerDeck.Count;
        for (int i = 0; i < deckCount; i++)
        {
            YaCht_WWESelectCard card = CreateSelectCard(
                YaCht_GameManager.nowPlayerData.playerDeck[i],
                m_ownedCardsParent,
                i,
                deckCount,
                m_ownedCardSpacing
            );
            m_ownedCardObjects.Add(card);
        }
    }
    
    private void GenerateSwapCards()
    {
        ClearCardList(m_swapCardObjects);
        
        List<YaCht_CardData> m_availableCardsPool = YaCht_GameManager.CardManager.GetAvailableCardsPool();
        for (int i = 0; i < m_swapCardCount; i++)
        {            
            YaCht_CardData randomCard = m_availableCardsPool[Random.Range(0, m_availableCardsPool.Count)];
            
            YaCht_WWESelectCard card = CreateSelectCard(
                randomCard,
                m_swapCardsParent,
                i,
                m_swapCardCount,
                m_swapCardSpacing
            );
            m_swapCardObjects.Add(card);
        }
    }
    
    private YaCht_WWESelectCard CreateSelectCard(YaCht_CardData cardData, Transform parent, int index, int totalCount, float spacing)
    {
        GameObject cardObj = Instantiate(m_selectCardPrefab, parent);
        YaCht_WWESelectCard card = cardObj.GetComponent<YaCht_WWESelectCard>();
        card.Init(cardData);
        
        RectTransform rectTransform = cardObj.GetComponent<RectTransform>();
        float centerOffset = (totalCount - 1) * 0.5f;
        float xPos = (index - centerOffset) * spacing;
        rectTransform.anchoredPosition = new Vector2(xPos, 0);
        
        return card;
    }
    
    private void ClearCardList(List<YaCht_WWESelectCard> cardList)
    {
        foreach (var card in cardList)
        {
            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }
        cardList.Clear();
    }
    
    public void OnCardClicked(YaCht_WWESelectCard card)
    {
        int ownedIndex = m_ownedCardObjects.IndexOf(card);
        if (ownedIndex >= 0)
        {
            OnOwnedCardClicked(card, ownedIndex);
            return;
        }
        
        int swapIndex = m_swapCardObjects.IndexOf(card);
        if (swapIndex >= 0)
        {
            OnSwapCardClicked(card);
            return;
        }
    }
    
    private void OnOwnedCardClicked(YaCht_WWESelectCard card, int index)
    {
        if (m_selectedOwnedCard != null)
        {
            m_selectedOwnedCard.SetHighlight(false);
        }
        
        if (m_selectedOwnedCard == card)
        {
            m_selectedOwnedCard = null;
            m_selectedOwnedIndex = -1;
            return;
        }
        
        m_selectedOwnedCard = card;
        m_selectedOwnedIndex = index;
        card.SetHighlight(true);
        
        if (m_selectedSwapCard != null)
        {
            SwapCards();
        }
    }
    
    private void OnSwapCardClicked(YaCht_WWESelectCard card)
    {
        if (m_selectedSwapCard != null)
        {
            m_selectedSwapCard.SetHighlight(false);
        }
        
        if (m_selectedSwapCard == card)
        {
            m_selectedSwapCard = null;
            return;
        }
        
        m_selectedSwapCard = card;
        card.SetHighlight(true);
        
        if (m_selectedOwnedCard != null && m_selectedOwnedIndex >= 0)
        {
            SwapCards();
        }
    }
    
    private void SwapCards()
    {
        if (m_selectedOwnedCard == null || m_selectedSwapCard == null || m_selectedOwnedIndex < 0)
        {
            return;
        }
        
        YaCht_GameManager.nowPlayerData.playerDeck[m_selectedOwnedIndex] = m_selectedSwapCard.GetCardData;
        m_ownedCardObjects[m_selectedOwnedIndex].UpdateCardData(m_selectedSwapCard.GetCardData);
        
        ClearSelection();
        GenerateSwapCards();
        
        Debug.Log($"카드 교환 완료! ({m_selectedSwapCard.GetCardData.m_name})");
    }
    
    private void ClearSelection()
    {
        if (m_selectedOwnedCard != null)
        {
            m_selectedOwnedCard.SetHighlight(false);
        }
        
        if (m_selectedSwapCard != null)
        {
            m_selectedSwapCard.SetHighlight(false);
        }
        
        m_selectedOwnedCard = null;
        m_selectedSwapCard = null;
        m_selectedOwnedIndex = -1;
    }
    
    private void OnStartButtonClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
    
    private void OnDestroy()
    {
        if (m_startButton != null)
        {
            m_startButton.onClick.RemoveListener(OnStartButtonClicked);
        }
    }
}
