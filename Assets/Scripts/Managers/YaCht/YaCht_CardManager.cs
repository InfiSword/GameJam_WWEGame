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
    private float m_cardSpacing = 1.5f;        // 부채꼴 정렬 시 간격 (각도 계산용)
    private float m_handCardYPosition = -1.5f; // 손패 카드의 Y 위치 (부채꼴)
    private float m_firstHandCardYPosition = -0.5f; // 처음 뽑았을 때 카드의 Y 위치 (직선)
    private float m_repositionDuration = 0.3f; // 부채꼴로 정렬될 때 걸리는 시간

    [Header("Machine Deal Settings (기계 연출)")]
    private Transform m_machineSpawnPoint;     // 기계가 위치한 트랜스폼 (오른쪽)
    private AnimationCurve m_dealSpeedCurve;   // X: 0~1(진행도), Y: 딜레이 시간 (점점 느려지게 설정)
    private float m_linearSpacing = 1.2f;      // 처음에 직선으로 깔릴 때의 간격
    private float m_dealFlyDuration = 0.4f;    // 카드가 기계에서 목표지점까지 날아가는 시간
    private float m_waitBeforeFan = 0.8f;      // 다 나온 뒤 부채꼴로 바뀌기 전 대기 시간

    // 기타 변수들
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

    // 현재 애니메이션이나 로직 처리 중인지 확인
    public bool IsProcessing { get; private set; } = false;

    public YaCht_WWECard GetPreviewCard() => m_previewCard;

    // 손패에서 특정 카드 데이터와 일치하는 카드 찾기
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
        m_dealSpeedCurve.AddKey(0f, 0.05f); // 처음엔 빠름
        m_dealSpeedCurve.AddKey(1f, 0.9f);  // 끝엔 느림

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

        // 기존 프리뷰 카드가 있으면 삭제
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

        // 프리뷰 카드가 없으면 다시 초기화 시도
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
            Debug.LogWarning("YaCht_CardManager: 이미 카드 처리 중입니다. 새 라운드 시작 대기 중...");
            yield break;
        }

        ClearAllHandCards();
        yield return new WaitForSeconds(0.2f);

        // 기계 연출로 10장 뽑기
        yield return StartCoroutine(DealCardsFromMachineRoutine(m_initialHandSize));
    }

    // 리롤 (손패만 버리고 다시 뽑기)
    public IEnumerator RerollHand()
    {
        if (IsProcessing)
        {
            Debug.LogWarning("YaCht_CardManager: 이미 카드 처리 중입니다. 리롤 대기 중...");
            yield break;
        }

        int cardsToRedraw = m_hand.Count;
        ClearAllHandCards();

        yield return new WaitForSeconds(0.2f);

        // 기계 연출로 빈 공간만큼 다시 뽑기
        yield return StartCoroutine(DealCardsFromMachineRoutine(cardsToRedraw));
    }

    // 기계 딜링 연출 메인 코루틴
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

                // 시작 위치: 기계 위치 (World Space)
                Vector3 spawnPos = m_machineSpawnPoint.position;
                card.transform.position = spawnPos;
                card.transform.rotation = Quaternion.identity;
                card.gameObject.SetActive(true);

                // 목표 위치: 화면 중앙 기준 직선 정렬 (Z = -1로 설정)
                float targetX = startX + (i * m_linearSpacing);
                Vector3 targetLocalPos = new Vector3(targetX, m_firstHandCardYPosition, -1f);
                Vector3 targetWorldPos = m_handTransform.TransformPoint(targetLocalPos);

                // 기계 -> 목표 위치로 이동
                StartCoroutine(MoveCardToTarget(card.transform, targetWorldPos, Quaternion.identity, m_dealFlyDuration));

                // 딜레이 계산 (AnimationCurve 사용: 갈수록 느려지게)
                float progress = (count > 1) ? (float)i / (count - 1) : 0f;
                float delay = m_dealSpeedCurve.Evaluate(progress);

                if (delay < 0.05f) delay = 0.05f;
                if(i == drawnCards.Count-1)
                    yield return new WaitForSeconds(m_dealFlyDuration);
                else
                    yield return new WaitForSeconds(delay);
            }

            // 다 나온 후 잠시 대기
            yield return new WaitForSeconds(m_waitBeforeFan);

            yield return StartCoroutine(RepositionAllCardsCoroutine());
        }
        IsProcessing = false;
    }

    // 카드 이동 헬퍼 코루틴 (Lerp)
    private IEnumerator MoveCardToTarget(Transform cardTr, Vector3 targetPos, Quaternion targetRot, float duration)
    {
        Vector3 startPos = cardTr.position;
        Quaternion startRot = cardTr.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (cardTr == null) yield break; // 카드가 파괴되었을 경우 방지

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

    // 순수하게 카드 데이터와 오브젝트만 생성하여 반환 (확률 기반)
    private YaCht_WWECard CreateCardObjectOnly()
    {
        if (YaCht_GameManager.nowPlayerData.playerDeck.Count == 0) return null;

        // 등급 기반 확률 드로우
        YaCht_CardData selectedCard;
        if (!TryDrawCardByRarity(out selectedCard))
        {
            // 확률 실패 시 null 반환
            return null;
        }

        GameObject cardObj = Instantiate(m_cardPrefab, m_handTransform);
        YaCht_WWECard wweCard = cardObj.GetComponent<YaCht_WWECard>();

        wweCard.Init(selectedCard);
        wweCard.SetDrawOrderId(m_nextDrawOrderId++);
        cardObj.SetActive(false); // 생성 직후엔 안보이게 (위치 잡고 켤 예정)

        return wweCard;
    }

    // 등급 기반 확률 드로우 시스템 (성공 여부 반환)
    private bool TryDrawCardByRarity(out YaCht_CardData drawnCard)
    {
        drawnCard = default(YaCht_CardData);
        List<YaCht_CardData> m_playerDeck = YaCht_GameManager.nowPlayerData.playerDeck;
        
        if (m_playerDeck.Count == 0)
        {
            Debug.LogError("플레이어 덱이 비어있습니다!");
            return false;
        }

        // 덱에서 랜덤하게 카드 선택
        int randomIndex = Random.Range(0, m_playerDeck.Count);
        YaCht_CardData candidateCard = m_playerDeck[randomIndex];

        // 등급에 따른 확률 체크 (높은 등급일수록 확률이 낮음)
        float rarityChance = GetRarityChance(candidateCard.m_rarity);
        float roll = Random.Range(0f, 100f);

        // 확률 성공 시 해당 카드 반환
        if (roll < rarityChance)
        {
            drawnCard = candidateCard;
            return true;
        }
        
        return false;
    }

    // 등급별 확률 반환 (유물 효과 적용)
    private float GetRarityChance(YaCht_CardRarity rarity)
    {
        float baseChance;
        
        switch (rarity)
        {
            case YaCht_CardRarity.S:
                baseChance = 10f; // 10%
                break;
            case YaCht_CardRarity.A:
                baseChance = 20f; // 20%
                break;
            case YaCht_CardRarity.B:
                baseChance = 30f; // 30%
                break;
            case YaCht_CardRarity.C:
                baseChance = 40f; // 40%
                break;
            case YaCht_CardRarity.D:
                baseChance = 50f; // 50%
                break;
            default:
                baseChance = 50f;
                break;
        }

        // 유물 효과 적용 (도박사의 가면 1, 2)
        return YaCht_GameManager.RelicManager.ModifyRarityChance(rarity, baseChance);
    }

    public void SetupCard(YaCht_WWECard card, int slotIndex)
    {
        if (IsProcessing)
        {
            Debug.LogWarning("YaCht_CardManager: 카드 처리 중에는 셋업을 할 수 없습니다.");
            return;
        }

        if (card == null) return;

        // 야추 방식: 셋업 카드 최대 6장 제한
        if (m_setupCards.Count >= MAX_SETUP_CARDS)
        {
            Debug.Log($"셋업 카드는 최대 {MAX_SETUP_CARDS}장까지만 가능합니다!");
            return;
        }

        if (m_hand.Contains(card))
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

            // 손패 재정렬 - 이 메서드가 카드 위치를 올바르게 설정함
            RepositionHandCards();
        }
    }

    // 현재 조합 정보 가져오기
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

    // 셋업 카드 전부 제거 (라운드 종료 시)
    public void ClearSetupCards()
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

    // 모든 손패 카드 제거
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

    // 부채꼴 정렬 시작 함수
    private void RepositionHandCards()
    {
        if (m_currentRepositionCoroutine != null)
        {
            StopCoroutine(m_currentRepositionCoroutine);
        }
        m_currentRepositionCoroutine = StartCoroutine(RepositionAllCardsCoroutine());
    }

    // 개별 카드 위치/회전값 계산
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

        // 부채꼴 정렬 시 Z값을 0으로 설정
        localPosition = new Vector3(x, m_handCardYPosition, 0f);
        localRotation = Quaternion.Euler(0f, 0f, -angle);
    }

    // 전체 카드 부채꼴 재정렬 코루틴
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

        // 최종 위치 확정
        for (int i = 0; i < m_hand.Count && i < targetPositions.Count; i++)
        {
            if (m_hand[i] == null) continue;

            m_hand[i].transform.localPosition = targetPositions[i];
            m_hand[i].transform.localRotation = targetRotations[i];
        }

        m_currentRepositionCoroutine = null;
    }
}