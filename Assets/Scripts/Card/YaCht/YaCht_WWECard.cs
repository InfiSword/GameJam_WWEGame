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
    public TextMeshProUGUI m_cardRarity;      // 카드 레어도 표시

    private bool m_isPreviewCard = false;
    public bool IsPreviewCard => m_isPreviewCard;

    private bool m_isSetup = false;
    public bool IsSetup => m_isSetup;

    private bool m_isFixedCard = false; // FixedMask로 생성된 고정 카드인지 여부
    public bool IsFixedCard => m_isFixedCard;

    private int m_setupSlotIndex = -1;
    public int SetupSlotIndex => m_setupSlotIndex;

    private int m_drawOrderId = -1;
    public int DrawOrderId => m_drawOrderId;

    private Transform m_originalParent;

    public void Init(YaCht_CardData _cardData, bool isPreviewCard = false)
    {
        m_cardData = _cardData;
        m_cardCanvas.worldCamera = Camera.main;

        // 카드 레어도 표시
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

        UpdateRarityDisplay();

        LoadCardImage();
    }

    private void LoadCardImage()
    {
        if (m_cardImage == null)
        {
            Debug.LogWarning("[Card] Image 컴포넌트가 존재하지 않습니다!");
            return;
        }

        if (!string.IsNullOrEmpty(m_cardData.m_imageResourcePath))
        {
            Sprite loadedSprite = Resources.Load<Sprite>(m_cardData.m_imageResourcePath);

            if (loadedSprite != null)
            {
                m_cardImage.sprite = loadedSprite;
                Debug.Log($"[Card] 카드 이미지 로드: {m_cardData.m_imageResourcePath}");
            }
        }
    }


    private void UpdateRarityDisplay()
    {
        if (m_cardRarity != null)
        {
            m_cardRarity.text = GetRarityText(m_cardData.m_rarity);
            m_cardRarity.color = GetRarityColor(m_cardData.m_rarity);
        }
    }

    /// <summary>
    /// 레어도에 따른 텍스트 반환
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

    /// <summary>
    /// 레어도에 따른 색상 반환
    /// </summary>
    private Color GetRarityColor(YaCht_CardRarity rarity)
    {
        switch (rarity)
        {
            case YaCht_CardRarity.S:
                return Color.yellow;
            case YaCht_CardRarity.A:
                return Color.red;
            case YaCht_CardRarity.B:
                return Color.skyBlue;
            case YaCht_CardRarity.C:
                return Color.green;
            case YaCht_CardRarity.D:
                return Color.grey;
            default:
                return Color.white;
        }
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
            // 고정 카드는 제거할 수 없음
            if (!m_isFixedCard)
            {
                testGame.RemoveTopCardFromSlot(m_setupSlotIndex);
            }
        }
        else
        {
            testGame.OnHandCardClicked(this);
        }
    }

    public void SetupCard(Transform setupParent, int slotIndex, Vector3 offset)
    {
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
        
        // 고정 카드 하이라이트 업데이트
        UpdateFixedCardHighlight();
    }
    
    /// <summary>
    /// 고정 카드로 설정
    /// </summary>
    public void SetFixedCard(bool isFixed)
    {
        m_isFixedCard = isFixed;
        UpdateFixedCardHighlight();
    }
    
    /// <summary>
    /// 고정 카드 하이라이트 업데이트
    /// </summary>
    private void UpdateFixedCardHighlight()
    {
        if (m_cardImage != null)
        {
            // 고정 카드는 노란색 하이라이트 적용
            if (m_isFixedCard && m_isSetup)
            {
                m_cardImage.color = new Color(1f, 1f, 0.7f, 1f); // 노란색 하이라이트
            }
            else
            {
                m_cardImage.color = Color.white; // 기본 색상
            }
        }
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
    /// 적 공격 코루틴 실행
    /// </summary>
    public IEnumerator AttackEnemyCoroutine(Transform enemyPosition, float attackDuration = 0.5f)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = enemyPosition.position;

        YaCht_CardAttackSettings attackSettings = m_cardData.m_attackSettings;

        // 차징 이동 실행
        if (attackSettings.chargeDuration > 0)
        {
            yield return StartCoroutine(ChargeMovement(startPosition, attackSettings, targetPosition));
        }

        // 차징 딜레이 실행
        if (attackSettings.chargeDelay > 0)
        {
            yield return new WaitForSeconds(attackSettings.chargeDelay);
        }

        // 공격 이동 실행
        yield return StartCoroutine(AttackMovementWithEffects(startPosition, targetPosition, attackSettings));
    }

    /// <summary>
    /// 차징 이동 (차징 이동 거리만큼 뒤로 이동)
    /// </summary>
    private IEnumerator ChargeMovement(Vector3 originalPosition, YaCht_CardAttackSettings settings, Vector3 targetPosition)
    {
        // 적 방향 계산
        Vector3 directionToEnemy = (targetPosition - originalPosition).normalized;
        // 차징 이동 방향 계산
        Vector3 chargeBackDirection = -directionToEnemy;
        // 차징 이동 위치 계산
        Vector3 chargeBackPosition = originalPosition + chargeBackDirection * settings.chargeMoveDistance;

        float elapsed = 0f;

        while (elapsed < settings.chargeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / settings.chargeDuration;

            // 이동 속도 계산
            float easeT = t * t;
            transform.position = Vector3.Lerp(originalPosition, chargeBackPosition, easeT);

            yield return null;
        }

        transform.position = chargeBackPosition;
    }

    /// <summary>
    /// 공격 이동 + 공격 이펙트 실행
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

            // 공격 사운드 실행
            if (!soundPlayed && t >= settings.soundTriggerTime)
            {
                PlayAttackSound();
                soundPlayed = true;
            }

            // 공격 이펙트 실행
            if (!effectPlayed && t >= settings.effectTriggerTime)
            {
                PlayAttackEffect(targetPosition);
                effectPlayed = true;
            }

            // Ease-out 곡선 계산
            float easeT = 1f - Mathf.Pow(1f - t, 3f);
            transform.position = Vector3.Lerp(startPosition, targetPosition, easeT);

            yield return null;
        }

        // 공격 위치 설정
        transform.position = targetPosition;

        // 공격 사운드 실행
        if (!soundPlayed)
        {
            PlayAttackSound();
        }
        if (!effectPlayed)
        {
            PlayAttackEffect(targetPosition);
        }

        YaCht_WWEMainGame wweMainGame = FindFirstObjectByType<YaCht_WWEMainGame>();

        if(wweMainGame.CurrentEnemy.GetShakeCoroutine != null)
            wweMainGame.CurrentEnemy.StopCoroutine(wweMainGame.CurrentEnemy.GetShakeCoroutine);
        if (wweMainGame.CurrentEnemy.GetFlashCoroutine != null)
            wweMainGame.CurrentEnemy.StopCoroutine(wweMainGame.CurrentEnemy.GetFlashCoroutine);
        
        StartCoroutine(wweMainGame.CurrentEnemy.ShakeCoroutine());
        StartCoroutine(wweMainGame.CurrentEnemy.FlashCoroutine());

        m_cardImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// 공격 사운드 실행
    /// </summary>
    private void PlayAttackSound()
    {
        // 공격 사운드 리소스 경로가 비어있지 않으면 실행
        if (!string.IsNullOrEmpty(m_cardData.m_soundResourcePath))
        {
            Debug.Log($"[Sound] {m_cardData.m_name}: {m_cardData.m_soundResourcePath} 실행");
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
    /// 공격 이펙트 실행
    /// </summary>
    private void PlayAttackEffect(Vector3 targetPosition)
    {
        // 공격 이펙트 리소스 경로가 비어있지 않으면 실행
        Debug.Log($"[Effect] {m_cardData.m_name}: 공격 이펙트 실행 - 공격 위치: {targetPosition}");
        // EffectManager.PlayEffect("CardAttackEffect", targetPosition);
    }
}
