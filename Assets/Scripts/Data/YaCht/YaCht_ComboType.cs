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

// 야추 조합 타입 (레슬러별 구조화)
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

// 야추 조합 데이터
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
    public string[] requiredCards;
    
    public YaCht_ComboData(YaCht_ComboType type, YaCht_WrestlerType wrestler, YaCht_ComboLevel level, string name, string desc, int score, float damage, params string[] cards)
    {
        comboType = type;
        wrestlerType = wrestler;
        comboLevel = level;
        comboName = name;
        description = desc;
        scoreMultiplier = score;
        damageMultiplier = damage;
        requiredCards = cards;
    }
}

// 야추 조합표 데이터베이스
public static class YaCht_ComboDatabase
{
    // 존 시나 콤보
    public static readonly YaCht_ComboData[] JohnCenaCombos = new YaCht_ComboData[]
    {
        new YaCht_ComboData(
            YaCht_ComboType.JohnCena_Combo3, 
            YaCht_WrestlerType.JohnCena, 
            YaCht_ComboLevel.Combo3,
            "존 시나 3콤보", 
            "촙! 촙! 로킥!", 
            30, 1.5f, 
            "촙", "촙", "로킥"),
            
        new YaCht_ComboData(
            YaCht_ComboType.JohnCena_Combo4, 
            YaCht_WrestlerType.JohnCena, 
            YaCht_ComboLevel.Combo4,
            "존 시나 4콤보", 
            "잽! 잽! 로킥! 박치기!", 
            50, 2.0f, 
            "잽", "잽", "로킥", "박치기"),
            
        new YaCht_ComboData(
            YaCht_ComboType.JohnCena_Combo5, 
            YaCht_WrestlerType.JohnCena, 
            YaCht_ComboLevel.Combo5,
            "존 시나 5콤보", 
            "하트 펀치 연타 콤보!", 
            80, 2.8f, 
            "하트 펀치", "하트 펀치", "리어 네이키드 초크", "슈퍼킥", "촙"),
            
        new YaCht_ComboData(
            YaCht_ComboType.JohnCena_Combo6, 
            YaCht_WrestlerType.JohnCena, 
            YaCht_ComboLevel.Combo6,
            "존 시나 6콤보 피니셔", 
            "U CAN'T SEE ME! RKO!", 
            120, 4.0f, 
            "박치기", "박치기", "하트 펀치", "하트 펀치", "RKO")
    };
    
    // 언더테이커 콤보
    public static readonly YaCht_ComboData[] UndertakerCombos = new YaCht_ComboData[]
    {
        new YaCht_ComboData(
            YaCht_ComboType.Undertaker_Combo3, 
            YaCht_WrestlerType.Undertaker, 
            YaCht_ComboLevel.Combo3,
            "언더테이커 3콤보", 
            "엘보! 엘보! 바디슬램!", 
            30, 1.5f, 
            "엘보", "엘보", "바디슬램"),
            
        new YaCht_ComboData(
            YaCht_ComboType.Undertaker_Combo4, 
            YaCht_WrestlerType.Undertaker, 
            YaCht_ComboLevel.Combo4,
            "언더테이커 4콤보", 
            "DDT 연타 콤보!", 
            50, 2.0f, 
            "DDT", "DDT", "스피어", "박치기"),
            
        new YaCht_ComboData(
            YaCht_ComboType.Undertaker_Combo5, 
            YaCht_WrestlerType.Undertaker, 
            YaCht_ComboLevel.Combo5,
            "언더테이커 5콤보", 
            "RKO & 슈퍼킥 연계!", 
            80, 2.8f, 
            "RKO", "RKO", "슈퍼킥", "슈퍼킥", "엘보"),
            
        new YaCht_ComboData(
            YaCht_ComboType.Undertaker_Combo6, 
            YaCht_WrestlerType.Undertaker, 
            YaCht_ComboLevel.Combo6,
            "언더테이커 6콤보 피니셔", 
            "REST IN PEACE!", 
            120, 4.0f, 
            "바디슬램", "바디슬램", "DDT", "DDT", "스피어", "스피어")
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
            "조합이 성립되지 않음", 
            0, 1.0f);
    }
}
