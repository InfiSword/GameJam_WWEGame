using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WWESelectCard : MonoBehaviour, IPointerClickHandler
{
    private CardData m_cardData;
    public CardData GetCardData => m_cardData;
    
    [SerializeField] private Image m_cardImage;
    [SerializeField] private TextMeshProUGUI m_cardDescr;    
    [SerializeField] private TextMeshProUGUI m_cardName;
    [SerializeField] private TextMeshProUGUI m_cardCost;
    
    private Color m_originalColor = Color.white;
    private bool m_isHighlighted = false;
    
    public void Init(CardData cardData)
    {
        m_cardData = cardData;
        UpdateCardUI();
        
        if (m_cardImage != null)
        {
            m_originalColor = m_cardImage.color;
        }
    }
    
    public void UpdateCardData(CardData cardData)
    {
        m_cardData = cardData;
        UpdateCardUI();
    }
    
    private void UpdateCardUI()
    {
        if (m_cardDescr != null)
        {
            m_cardDescr.text = m_cardData.m_description;
        }
        
        if (m_cardName != null)
        {
            m_cardName.text = m_cardData.m_name;
        }
        
        if (m_cardCost != null)
        {
            m_cardCost.text = m_cardData.m_cost.ToString();
        }
    }
    
    public void SetHighlight(bool highlight)
    {
        if (m_cardImage == null) return;
        
        m_isHighlighted = highlight;
        m_cardImage.color = highlight ? Color.yellow : m_originalColor;
    }
    
    public bool IsHighlighted() => m_isHighlighted;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        CardSelectScene selectScene = FindFirstObjectByType<CardSelectScene>();
        if (selectScene != null)
        {
            selectScene.OnCardClicked(this);
        }
    }
}
