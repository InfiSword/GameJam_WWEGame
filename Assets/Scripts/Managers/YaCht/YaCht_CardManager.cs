using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

public class YaCht_CardManager : MonoBehaviour
{
    [Header("Basic Settings")]
    private Transform m_handTransform;
    private GameObject m_cardPrefab;

    [Header("Fan Layout Settings")]
    private float m_maxFanAngle = 60f;
    private float m_cardSpacing = 1.5f;        // 손패에서 카드 간 좌우 간격 (팬 배열)
    private float m_handCardYPosition = -1.5f; // 손패에서 카드의 Y 위치 (팬 배열)
    private float m_firstHandCardYPosition = -0.5f; // 첫 카드의 Y 위치 (팬 배열)
    private float m_repositionDuration = 0.3f; // 재배열 시간

    [Header("Machine Deal Settings (카드 배분)")]
    private Transform m_machineSpawnPoint;     // 기계 위치
    private AnimationCurve m_dealSpeedCurve;   // X: 0~1(왼쪽에서 오른쪽으로), Y: 배분 시간 (왼쪽에서 오른쪽으로 배분)
    private float m_linearSpacing = 1.2f;      // 왼쪽에서 오른쪽으로 배분 시간
    private float m_dealFlyDuration = 0.4f;    // 카드 이동 시간
    private float m_waitBeforeFan = 0.8f;      // 팬 배열 전 대기 시간

    // 미리보기 카드 위치
    private Vector3 m_previewCardPosition = new Vector3(0f, 0f, -2f);
    private float m_previewCardScale = 3.0f;

    private List<YaCht_WWECard> m_hand = new List<YaCht_WWECard>();
    private List<YaCht_WWECard> m_setupCards = new List<YaCht_WWECard>();

    private int m_initialHandSize = 10;
    private const int MAX_SETUP_CARDS = 6;
    private int m_nextDrawOrderId = 0;

    private Coroutine m_currentRepositionCoroutine = null;

    private List<YaCht_CardData> m_availableCardsPool = new List<YaCht_CardData>();
    private YaCht_WWECard m_previewCard;

    // 처리 중인지 여부
    public bool IsProcessing { get; private set; } = false;

    public YaCht_WWECard GetPreviewCard() => m_previewCard;

    // 손패에서 카드 데이터를 찾는 메서드
    public YaCht_WWECard FindCardInHand(YaCht_CardData cardData)
    {
        foreach (var card in m_hand)
        {
            if (card.GetCardData.m_name == cardData.m_name)
            {
                return card;
            }
        }
        return null;
    }

    public void Init()
    {
        m_cardPrefab = Resources.Load<GameObject>("Prefabs/Card");
        InitializeAvailableCardsPool();
        InitializePreviewCard();


        m_dealSpeedCurve = new AnimationCurve();
        m_dealSpeedCurve.AddKey(0f, 0.05f); 
        m_dealSpeedCurve.AddKey(1f, 0.9f);  

    }

    private void InitializeAvailableCardsPool()
    {
        m_availableCardsPool.Clear();
        YaCht_CardData[] allCards = YaCht_CardDatabase.GetAllCards();
        m_availableCardsPool.AddRange(allCards);
    }

    private void InitializePreviewCard()
    {
        if (m_cardPrefab == null)
        {
            Debug.LogError("[CardManager] 카드 프리팹이 없습니다!");
            return;
        }

        if (YaCht_GameManager.nowPlayerData.playerDeck.Count == 0)
        {
            return;
        }

        // 미리보기 카드 초기화
        if (m_previewCard != null)
        {
            Destroy(m_previewCard.gameObject);
        }

        GameObject previewObj = Instantiate(m_cardPrefab);
        m_previewCard = previewObj.GetComponent<YaCht_WWECard>();

        if (m_previewCard != null)
        {
            m_previewCard.Init(YaCht_GameManager.nowPlayerData.playerDeck[0], isPreviewCard: true);
            previewObj.transform.position = m_previewCardPosition;
            previewObj.transform.localScale = Vector3.one * m_previewCardScale;
            previewObj.transform.rotation = Quaternion.identity;
            previewObj.gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        if (m_handTransform == null)
        {
            GameObject handObj = GameObject.Find("HandCardTransform");
            if (handObj) m_handTransform = handObj.transform;
        }

        if (m_machineSpawnPoint == null)
        {
            GameObject machineObj = GameObject.Find("CardMachineSpawnPoint");
            if (machineObj) m_machineSpawnPoint = machineObj.transform;
        }

        // 미리보기 카드 초기화
        if (m_previewCard == null && YaCht_GameManager.nowPlayerData.playerDeck.Count > 0)
        {
            InitializePreviewCard();
        }

        StartCoroutine(StartNewRound());
    }

    // 새 라운드 시작
    public IEnumerator StartNewRound()
    {
        if (IsProcessing)
        {
            Debug.LogWarning("YaCht_CardManager: 처리 중입니다. 새 라운드 시작 불가");
            yield break;
        }

        ClearAllHandCards();
        yield return new WaitForSeconds(0.2f);

        // 기계에서 카드 배분
        yield return StartCoroutine(DealCardsFromMachineRoutine(m_initialHandSize));
    }

    // 손패 재배열
    public IEnumerator RerollHand()
    {
        if (IsProcessing)
        {
            Debug.LogWarning("YaCht_CardManager: 처리 중입니다. 손패 재배열 불가");
            yield break;
        }

        int cardsToRedraw = m_hand.Count;
        
        // 리롤 전에 A랭크 카드 개수 계산 (IHateS 효과용)
        int aCardCount = 0;
        foreach (var card in m_hand)
        {
            if (card != null && card.GetCardData.m_rarity == YaCht_CardRarity.A)
            {
                aCardCount++;
            }
        }
        
        // IHateS 효과 적용
        if (aCardCount > 0)
        {
            YaCht_GameManager.RelicManager.OnRerollWithACards(aCardCount);
        }

        // JjolBoy 효과 적용
        YaCht_GameManager.RelicManager.OnReroll();

        ClearAllHandCards();

        yield return new WaitForSeconds(0.2f);

        // 손패 재배열
        yield return StartCoroutine(DealCardsFromMachineRoutine(cardsToRedraw));
    }

    // 카드 배분
    private IEnumerator DealCardsFromMachineRoutine(int count)
    {
        IsProcessing = true;

        if (m_currentRepositionCoroutine != null)
            StopCoroutine(m_currentRepositionCoroutine);

        if (count > 0)
        {
            float totalLinearWidth = (count - 1) * m_linearSpacing;
            float startX = -totalLinearWidth * 0.5f;

            List<YaCht_WWECard> drawnCards = new List<YaCht_WWECard>();
            
            for (int i = 0; i < count; i++)
            {
                YaCht_WWECard card = null;
                
                while (card == null)
                {
                    card = CreateCardObjectOnly();
                }
                
                drawnCards.Add(card);
                m_hand.Add(card);
            }

            for (int i = 0; i < drawnCards.Count; i++)
            {
                YaCht_WWECard card = drawnCards[i];

                // 기계 위치: 월드 스페이스
                Vector3 spawnPos = m_machineSpawnPoint.position;
                card.transform.position = spawnPos;
                card.transform.rotation = Quaternion.identity;
                card.gameObject.SetActive(true);

                // 목표 위치: 왼쪽에서 오른쪽으로 배분 (Z = -1)
                float targetX = startX + (i * m_linearSpacing);
                Vector3 targetLocalPos = new Vector3(targetX, m_firstHandCardYPosition, -1f);
                Vector3 targetWorldPos = m_handTransform.TransformPoint(targetLocalPos);

                // 손패 -> 목표 위치로 이동
                StartCoroutine(MoveCardToTarget(card.transform, targetWorldPos, Quaternion.identity, m_dealFlyDuration));

                // 배분 시간 (AnimationCurve: 배분 시간)
                float progress = (count > 1) ? (float)i / (count - 1) : 0f;
                float delay = m_dealSpeedCurve.Evaluate(progress);

                if (delay < 0.05f) delay = 0.05f;
                if(i == drawnCards.Count-1)
                    yield return new WaitForSeconds(m_dealFlyDuration);
                else
                    yield return new WaitForSeconds(delay);
            }

            // 팬 배열 전 대기 시간
            yield return new WaitForSeconds(m_waitBeforeFan);

            yield return StartCoroutine(RepositionAllCardsCoroutine());
        }
        IsProcessing = false;
    }

    // 카드 이동 (Lerp)
    private IEnumerator MoveCardToTarget(Transform cardTr, Vector3 targetPos, Quaternion targetRot, float duration)
    {
        Vector3 startPos = cardTr.position;
        Quaternion startRot = cardTr.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (cardTr == null) yield break; // 카드가 없으면 종료

            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float easeT = t * (2 - t);

            cardTr.position = Vector3.Lerp(startPos, targetPos, easeT);
            cardTr.rotation = Quaternion.Lerp(startRot, targetRot, easeT);
            yield return null;
        }

        if (cardTr != null)
        {
            cardTr.position = targetPos;
            cardTr.rotation = targetRot;
        }
    }

    // 카드 생성 (카드 데이터만 사용)
    private YaCht_WWECard CreateCardObjectOnly()
    {
        if (YaCht_GameManager.nowPlayerData.playerDeck.Count == 0) return null;

        // 카드 데이터 선택
        YaCht_CardData selectedCard;
        if (!TryDrawCardByRarity(out selectedCard))
        {
            // 카드 데이터 선택 실패
            return null;
        }

        GameObject cardObj = Instantiate(m_cardPrefab, m_handTransform);
        YaCht_WWECard wweCard = cardObj.GetComponent<YaCht_WWECard>();

        wweCard.Init(selectedCard);
        wweCard.SetDrawOrderId(m_nextDrawOrderId++);
        cardObj.SetActive(false); // 카드 비활성화

        return wweCard;
    }

    /// <summary>
    /// FixedMask용 고정 카드 생성 (덱에서 직접 생성)
    /// </summary>
    public YaCht_WWECard CreateFixedMaskCard(YaCht_CardData cardData)
    {
        if (m_cardPrefab == null)
        {
            Debug.LogError("[CardManager] 카드 프리팹이 없습니다!");
            return null;
        }

        GameObject cardObj = Instantiate(m_cardPrefab);
        YaCht_WWECard wweCard = cardObj.GetComponent<YaCht_WWECard>();

        if (wweCard != null)
        {
            wweCard.Init(cardData);
            wweCard.SetFixedCard(true); // 고정 카드로 설정
            wweCard.SetDrawOrderId(m_nextDrawOrderId++);
            cardObj.SetActive(true);
        }

        return wweCard;
    }

    // 카드 데이터 선택 (카드 데이터만 사용)
    private bool TryDrawCardByRarity(out YaCht_CardData drawnCard)
    {
        drawnCard = default(YaCht_CardData);
        List<YaCht_CardData> m_playerDeck = YaCht_GameManager.nowPlayerData.playerDeck;
        
        if (m_playerDeck.Count == 0)
        {
            Debug.LogError("플레이어 덱이 없습니다!");
            return false;
        }

        // 카드 데이터 선택
        int randomIndex = Random.Range(0, m_playerDeck.Count);
        YaCht_CardData candidateCard = m_playerDeck[randomIndex];

        // 카드 레어도 추천
        float rarityChance = GetRarityChance(candidateCard.m_rarity);
        float roll = Random.Range(0f, 100f);

        // 카드 레어도 추천 성공
        if (roll < rarityChance)
        {
            drawnCard = candidateCard;
            
            // IHateS 효과: S랭크 카드 획득 시 중첩 초기화
            if (drawnCard.m_rarity == YaCht_CardRarity.S)
            {
                YaCht_GameManager.RelicManager.OnSRankCardObtained();
            }
            
            return true;
        }
        
        return false;
    }

    // 카드 레어도 추천 성공
    private float GetRarityChance(YaCht_CardRarity rarity)
    {
        float baseChance;
        
        switch (rarity)
        {
            case YaCht_CardRarity.S:
                baseChance = 10f; // S 레어도 10%
                break;
            case YaCht_CardRarity.A:
                baseChance = 20f; // A 레어도 20%
                break;
            case YaCht_CardRarity.B:
                baseChance = 30f; // B 레어도 30%
                break;
            case YaCht_CardRarity.C:
                baseChance = 40f; // C 레어도 40%
                break;
            case YaCht_CardRarity.D:
                baseChance = 50f; // D 레어도 50%
                break;
            default:
                baseChance = 50f;
                break;
        }

        // 카드 레어도 추천 성공
        return YaCht_GameManager.RelicManager.ModifyRarityChance(rarity, baseChance);
    }

    public void SetupCard(YaCht_WWECard card, int slotIndex)
    {
        if (IsProcessing)
        {
            Debug.LogWarning("YaCht_CardManager: 처리 중입니다. 카드 설정 불가");
            return;
        }

        if (card == null) return;

        // 카드 설정 최대 개수 초과
        if (m_setupCards.Count >= MAX_SETUP_CARDS)
        {
            Debug.Log($"카드 설정 최대 개수 초과: {MAX_SETUP_CARDS}개");
            return;
        }

        // 고정 카드는 손패에 없으므로 바로 셋업에 추가
        if (card.IsFixedCard)
        {
            if (!m_setupCards.Contains(card))
            {
                m_setupCards.Add(card);
            }
        }
        else if (m_hand.Contains(card))
        {
            m_hand.Remove(card);
            m_setupCards.Add(card);

            RepositionHandCards();
        }
    }

    public void ReleaseCardFromSetup(YaCht_WWECard card)
    {
        if (IsProcessing)
        {
            return;
        }

        if (card == null) return;

        // 고정 카드는 손패로 돌아가지 않음
        if (card.IsFixedCard)
        {
            Debug.LogWarning("[CardManager] 고정 카드는 손패로 돌아갈 수 없습니다.");
            return;
        }

        if (m_setupCards.Contains(card))
        {
            m_setupCards.Remove(card);

            int insertIndex = m_hand.Count;
            for (int i = 0; i < m_hand.Count; i++)
            {
                if (m_hand[i].DrawOrderId > card.DrawOrderId)
                {
                    insertIndex = i;
                    break;
                }
            }

            m_hand.Insert(insertIndex, card);

            // 카드 재배열
            RepositionHandCards();
        }
    }

    // 현재 콤보 정보 가져오기
    public string GetCurrentComboInfo()
    {
        List<YaCht_CardData> setupCardData = new List<YaCht_CardData>();
        foreach (var card in m_setupCards)
        {
            setupCardData.Add(card.GetCardData);
        }

        YaCht_WrestlerType wrestlerType = YaCht_GameManager.nowPlayerData.wrestlerType;
        return YaCht_ComboChecker.GetComboInfo(setupCardData, wrestlerType);
    }

    // 카드 설정 초기화 (고정 카드는 제외)
    public void ClearSetupCards()
    {
        List<YaCht_WWECard> cardsToRemove = new List<YaCht_WWECard>();
        
        foreach (var card in m_setupCards)
        {
            // 고정 카드는 유지
            if (card != null && !card.IsFixedCard)
            {
                cardsToRemove.Add(card);
                Destroy(card.gameObject);
            }
        }
        
        foreach (var card in cardsToRemove)
        {
            m_setupCards.Remove(card);
        }
    }
    
    // 모든 셋업 카드 초기화 (고정 카드 포함)
    public void ClearAllSetupCards()
    {
        foreach (var card in m_setupCards)
        {
            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }
        m_setupCards.Clear();
    }

    // 손패 초기화
    public void ClearAllHandCards()
    {
        foreach (var card in m_hand)
        {
            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }
        m_hand.Clear();
    }

    // 손패 재배열
    private void RepositionHandCards()
    {
        if (m_currentRepositionCoroutine != null)
        {
            StopCoroutine(m_currentRepositionCoroutine);
        }
        m_currentRepositionCoroutine = StartCoroutine(RepositionAllCardsCoroutine());
    }

    // 카드 위치 계산
    private void CalculateCardTransform(int index, int totalCardCount, out Vector3 localPosition, out Quaternion localRotation)
    {
        float normalizedIndex = 0f;
        if (totalCardCount > 1)
        {
            normalizedIndex = (float)index / (totalCardCount - 1) - 0.5f;
            normalizedIndex *= 2f;
        }

        float angle = normalizedIndex * (m_maxFanAngle * 0.5f);

        float totalWidth = (totalCardCount - 1) * m_cardSpacing;
        float x = index * m_cardSpacing - totalWidth * 0.5f;

        // 손패 재배열 Z 위치 0
        localPosition = new Vector3(x, m_handCardYPosition, 0f);
        localRotation = Quaternion.Euler(0f, 0f, -angle);
    }

    // 카드 재배열
    private IEnumerator RepositionAllCardsCoroutine()
    {
        int totalCards = m_hand.Count;

        List<Vector3> startPositions = new List<Vector3>();
        List<Quaternion> startRotations = new List<Quaternion>();
        List<Vector3> targetPositions = new List<Vector3>();
        List<Quaternion> targetRotations = new List<Quaternion>();

        for (int i = 0; i < totalCards; i++)
        {
            if (m_hand[i] == null) continue;

            startPositions.Add(m_hand[i].transform.localPosition);
            startRotations.Add(m_hand[i].transform.localRotation);

            Vector3 targetPos;
            Quaternion targetRot;
            CalculateCardTransform(i, totalCards, out targetPos, out targetRot);
            targetPositions.Add(targetPos);
            targetRotations.Add(targetRot);
        }

        float elapsedTime = 0f;

        while (elapsedTime < m_repositionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / m_repositionDuration;
            float easedT = 1f - (1f - t) * (1f - t);

            for (int i = 0; i < m_hand.Count && i < startPositions.Count; i++)
            {
                if (m_hand[i] == null) continue;

                m_hand[i].transform.localPosition = Vector3.Lerp(startPositions[i], targetPositions[i], easedT);
                m_hand[i].transform.localRotation = Quaternion.Lerp(startRotations[i], targetRotations[i], easedT);
            }

            yield return null;
        }

        // 카드 재배열 완료
        for (int i = 0; i < m_hand.Count && i < targetPositions.Count; i++)
        {
            if (m_hand[i] == null) continue;

            m_hand[i].transform.localPosition = targetPositions[i];
            m_hand[i].transform.localRotation = targetRotations[i];
        }

        m_currentRepositionCoroutine = null;
    }
}