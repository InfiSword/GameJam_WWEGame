using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    private GameObject m_cardPrefab;

    [SerializeField] private Transform m_handTransform;

    [SerializeField] private float m_maxFanAngle = 60f;
    [SerializeField] private float m_cardSpacing = 1.5f;
    [SerializeField] private int m_maxHandSize = 10;

    [SerializeField] private Vector3 m_drawStartOffset = new Vector3(0f, -5f, 0f);
    [SerializeField] private float m_repositionDuration = 0.3f;

    [SerializeField] private Vector3 m_previewCardPosition = new Vector3(0f, 0f, 0f);
    [SerializeField] private float m_previewCardScale = 3.0f;

    private List<WWECard> m_hand = new List<WWECard>();
    private List<WWECard> m_setupCards = new List<WWECard>();

    private int m_initialHandSize = 5;

    private Coroutine m_currentRepositionCoroutine = null;
    private List<CardData> m_availableCards = new List<CardData>();
    private WWECard m_previewCard;

    public WWECard GetPreviewCard() => m_previewCard;
    public int GetCardIndexInHand(WWECard card) => m_hand.IndexOf(card);
    public int GetHandCount() => m_hand.Count;
    public List<WWECard> GetHand() => m_hand;

    public void Init()
    {
        m_cardPrefab = Resources.Load<GameObject>("Prefabs/Card");
        InitializeCardPool();
        InitializePreviewCard();
    }

    private void InitializePreviewCard()
    {
        if (m_cardPrefab == null) return;

        GameObject previewObj = Instantiate(m_cardPrefab);
        m_previewCard = previewObj.GetComponent<WWECard>();

        if (m_previewCard != null)
        {
            m_previewCard.Init(CardDatabase.Chop, true);
            previewObj.transform.position = m_previewCardPosition;
            previewObj.transform.localScale = Vector3.one * m_previewCardScale;
            previewObj.transform.rotation = Quaternion.identity;
            previewObj.gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        StartCoroutine(DrawInitialHandCoroutine());
    }

    private void InitializeCardPool()
    {
        m_availableCards.Clear();
        m_availableCards.Add(CardDatabase.Chop);
        m_availableCards.Add(CardDatabase.LowKick);
        m_availableCards.Add(CardDatabase.Jab);
        m_availableCards.Add(CardDatabase.Headbutt);
        m_availableCards.Add(CardDatabase.RearNakedChoke);
        m_availableCards.Add(CardDatabase.HeartPunch);
        m_availableCards.Add(CardDatabase.Superkick);
        m_availableCards.Add(CardDatabase.RKO);
    }

    private IEnumerator DrawInitialHandCoroutine()
    {
        for (int i = 0; i < m_initialHandSize; i++)
        {
            DrawCard();
            yield return new WaitForSeconds(0.15f);
        }
    }

    public void DrawCard()
    {
        // 손패 + 셋업된 카드의 총 개수가 최대치를 넘으면 드로우 불가
        if (m_hand.Count + m_setupCards.Count >= m_maxHandSize || m_availableCards.Count == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, m_availableCards.Count);
        CardData randomCard = m_availableCards[randomIndex];
        StartCoroutine(CreateCardWithAnimationCoroutine(randomCard));
    }

    private IEnumerator CreateCardWithAnimationCoroutine(CardData cardData)
    {
        GameObject cardObj = Instantiate(m_cardPrefab, m_handTransform);
        WWECard wweCard = cardObj.GetComponent<WWECard>();

        wweCard.Init(cardData);
        m_hand.Add(wweCard);

        // 시작 위치 설정
        cardObj.transform.localPosition = m_drawStartOffset;
        cardObj.transform.localRotation = Quaternion.identity;

        if (m_currentRepositionCoroutine != null)
        {
            StopCoroutine(m_currentRepositionCoroutine);
        }

        m_currentRepositionCoroutine = StartCoroutine(RepositionAllCardsCoroutine());
        yield break;
    }

    public void SetupCard(WWECard card, int slotIndex)
    {
        if (card == null) return;

        // 손패에서 제거
        if (m_hand.Contains(card))
        {
            m_hand.Remove(card);
            m_setupCards.Add(card);

            // 손패 재배치
            RepositionHandCards();
        }
    }

    public void ReleaseCardFromSetup(WWECard card)
    {
        if (card == null) return;

        // 셋업에서 제거하고 손패에 추가
        if (m_setupCards.Contains(card))
        {
            m_setupCards.Remove(card);

            // 원래 인덱스 위치에 삽입
            int originalIndex = card.OriginalHandIndex;
            if (originalIndex >= 0 && originalIndex <= m_hand.Count)
            {
                m_hand.Insert(originalIndex, card);
            }
            else
            {
                m_hand.Add(card);
            }

            // 손패 재배치
            RepositionHandCards();
        }
    }

    private void RepositionHandCards()
    {
        if (m_currentRepositionCoroutine != null)
        {
            StopCoroutine(m_currentRepositionCoroutine);
        }
        m_currentRepositionCoroutine = StartCoroutine(RepositionAllCardsCoroutine());
    }

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

        localPosition = new Vector3(x, 0f, 0f);
        localRotation = Quaternion.Euler(0f, 0f, -angle);
    }

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

        for (int i = 0; i < m_hand.Count && i < targetPositions.Count; i++)
        {
            if (m_hand[i] == null) continue;

            m_hand[i].transform.localPosition = targetPositions[i];
            m_hand[i].transform.localRotation = targetRotations[i];
        }
        
        m_currentRepositionCoroutine = null;
    }
}
