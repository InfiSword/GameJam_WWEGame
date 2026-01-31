using UnityEngine;
using System.Collections.Generic;

// ==================================================================================
// 카드 등급 Enum
// ==================================================================================
public enum YaCht_CardRarity
{
    S,  // 10% 확률
    A,  // 20% 확률
    B,  // 30% 확률
    C,  // 40% 확률
    D   // 50% 확률 (기본 확률이 가장 높음)
}

// ==================================================================================
// 카드 데이터 구조체
// ==================================================================================
public struct YaCht_CardData
{
    public string m_name;           // 기술 이름 (조합 체크용)
    public float m_baseDamage;      // 기본 데미지
    public string m_description;    // 설명
    public YaCht_CardRarity m_rarity; // 등급 (확률 계산용)
    public string m_imageResourcePath; // 카드 이미지 리소스 경로 (Resources 폴더 기준)
    
    public YaCht_CardData(string name, float damage, string desc, YaCht_CardRarity rarity = YaCht_CardRarity.D, string imagePath = "")
    {
        m_name = name;
        m_baseDamage = damage;
        m_description = desc;
        m_rarity = rarity;
        m_imageResourcePath = imagePath;
    }
}

// ==================================================================================
// 카드 세트 데이터 클래스
// ==================================================================================
public class YaCht_CardSetData
{
    public string setName;
    public string setDescription;
    public YaCht_WrestlerType wrestlerType;  
    public List<YaCht_CardData> cards;

    public YaCht_CardSetData(string name, string description, YaCht_WrestlerType wrestler, List<YaCht_CardData> cardList)
    {
        setName = name;
        setDescription = description;
        wrestlerType = wrestler;
        cards = cardList;
    }
}

// ==================================================================================
// 카드 데이터베이스 (모든 카드 정의)
// ==================================================================================
public static class YaCht_CardDatabase
{
    // === 존 시나 테마 카드 ===
    
    // S랭크 (10% 확률)
    public static YaCht_CardData AA = new YaCht_CardData("AA", 20, "애티튜드 어저스트먼트 - 존 시나의 피니셔 무브", YaCht_CardRarity.S, "Sprites/Cards/JohnCena/AA");
    public static YaCht_CardData RKO = new YaCht_CardData("RKO", 18, "상대를 바닥에 내리꽂는 피니셔 무브", YaCht_CardRarity.S, "Sprites/Cards/JohnCena/RKO");

    // A랭크 (20% 확률) Naked Choke
    public static YaCht_CardData NakedChoke = new YaCht_CardData("네이키드 초크", 12, "팔을 이용해 상대방의 경동맥을 조르는 기술", YaCht_CardRarity.A, "Sprites/Cards/JohnCena/NakedChoke");
    public static YaCht_CardData Superkick = new YaCht_CardData("슈퍼킥", 10, "높이 점프하여 상대의 턱을 가격하는 강력한 킥", YaCht_CardRarity.A, "Sprites/Cards/JohnCena/Superkick");
    
    // B랭크 (30% 확률)
    public static YaCht_CardData ShoulderTackle = new YaCht_CardData("숄더 태클", 8, "어깨로 상대를 밀어붙이는 기술", YaCht_CardRarity.B, "Sprites/Cards/JohnCena/ShoulderTackle");
    public static YaCht_CardData Dropkick = new YaCht_CardData("드롭킥", 7, "공중에서 양발로 차는 기술", YaCht_CardRarity.B, "Sprites/Cards/JohnCena/Dropkick");
    
    // C랭크 (40% 확률)
    public static YaCht_CardData IrishWhip = new YaCht_CardData("아이리쉬 윕", 6, "상대를 로프로 던지는 기술", YaCht_CardRarity.C, "Sprites/Cards/JohnCena/IrishWhip");
    public static YaCht_CardData KitchenSink = new YaCht_CardData("키친 싱크", 6, "무릎을 들어 상대의 복부를 가격", YaCht_CardRarity.C, "Sprites/Cards/JohnCena/KitchenSink");
    
    // D랭크 (50% 확률)
    public static YaCht_CardData KneeStrike = new YaCht_CardData("니 스트라이크", 4, "무릎으로 가격하는 기본 기술", YaCht_CardRarity.D, "Sprites/Cards/JohnCena/KneeStrike");
    public static YaCht_CardData Headbutt = new YaCht_CardData("박치기", 5, "머리를 사용해서 상대를 치는 기술", YaCht_CardRarity.D, "Sprites/Cards/JohnCena/Headbutt");
    
    // === 언더테이커 테마 카드 ===
    
    // S랭크 (10% 확률)
    public static YaCht_CardData TombstonePiledriver = new YaCht_CardData("툼스톤 파일드라이버", 22, "언더테이커의 최강 피니셔 무브", YaCht_CardRarity.S, "Sprites/Cards/Undertaker/TombstonePiledriver");
    public static YaCht_CardData HellsGate = new YaCht_CardData("헬즈 게이트", 20, "상대를 제압하는 서브미션 기술", YaCht_CardRarity.S, "Sprites/Cards/Undertaker/HellsGate");
    
    // A랭크 (20% 확률)
    public static YaCht_CardData MuscleBuster = new YaCht_CardData("머슬 버스터", 13, "상대를 들어올려 내려찍는 강력한 기술", YaCht_CardRarity.A, "Sprites/Cards/Undertaker/MuscleBuster");
    public static YaCht_CardData RearNakedChoke = new YaCht_CardData("리어 네이키드 초크", 11, "상대의 뒤에서 안고 목을 조르는 기술", YaCht_CardRarity.A, "Sprites/Cards/Undertaker/RearNakedChoke");
    
    // B랭크 (30% 확률)
    public static YaCht_CardData ElbowDrop = new YaCht_CardData("엘보우 드롭", 8, "팔꿈치를 떨어뜨리는 기술", YaCht_CardRarity.B, "Sprites/Cards/Undertaker/ElbowDrop");
    public static YaCht_CardData CornerBodySplash = new YaCht_CardData("코너 바디 스플래쉬", 8, "코너에서 몸으로 압박하는 기술", YaCht_CardRarity.B, "Sprites/Cards/Undertaker/CornerBodySplash");
    public static YaCht_CardData OldSchool = new YaCht_CardData("올드 스쿨", 9, "로프를 타고 팔을 내리치는 언더테이커 시그니처 무브", YaCht_CardRarity.B, "Sprites/Cards/Undertaker/OldSchool");
    
    // C랭크 (40% 확률)
    public static YaCht_CardData CornerIrishWhip = new YaCht_CardData("코너 아이리시 윕", 6, "상대를 코너로 던지는 기술", YaCht_CardRarity.C, "Sprites/Cards/Undertaker/CornerIrishWhip");
    public static YaCht_CardData LowKick = new YaCht_CardData("로우킥", 5, "낮은 다리 높이로 빠른 속도로 기술", YaCht_CardRarity.C, "Sprites/Cards/Undertaker/LowKick");
    
    // D랭크 (50% 확률)
    public static YaCht_CardData Chop = new YaCht_CardData("찹", 4, "손바닥으로 치는 기술", YaCht_CardRarity.D, "Sprites/Cards/Undertaker/Chop");
        
    // 모든 카드 목록
    public static YaCht_CardData[] GetAllCards()
    {
        return new YaCht_CardData[]
        {
            // 존 시나 테마
            AA, RKO, NakedChoke, Superkick,
            ShoulderTackle, Dropkick, IrishWhip, KitchenSink,
            KneeStrike, Headbutt,
            
            // 언더테이커 테마
            TombstonePiledriver, HellsGate, MuscleBuster,
            RearNakedChoke, ElbowDrop, CornerBodySplash, OldSchool,
            CornerIrishWhip, LowKick, Chop
        };
    }
}

// ==================================================================================
// 카드 세트 데이터베이스 (미리 정의된 덱들)
// ==================================================================================
public static class YaCht_CardSetDatabase
{
    // 존 시나 덱 - 존 시나 테마의 카드들로 구성
    public static YaCht_CardSetData JohnCenaSet = new YaCht_CardSetData(
        "존 시나 덱",
        "U CAN'T SEE ME! 존 시나의 시그니처 무브들로 구성되어 있습니다.",
        YaCht_WrestlerType.JohnCena,
        new List<YaCht_CardData>
        {
            // S랭크 (10%)
            YaCht_CardDatabase.AA,
            YaCht_CardDatabase.RKO,
            
            // A랭크 (20%)
            YaCht_CardDatabase.NakedChoke,
            YaCht_CardDatabase.Superkick,
            
            // B랭크 (30%)
            YaCht_CardDatabase.ShoulderTackle,
            YaCht_CardDatabase.Dropkick,
            
            // C랭크 (40%)
            YaCht_CardDatabase.IrishWhip,
            YaCht_CardDatabase.KitchenSink,
            
            // D랭크 (50%)
            YaCht_CardDatabase.KneeStrike,
            YaCht_CardDatabase.Headbutt
        }
    );
    
    // 언더테이커 덱 - 언더테이커 테마의 카드들로 구성
    public static YaCht_CardSetData UndertakerSet = new YaCht_CardSetData(
        "언더테이커 덱",
        "REST IN PEACE! 언더테이커의 시그니처 무브들로 구성되어 있습니다.",
        YaCht_WrestlerType.Undertaker,
        new List<YaCht_CardData>
        {
            // S랭크 (10%)
            YaCht_CardDatabase.TombstonePiledriver,
            YaCht_CardDatabase.HellsGate,
            
            // A랭크 (20%)
            YaCht_CardDatabase.MuscleBuster,            
            YaCht_CardDatabase.RearNakedChoke,
            
            // B랭크 (30%)
            YaCht_CardDatabase.ElbowDrop,
            YaCht_CardDatabase.CornerBodySplash,
            YaCht_CardDatabase.OldSchool,
            
            // C랭크 (40%)
            YaCht_CardDatabase.CornerIrishWhip,
            YaCht_CardDatabase.LowKick,
            
            // D랭크 (50%)
            YaCht_CardDatabase.Chop
        }
    );

    public static YaCht_CardSetData TestSet = new YaCht_CardSetData(
        "기본 기술 세트",
        "D랭크 기술만으로 구성된 테스트 세트입니다.",
        YaCht_WrestlerType.None,
        new List<YaCht_CardData>
        {
            YaCht_CardDatabase.KneeStrike,
            YaCht_CardDatabase.KneeStrike,
            YaCht_CardDatabase.KneeStrike,
            YaCht_CardDatabase.KneeStrike,
            YaCht_CardDatabase.KneeStrike,
            YaCht_CardDatabase.Headbutt,
            YaCht_CardDatabase.Headbutt,
            YaCht_CardDatabase.Headbutt,
            YaCht_CardDatabase.Chop,
            YaCht_CardDatabase.Chop
        }
    );
    
    // 모든 카드 세트 목록
    public static YaCht_CardSetData[] GetAllSets()
    {
        return new YaCht_CardSetData[]
        {
            JohnCenaSet,
            UndertakerSet,
            TestSet
        };
    }
}
