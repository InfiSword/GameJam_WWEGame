using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class YaCht_ComboChecker
{    
    public static YaCht_ComboType CheckCombo(List<YaCht_CardData> setupCards, YaCht_WrestlerType wrestlerType)
    {
        if (setupCards == null || setupCards.Count == 0)
            return YaCht_ComboType.None;
        
        Dictionary<string, int> nameCount = new Dictionary<string, int>();
        Dictionary<YaCht_CardRarity, int> rarityCount = new Dictionary<YaCht_CardRarity, int>();
        
        foreach (var card in setupCards)
        {            
            string baseName = GetBaseName(card.m_name);
            
            if (!nameCount.ContainsKey(baseName))
                nameCount[baseName] = 0;
            nameCount[baseName]++;
            
            if (!rarityCount.ContainsKey(card.m_rarity))
                rarityCount[card.m_rarity] = 0;
            rarityCount[card.m_rarity]++;
        }
        
        // 레슬러 타입에 따른 다른 콤보 체크
        if (wrestlerType == YaCht_WrestlerType.JohnCena)
        {
            return CheckJohnCenaCombo(setupCards, rarityCount, nameCount);
        }
        else if (wrestlerType == YaCht_WrestlerType.Undertaker)
        {
            return CheckUndertakerCombo(setupCards, rarityCount, nameCount);
        }
        
        return YaCht_ComboType.None;
    }
    
    /// <summary>
    /// 카드 이름에서 기본 이름 추출 (중복 문자 제거)
    /// 예: "AA" -> "A", "RKO" -> "RKO"
    /// </summary>
    private static string GetBaseName(string cardName)
    {
        if (string.IsNullOrEmpty(cardName))
            return cardName;
        
        // 모든 문자가 같은지 체크 (예: "AA", "BB")
        if (cardName.Length > 1 && cardName.All(c => c == cardName[0]))
        {
            return cardName[0].ToString();
        }
        
        return cardName;
    }
    
    /// <summary>
    /// 존 시나 콤보 체크
    /// </summary>
    private static YaCht_ComboType CheckJohnCenaCombo(List<YaCht_CardData> setupCards, Dictionary<YaCht_CardRarity, int> rarityCount, Dictionary<string, int> nameCount)
    {
        // 우선순위 체크 (높은 콤보부터)
        if (setupCards.Count == 6)
        {
            // Triple Pair: S급 2장 + A급 2장 + B급 2장
            if (HasPair(rarityCount, YaCht_CardRarity.S) &&
                HasPair(rarityCount, YaCht_CardRarity.A) &&
                HasPair(rarityCount, YaCht_CardRarity.B))
            {
                return YaCht_ComboDatabase.GetComboType(YaCht_WrestlerType.JohnCena, YaCht_ComboLevel.Combo6);
            }
        }
        
        if (setupCards.Count == 5)
        {
            // Full House: B급 2장 + A급 3장
            if (HasPair(rarityCount, YaCht_CardRarity.B) &&
                HasThreeOfKind(rarityCount, YaCht_CardRarity.A))
            {
                return YaCht_ComboDatabase.GetComboType(YaCht_WrestlerType.JohnCena, YaCht_ComboLevel.Combo5);
            }
        }
        
        if (setupCards.Count == 4)
        {
            // Two Pair: C급 2장 + B급 2장
            if (HasPair(rarityCount, YaCht_CardRarity.C) &&
                HasPair(rarityCount, YaCht_CardRarity.B))
            {
                return YaCht_ComboDatabase.GetComboType(YaCht_WrestlerType.JohnCena, YaCht_ComboLevel.Combo4);
            }
        }
        
        if (setupCards.Count == 3)
        {
            // Three of a Kind: 같은 이름의 카드 3장 (C 또는 D 등급)
            foreach (var kvp in nameCount)
            {
                if (kvp.Value >= 3)
                {
                    // C나 D 등급 카드인지 확인
                    bool hasValidRarity = false;
                    foreach (var card in setupCards)
                    {
                        string baseName = GetBaseName(card.m_name);
                        if (baseName == kvp.Key && (card.m_rarity == YaCht_CardRarity.C || card.m_rarity == YaCht_CardRarity.D))
                        {
                            hasValidRarity = true;
                            break;
                        }
                    }
                    
                    if (hasValidRarity)
                    {
                        return YaCht_ComboDatabase.GetComboType(YaCht_WrestlerType.JohnCena, YaCht_ComboLevel.Combo3);
                    }
                }
            }
        }
        
        return YaCht_ComboType.None;
    }
    
    /// <summary>
    /// 언더테이커 콤보 체크
    /// </summary>
    private static YaCht_ComboType CheckUndertakerCombo(List<YaCht_CardData> setupCards, Dictionary<YaCht_CardRarity, int> rarityCount, Dictionary<string, int> nameCount)
    {
        // 우선순위 체크 (높은 콤보부터)
        if (setupCards.Count == 6)
        {
            // Triple Pair: S급 2장 + A급 3장 + B급 1장
            if (HasPair(rarityCount, YaCht_CardRarity.S) &&
                HasAtLeast(rarityCount, YaCht_CardRarity.A, 3) &&
                HasAtLeast(rarityCount, YaCht_CardRarity.B, 1))
            {
                return YaCht_ComboDatabase.GetComboType(YaCht_WrestlerType.Undertaker, YaCht_ComboLevel.Combo6);
            }
        }
        
        if (setupCards.Count >= 4 && setupCards.Count <= 5)
        {
            // Four Cards: A급 4장 이상
            if (HasAtLeast(rarityCount, YaCht_CardRarity.A, 4))
            {
                return YaCht_ComboDatabase.GetComboType(YaCht_WrestlerType.Undertaker, YaCht_ComboLevel.Combo5);
            }
        }
        
        if (setupCards.Count == 4)
        {
            // Four of a Kind: C급 1장 + B급 3장
            if (HasAtLeast(rarityCount, YaCht_CardRarity.C, 1) &&
                HasAtLeast(rarityCount, YaCht_CardRarity.B, 3))
            {
                return YaCht_ComboDatabase.GetComboType(YaCht_WrestlerType.Undertaker, YaCht_ComboLevel.Combo4);
            }
        }
        
        if (setupCards.Count == 3)
        {
            // Three of a Kind: C 또는 D 급 카드가 최소 1장 포함
            bool hasC = rarityCount.ContainsKey(YaCht_CardRarity.C) && rarityCount[YaCht_CardRarity.C] >= 1;
            bool hasD = rarityCount.ContainsKey(YaCht_CardRarity.D) && rarityCount[YaCht_CardRarity.D] >= 1;
            
            if (hasC || hasD)
            {
                return YaCht_ComboDatabase.GetComboType(YaCht_WrestlerType.Undertaker, YaCht_ComboLevel.Combo3);
            }
        }
        
        return YaCht_ComboType.None;
    }
    
    /// <summary>
    /// 특정 등급이 N개 이상인지 체크
    /// </summary>
    private static bool HasAtLeast(Dictionary<YaCht_CardRarity, int> rarityCount, YaCht_CardRarity rarity, int count)
    {
        return rarityCount.ContainsKey(rarity) && rarityCount[rarity] >= count;
    }
    
    /// <summary>
    /// 특정 등급이 2개 이상인지 체크
    /// </summary>
    private static bool HasPair(Dictionary<YaCht_CardRarity, int> rarityCount, YaCht_CardRarity rarity)
    {
        return rarityCount.ContainsKey(rarity) && rarityCount[rarity] >= 2;
    }
    
    /// <summary>
    /// 특정 등급이 3개 이상인지 체크
    /// </summary>
    private static bool HasThreeOfKind(Dictionary<YaCht_CardRarity, int> rarityCount, YaCht_CardRarity rarity)
    {
        return rarityCount.ContainsKey(rarity) && rarityCount[rarity] >= 3;
    }
    
    /// <summary>
    /// 콤보에 따른 데미지 계산
    /// </summary>
    public static float CalculateComboDamage(List<YaCht_CardData> setupCards, YaCht_WrestlerType wrestlerType, YaCht_ComboType comboType)
    {
        float baseDamage = 0f;
        
        // 모든 카드의 기본 데미지 합산
        foreach (var card in setupCards)
        {
            baseDamage += card.m_baseDamage;
        }
        
        // 콤보 배수 적용
        YaCht_ComboData comboData = YaCht_ComboDatabase.GetComboData(wrestlerType, comboType);
        return baseDamage * comboData.damageMultiplier;
    }
    
    /// <summary>
    /// 콤보 정보 문자열로 반환 (디버그/UI용)
    /// </summary>
    public static string GetComboInfo(List<YaCht_CardData> setupCards, YaCht_WrestlerType wrestlerType)
    {
        if (setupCards == null || setupCards.Count == 0)
            return "셋업된 카드가 없습니다.";
        
        YaCht_ComboType comboType = CheckCombo(setupCards, wrestlerType);
        YaCht_ComboData comboData = YaCht_ComboDatabase.GetComboData(wrestlerType, comboType);
        float totalDamage = CalculateComboDamage(setupCards, wrestlerType, comboType);
        
        // 카드명 개수 세기 (중복 제거)
        Dictionary<string, int> nameCount = new Dictionary<string, int>();
        foreach (var card in setupCards)
        {
            string baseName = GetBaseName(card.m_name);
            if (!nameCount.ContainsKey(baseName))
                nameCount[baseName] = 0;
            nameCount[baseName]++;
        }
        
        // 등급별 카드 개수 세기
        Dictionary<YaCht_CardRarity, int> rarityCount = new Dictionary<YaCht_CardRarity, int>();
        foreach (var card in setupCards)
        {
            if (!rarityCount.ContainsKey(card.m_rarity))
                rarityCount[card.m_rarity] = 0;
            rarityCount[card.m_rarity]++;
        }
        
        string cardInfo = "카드 구성:\n";
        
        // 이름별 정보
        cardInfo += "  [이름별]\n";
        foreach (var kvp in nameCount.OrderByDescending(x => x.Value))
        {
            cardInfo += $"    {kvp.Key} x{kvp.Value}\n";
        }
        
        // 등급별 정보
        cardInfo += "  [등급별]\n";
        foreach (var kvp in rarityCount.OrderByDescending(x => x.Key))
        {
            cardInfo += $"    {kvp.Key}급 x{kvp.Value}\n";
        }
        
        return $"{cardInfo}\n콤보: {comboData.comboName}\n설명: {comboData.description}\n총 데미지: {totalDamage:F1}";
    }
}
