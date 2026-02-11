using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 엠블럼 효과 관리
public class YaCht_RelicManager : MonoBehaviour
{
    // RTKO 효과
    private bool m_rtkoActivated = false;           // RTKO 활성화 여부                                                    
    private int m_rtkoStageUseCount = 0;            // 스테이지당 RKO 사용 횟수 (최대 3)
    private float m_rtkoPermanentMultiplier = 1.0f; // 영구 배율 (1.2배씩 누적)
    private float m_purpleGlovePermanentMultiplier = 1.0f; // 퍼플 건드리 영구 배율 (1.1배씩 누적)
    private int m_purpleGloveStageCount = 0; // 스테이지당 A랭크 카드 사용 횟수 (최대 3)

    // SoulBell 효과
    private List<float> m_soulBellDefeatedEnemyHP = new List<float>(); // 처치한 적의 HP 저장

    // 합주 효과 카운트
    private int m_harmonyMaskComboCount = 0;

    // YouCantSeeMe 효과
    private List<float> m_youCantSeeMeDamageHistory = new List<float>(); // 최근 2턴 데미지 히스토리

    // IHateS 효과
    private int m_iHateSACardCount = 0; // 리롤에 사용된 A랭크 카드 개수

    // JjolBoy 효과
    private float m_jjolBoyDamageMultiplier = 1.0f; // 데미지 배율 (리롤 시 변동)

    // UnderDogMask 효과
    private float m_underDogMaskPermanentMultiplier = 1.0f; // 영구 배율 (1.015배씩 누적)

    // 초기화 (엠블럼 게임 효과 초기화)
    public void Init()
    {
        ResetGameEffects();  // 엠블럼 게임 효과 초기화
    }

    // 모든 효과 초기화
    public void ResetAllEffects()
    {
        // RTKO 효과 초기화 (영구 배율은 유지)
        m_rtkoActivated = false;
        m_rtkoStageUseCount = 0;
        m_rtkoPermanentMultiplier = 1.0f;

        m_purpleGlovePermanentMultiplier = 1.0f;
        m_purpleGloveStageCount = 0;
        m_harmonyMaskComboCount = 0;
        m_youCantSeeMeDamageHistory.Clear();
        m_iHateSACardCount = 0;
        m_soulBellDefeatedEnemyHP.Clear(); // 게임 초기화 시 처치한 적 HP 리스트 초기화
        ResetTurnEffects();
    }

    // 엠블럼 스테이지 효과 초기화 (턴 효과 초기화)
    public void ResetStageEffects()
    {
        // RTKO 스테이지당 사용 횟수 초기화 (영구 배율은 유지)
        m_rtkoStageUseCount = 0;

        // PurpleGlove 스테이지당 사용 횟수 초기화 (영구 배율은 유지)
        m_purpleGloveStageCount = 0;

        // IHateS 효과 초기화
        m_iHateSACardCount = 0;
        
        // JjolBoy 효과 초기화 (스테이지 이동 시)
        m_jjolBoyDamageMultiplier = 1.0f;

        ResetTurnEffects();

        Debug.Log("[RelicManager] 엠블럼 스테이지 효과 초기화 - 턴 효과 초기화");
        if (m_rtkoActivated)
            Debug.Log($"[RelicManager] RTKO 효과 - 스테이지 사용 횟수: {m_rtkoStageUseCount}회, 영구 배율: x{m_rtkoPermanentMultiplier:F2}");
        if (m_purpleGlovePermanentMultiplier > 1.0f)
            Debug.Log($"[RelicManager] 퍼플 건드리 효과 초기화 - 영구 배율: x{m_purpleGlovePermanentMultiplier:F2} (스테이지 사용 횟수: {m_purpleGloveStageCount}회)");
        if (m_harmonyMaskComboCount > 0)
            Debug.Log($"[RelicManager] 합주 효과 초기화 - 카운트: {m_harmonyMaskComboCount}회");
        if (m_iHateSACardCount > 0)
            Debug.Log($"[RelicManager] IHateS 효과 초기화 - A랭크 카드 개수: {m_iHateSACardCount}개");
    }

    // 엠블럼 게임 효과 초기화 (턴 효과 초기화)
    public void ResetGameEffects()
    {
        m_rtkoActivated = false;
        m_rtkoStageUseCount = 0;
        m_rtkoPermanentMultiplier = 1.0f;
        m_purpleGlovePermanentMultiplier = 1.0f;
        m_purpleGloveStageCount = 0;
        m_harmonyMaskComboCount = 0;
        m_youCantSeeMeDamageHistory.Clear();
        m_iHateSACardCount = 0;
        m_jjolBoyDamageMultiplier = 1.0f;
        m_underDogMaskPermanentMultiplier = 1.0f;
        m_soulBellDefeatedEnemyHP.Clear(); // 게임 초기화 시 처치한 적 HP 리스트 초기화
        ResetTurnEffects();

        Debug.Log("[RelicManager] 엠블럼 게임 효과 초기화 - 턴 효과 초기화");
    }

    // 턴 효과 초기화
    public void ResetTurnEffects()
    {
        // 4턴일 때 YouCantSeeMe 데미지 히스토리 초기화
        if (YaCht_GameManager.currentRound == 4)
        {
            m_youCantSeeMeDamageHistory.Clear();
        }
    }

    // 엠블럼 보유 추가
    public void AddRelic(YaCht_RelicType relicType)
    {

        // 플레이어 데이터에도 유물 추가
        if (YaCht_GameManager.nowPlayerData != null && !YaCht_GameManager.nowPlayerData.playerRelics.Contains(relicType))
        {
            YaCht_GameManager.nowPlayerData.AddRelic(relicType);
        }

        Debug.Log($"엠블럼 보유 추가: {YaCht_RelicDatabase.GetRelicData(relicType).name}");
    }

    // 엠블럼 보유 여부 체크
    public bool HasRelic(YaCht_RelicType relicType)
    {
        return YaCht_GameManager.nowPlayerData.playerRelics.Contains(relicType);
    }

    // ==============================================
    // 엠블럼 등급 조정 효과
    // ==============================================
    public float ModifyRarityChance(YaCht_CardRarity rarity, float baseChance)
    {
        float modifiedChance = baseChance;

        // 게이브러 마스크 I (S등급 +15%, D등급 -15%)
        if (HasRelic(YaCht_RelicType.GamblerMask1))
        {
            if (rarity == YaCht_CardRarity.S)
                modifiedChance += 15f;
            else if (rarity == YaCht_CardRarity.D)
                modifiedChance -= 15f;
        }

        // IHateS: S랭크 확률 1%로 변경
        if (HasRelic(YaCht_RelicType.IHateS))
        {
            if (rarity == YaCht_CardRarity.S)
            {
                modifiedChance = 1f; // S랭크 확률을 1%로 고정
            }
        }

        // UnderDogMask: 모든 카드를 C, D 랭크로 강제 변경
        if (HasRelic(YaCht_RelicType.UnderDogMask))
        {
            // C, D 랭크만 가능하도록 확률 조정
            if (rarity == YaCht_CardRarity.S || rarity == YaCht_CardRarity.A || rarity == YaCht_CardRarity.B)
            {
                modifiedChance = 0f; // S, A, B 랭크는 0%
            }
            else if (rarity == YaCht_CardRarity.C)
            {
                modifiedChance = 50f; // C 랭크 50%
            }
            else if (rarity == YaCht_CardRarity.D)
            {
                modifiedChance = 50f; // D 랭크 50%
            }
        }

        // 최저 0%, 최고 100%
        return Mathf.Clamp(modifiedChance, 0f, 100f);
    }

    // ==============================================
    // 최종 데미지 계산
    // ==============================================
    public float CalculateFinalDamage(float baseDamage, List<YaCht_CardData> usedCards)
    {
        float finalDamage = baseDamage;
        float multiplier = 1.0f;

        Debug.Log($"[RelicManager] 최종 데미지 계산 - 기본 데미지: {baseDamage:F1}");
        Debug.Log($"[RelicManager] 엠블럼 보유 목록: {YaCht_GameManager.nowPlayerData.playerRelics.Count}");
        foreach (var relic in YaCht_GameManager.nowPlayerData.playerRelics)
        {
            YaCht_RelicData relicData = YaCht_RelicDatabase.GetRelicData(relic);
            Debug.Log($"  - {relicData.name} 엠블럼 보유");
        }       

        // 3. RTKO (영구 배율 적용)
        if (m_rtkoPermanentMultiplier > 1.0f)
        {
            multiplier *= m_rtkoPermanentMultiplier;
            finalDamage *= m_rtkoPermanentMultiplier;
            Debug.Log($"[RelicManager] RTKO 효과 활성화: 영구 배율 x{m_rtkoPermanentMultiplier:F2} → {finalDamage:F1}");
        }

        // 4. RestTombstone (각 S랭크 카드의 데미지를 개별적으로 0.7배로 줄임)
        if (HasRelic(YaCht_RelicType.RestTombstone))
        {
            // S랭크 카드의 원래 기본 데미지 합산 (각 카드마다)
            float sRankOriginalDamage = 0f;
            int sRankCardCount = 0;
            foreach (var card in usedCards)
            {
                if (card.m_rarity == YaCht_CardRarity.S)
                {
                    sRankOriginalDamage += card.m_baseDamage;
                    sRankCardCount++;
                }
            }
            
            if (sRankOriginalDamage > 0f)
            {
                // 전체 카드의 원래 기본 데미지 합산
                float totalOriginalDamage = 0f;
                foreach (var card in usedCards)
                {
                    totalOriginalDamage += card.m_baseDamage;
                }
                
                if (totalOriginalDamage > 0f)
                {
                    // S랭크 카드의 데미지 비율
                    float sRankRatio = sRankOriginalDamage / totalOriginalDamage;
                    // baseDamage에서 S랭크 카드의 기여도 추정
                    float sRankContribution = baseDamage * sRankRatio;
                    // 각 S랭크 카드의 데미지를 0.7배로 줄임 (각 카드마다 개별 적용)
                    float adjustedSRankContribution = sRankContribution * 0.7f;
                    // 최종 데미지 계산: baseDamage에서 S랭크 기여도를 빼고 0.7배한 값을 더함
                    finalDamage = baseDamage - sRankContribution + adjustedSRankContribution;
                    
                    Debug.Log($"[RelicManager] RestTombstone 효과: S랭크 카드 {sRankCardCount}장의 데미지를 각각 0.7배 적용 (원래 S랭크 기여도: {sRankContribution:F1} → 조정된 기여도: {adjustedSRankContribution:F1}, 감소: {sRankContribution - adjustedSRankContribution:F1}) → {finalDamage:F1}");
                }
            }
        }

        // 8. IHateS (A랭크 카드 개수에 따른 데미지 증가)
        if (HasRelic(YaCht_RelicType.IHateS) && m_iHateSACardCount > 0)
        {
            float iHateSMultiplier = 1.25f * m_iHateSACardCount; // 최소 1.25배
            multiplier *= iHateSMultiplier;
            finalDamage *= iHateSMultiplier;
            Debug.Log($"[RelicManager] IHateS 효과 활성화: (A랭크 카드: {m_iHateSACardCount}개) x{iHateSMultiplier:F2} → {finalDamage:F1}");
        }

        // 5. SoulBell (처치한 적들의 HP를 스테이지별 비율로 추가 데미지 적용)
        if (HasRelic(YaCht_RelicType.SoulBell) && m_soulBellDefeatedEnemyHP.Count > 0)
        {
            int currentStage = YaCht_GameManager.StageManager != null ? YaCht_GameManager.StageManager.CurrentStageNumber : 1;
            float additionalDamage = 0f;
            
            // 스테이지별 비율 결정
            float percentage = 0f;
            if (currentStage == 1)
                percentage = 0.20f; // 20%
            else if (currentStage == 2)
                percentage = 0.10f; // 10%
            else
                percentage = 0.05f; // 5%
            
            // 처치한 적들의 HP를 스테이지별 비율로 계산
            foreach (var enemyHP in m_soulBellDefeatedEnemyHP)
            {
                additionalDamage += enemyHP * percentage;
            }
            
            finalDamage += additionalDamage;
            Debug.Log($"[RelicManager] SoulBell 효과: 처치한 적 {m_soulBellDefeatedEnemyHP.Count}명의 HP ({percentage * 100}%) 추가 데미지: +{additionalDamage:F1} → {finalDamage:F1}");
        }

        // 6. 퍼플 건드리 (영구 배율 적용)
        if (m_purpleGlovePermanentMultiplier > 1.0f)
        {
            multiplier *= m_purpleGlovePermanentMultiplier;
            finalDamage *= m_purpleGlovePermanentMultiplier;
            Debug.Log($"[RelicManager] 퍼플 건드리: 영구 배율 x{m_purpleGlovePermanentMultiplier:F2} → {finalDamage:F1}");
        }

        // 7. 합주 효과 (합주 효과 카운
        if (HasRelic(YaCht_RelicType.HarmonyMask))
        {
            float harmonyBonus = Mathf.Min(m_harmonyMaskComboCount * 0.04f, 0.4f);
            float harmonyMultiplier = 1.0f + harmonyBonus;
            multiplier *= harmonyMultiplier;
            finalDamage *= harmonyMultiplier;
            Debug.Log($"[RelicManager] 합주 효과: x{harmonyMultiplier:F2} (카운트: {m_harmonyMaskComboCount}회) → {finalDamage:F1}");
        }

        Debug.Log($"[RelicManager] 곱열: x{multiplier:F2}");
        Debug.Log($"[RelicManager] 최종 데미지: {baseDamage:F1} x {multiplier:F2} = {finalDamage:F1}");

        return finalDamage;
    }

    // ==============================================
    // 카드 사용 이벤트
    // ==============================================

    // 카드 사용 이벤트
    public void OnCardsUsed(List<YaCht_CardData> usedCards)
    {
        foreach (var card in usedCards)
        {           
            // RTKO: RKO 효과 활성화 여부 (스테이지당 3번 제한, 1.2배 누적)
            if (HasRelic(YaCht_RelicType.RTKO) && card.m_name == "RKO")
            {
                if (m_rtkoStageUseCount < 3)
                {
                    m_rtkoActivated = true;
                    m_rtkoStageUseCount++;
                    m_rtkoPermanentMultiplier *= 1.2f; // 1.2배씩 누적
                    Debug.Log($"[RelicManager] RTKO 효과 활성화: (스테이지 사용 횟수: {m_rtkoStageUseCount}/3) 영구 배율: x{m_rtkoPermanentMultiplier:F2}");
                }
                else
                {
                    Debug.Log($"[RelicManager] RTKO 효과: 스테이지당 3번 제한에 도달했습니다. (현재 배율: x{m_rtkoPermanentMultiplier:F2})");
                }
            }


            // 퍼플 건드리: A랭크 기술 적중 시마다 영구적으로 1.1배씩 상승 (스테이지당 최대 3번)
            if (HasRelic(YaCht_RelicType.PurpleGlove) && card.m_rarity == YaCht_CardRarity.A)
            {
                if (m_purpleGloveStageCount < 3)
                {
                    m_purpleGloveStageCount++;
                    m_purpleGlovePermanentMultiplier *= 1.1f; // 1.1배씩 누적
                    Debug.Log($"[RelicManager] 퍼플 건드리 효과 활성화: (스테이지 사용 횟수: {m_purpleGloveStageCount}/3) 영구 배율: x{m_purpleGlovePermanentMultiplier:F2}");
                }
                else
                {
                    Debug.Log($"[RelicManager] 퍼플 건드리 효과: 스테이지당 3번 제한에 도달했습니다. (현재 배율: x{m_purpleGlovePermanentMultiplier:F2})");
                }
            }

            // UnderDogMask: 기술 사용 시마다 데미지 영구 1.015배 증가
            if (HasRelic(YaCht_RelicType.UnderDogMask))
            {
                m_underDogMaskPermanentMultiplier *= 1.015f; // 1.015배씩 누적
                Debug.Log($"[RelicManager] UnderDogMask 효과: 기술 사용으로 영구 배율 증가 x{m_underDogMaskPermanentMultiplier:F2}");
            }
        }
    }

    // 레스토 톰스톤: S랭크 기술로 즉시 처치 (HP 10% 이하)
    public bool CheckRestTombstoneInstantKill(List<YaCht_CardData> usedCards, float enemyHealthPercent)
    {
        if (!HasRelic(YaCht_RelicType.RestTombstone))
            return false;

        if (enemyHealthPercent > 10f)
            return false;

        // S랭크 기술이 있는지 확인
        foreach (var card in usedCards)
        {
            if (card.m_rarity == YaCht_CardRarity.S)
            {
                Debug.Log("[RelicManager] RestTombstone 효과 활성화: S랭크 기술로 즉시 처치");
                return true;
            }
        }

        return false;
    }

    // 합주 효과: 합주 효과 카운트 증가
    public void OnComboAchieved()
    {
        if (HasRelic(YaCht_RelicType.HarmonyMask))
        {
            m_harmonyMaskComboCount++;
            Debug.Log($"[RelicManager] 합주 효과: 카운트: {m_harmonyMaskComboCount} (곱열: +{Mathf.Min(m_harmonyMaskComboCount * 4, 40)}%)");
        }
    }

    // 고정 마스크: 고정 마스크 활성화 여부
    public YaCht_CardData? GetFixedMaskCard(List<YaCht_CardData> playerDeck)
    {
        if (!HasRelic(YaCht_RelicType.FixedMask))
            return null;

        if (playerDeck.Count == 0)
            return null;

        // 고정 마스크 카드 선택    
        int randomIndex = Random.Range(0, playerDeck.Count);
        Debug.Log($"[RelicManager] 고정 마스크 효과 활성화: {playerDeck[randomIndex].m_name} 선택");
        return playerDeck[randomIndex];
    }

    // ==============================================
    // YouCantSeeMe 효과
    // ==============================================

    /// <summary>
    /// 데미지 기록 (YouCantSeeMe용)
    /// </summary>
    public void OnDamageDealt(float damage)
    {
        if (!HasRelic(YaCht_RelicType.YouCantSeeMe))
            return;

        // 2턴(round 2, 3)의 데미지만 저장
        if (YaCht_GameManager.currentRound == 2 || YaCht_GameManager.currentRound == 3)
        {
            m_youCantSeeMeDamageHistory.Add(damage);
            Debug.Log($"[RelicManager] YouCantSeeMe 데미지 기록: {damage:F1} (턴: {YaCht_GameManager.currentRound})");
        }
    }

    /// <summary>
    /// YouCantSeeMe 공격 횟수 계산
    /// </summary>
    public int GetYouCantSeeMeAttackCount(int cardCount)
    {
        if (!HasRelic(YaCht_RelicType.YouCantSeeMe))
            return 1; // 기본 1회

        // 4턴일 때만 발동
        if (YaCht_GameManager.currentRound != 4)
            return 1;

        // 2턴 동안의 총 데미지 계산
        float totalDamage = 0f;
        foreach (var damage in m_youCantSeeMeDamageHistory)
        {
            totalDamage += damage;
        }

        Debug.Log($"[RelicManager] YouCantSeeMe 총 데미지: {totalDamage:F1}");

        // 조건에 따라 공격 횟수 결정
        if (totalDamage >= 400f)
            return 5; // 5회
        else if (totalDamage >= 300f)
            return 4; // 4회
        else if (totalDamage >= 200f)
            return 3; // 3회
        else
            return 1; // 기본 1회
    }

    // ==============================================
    // IHateS 효과
    // ==============================================

    /// <summary>
    /// 리롤 시 A랭크 카드 개수 기록
    /// </summary>
    public void OnRerollWithACards(int aCardCount)
    {
        if (!HasRelic(YaCht_RelicType.IHateS))
            return;

        m_iHateSACardCount += aCardCount;
        Debug.Log($"[RelicManager] IHateS 효과: A랭크 카드 {aCardCount}개 추가 (총: {m_iHateSACardCount}개, 배율: x{1.25f * m_iHateSACardCount:F2})");
    }

    /// <summary>
    /// S랭크 카드 획득 시 중첩 초기화
    /// </summary>
    public void OnSRankCardObtained()
    {
        if (!HasRelic(YaCht_RelicType.IHateS))
            return;

        if (m_iHateSACardCount > 0)
        {
            Debug.Log($"[RelicManager] IHateS 효과: S랭크 카드 획득으로 중첩 초기화 (이전: {m_iHateSACardCount}개)");
            m_iHateSACardCount = 0;
        }
    }

    // ==============================================
    // SoulBell 효과
    // ==============================================

    /// <summary>
    /// 적 처치 시 HP 저장 (SoulBell 효과용)
    /// </summary>
    public void OnEnemyDefeated(float enemyMaxHP)
    {
        if (!HasRelic(YaCht_RelicType.SoulBell))
            return;

        m_soulBellDefeatedEnemyHP.Add(enemyMaxHP);
        Debug.Log($"[RelicManager] SoulBell 효과: 처치한 적 HP 저장: {enemyMaxHP:F1} (총 {m_soulBellDefeatedEnemyHP.Count}명)");
    }

    // ==============================================
    // GamblerMask2 효과
    // ==============================================

    /// <summary>
    /// GamblerMask2: 랭크별 추가 공격 확률 반환
    /// </summary>
    public float GetGamblerMask2AttackChance(YaCht_CardRarity rarity)
    {
        if (!HasRelic(YaCht_RelicType.GamblerMask2))
            return 0f;

        switch (rarity)
        {
            case YaCht_CardRarity.D:
                return 90f; // 90%
            case YaCht_CardRarity.C:
                return 75f; // 75%
            case YaCht_CardRarity.B:
                return 50f; // 50%
            case YaCht_CardRarity.A:
                return 15f; // 15%
            case YaCht_CardRarity.S:
                return 3f;  // 3%
            default:
                return 0f;
        }
    }

    // ==============================================
    // JjolBoy 효과
    // ==============================================

    /// <summary>
    /// 리롤 시 JjolBoy 효과 적용 (50% 확률로 데미지 10%/30%/50% 증가 혹은 감소)
    /// </summary>
    public void OnReroll()
    {
        if (!HasRelic(YaCht_RelicType.JjolBoy))
            return;

        // 50% 확률로 효과 발동
        float roll = Random.Range(0f, 100f);
        if (roll < 50f)
        {
            // 10%, 30%, 50% 중 랜덤 선택
            float[] percentages = { 0.10f, 0.30f, 0.50f };
            float selectedPercentage = percentages[Random.Range(0, percentages.Length)];
            
            // 증가 또는 감소 (50% 확률)
            bool isIncrease = Random.Range(0f, 100f) < 50f;
            
            if (isIncrease)
            {
                m_jjolBoyDamageMultiplier = 1.0f + selectedPercentage;
                Debug.Log($"[RelicManager] JjolBoy 효과: 리롤 시 데미지 {selectedPercentage * 100}% 증가 → x{m_jjolBoyDamageMultiplier:F2}");
            }
            else
            {
                m_jjolBoyDamageMultiplier = 1.0f - selectedPercentage;
                Debug.Log($"[RelicManager] JjolBoy 효과: 리롤 시 데미지 {selectedPercentage * 100}% 감소 → x{m_jjolBoyDamageMultiplier:F2}");
            }
        }
        else
        {
            // 효과 미발동
            m_jjolBoyDamageMultiplier = 1.0f;
            Debug.Log($"[RelicManager] JjolBoy 효과: 리롤 시 효과 미발동 (확률: {roll:F1}%)");
        }
    }

    // ==============================================
    // 유물 상태 정보 조회
    // ==============================================

    /// <summary>
    /// 유물의 현재 상태 정보를 반환합니다.
    /// </summary>
    public Dictionary<string, string> GetRelicStatusInfo(YaCht_RelicType relicType)
    {
        Dictionary<string, string> statusInfo = new Dictionary<string, string>();

        if (!HasRelic(relicType))
        {
            return statusInfo;
        }

        switch (relicType)
        {
            case YaCht_RelicType.RTKO:
                statusInfo["영구 배율"] = $"x{m_rtkoPermanentMultiplier:F2}";
                statusInfo["스테이지 사용 횟수"] = $"{m_rtkoStageUseCount}/3";
                break;

            case YaCht_RelicType.YouCantSeeMe:
                float totalDamage = 0f;
                foreach (var damage in m_youCantSeeMeDamageHistory)
                {
                    totalDamage += damage;
                }
                statusInfo["2턴 총 데미지"] = $"{totalDamage:F1}";
                int attackCount = GetYouCantSeeMeAttackCount(1);
                statusInfo["예상 공격 횟수"] = $"{attackCount}회 (4턴 기준)";
                break;

            case YaCht_RelicType.IHateS:
                float iHateSMultiplier = m_iHateSACardCount > 0 ? 1.25f * m_iHateSACardCount : 1.0f;
                statusInfo["A랭크 카드 개수"] = $"{m_iHateSACardCount}개";
                statusInfo["현재 배율"] = $"x{iHateSMultiplier:F2}";
                break;

            case YaCht_RelicType.RestTombstone:
                statusInfo["S랭크 데미지 감소율"] = "0.7배";
                statusInfo["즉사 조건"] = "HP 10% 이하";
                break;

            case YaCht_RelicType.SoulBell:
                int currentStage = YaCht_GameManager.StageManager != null ? YaCht_GameManager.StageManager.CurrentStageNumber : 1;
                float percentage = 0f;
                if (currentStage == 1)
                    percentage = 0.20f;
                else if (currentStage == 2)
                    percentage = 0.10f;
                else
                    percentage = 0.05f;
                
                float additionalDamage = 0f;
                foreach (var enemyHP in m_soulBellDefeatedEnemyHP)
                {
                    additionalDamage += enemyHP * percentage;
                }
                statusInfo["처치한 적 수"] = $"{m_soulBellDefeatedEnemyHP.Count}명";
                statusInfo["추가 데미지"] = $"+{additionalDamage:F1} ({percentage * 100}%)";
                break;

            case YaCht_RelicType.PurpleGlove:
                statusInfo["영구 배율"] = $"x{m_purpleGlovePermanentMultiplier:F2}";
                statusInfo["스테이지 사용 횟수"] = $"{m_purpleGloveStageCount}/3";
                break;

            case YaCht_RelicType.HarmonyMask:
                float harmonyBonus = Mathf.Min(m_harmonyMaskComboCount * 0.04f, 0.4f);
                float harmonyMultiplier = 1.0f + harmonyBonus;
                statusInfo["콤보 카운트"] = $"{m_harmonyMaskComboCount}회";
                statusInfo["현재 배율"] = $"x{harmonyMultiplier:F2} (+{harmonyBonus * 100:F0}%)";
                break;

            case YaCht_RelicType.GamblerMask2:
                statusInfo["D랭크 추가 공격 확률"] = "90%";
                statusInfo["C랭크 추가 공격 확률"] = "75%";
                statusInfo["B랭크 추가 공격 확률"] = "50%";
                statusInfo["A랭크 추가 공격 확률"] = "15%";
                statusInfo["S랭크 추가 공격 확률"] = "3%";
                break;

            case YaCht_RelicType.JjolBoy:
                string multiplierText = m_jjolBoyDamageMultiplier >= 1.0f 
                    ? $"+{(m_jjolBoyDamageMultiplier - 1.0f) * 100:F0}%" 
                    : $"{(m_jjolBoyDamageMultiplier - 1.0f) * 100:F0}%";
                statusInfo["현재 데미지 배율"] = $"x{m_jjolBoyDamageMultiplier:F2} ({multiplierText})";
                break;

            case YaCht_RelicType.UnderDogMask:
                statusInfo["영구 배율"] = $"x{m_underDogMaskPermanentMultiplier:F2}";
                break;

            case YaCht_RelicType.FixedMask:
                YaCht_CardData? fixedCard = GetFixedMaskCard(YaCht_GameManager.nowPlayerData.playerDeck);
                if (fixedCard.HasValue)
                {
                    statusInfo["고정 카드"] = fixedCard.Value.m_name;
                }
                else
                {
                    statusInfo["고정 카드"] = "없음";
                }
                break;

            case YaCht_RelicType.GamblerMask1:
                statusInfo["S랭크 확률 증가"] = "+15%";
                statusInfo["D랭크 확률 감소"] = "-15%";
                break;
        }

        return statusInfo;
    }
}
