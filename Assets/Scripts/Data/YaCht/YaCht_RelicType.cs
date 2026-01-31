// 유물 타입 정의
public enum YaCht_RelicType
{
    // === 존 시나 고유 유물 ===
    RTKO,                   // RKO 사용 후 영구 데미지 1.2배 증가 (스테이지당 3번 제한)
    YouCantSeeMe,           // 2턴 동안 입힌 데미지 조건에 따라 마지막 턴에 카드당 3~5번 공격
    IHateS,                 // 리롤에 사용된 A랭크 카드 개수에 따라 데미지 증가, S랭크 확률 1%
    
    // === 언더테이커 고유 유물 ===
    RestTombstone,          // 적 체력 40% 이하 시 툼스톤 즉사
    SoulBell,               // 헬즈 게이트 적중 시 모든 데미지 2배
    PurpleGlove,            // 올드 스쿨 적중마다 30% 증가
    
    // === 공용 유물 ===
    FixedMask,              // 카드 한 장 무조건 셋업
    HarmonyMask,            // 달성한 콤보 수만큼 데미지 증가 (최대 40%)
    GamblerMask2,           // S급 확률 15% 증가
    GamblerMask1,           // A급 확률 10% 증가
    JjolBoy,                // 리롤 시 50% 확률로 데미지 10%/30%/50% 증가 혹은 감소
    UnderDogMask            // 모든 카드가 C, D 랭크로 변경, 기술 사용 시 데미지 영구 1.015배 증가
}

// 유물 등급
public enum YaCht_RelicRarity
{
    Common,     // 공용
    Unique      // 고유 (캐릭터 전용)
}
