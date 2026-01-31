using UnityEngine;

// 레슬러 타입
public enum YaCht_WrestlerType
{
    None,
    JohnCena,
    Undertaker
}

// 콤보 레벨
public enum YaCht_ComboLevel
{
    None = 0,
    Combo3 = 3,
    Combo4 = 4,
    Combo5 = 5,
    Combo6 = 6
}

// 조합 종류 타입
public enum YaCht_ComboType
{
    None,
    
    // 존 시나 콤보
    JohnCena_Combo3,
    JohnCena_Combo4,
    JohnCena_Combo5,
    JohnCena_Combo6,
    
    // 언더테이커 콤보
    Undertaker_Combo3,
    Undertaker_Combo4,
    Undertaker_Combo5,
    Undertaker_Combo6
}

// 조합 정보 데이터
[System.Serializable]
public class YaCht_ComboData
{
    public YaCht_ComboType comboType;
    public YaCht_WrestlerType wrestlerType;
    public YaCht_ComboLevel comboLevel;
    public string comboName;
    public string description;
    public int scoreMultiplier;
    public float damageMultiplier;
    
    public YaCht_ComboData(
        YaCht_ComboType type, 
        YaCht_WrestlerType wrestler, 
        YaCht_ComboLevel level, 
        string name, 
        string desc, 
        int score, 
        float damage)
    {
        comboType = type;
        wrestlerType = wrestler;
        comboLevel = level;
        comboName = name;
        description = desc;
        scoreMultiplier = score;
        damageMultiplier = damage;
    }
}

// 조합 데이터베이스
public static class YaCht_ComboDatabase
{
    // 존 시나 콤보 (야추 규칙 적용)
    public static readonly YaCht_ComboData[] JohnCenaCombos = new YaCht_ComboData[]
    {
        // 3개 조합: Three of a Kind (C/D 등급 3개)
        new YaCht_ComboData(
            YaCht_ComboType.JohnCena_Combo3, 
            YaCht_WrestlerType.JohnCena, 
            YaCht_ComboLevel.Combo3,
            "Three of a Kind", 
            "C or D 등급 카드 3개 연속\nEx) 박치기 x3", 
            100, 
            1.5f),
            
        // 4개 조합: Two Pair (C등급 2장 + B등급 2장)
        new YaCht_ComboData(
            YaCht_ComboType.JohnCena_Combo4, 
            YaCht_WrestlerType.JohnCena, 
            YaCht_ComboLevel.Combo4,
            "Two Pair", 
            "C등급 2장 + B등급 2장\nEx) 아이리쉬 윕 x2 + 숄더 태클 x2", 
            250, 
            2.0f),
            
        // 5개 조합: Full House (B등급 2장 + A등급 3장)
        new YaCht_ComboData(
            YaCht_ComboType.JohnCena_Combo5, 
            YaCht_WrestlerType.JohnCena, 
            YaCht_ComboLevel.Combo5,
            "Full House", 
            "B등급 2장 + A등급 3장\nEx) 숄더 태클 x2 + 파이브 너클 셔플 x3", 
            500, 
            3.0f),
            
        // 6개 조합: Triple Pair (S등급 2장 + A등급 2장 + B등급 2장)
        new YaCht_ComboData(
            YaCht_ComboType.JohnCena_Combo6, 
            YaCht_WrestlerType.JohnCena, 
            YaCht_ComboLevel.Combo6,
            "Triple Pair", 
            "S등급 2장 + A등급 2장 + B등급 2장\nEx) AA x2 + RKO x2 + 슈퍼킥 x2 + 드롭킥 x2", 
            1000, 
            5.0f)
    };
    
    // 언더테이커 콤보 (야추 규칙 적용)
    public static readonly YaCht_ComboData[] UndertakerCombos = new YaCht_ComboData[]
    {
        // 3개 조합: Three of a Kind (C/D 각 1개씩)
        new YaCht_ComboData(
            YaCht_ComboType.Undertaker_Combo3, 
            YaCht_WrestlerType.Undertaker, 
            YaCht_ComboLevel.Combo3,
            "Three of a Kind", 
            "C or D 등급 카드 각 1개씩 3개 배치\nEx) 로우킥 + 찹 + 코너 아이리쉬 윕", 
            100, 
            1.5f),
            
        // 4개 조합: Four of a Kind (C 1개 + B 3개)
        new YaCht_ComboData(
            YaCht_ComboType.Undertaker_Combo4, 
            YaCht_WrestlerType.Undertaker, 
            YaCht_ComboLevel.Combo4,
            "Four of a Kind", 
            "C등급 1장 + B등급 3장\nEx) 코너 아이리쉬 윕 + 올드 스쿨 x2 + 엘보우 드롭", 
            250, 
            2.0f),
            
        // 5개 조합: Four Cards (A등급 4장)
        new YaCht_ComboData(
            YaCht_ComboType.Undertaker_Combo5, 
            YaCht_WrestlerType.Undertaker, 
            YaCht_ComboLevel.Combo5,
            "Four Cards", 
            "A등급 4장 이상\nEx) 머슬 버스터 + 리어 네이키드 초크 x3", 
            500, 
            3.0f),
            
        // 6개 조합: Triple Pair (S 2개 + A 3개 + B 1개)
        new YaCht_ComboData(
            YaCht_ComboType.Undertaker_Combo6, 
            YaCht_WrestlerType.Undertaker, 
            YaCht_ComboLevel.Combo6,
            "Triple Pair", 
            "S등급 2장 + A등급 3장 + B등급 1장\nEx) 툼스톤 x2 + 라스트 라이드 x3 + 레그 드롭", 
            1000, 
            5.0f)
    };
    
    // 레슬러 타입에 따른 콤보 가져오기
    public static YaCht_ComboData[] GetCombosByWrestler(YaCht_WrestlerType wrestlerType)
    {
        switch (wrestlerType)
        {
            case YaCht_WrestlerType.JohnCena:
                return JohnCenaCombos;
            case YaCht_WrestlerType.Undertaker:
                return UndertakerCombos;
            default:
                return new YaCht_ComboData[0];
        }
    }
    
    // 레슬러와 콤보 레벨로 콤보 타입 가져오기
    public static YaCht_ComboType GetComboType(YaCht_WrestlerType wrestlerType, YaCht_ComboLevel comboLevel)
    {
        if (comboLevel == YaCht_ComboLevel.None)
            return YaCht_ComboType.None;
            
        switch (wrestlerType)
        {
            case YaCht_WrestlerType.JohnCena:
                switch (comboLevel)
                {
                    case YaCht_ComboLevel.Combo3: return YaCht_ComboType.JohnCena_Combo3;
                    case YaCht_ComboLevel.Combo4: return YaCht_ComboType.JohnCena_Combo4;
                    case YaCht_ComboLevel.Combo5: return YaCht_ComboType.JohnCena_Combo5;
                    case YaCht_ComboLevel.Combo6: return YaCht_ComboType.JohnCena_Combo6;
                }
                break;
                
            case YaCht_WrestlerType.Undertaker:
                switch (comboLevel)
                {
                    case YaCht_ComboLevel.Combo3: return YaCht_ComboType.Undertaker_Combo3;
                    case YaCht_ComboLevel.Combo4: return YaCht_ComboType.Undertaker_Combo4;
                    case YaCht_ComboLevel.Combo5: return YaCht_ComboType.Undertaker_Combo5;
                    case YaCht_ComboLevel.Combo6: return YaCht_ComboType.Undertaker_Combo6;
                }
                break;
        }
        
        return YaCht_ComboType.None;
    }
    
    // 콤보 데이터 가져오기
    public static YaCht_ComboData GetComboData(YaCht_WrestlerType wrestlerType, YaCht_ComboType type)
    {
        switch(wrestlerType)
        {
            case YaCht_WrestlerType.JohnCena:
                foreach (var combo in JohnCenaCombos)
                {
                    if (combo.comboType == type)
                        return combo;
                }
                break;
            case YaCht_WrestlerType.Undertaker:
                foreach (var combo in UndertakerCombos)
                {
                    if (combo.comboType == type)
                        return combo;
                }
                break;  
        }
        
        // None 반환
        return new YaCht_ComboData(
            YaCht_ComboType.None, 
            YaCht_WrestlerType.None, 
            YaCht_ComboLevel.None,
            "조합 없음", 
            "카드를 조합하세요", 
            0, 
            1.0f);
    }
}
