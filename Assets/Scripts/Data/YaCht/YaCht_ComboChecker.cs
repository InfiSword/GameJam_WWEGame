using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 야추 조합 체크 시스템 (특정 기술 조합 기반)
public class YaCht_ComboChecker
{
    // 셋업된 카드들로 조합 판정 (레슬러 타입 기반)
    public static YaCht_ComboType CheckCombo(List<YaCht_CardData> setupCards, YaCht_WrestlerType wrestlerType)
    {
        if (setupCards == null || setupCards.Count == 0)
            return YaCht_ComboType.None;
        
        // 카드 이름 리스트 생성
        List<string> cardNames = setupCards.Select(card => card.m_name).ToList();
        
        // 해당 레슬러의 콤보만 가져오기
        YaCht_ComboData[] availableCombos = YaCht_ComboDatabase.GetCombosByWrestler(wrestlerType);
        
        // 모든 콤보를 체크 (높은 점수부터)
        YaCht_ComboType bestCombo = YaCht_ComboType.None;
        int bestScore = 0;
        
        foreach (var comboData in availableCombos)
        {
            if (CheckComboMatch(cardNames, comboData.requiredCards))
            {
                if (comboData.scoreMultiplier > bestScore)
                {
                    bestCombo = comboData.comboType;
                    bestScore = comboData.scoreMultiplier;
                }
            }
        }
        
        return bestCombo;
    }
    
    // 특정 콤보 조합이 성립하는지 체크
    private static bool CheckComboMatch(List<string> cardNames, string[] requiredCards)
    {
        if (requiredCards == null || requiredCards.Length == 0)
            return false;
        
        // 카드 개수가 맞지 않으면 실패
        if (cardNames.Count != requiredCards.Length)
            return false;
        
        // 필요한 카드별 개수 계산
        Dictionary<string, int> requiredCount = new Dictionary<string, int>();
        foreach (var card in requiredCards)
        {
            if (!requiredCount.ContainsKey(card))
                requiredCount[card] = 0;
            requiredCount[card]++;
        }
        
        // 실제 카드별 개수 계산
        Dictionary<string, int> actualCount = new Dictionary<string, int>();
        foreach (var card in cardNames)
        {
            if (!actualCount.ContainsKey(card))
                actualCount[card] = 0;
            actualCount[card]++;
        }
        
        // 모든 필요한 카드가 정확히 일치하는지 확인
        foreach (var kvp in requiredCount)
        {
            if (!actualCount.ContainsKey(kvp.Key) || actualCount[kvp.Key] != kvp.Value)
                return false;
        }
        
        return true;
    }
    
    // 조합의 총 데미지 계산
    public static float CalculateComboDamage(List<YaCht_CardData> setupCards, YaCht_WrestlerType wrestlerType, YaCht_ComboType comboType)
    {
        float baseDamage = 0f;
        
        foreach (var card in setupCards)
        {
            baseDamage += card.m_baseDamage;
        }
        
        YaCht_ComboData comboData = YaCht_ComboDatabase.GetComboData(wrestlerType, comboType);
        return baseDamage * comboData.damageMultiplier;
    }
    
    // 조합 정보를 문자열로 반환 (디버그/UI용)
    public static string GetComboInfo(List<YaCht_CardData> setupCards, YaCht_WrestlerType wrestlerType)
    {
        if (setupCards == null || setupCards.Count == 0)
            return "셋업된 카드가 없습니다.";
        
        YaCht_ComboType comboType = CheckCombo(setupCards, wrestlerType);
        YaCht_ComboData comboData = YaCht_ComboDatabase.GetComboData(wrestlerType, comboType);
        float totalDamage = CalculateComboDamage(setupCards, wrestlerType, comboType);
        
        // 기술 이름별 개수 표시
        Dictionary<string, int> nameCount = new Dictionary<string, int>();
        foreach (var card in setupCards)
        {
            if (!nameCount.ContainsKey(card.m_name))
                nameCount[card.m_name] = 0;
            nameCount[card.m_name]++;
        }
        
        string cardInfo = "셋업 카드:\n";
        foreach (var kvp in nameCount)
        {
            cardInfo += $"  {kvp.Key} x{kvp.Value}\n";
        }
        
        return $"{cardInfo}\n조합: {comboData.comboName}\n설명: {comboData.description}\n총 데미지: {totalDamage:F1}";
    }
}
