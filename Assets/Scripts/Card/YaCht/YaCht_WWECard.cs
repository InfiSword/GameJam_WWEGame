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
    public TextMeshProUGUI m_cardDamage;

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
        
        // 카드 UI 업데이트
        if (m_cardName != null)
            m_cardName.text = m_cardData.m_name;
        
        if (m_cardDescr != null)
            m_cardDescr.text = m_cardData.m_description;
        
        if (m_cardDamage != null)
            m_cardDamage.text = m_cardData.m_baseDamage.ToString();
        
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
        
        if (m_cardName != null)
            m_cardName.text = m_cardData.m_name;
        
        if (m_cardDescr != null)
            m_cardDescr.text = m_cardData.m_description;
        
        if (m_cardDamage != null)
            m_cardDamage.text = m_cardData.m_baseDamage.ToString();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        YaCht_WWEMainGame testGame = FindFirstObjectByType<YaCht_WWEMainGame>();
        if (testGame == null) return;

        // 카드 상태에 따라 적절한 메서드 호출
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
        // 부모가 변경될 때만 m_originalParent 저장 (한 번만!)
        if (!m_isSetup && transform.parent != setupParent)
        {
            m_originalParent = transform.parent;
        }
        
        m_isSetup = true;
        m_setupSlotIndex = slotIndex;
                
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
}
