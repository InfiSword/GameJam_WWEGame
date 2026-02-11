using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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

    // 생성된 이펙트 오브젝트 추적 리스트
    private List<GameObject> m_activeEffects = new List<GameObject>();

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

        if (wweMainGame.CurrentEnemy.GetShakeCoroutine != null)
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
        YaCht_BGMManager.Instance.PlayCardSound(m_cardData.m_rarity, m_cardData.m_name);
    }

    /// <summary>
    /// 공격 이펙트 실행
    /// 레어도에 따라 다른 이펙트를 적 주변 랜덤 위치에 생성
    /// </summary>
    private void PlayAttackEffect(Vector3 targetPosition)
    {
        YaCht_WWEMainGame wweMainGame = FindFirstObjectByType<YaCht_WWEMainGame>();
        if (wweMainGame == null || wweMainGame.CurrentEnemy == null)
        {
            Debug.LogWarning("[Effect] 적을 찾을 수 없습니다!");
            return;
        }

        Transform enemyTransform = wweMainGame.CurrentEnemy.transform;
        YaCht_CardRarity rarity = m_cardData.m_rarity;

        // 레어도에 따라 이펙트 경로 결정
        string effectPath = "";
        if (rarity == YaCht_CardRarity.S)
        {
            // S급: 강공격
            effectPath = "Sprites/UI/타격 이팩트";
        }
        else if (rarity == YaCht_CardRarity.A || rarity == YaCht_CardRarity.B)
        {
            // A~B급: 중공격
            effectPath = "Sprites/UI/타격 이팩트 중공격";
        }
        else if (rarity == YaCht_CardRarity.C || rarity == YaCht_CardRarity.D)
        {
            // C~D급: 약공격
            effectPath = "Sprites/UI/타격이팩트 약공격";
        }

        if (string.IsNullOrEmpty(effectPath))
        {
            Debug.LogWarning($"[Effect] 레어도 {rarity}에 대한 이펙트 경로가 없습니다!");
            return;
        }

        // 이펙트 스프라이트 로드
        Sprite effectSprite = Resources.Load<Sprite>(effectPath);
        if (effectSprite == null)
        {
            Debug.LogWarning($"[Effect] 이펙트 스프라이트를 찾을 수 없습니다: {effectPath}");
            return;
        }

        // 적 주변 랜덤 위치에 이펙트 생성 (3~5개)
        int effectCount = Random.Range(3, 6);
        for (int i = 0; i < effectCount; i++)
        {
            // 적 위치 주변 랜덤 오프셋 (X: -1.5 ~ 1.5, Y: -1.5 ~ 1.5, Z: 0)
            float randomX = Random.Range(-1.5f, 1.5f);
            float randomY = Random.Range(-1.5f, 1.5f);
            Vector3 randomOffset = new Vector3(randomX, randomY, 0f);
            Vector3 effectPosition = enemyTransform.position + randomOffset;

            // 이펙트 오브젝트 생성
            GameObject effectObj = new GameObject($"AttackEffect_{GetInstanceID()}_{i}");
            effectObj.transform.position = effectPosition;
            effectObj.transform.rotation = Quaternion.identity;

            // SpriteRenderer 추가
            SpriteRenderer spriteRenderer = effectObj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = effectSprite;
            spriteRenderer.sortingOrder = 10; // 적 위에 표시

            // 이펙트 리스트에 추가
            m_activeEffects.Add(effectObj);

            if (wweMainGame != null)
            {
                wweMainGame.RegisterAttackEffect(effectObj);
            }

            // 페이드아웃 코루틴 시작
            StartCoroutine(FadeOutEffect(effectObj, 1.0f));
        }

        Debug.Log($"[Effect] {m_cardData.m_name} ({rarity}): {effectCount}개의 이펙트 생성 - {effectPath}");
    }

    /// <summary>
    /// 이펙트 페이드아웃 코루틴
    /// </summary>
    private IEnumerator FadeOutEffect(GameObject effectObj, float duration)
    {
        if (effectObj == null) yield break;

        SpriteRenderer spriteRenderer = effectObj.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            // 스프라이트 렌더러가 없으면 즉시 제거
            if (effectObj != null)
            {
                m_activeEffects.Remove(effectObj);
                Destroy(effectObj);
            }
            yield break;
        }

        float elapsed = 0f;
        Color originalColor = spriteRenderer.color;
        float maxDuration = duration + 0.5f; // 안전장치: 최대 지속 시간

        while (elapsed < maxDuration)
        {
            if (effectObj == null || spriteRenderer == null) break;

            elapsed += Time.deltaTime;
            
            if (elapsed < duration)
            {
                float t = elapsed / duration;
                // 알파값을 1에서 0으로 감소
                Color newColor = originalColor;
                newColor.a = Mathf.Lerp(1f, 0f, t);
                spriteRenderer.color = newColor;
            }

            yield return null;
        }

        // 완전히 투명해지면 오브젝트 제거
        if (effectObj != null)
        {
            m_activeEffects.Remove(effectObj);
            Destroy(effectObj);
        }
    }

    /// <summary>
    /// 모든 활성 이펙트 정리
    /// </summary>
    private void ClearAllEffects()
    {
        YaCht_WWEMainGame wweMainGame = FindFirstObjectByType<YaCht_WWEMainGame>();
        
        foreach (var effect in m_activeEffects)
        {
            if (effect != null)
            {
                // WWEMainGame에서 이펙트 등록 해제
                if (wweMainGame != null)
                {
                    wweMainGame.UnregisterAttackEffect(effect);
                }
                Destroy(effect);
            }
        }
        m_activeEffects.Clear();
    }

    /// <summary>
    /// 오브젝트 파괴 시 모든 이펙트 정리
    /// </summary>
    private void OnDestroy()
    {
        ClearAllEffects();
    }
}
