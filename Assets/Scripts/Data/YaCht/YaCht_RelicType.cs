// 유물 타입 정의
public enum YaCht_RelicType
{
    // === 존 시나 고유 유물 ===
    AAA,                    // AA 사용 후 데미지 2배
    RTKO,                   // RKO 사용 후 영구 데미지 2배
    DiamondKnuckle,         // 기본 데미지 20% 업, 파이브 너클 셔플 2배
    
    // === 언더테이커 고유 유물 ===
    RestTombstone,          // 적 체력 40% 이하 시 툼스톤 즉사
    SoulBell,               // 헬즈 게이트 적중 시 모든 데미지 2배
    PurpleGlove,            // 올드 스쿨 적중마다 30% 증가
    
    // === 공용 유물 ===
    FixedMask,              // 카드 한 장 무조건 셋업
    HarmonyMask,            // 달성한 콤보 수만큼 데미지 증가 (최대 40%)
    GamblerMask2,           // S급 확률 15% 증가
    MercyMask,              // Easy/Normal 성공 시 다음 턴 리롤 +1
    RageMask,               // 데미지 20% 증가
    GamblerMask1            // A급 확률 10% 증가
}

// 유물 등급
public enum YaCht_RelicRarity
{
    Common,     // 공용
    Unique      // 고유 (캐릭터 전용)
}
