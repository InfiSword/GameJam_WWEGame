using System.Collections.Generic;
using UnityEngine;

// 유물 효과 관리자
public class YaCht_RelicManager : MonoBehaviour
{
    // 플레이어가 보유한 유물 목록
    private List<YaCht_RelicType> m_ownedRelics = new List<YaCht_RelicType>();

    // 영구 효과 플래그
    private bool m_rtkoActivated = false;           // RTKO 활성화 여부
    private float m_purpleGloveMultiplier = 1.0f;  // 보라색 장갑 배율
    
    // 턴별 효과 플래그
    private bool m_aaaActivatedThisTurn = false;    // AAA 이번 턴 활성화
    private bool m_soulBellActivatedThisTurn = false; // 영혼의 종 이번 턴 활성화
    
    // 화합의 가면 (게임 전체)
    private int m_harmonyMaskComboCount = 0;
    
    // 자비의 가면 (다음 턴)
    private int m_mercyMaskBonusReroll = 0;

    // 초기화
    public void Init()
    {
        m_ownedRelics.Clear();
        ResetAllEffects();
    }

    // 모든 효과 초기화
    public void ResetAllEffects()
    {
        m_rtkoActivated = false;
        m_purpleGloveMultiplier = 1.0f;
        m_harmonyMaskComboCount = 0;
        m_mercyMaskBonusReroll = 0;
        ResetTurnEffects();
    }

    // 턴별 효과 초기화
    public void ResetTurnEffects()
    {
        m_aaaActivatedThisTurn = false;
        m_soulBellActivatedThisTurn = false;
    }

    // 유물 추가
    public void AddRelic(YaCht_RelicType relicType)
    {
        if (!m_ownedRelics.Contains(relicType))
        {
            m_ownedRelics.Add(relicType);
            Debug.Log($"유물 획득: {YaCht_RelicDatabase.GetRelicData(relicType).name}");
        }
    }

    // 유물 제거
    public void RemoveRelic(YaCht_RelicType relicType)
    {
        m_ownedRelics.Remove(relicType);
    }

    // 유물 보유 여부
    public bool HasRelic(YaCht_RelicType relicType)
    {
        return m_ownedRelics.Contains(relicType);
    }

    // 보유 유물 목록
    public List<YaCht_RelicType> GetOwnedRelics()
    {
        return new List<YaCht_RelicType>(m_ownedRelics);
    }

    // ==============================================
    // 카드 드로우 확률 수정
    // ==============================================
    public float ModifyRarityChance(YaCht_CardRarity rarity, float baseChance)
    {
        float modifiedChance = baseChance;

        // 도박사의 가면 II (S급 +15%)
        if (HasRelic(YaCht_RelicType.GamblerMask2))
        {
            if (rarity == YaCht_CardRarity.S)
                modifiedChance += 15f;
            else if (rarity == YaCht_CardRarity.D)
                modifiedChance -= 15f;
        }

        // 도박사의 가면 I (A급 +10%)
        if (HasRelic(YaCht_RelicType.GamblerMask1))
        {
            if (rarity == YaCht_CardRarity.A)
                modifiedChance += 10f;
            else if (rarity == YaCht_CardRarity.D)
                modifiedChance -= 10f;
        }

        // 최소 0%, 최대 100%
        return Mathf.Clamp(modifiedChance, 0f, 100f);
    }

    // ==============================================
    // 데미지 계산 수정
    // ==============================================
    public float CalculateFinalDamage(float baseDamage, List<YaCht_CardData> usedCards)
    {
        float finalDamage = baseDamage;

        // 1. 분노의 가면 (+20%)
        if (HasRelic(YaCht_RelicType.RageMask))
        {
            finalDamage *= 1.2f;
        }

        // 2. 다이아몬드 너클 (+20%)
        if (HasRelic(YaCht_RelicType.DiamondKnuckle))
        {
            finalDamage *= 1.2f;
            
            // 파이브 너클 셔플 사용 시 추가 2배
            foreach (var card in usedCards)
            {
                if (card.m_name == "파이브 너클 셔플")
                {
                    finalDamage *= 2.0f;
                    break;
                }
            }
        }

        // 3. AAA (이번 턴 AA 사용 시 2배)
        if (m_aaaActivatedThisTurn)
        {
            finalDamage *= 2.0f;
        }

        // 4. RTKO (영구 2배)
        if (m_rtkoActivated)
        {
            finalDamage *= 2.0f;
        }

        // 5. 영혼의 종 (이번 턴 헬즈 게이트 사용 시 2배)
        if (m_soulBellActivatedThisTurn)
        {
            finalDamage *= 2.0f;
        }

        // 6. 보라색 장갑 (올드 스쿨 누적)
        finalDamage *= m_purpleGloveMultiplier;

        // 7. 화합의 가면 (콤보당 4%, 최대 40%)
        if (HasRelic(YaCht_RelicType.HarmonyMask))
        {
            float harmonyBonus = Mathf.Min(m_harmonyMaskComboCount * 0.04f, 0.4f);
            finalDamage *= (1.0f + harmonyBonus);
        }

        return finalDamage;
    }

    // ==============================================
    // 유물 효과 트리거
    // ==============================================
    
    // 카드 사용 시 호출
    public void OnCardsUsed(List<YaCht_CardData> usedCards)
    {
        foreach (var card in usedCards)
        {
            // AAA: AA 사용 감지
            if (HasRelic(YaCht_RelicType.AAA) && card.m_name == "AA")
            {
                m_aaaActivatedThisTurn = true;
                Debug.Log("[유물] AAA 발동! 이번 턴 데미지 2배");
            }

            // RTKO: RKO 사용 감지
            if (HasRelic(YaCht_RelicType.RTKO) && card.m_name == "RKO" && !m_rtkoActivated)
            {
                m_rtkoActivated = true;
                Debug.Log("[유물] RTKO 발동! 영구 데미지 2배");
            }

            // 영혼의 종: 헬즈 게이트 사용 감지
            if (HasRelic(YaCht_RelicType.SoulBell) && card.m_name == "헬즈 게이트")
            {
                m_soulBellActivatedThisTurn = true;
                Debug.Log("[유물] 영혼의 종 발동! 이번 턴 데미지 2배");
            }

            // 보라색 장갑: 올드 스쿨 사용 감지
            if (HasRelic(YaCht_RelicType.PurpleGlove) && card.m_name == "올드 스쿨")
            {
                m_purpleGloveMultiplier += 0.3f;
                Debug.Log($"[유물] 보라색 장갑 발동! 데미지 배율: {m_purpleGloveMultiplier:F2}배");
            }
        }
    }

    // 안식의 비석: 툼스톤 파일드라이버 즉사 체크
    public bool CheckRestTombstoneInstantKill(List<YaCht_CardData> usedCards, float enemyHealthPercent)
    {
        if (!HasRelic(YaCht_RelicType.RestTombstone))
            return false;

        if (enemyHealthPercent > 40f)
            return false;

        foreach (var card in usedCards)
        {
            if (card.m_name == "툼스톤 파일드라이버")
            {
                Debug.Log("[유물] 안식의 비석 발동! 즉시 처치!");
                return true;
            }
        }

        return false;
    }

    // 화합의 가면: 콤보 달성 시 호출
    public void OnComboAchieved()
    {
        if (HasRelic(YaCht_RelicType.HarmonyMask))
        {
            m_harmonyMaskComboCount++;
            Debug.Log($"[유물] 화합의 가면: 콤보 카운트 {m_harmonyMaskComboCount} (데미지 +{Mathf.Min(m_harmonyMaskComboCount * 4, 40)}%)");
        }
    }

    // 자비의 가면: Easy/Normal 성공 시 호출
    public void OnEasyNormalComboSuccess()
    {
        if (HasRelic(YaCht_RelicType.MercyMask))
        {
            m_mercyMaskBonusReroll = 1;
            Debug.Log("[유물] 자비의 가면 발동! 다음 턴 리롤 +1");
        }
    }

    // 자비의 가면 보너스 리롤 가져오기 및 소비
    public int ConsumeMercyMaskBonus()
    {
        int bonus = m_mercyMaskBonusReroll;
        m_mercyMaskBonusReroll = 0;
        return bonus;
    }

    // 고정의 가면: 새 라운드 시작 시 호출
    public YaCht_CardData? GetFixedMaskCard(List<YaCht_CardData> playerDeck)
    {
        if (!HasRelic(YaCht_RelicType.FixedMask))
            return null;

        if (playerDeck.Count == 0)
            return null;

        // 덱에서 완전 랜덤 선택
        int randomIndex = Random.Range(0, playerDeck.Count);
        Debug.Log($"[유물] 고정의 가면 발동! {playerDeck[randomIndex].m_name} 자동 셋업");
        return playerDeck[randomIndex];
    }
}
