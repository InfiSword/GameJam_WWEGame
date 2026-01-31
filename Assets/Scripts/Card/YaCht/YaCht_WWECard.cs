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
    public TextMeshProUGUI m_cardRarity;      // 등급 표시 텍스트

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
        
        // 등급 표시 업데이트
        UpdateRarityDisplay();
        
        // 카드 이미지 로드
        LoadCardImage();
        
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
        
        // 등급 표시 업데이트
        UpdateRarityDisplay();
        
        // 카드 이미지 로드
        LoadCardImage();
    }
    
    /// <summary>
    /// 카드 이미지를 Resources에서 로드
    /// </summary>
    private void LoadCardImage()
    {
        if (m_cardImage == null)
        {
            Debug.LogWarning("[Card] Image 컴포넌트가 없습니다!");
            return;
        }

        // 이미지 경로가 지정되어 있으면 로드
        if (!string.IsNullOrEmpty(m_cardData.m_imageResourcePath))
        {
            Sprite loadedSprite = Resources.Load<Sprite>(m_cardData.m_imageResourcePath);

            if (loadedSprite != null)
            {
                m_cardImage.sprite = loadedSprite;
                Debug.Log($"[Card] 이미지 로드 성공: {m_cardData.m_imageResourcePath}");
            }
        }
    }   
    
    /// <summary>
    /// 등급 표시 업데이트
    /// </summary>
    private void UpdateRarityDisplay()
    {
        if (m_cardRarity != null)
        {
            m_cardRarity.text = GetRarityText(m_cardData.m_rarity);
        }         
    }
    
    /// <summary>
    /// 등급 텍스트 반환
    /// </summary>
    private string GetRarityText(YaCht_CardRarity rarity)
    {
        switch (rarity)
        {
            case YaCht_CardRarity.S:
                return "S";
            case YaCht_CardRarity.A:
                return "A";
            case YaCht_CardRarity.B:
                return "B";
            case YaCht_CardRarity.C:
                return "C";
            case YaCht_CardRarity.D:
                return "D";
            default:
                return "?";
        }
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
