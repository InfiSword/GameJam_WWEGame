using UnityEngine;
using UnityEngine.EventSystems;

public class WWECard : MonoBehaviour, IPointerClickHandler
{
    private CardData m_cardData;
    public CardData GetCardData => m_cardData;
    public Canvas m_cardCanvas;
    
    private bool m_isSetup = false;
    public bool IsSetup => m_isSetup;
    
    private int m_setupSlotIndex = -1;
    public int SetupSlotIndex => m_setupSlotIndex;
    
    private Transform m_originalParent;
    
    public void Init(CardData _cardData)
    {
        m_cardData = _cardData;
        m_cardCanvas = GetComponentInChildren<Canvas>();
        m_cardCanvas.worldCamera = Camera.main;
    }

    void Update()
    {

    }    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_isSetup)
        {
            // 셋업된 카드를 클릭하면 해제
            CardManager cardManager = GameManager.CardManager;
            if (cardManager != null)
            {
                cardManager.RemoveCardFromSetup(this);
            }
        }
        else
        {
            // 손패의 카드를 클릭하면 셋업
            TestCardGame testGame = FindFirstObjectByType<TestCardGame>();
            if (testGame != null)
            {
                testGame.OnCardClicked(this);
            }
        }
    }
    
    public void SetupCard(Transform setupParent, int slotIndex)
    {
        m_isSetup = true;
        m_setupSlotIndex = slotIndex;        
        m_originalParent = transform.parent;
                
        transform.SetParent(setupParent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
    
    public void ReleaseSetup()
    {
        m_isSetup = false;
        m_setupSlotIndex = -1;
        
        if (m_originalParent != null)
        {
            transform.SetParent(m_originalParent);
        }
    }

    // 카드 사용 시 호출되는 핵심 메서드
    public void UseCard(TargetState target)
    {
        float finalDamage = 0f;
        finalDamage = m_cardData.m_damageCalculator(m_cardData, target);
        target.m_currentHealth -= finalDamage;

        m_cardData.m_abilityTrigger(m_cardData, target);
    }
}
