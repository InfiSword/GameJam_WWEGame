using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class YaCht_WWECard : MonoBehaviour, IPointerClickHandler
{
    private YaCht_CardData m_cardData;
    public YaCht_CardData GetCardData => m_cardData;
    
    public Canvas m_cardCanvas;
    public Image m_cardImage;
    public TextMeshProUGUI m_cardDescr;    
    public TextMeshProUGUI m_cardName;
    public TextMeshProUGUI m_cardCost;

    private bool m_isPreviewCard = false;
    public bool IsPreviewCard => m_isPreviewCard;

    private bool m_isSetup = false;
    public bool IsSetup => m_isSetup;
    
    private int m_setupSlotIndex = -1;
    public int SetupSlotIndex => m_setupSlotIndex;
    
    private int m_drawOrderId = -1;
    public int DrawOrderId => m_drawOrderId;
    
    private Transform m_originalParent;
    
    public void Init(YaCht_CardData _cardData, bool isPreviewCard = false)
    {
        m_cardData = _cardData;        
        m_cardCanvas.worldCamera = Camera.main;
        m_cardDescr.text = m_cardData.m_description;
        //m_cardName.text = m_cardData.m_name;
        //m_cardCost.text = m_cardData.m_cost.ToString();
        m_isPreviewCard = isPreviewCard;
        
        if (m_isPreviewCard)
        {
            m_cardCanvas.sortingOrder = 100;
        }
    }

    public void SetDrawOrderId(int orderId)
    {
        m_drawOrderId = orderId;
    }

    public void UpdateCardData(YaCht_CardData _cardData)
    {
        m_cardData = _cardData;
        m_cardDescr.text = m_cardData.m_description;
        //m_cardName.text = m_cardData.m_name;
        //m_cardCost.text = m_cardData.m_cost.ToString();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        YaCht_WWEMainGame testGame = FindFirstObjectByType<YaCht_WWEMainGame>();
        if (testGame == null) return;

        if (m_isPreviewCard)
        {
            testGame.OnPreviewCardClicked();
        }
        else if (m_isSetup)
        {
            testGame.RemoveTopCardFromSlot(m_setupSlotIndex);
        }
        else
        {
            testGame.OnHandCardClicked(this);
        }
    }
    
    public void SetupCard(Transform setupParent, int slotIndex, Vector3 offset)
    {
        m_isSetup = true;
        m_setupSlotIndex = slotIndex;
        m_originalParent = transform.parent;
                
        transform.SetParent(setupParent);
        transform.localPosition = offset;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        transform.SetAsLastSibling();
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

    public void UseCard(YaCht_TargetState target)
    {
        float finalDamage = m_cardData.m_damageCalculator(m_cardData, target);
        target.m_currentHealth -= finalDamage;
        m_cardData.m_abilityTrigger(m_cardData, target);
    }
}
