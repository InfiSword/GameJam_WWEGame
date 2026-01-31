using UnityEngine;

// 유물 데이터 구조체
public struct YaCht_RelicData
{
    public YaCht_RelicType relicType;
    public string name;
    public string description;
    public YaCht_RelicRarity rarity;
    public YaCht_WrestlerType requiredWrestler; // None이면 공용

    public YaCht_RelicData(YaCht_RelicType type, string name, string desc, YaCht_RelicRarity rarity, YaCht_WrestlerType wrestler = YaCht_WrestlerType.None)
    {
        relicType = type;
        this.name = name;
        description = desc;
        this.rarity = rarity;
        requiredWrestler = wrestler;
    }
}

// 유물 데이터베이스
public static class YaCht_RelicDatabase
{
    // === 존 시나 고유 유물 ===
    public static YaCht_RelicData AAA = new YaCht_RelicData(
        YaCht_RelicType.AAA,
        "AAA",
        "AA를 사용한 이후 해당 턴의 데미지가 2배가 됩니다.",
        YaCht_RelicRarity.Unique,
        YaCht_WrestlerType.JohnCena
    );

    public static YaCht_RelicData RTKO = new YaCht_RelicData(
        YaCht_RelicType.RTKO,
        "RTKO",
        "RKO를 사용하면 앞으로의 모든 데미지가 영구적으로 2배가 됩니다.",
        YaCht_RelicRarity.Unique,
        YaCht_WrestlerType.JohnCena
    );

    public static YaCht_RelicData DiamondKnuckle = new YaCht_RelicData(
        YaCht_RelicType.DiamondKnuckle,
        "다이아몬드 너클",
        "기본 데미지 20% 증가. 파이브 너클 셔플의 데미지는 2배가 됩니다.",
        YaCht_RelicRarity.Unique,
        YaCht_WrestlerType.JohnCena
    );

    // === 언더테이커 고유 유물 ===
    public static YaCht_RelicData RestTombstone = new YaCht_RelicData(
        YaCht_RelicType.RestTombstone,
        "안식의 비석",
        "체력이 40% 이하인 적에게 툼스톤 파일드라이버 적중 시 즉시 처치합니다.",
        YaCht_RelicRarity.Unique,
        YaCht_WrestlerType.Undertaker
    );

    public static YaCht_RelicData SoulBell = new YaCht_RelicData(
        YaCht_RelicType.SoulBell,
        "영혼의 종",
        "헬즈 게이트 적중 시 해당 턴의 모든 데미지가 2배가 됩니다.",
        YaCht_RelicRarity.Unique,
        YaCht_WrestlerType.Undertaker
    );

    public static YaCht_RelicData PurpleGlove = new YaCht_RelicData(
        YaCht_RelicType.PurpleGlove,
        "보라색 장갑",
        "올드 스쿨 적중 시마다 데미지가 30%씩 영구적으로 상승합니다.",
        YaCht_RelicRarity.Unique,
        YaCht_WrestlerType.Undertaker
    );

    // === 공용 유물 ===
    public static YaCht_RelicData FixedMask = new YaCht_RelicData(
        YaCht_RelicType.FixedMask,
        "고정의 가면",
        "매 턴마다 덱에서 무작위 카드 1장이 자동으로 셋업됩니다.",
        YaCht_RelicRarity.Common
    );

    public static YaCht_RelicData HarmonyMask = new YaCht_RelicData(
        YaCht_RelicType.HarmonyMask,
        "화합의 가면",
        "달성한 콤보 수만큼 데미지 증가 (콤보당 4%, 최대 40%).",
        YaCht_RelicRarity.Common
    );

    public static YaCht_RelicData GamblerMask2 = new YaCht_RelicData(
        YaCht_RelicType.GamblerMask2,
        "도박사의 가면 II",
        "S급 카드 출현 확률 15% 증가 (D급 확률 15% 감소).",
        YaCht_RelicRarity.Common
    );

    public static YaCht_RelicData MercyMask = new YaCht_RelicData(
        YaCht_RelicType.MercyMask,
        "자비의 가면",
        "Easy/Normal 난이도 콤보 성공 시 다음 턴 리롤 횟수 +1.",
        YaCht_RelicRarity.Common
    );

    public static YaCht_RelicData RageMask = new YaCht_RelicData(
        YaCht_RelicType.RageMask,
        "분노의 가면",
        "모든 데미지 20% 증가.",
        YaCht_RelicRarity.Common
    );

    public static YaCht_RelicData GamblerMask1 = new YaCht_RelicData(
        YaCht_RelicType.GamblerMask1,
        "도박사의 가면 I",
        "A급 카드 출현 확률 10% 증가 (D급 확률 10% 감소).",
        YaCht_RelicRarity.Common
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
