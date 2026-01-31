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
    public string imageIconResourcePath; // 아이콘 이미지 리소스 경로 추가

    public YaCht_RelicData(YaCht_RelicType type, string name, string desc, YaCht_RelicRarity rarity, YaCht_WrestlerType wrestler = YaCht_WrestlerType.None, string imagePath = "", string iconPath = "")
    {
        relicType = type;
        this.name = name;
        description = desc;
        this.rarity = rarity;
        requiredWrestler = wrestler;
        imageResourcePath = imagePath;
        imageIconResourcePath = iconPath; 
    }
}

// 유물 데이터베이스
public static class YaCht_RelicDatabase
{
    // === 존 시나 전용 유물 ===
    public static YaCht_RelicData RTKO = new YaCht_RelicData(
        YaCht_RelicType.RTKO,
        "RTKO",
        "RKO 카드를 사용하면 데미지가 영구적으로 1.2배 증가합니다. 단, 스테이지당 3번이 제한됩니다.",
        YaCht_RelicRarity.Unique,
        YaCht_WrestlerType.JohnCena,
        "Sprites/Cards/Relics/RTKO",
        "Sprites/Cards/Relics/Icons/RTKO"
    );

    public static YaCht_RelicData YouCantSeeMe = new YaCht_RelicData(
        YaCht_RelicType.YouCantSeeMe,
        "You Can't See Me",
        "2턴 동안 입힌 데미지의 조건에 맞으면 마지막 턴에 업셋한 카드 1장당 3~5번 공격합니다. (3회=200+, 4회=300+, 5회=400+)",
        YaCht_RelicRarity.Unique,
        YaCht_WrestlerType.JohnCena,
        "Sprites/Cards/Relics/YouCantSeeMe",
        "Sprites/Cards/Relics/Icons/YouCantSeeMe"
    );

    public static YaCht_RelicData IHateS = new YaCht_RelicData(
        YaCht_RelicType.IHateS,
        "I Hate S",
        "리롤에 사용된 A랭크 기술들의 개수에 따라 최소 1.25배에서 무한정 증가합니다. (스테이지 이동 시 초기화, S랭크 확률 1%, S랭크 획득 시 중첩 0)",
        YaCht_RelicRarity.Unique,
        YaCht_WrestlerType.JohnCena,
        "Sprites/Cards/Relics/IHateS",
        "Sprites/Cards/Relics/Icons/IHateS"
    );

    // === 언더테이커 전용 유물 ===
    public static YaCht_RelicData RestTombstone = new YaCht_RelicData(
        YaCht_RelicType.RestTombstone,
        "안식의 비석",
        "S랭크 기술들의 대미지가 0.7배가 됩니다. HP가 10% 이하인 적에게 S랭크 기술 적중 시 즉시 처치합니다.",
        YaCht_RelicRarity.Unique,
        YaCht_WrestlerType.Undertaker,
        "Sprites/Cards/Relics/RestTombstone",
        "Sprites/Cards/Relics/Icons/RestTombstone"
    );

    public static YaCht_RelicData SoulBell = new YaCht_RelicData(
        YaCht_RelicType.SoulBell,
        "영혼의 종",
        "처치한 상대방들의 HP의 (20% / 10% / 5%)가 추가 대미지로 적용됩니다. (1스테이지: 20%, 2스테이지: 10%, 3스테이지 이상: 5%)",
        YaCht_RelicRarity.Unique,
        YaCht_WrestlerType.Undertaker,
        "Sprites/Cards/Relics/SoulBell",
        "Sprites/Cards/Relics/Icons/SoulBell"
    );

    public static YaCht_RelicData PurpleGlove = new YaCht_RelicData(
        YaCht_RelicType.PurpleGlove,
        "보라색 장갑",
        "A랭크 기술 적중 시마다 영구적으로 대미지 1.1배씩 상승합니다. (스테이지 최대 3번 중첩 가능)",
        YaCht_RelicRarity.Unique,
        YaCht_WrestlerType.Undertaker,
        "Sprites/Cards/Relics/PurpleGlove",
        "Sprites/Cards/Relics/Icons/PurpleGlove"
    );

    // === 공통 유물 ===
    public static YaCht_RelicData FixedMask = new YaCht_RelicData(
        YaCht_RelicType.FixedMask,
        "고정의 가면",
        "각 라운드 시작마다 랜덤한 카드 1장이 셋업됩니다. (셋업에서 내릴 수 없음)",
        YaCht_RelicRarity.Common,
        YaCht_WrestlerType.None,
        "Sprites/Cards/Relics/FixedMask",
        "Sprites/Cards/Relics/Icons/FixedMask"
    );

    public static YaCht_RelicData HarmonyMask = new YaCht_RelicData(
        YaCht_RelicType.HarmonyMask,  
        "화합의 가면",
        "달성한 콤보 갯수 만큼 대미지 4% 상승 (최대 40% 중첩)",
        YaCht_RelicRarity.Common,
        YaCht_WrestlerType.None,
        "Sprites/Cards/Relics/HarmonyMask",
        "Sprites/Cards/Relics/Icons/HarmonyMask"
    );

    public static YaCht_RelicData GamblerMask2 = new YaCht_RelicData(
        YaCht_RelicType.GamblerMask2,
        "행운의 가면 II",
        "공격 시 일정 확률로 한 번 더 공격합니다. 확률은 랭크에 따라 다릅니다. (D:90%, C:75%, B:50%, A:15%, S:3%)",
        YaCht_RelicRarity.Common,
        YaCht_WrestlerType.None,
        "Sprites/Cards/Relics/GamblerMask2",
        "Sprites/Cards/Relics/Icons/GamblerMask2"
    );

    public static YaCht_RelicData GamblerMask1 = new YaCht_RelicData(
        YaCht_RelicType.GamblerMask1,
        "행운의 가면 I",
        "S급 카드 확률 15% 증가 (D급 카드 확률 15% 감소)",
        YaCht_RelicRarity.Common,
        YaCht_WrestlerType.None,
        "Sprites/Cards/Relics/GamblerMask1",
        "Sprites/Cards/Relics/Icons/GamblerMask1"
    );

    public static YaCht_RelicData JjolBoy = new YaCht_RelicData(
        YaCht_RelicType.JjolBoy,
        "쫄리는 보이",
        "리롤을 돌릴 때마다 50% 확률로 데미지가 10%/30%/50% 증가 혹은 감소합니다.",
        YaCht_RelicRarity.Common,
        YaCht_WrestlerType.None,
        "Sprites/Cards/Relics/JjolBoy",
        "Sprites/Cards/Relics/Icons/JjolBoy"
    );

    public static YaCht_RelicData UnderDogMask = new YaCht_RelicData(
        YaCht_RelicType.UnderDogMask,
        "언더독의 가면",
        "모든 카드들이 C, D 랭크 카드로 바뀝니다. 기술을 사용할 때마다 데미지가 영구적으로 1.015배씩 올라갑니다.",
        YaCht_RelicRarity.Common,
        YaCht_WrestlerType.None,
        "Sprites/Cards/Relics/UnderDogMask",
        "Sprites/Cards/Relics/Icons/UnderDogMask"
    );

    // 모든 유물 목록
    public static YaCht_RelicData[] GetAllRelics()
    {
        return new YaCht_RelicData[]
        {
            // 존 시나
            RTKO, YouCantSeeMe, IHateS,
            // 언더테이커
            RestTombstone, SoulBell, PurpleGlove,
            // 공용
            FixedMask, HarmonyMask, GamblerMask2, GamblerMask1, JjolBoy, UnderDogMask
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
