using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

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

    /// <summary>
    /// 카드가 적을 향해 돌진하며 타격하는 연출
    /// </summary>
    public IEnumerator AttackEnemyCoroutine(Transform enemyPosition, float attackDuration = 0.5f)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = enemyPosition.position;
        
        YaCht_CardAttackSettings attackSettings = m_cardData.m_attackSettings;

        // 차징 단계
        if (attackSettings.chargeDuration > 0)
        {
            yield return StartCoroutine(ChargeMovement(startPosition, attackSettings, targetPosition));
        }

        // 차징 후 딜레이
        if (attackSettings.chargeDelay > 0)
        {
            yield return new WaitForSeconds(attackSettings.chargeDelay);
        }

        // 돌진 단계 - 사운드와 이펙트 타이밍 포함
        yield return StartCoroutine(AttackMovementWithEffects(startPosition, targetPosition, attackSettings));       
    }

    /// <summary>
    /// 차징 무브먼트 (적과 반대 방향으로 뒤로 물러나는 효과)
    /// </summary>
    private IEnumerator ChargeMovement(Vector3 originalPosition, YaCht_CardAttackSettings settings, Vector3 targetPosition)
    {
        // 적 방향 계산
        Vector3 directionToEnemy = (targetPosition - originalPosition).normalized;
        // 적과 반대 방향
        Vector3 chargeBackDirection = -directionToEnemy;
        // 차징 위치 계산 (반대 방향으로 이동)
        Vector3 chargeBackPosition = originalPosition + chargeBackDirection * settings.chargeMoveDistance;
        
        float elapsed = 0f;

        while (elapsed < settings.chargeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / settings.chargeDuration;
            
            // 부드러운 이동
            float easeT = t * t;
            transform.position = Vector3.Lerp(originalPosition, chargeBackPosition, easeT);
            
            yield return null;
        }

        transform.position = chargeBackPosition;
    }

    /// <summary>
    /// 돌진 무브먼트 + 사운드/이펙트 타이밍
    /// </summary>
    private IEnumerator AttackMovementWithEffects(Vector3 startPosition, Vector3 targetPosition, 
        YaCht_CardAttackSettings settings)
    {
        float elapsed = 0f;
        bool soundPlayed = false;
        bool effectPlayed = false;

        while (elapsed < settings.attackDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / settings.attackDuration;
            
            // 사운드 타이밍
            if (!soundPlayed && t >= settings.soundTriggerTime)
            {
                PlayAttackSound();
                soundPlayed = true;
            }

            // 이펙트 타이밍
            if (!effectPlayed && t >= settings.effectTriggerTime)
            {
                PlayAttackEffect(targetPosition);
                effectPlayed = true;
            }
            
            // Ease-out 곡선으로 자연스러운 움직임
            float easeT = 1f - Mathf.Pow(1f - t, 3f);
            transform.position = Vector3.Lerp(startPosition, targetPosition, easeT);
            
            yield return null;
        }

        // 최종 위치 확정
        transform.position = targetPosition;

        // 아직 사운드/이펙트가 재생되지 않았으면 마지막에 실행
        if (!soundPlayed)
        {
            PlayAttackSound();
        }
        if (!effectPlayed)
        {
            PlayAttackEffect(targetPosition);
        }

        m_cardImage.enabled = false;
    }

    /// <summary>
    /// 카드 사운드 재생
    /// </summary>
    private void PlayAttackSound()
    {
        // 게임의 효과음 시스템이 있다면 여기에 통합
        if (!string.IsNullOrEmpty(m_cardData.m_soundResourcePath))
        {
            Debug.Log($"[Sound] {m_cardData.m_name}: {m_cardData.m_soundResourcePath} 재생");
            // AudioClip soundClip = Resources.Load<AudioClip>(m_cardData.m_soundResourcePath);
            // if (soundClip != null)
            // {
            //     AudioSource audioSource = GetComponent<AudioSource>();
            //     if (audioSource != null)
            //     {
            //         audioSource.PlayOneShot(soundClip);
            //     }
            // }
        }
    }

    /// <summary>
    /// 카드 이펙트 재생
    /// </summary>
    private void PlayAttackEffect(Vector3 targetPosition)
    {
        // 게임의 이펙트 시스템이 있다면 여기에 통합
        Debug.Log($"[Effect] {m_cardData.m_name}: 이펙트 재생 - 적 위치: {targetPosition}");
        // EffectManager.PlayEffect("CardAttackEffect", targetPosition);
    }
}
