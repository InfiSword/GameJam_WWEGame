using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class YaCht_CardManager : MonoBehaviour
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

    private List<YaCht_WWECard> m_hand = new List<YaCht_WWECard>();
    private List<YaCht_WWECard> m_setupCards = new List<YaCht_WWECard>();

    private int m_initialHandSize = 5;
    private int m_nextDrawOrderId = 0;

    private Coroutine m_currentRepositionCoroutine = null;

    private List<YaCht_CardData> m_availableCardsPool = new List<YaCht_CardData>();
    public List<YaCht_CardData> GetAvailableCardsPool() => m_availableCardsPool;
    private List<YaCht_CardData> m_playerDeck = new List<YaCht_CardData>();
    private YaCht_WWECard m_previewCard;

    public YaCht_WWECard GetPreviewCard() => m_previewCard;
    public int GetCardIndexInHand(YaCht_WWECard card) => m_hand.IndexOf(card);
    public int GetHandCount() => m_hand.Count;
    public List<YaCht_WWECard> GetHand() => m_hand;

    public void Init()
    {
        m_cardPrefab = Resources.Load<GameObject>("Prefabs/Card");
        InitializeAvailableCardsPool();
        InitializePlayerDeck();
        InitializePreviewCard();
    }

    private void InitializeAvailableCardsPool()
    {
        m_availableCardsPool.Clear();
        m_availableCardsPool.Add(YaCht_CardDatabase.Chop);
        m_availableCardsPool.Add(YaCht_CardDatabase.LowKick);
        m_availableCardsPool.Add(YaCht_CardDatabase.Jab);
        m_availableCardsPool.Add(YaCht_CardDatabase.Headbutt);
        m_availableCardsPool.Add(YaCht_CardDatabase.RearNakedChoke);
        m_availableCardsPool.Add(YaCht_CardDatabase.HeartPunch);
        m_availableCardsPool.Add(YaCht_CardDatabase.Superkick);
        m_availableCardsPool.Add(YaCht_CardDatabase.RKO);
    }
    private void InitializePreviewCard()
    {
        if (m_cardPrefab == null || m_playerDeck.Count == 0) return;

        GameObject previewObj = Instantiate(m_cardPrefab);
        m_previewCard = previewObj.GetComponent<YaCht_WWECard>();

        if (m_previewCard != null)
        {
            m_previewCard.Init(m_playerDeck[0], isPreviewCard: true);
            previewObj.transform.position = m_previewCardPosition;
            previewObj.transform.localScale = Vector3.one * m_previewCardScale;
            previewObj.transform.rotation = Quaternion.identity;
            previewObj.gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        m_handTransform = GameObject.Find("HandTransform").transform;
        StartCoroutine(DrawInitialHandCoroutine());
    }

    private void InitializePlayerDeck()
    {
        m_playerDeck.Clear();
        m_playerDeck.AddRange(YaCht_GameManager.nowPlayerData.playerDeck);
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
        if (m_hand.Count + m_setupCards.Count >= m_maxHandSize || m_playerDeck.Count == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, m_playerDeck.Count);
        YaCht_CardData randomCard = m_playerDeck[randomIndex];
        StartCoroutine(CreateCardWithAnimationCoroutine(randomCard));
    }

    private IEnumerator CreateCardWithAnimationCoroutine(YaCht_CardData cardData)
    {
        GameObject cardObj = Instantiate(m_cardPrefab, m_handTransform);
        YaCht_WWECard wweCard = cardObj.GetComponent<YaCht_WWECard>();

        wweCard.Init(cardData);
        wweCard.SetDrawOrderId(m_nextDrawOrderId++);
        m_hand.Add(wweCard);

        cardObj.transform.localPosition = m_drawStartOffset;
        cardObj.transform.localRotation = Quaternion.identity;

        if (m_currentRepositionCoroutine != null)
        {
            StopCoroutine(m_currentRepositionCoroutine);
        }

        m_currentRepositionCoroutine = StartCoroutine(RepositionAllCardsCoroutine());
        yield break;
    }

    public void SetupCard(YaCht_WWECard card, int slotIndex)
    {
        if (card == null) return;

        if (m_hand.Contains(card))
        {
            m_hand.Remove(card);
            m_setupCards.Add(card);

            RepositionHandCards();
        }
    }

    public void ReleaseCardFromSetup(YaCht_WWECard card)
    {
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
