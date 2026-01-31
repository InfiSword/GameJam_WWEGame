using UnityEngine;

// 유물 데이터 구조체
public struct YaCht_RelicData
{
    public YaCht_RelicType relicType;
    public string name;
    public string description;
    public YaCht_RelicRarity rarity;
    public YaCht_WrestlerType requiredWrestler; // None이면 공용
    public string imageResourcePath; // 이미지 리소스 경로 추가

    public YaCht_RelicData(YaCht_RelicType type, string name, string desc, YaCht_RelicRarity rarity, YaCht_WrestlerType wrestler = YaCht_WrestlerType.None, string imagePath = "")
    {
        relicType = type;
        this.name = name;
        description = desc;
        this.rarity = rarity;
        requiredWrestler = wrestler;
        imageResourcePath = imagePath;
    }
}

// 유물 데이터베이스
public static class YaCht_RelicDatabase
{
    // === 존 시나 전용 유물 ===
    public static YaCht_RelicData AAA = new YaCht_RelicData(
        YaCht_RelicType.AAA,
        "AAA",
        "AA를 사용한 턴에 해당 턴의 데미지가 2배가 됩니다.",
        YaCht_RelicRarity.Unique,
        YaCht_WrestlerType.JohnCena,
        "Sprites/Cards/Relics/AAA"
    );

    public static YaCht_RelicData RTKO = new YaCht_RelicData(
        YaCht_RelicType.RTKO,
        "RTKO",
        "RKO를 사용하면 사용횟수만큼 영구적으로 데미지가 2배씩 증가합니다.",
        YaCht_RelicRarity.Unique,
        YaCht_WrestlerType.JohnCena,
        "Sprites/Cards/Relics/RTKO"
    );

    public static YaCht_RelicData DiamondKnuckle = new YaCht_RelicData(
        YaCht_RelicType.DiamondKnuckle,
        "다이아몬드 너클",
        "기본 데미지 20% 증가. 파이브 너클 셔플을 사용하면 2배가 됩니다.",
        YaCht_RelicRarity.Unique,
        YaCht_WrestlerType.JohnCena,
        "Sprites/Cards/Relics/DiamondKnuckle"
    );

    // === 언더테이커 전용 유물 ===
    public static YaCht_RelicData RestTombstone = new YaCht_RelicData(
        YaCht_RelicType.RestTombstone,
        "안식의 비석",
        "체력이 40% 이하인 적에게 툼스톤 파일드라이버 사용 시 즉시 처치합니다.",
        YaCht_RelicRarity.Unique,
        YaCht_WrestlerType.Undertaker,
        "Sprites/Cards/Relics/RestTombstone"
    );

    public static YaCht_RelicData SoulBell = new YaCht_RelicData(
        YaCht_RelicType.SoulBell,
        "영혼의 종",
        "헬즈 게이트 사용 시 해당 턴의 모든 데미지가 2배가 됩니다.",
        YaCht_RelicRarity.Unique,
        YaCht_WrestlerType.Undertaker,
        "Sprites/Cards/Relics/SoulBell"
    );

    public static YaCht_RelicData PurpleGlove = new YaCht_RelicData(
        YaCht_RelicType.PurpleGlove,
        "보라색 장갑",
        "올드 스쿨 사용 시마다 데미지가 30%씩 영구적으로 증가합니다.",
        YaCht_RelicRarity.Unique,
        YaCht_WrestlerType.Undertaker,
        "Sprites/Cards/Relics/PurpleGlove"
    );

    // === 공통 유물 ===
    public static YaCht_RelicData FixedMask = new YaCht_RelicData(
        YaCht_RelicType.FixedMask,
        "고정의 가면",
        "각 라운드 시작마다 랜덤한 카드 1장이 자동으로 셋업됩니다.",
        YaCht_RelicRarity.Common,
        YaCht_WrestlerType.None,
        "Sprites/Cards/Relics/FixedMask"
    );

    public static YaCht_RelicData HarmonyMask = new YaCht_RelicData(
        YaCht_RelicType.HarmonyMask,
        "화합의 가면",
        "달성한 콤보 개수만큼 데미지 증가 (콤보당 4%, 최대 40%).",
        YaCht_RelicRarity.Common,
        YaCht_WrestlerType.None,
        "Sprites/Cards/Relics/HarmonyMask"
    );

    public static YaCht_RelicData GamblerMask2 = new YaCht_RelicData(
        YaCht_RelicType.GamblerMask2,
        "도박사의 가면 II",
        "S급 카드 드로우 확률 15% 증가 (D급 확률 15% 감소).",
        YaCht_RelicRarity.Common,
        YaCht_WrestlerType.None,
        "Sprites/Cards/Relics/GamblerMask2"
    );

    public static YaCht_RelicData MercyMask = new YaCht_RelicData(
        YaCht_RelicType.MercyMask,
        "자비의 가면",
        "Easy/Normal 난이도 콤보 성공 시 다음 턴 리롤 횟수 +1.",
        YaCht_RelicRarity.Common,
        YaCht_WrestlerType.None,
        "Sprites/Cards/Relics/MercyMask"
    );

    public static YaCht_RelicData RageMask = new YaCht_RelicData(
        YaCht_RelicType.RageMask,
        "분노의 가면",
        "모든 데미지 20% 증가.",
        YaCht_RelicRarity.Common,
        YaCht_WrestlerType.None,
        "Sprites/Cards/Relics/RageMask"
    );

    public static YaCht_RelicData GamblerMask1 = new YaCht_RelicData(
        YaCht_RelicType.GamblerMask1,
        "도박사의 가면 I",
        "A급 카드 드로우 확률 10% 증가 (D급 확률 10% 감소).",
        YaCht_RelicRarity.Common,
        YaCht_WrestlerType.None,
        "Sprites/Cards/Relics/GamblerMask1"
    );

    // 모든 유물 목록
    public static YaCht_RelicData[] GetAllRelics()
    {
        return new YaCht_RelicData[]
        {
            // 존 시나
            AAA, RTKO, DiamondKnuckle,
            // 언더테이커
            RestTombstone, SoulBell, PurpleGlove,
            // 공용
            FixedMask, HarmonyMask, GamblerMask2, MercyMask, RageMask, GamblerMask1
        };
    }

    // 유물 타입으로 데이터 가져오기
    public static YaCht_RelicData GetRelicData(YaCht_RelicType type)
    {
        foreach (var relic in GetAllRelics())
        {
            if (relic.relicType == type)
                return relic;
        }
        return default(YaCht_RelicData);
    }
}
