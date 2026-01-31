using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class YaCht_ComboChecker
{    
    public static List<YaCht_ComboType> CheckAllCombos(List<YaCht_CardData> setupCards, YaCht_WrestlerType wrestlerType)
    {
        List<YaCht_ComboType> combos = new List<YaCht_ComboType>();
        
        if (setupCards == null || setupCards.Count == 0)
            return combos;
        
        Dictionary<YaCht_CardRarity, int> rarityCount = new Dictionary<YaCht_CardRarity, int>();
        
        foreach (var card in setupCards)
        {
            if (!rarityCount.ContainsKey(card.m_rarity))
                rarityCount[card.m_rarity] = 0;
            rarityCount[card.m_rarity]++;
        }
        
        if (wrestlerType == YaCht_WrestlerType.JohnCena)
        {
            return CheckAllJohnCenaCombos(setupCards, rarityCount);
        }
        else if (wrestlerType == YaCht_WrestlerType.Undertaker)
        {
            return CheckAllUndertakerCombos(setupCards, rarityCount);
        }
        
        return combos;
    }
    
    /// <summary>
    /// 존 시나 모든 콤보 체크 - 등급 기반, 조합 조건만 충족되면 무조건 인정
    /// </summary>
    private static List<YaCht_ComboType> CheckAllJohnCenaCombos(List<YaCht_CardData> setupCards, Dictionary<YaCht_CardRarity, int> rarityCount)
    {
        List<YaCht_ComboType> combos = new List<YaCht_ComboType>();
        
        Debug.Log("=== 존 시나 콤보 체크 시작 (등급 기반) ===");
        Debug.Log($"총 카드 수: {setupCards.Count}");
        Debug.Log("등급별 카운트:");
        foreach (var kvp in rarityCount)
        {
            Debug.Log($"  {kvp.Key}급: {kvp.Value}개");
        }
        
        // 우선순위: 6 > 5 > 4 > 3 (높은 콤보부터 체크)
        
        // 6콤보 체크: S급 2장 + A급 2장 + B급 2장 (6장 이상)
        if (setupCards.Count >= 6 &&
            HasAtLeast(rarityCount, YaCht_CardRarity.S, 2) &&
            HasAtLeast(rarityCount, YaCht_CardRarity.A, 2) &&
            HasAtLeast(rarityCount, YaCht_CardRarity.B, 2))
        {
            combos.Add(YaCht_ComboDatabase.GetComboType(YaCht_WrestlerType.JohnCena, YaCht_ComboLevel.Combo6));
            Debug.Log("6콤보 발견! (S급 2장 + A급 2장 + B급 2장)");
            return combos; // 최고 콤보 발견 시 즉시 반환
        }
        
        // 5콤보 체크: B급 2장 + A급 3장 (5장 이상)
        if (setupCards.Count >= 5 &&
            HasAtLeast(rarityCount, YaCht_CardRarity.B, 2) &&
            HasAtLeast(rarityCount, YaCht_CardRarity.A, 3))
        {
            combos.Add(YaCht_ComboDatabase.GetComboType(YaCht_WrestlerType.JohnCena, YaCht_ComboLevel.Combo5));
            Debug.Log("5콤보 발견! (B급 2장 + A급 3장)");
            return combos;
        }
        
        // 4콤보 체크: C급 2장 + B급 2장 (4장 이상) ← 수정!
        if (setupCards.Count >= 4 &&
            HasAtLeast(rarityCount, YaCht_CardRarity.C, 2) &&
            HasAtLeast(rarityCount, YaCht_CardRarity.B, 2))
        {
            combos.Add(YaCht_ComboDatabase.GetComboType(YaCht_WrestlerType.JohnCena, YaCht_ComboLevel.Combo4));
            Debug.Log("4콤보 발견! (C급 2장 + B급 2장)");
            return combos;
        }
        
        // 3콤보 체크 - 등급별로 여러 개 가능!
        if (setupCards.Count >= 3)
        {
            Debug.Log("3콤보 체크 시작 (등급 기반)...");
            
            // C급과 D급 각각 3장씩 체크
            int cComboCount = 0;
            int dComboCount = 0;
            
            if (rarityCount.ContainsKey(YaCht_CardRarity.C) && rarityCount[YaCht_CardRarity.C] >= 3)
            {
                cComboCount = rarityCount[YaCht_CardRarity.C] / 3;
                Debug.Log($"C급 카드로 {cComboCount}개의 3콤보 발견!");
            }
            
            if (rarityCount.ContainsKey(YaCht_CardRarity.D) && rarityCount[YaCht_CardRarity.D] >= 3)
            {
                dComboCount = rarityCount[YaCht_CardRarity.D] / 3;
                Debug.Log($"D급 카드로 {dComboCount}개의 3콤보 발견!");
            }
            
            int totalComboCount = cComboCount + dComboCount;
            for (int i = 0; i < totalComboCount; i++)
            {
                combos.Add(YaCht_ComboDatabase.GetComboType(YaCht_WrestlerType.JohnCena, YaCht_ComboLevel.Combo3));
            }
            
            Debug.Log($"총 {totalComboCount}개의 3콤보 추가!");
        }
        
        Debug.Log($"=== 콤보 체크 완료: 총 {combos.Count}개 콤보 발견 ===");
        return combos;
    }
    
    /// <summary>
    /// 언더테이커 모든 콤보 체크 - 등급 기반, 조합 조건만 충족되면 무조건 인정
    /// </summary>
    private static List<YaCht_ComboType> CheckAllUndertakerCombos(List<YaCht_CardData> setupCards, Dictionary<YaCht_CardRarity, int> rarityCount)
    {
        List<YaCht_ComboType> combos = new List<YaCht_ComboType>();
        
        Debug.Log("=== 언더테이커 콤보 체크 시작 (등급 기반) ===");
        Debug.Log($"총 카드 수: {setupCards.Count}");
        Debug.Log("등급별 카운트:");
        foreach (var kvp in rarityCount)
        {
            Debug.Log($"  {kvp.Key}급: {kvp.Value}개");
        }
        
        // 6콤보 체크: S급 2장 + A급 3장 + B급 1장 (6장 이상)
        if (setupCards.Count >= 6 &&
            HasAtLeast(rarityCount, YaCht_CardRarity.S, 2) &&
            HasAtLeast(rarityCount, YaCht_CardRarity.A, 3) &&
            HasAtLeast(rarityCount, YaCht_CardRarity.B, 1))
        {
            combos.Add(YaCht_ComboDatabase.GetComboType(YaCht_WrestlerType.Undertaker, YaCht_ComboLevel.Combo6));
            Debug.Log("6콤보 발견! (S급 2장 + A급 3장 + B급 1장)");
            return combos;
        }
        
        // 5콤보 체크: A급 4장 이상
        if (HasAtLeast(rarityCount, YaCht_CardRarity.A, 4))
        {
            combos.Add(YaCht_ComboDatabase.GetComboType(YaCht_WrestlerType.Undertaker, YaCht_ComboLevel.Combo5));
            Debug.Log("5콤보 발견! (A급 4장 이상)");
            return combos;
        }
        
        // 4콤보 체크: C급 1장 + B급 3장 (4장 이상) ← 수정!
        if (setupCards.Count >= 4 &&
            HasAtLeast(rarityCount, YaCht_CardRarity.C, 1) &&
            HasAtLeast(rarityCount, YaCht_CardRarity.B, 3))
        {
            combos.Add(YaCht_ComboDatabase.GetComboType(YaCht_WrestlerType.Undertaker, YaCht_ComboLevel.Combo4));
            Debug.Log("4콤보 발견! (C급 1장 + B급 3장)");
            return combos;
        }
        
        // 3콤보 체크 - 등급별로 여러 개 가능!
        if (setupCards.Count >= 3)
        {
            Debug.Log("3콤보 체크 시작 (등급 기반)...");
            
            // C급과 D급 각각 3장씩 체크
            int cComboCount = 0;
            int dComboCount = 0;
            
            if (rarityCount.ContainsKey(YaCht_CardRarity.C) && rarityCount[YaCht_CardRarity.C] >= 3)
            {
                cComboCount = rarityCount[YaCht_CardRarity.C] / 3;
                Debug.Log($"C급 카드로 {cComboCount}개의 3콤보 발견!");
            }
            
            if (rarityCount.ContainsKey(YaCht_CardRarity.D) && rarityCount[YaCht_CardRarity.D] >= 3)
            {
                dComboCount = rarityCount[YaCht_CardRarity.D] / 3;
                Debug.Log($"D급 카드로 {dComboCount}개의 3콤보 발견!");
            }
            
            int totalComboCount = cComboCount + dComboCount;
            for (int i = 0; i < totalComboCount; i++)
            {
                combos.Add(YaCht_ComboDatabase.GetComboType(YaCht_WrestlerType.Undertaker, YaCht_ComboLevel.Combo3));
            }
            
            // C 또는 D가 3장 미만이지만 있는 경우 1콤보로 인정
            if (totalComboCount == 0 && setupCards.Count == 3)
            {
                bool hasC = rarityCount.ContainsKey(YaCht_CardRarity.C) && rarityCount[YaCht_CardRarity.C] >= 1;
                bool hasD = rarityCount.ContainsKey(YaCht_CardRarity.D) && rarityCount[YaCht_CardRarity.D] >= 1;
                
                if (hasC || hasD)
                {
                    combos.Add(YaCht_ComboDatabase.GetComboType(YaCht_WrestlerType.Undertaker, YaCht_ComboLevel.Combo3));
                    Debug.Log("3콤보 발견! (C 또는 D급 포함 3장)");
                }
            }
            else
            {
                Debug.Log($"총 {totalComboCount}개의 3콤보 추가!");
            }
        }
        
        Debug.Log($"=== 콤보 체크 완료: 총 {combos.Count}개 콤보 발견 ===");
        return combos;
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
    /// 콤보에 따른 데미지 계산 - 모든 콤보의 데미지를 합산
    /// </summary>
    public static float CalculateComboDamage(List<YaCht_CardData> setupCards, YaCht_WrestlerType wrestlerType, YaCht_ComboType comboType)
    {
        // 모든 콤보를 찾음
        List<YaCht_ComboType> allCombos = CheckAllCombos(setupCards, wrestlerType);
        
        if (allCombos.Count == 0)
        {
            // 콤보가 없으면 기본 데미지만 반환
            float baseDamage = 0f;
            foreach (var card in setupCards)
            {
                baseDamage += card.m_baseDamage;
            }
            Debug.Log($"[콤보 없음] 기본 데미지만 적용: {baseDamage:F1}");
            return baseDamage;
        }
        
        // 전체 카드의 기본 데미지 합산
        float totalBaseDamage = 0f;
        foreach (var card in setupCards)
        {
            totalBaseDamage += card.m_baseDamage;
        }
        
        // 각 콤보마다 배수 적용
        float totalDamage = 0f;
        
        foreach (var combo in allCombos)
        {
            YaCht_ComboData comboData = YaCht_ComboDatabase.GetComboData(wrestlerType, combo);
            
            // 각 콤보당 카드 수만큼의 데미지를 배수 적용
            int cardsInCombo = (int)comboData.comboLevel;
            
            // 콤보에 사용된 카드들의 평균 데미지 계산
            float avgDamagePerCard = totalBaseDamage / setupCards.Count;
            float comboDamage = avgDamagePerCard * cardsInCombo * comboData.damageMultiplier;
            
            totalDamage += comboDamage;
            
            Debug.Log($"[콤보 {allCombos.IndexOf(combo) + 1}] {comboData.comboName}: 카드 {cardsInCombo}장 (평균 {avgDamagePerCard:F1}) x {comboData.damageMultiplier} = {comboDamage:F1}");
        }
        
        Debug.Log($"[총 {allCombos.Count}개 콤보] 최종 데미지: {totalDamage:F1}");
        
        return totalDamage;
    }
    
    /// <summary>
    /// 콤보 정보 문자열로 반환 (디버그/UI용) - 상세 데미지 정보 포함
    /// </summary>
    public static string GetComboInfo(List<YaCht_CardData> setupCards, YaCht_WrestlerType wrestlerType)
    {
        if (setupCards == null || setupCards.Count == 0)
            return "셋업된 카드가 없습니다.";
        
        // 모든 콤보 찾기
        List<YaCht_ComboType> allCombos = CheckAllCombos(setupCards, wrestlerType);
        
        // 카드별 데미지 합산
        float totalCardDamage = 0f;
        string cardDamageDetails = "카드별 데미지:\n";
        foreach (var card in setupCards)
        {
            totalCardDamage += card.m_baseDamage;
            cardDamageDetails += $"  {card.m_name} ({card.m_rarity}급): {card.m_baseDamage}\n";
        }
        
        // 콤보 데미지 계산
        float comboDamage = CalculateComboDamage(setupCards, wrestlerType, YaCht_ComboType.None);
        
        // 등급별 카드 개수 세기
        Dictionary<YaCht_CardRarity, int> rarityCount = new Dictionary<YaCht_CardRarity, int>();
        foreach (var card in setupCards)
        {
            if (!rarityCount.ContainsKey(card.m_rarity))
                rarityCount[card.m_rarity] = 0;
            rarityCount[card.m_rarity]++;
        }
        
        string cardInfo = "카드 구성:\n";
        
        // 등급별 정보
        cardInfo += "  [등급별]\n";
        foreach (var kvp in rarityCount.OrderByDescending(x => x.Key))
        {
            cardInfo += $"    {kvp.Key}급 x{kvp.Value}\n";
        }
        
        // 콤보 정보
        string comboInfoText = "";
        if (allCombos.Count == 0)
        {
            comboInfoText = $"콤보: 콤보 없음\n기본 데미지: {totalCardDamage:F1}";
        }
        else if (allCombos.Count == 1)
        {
            YaCht_ComboData comboData = YaCht_ComboDatabase.GetComboData(wrestlerType, allCombos[0]);
            comboInfoText = $"콤보: {comboData.comboName} (x{comboData.damageMultiplier}배)\n설명: {comboData.description}";
        }
        else
        {
            // 여러 콤보가 있을 때
            YaCht_ComboData firstCombo = YaCht_ComboDatabase.GetComboData(wrestlerType, allCombos[0]);
            comboInfoText = $"콤보: {firstCombo.comboName} x{allCombos.Count}회 (x{firstCombo.damageMultiplier}배)\n설명: {firstCombo.description}";
        }
        
        return $"{cardInfo}\n{cardDamageDetails}\n{comboInfoText}\n\n카드 합계: {totalCardDamage:F1}\n콤보 적용: {comboDamage:F1}";
    }
}
