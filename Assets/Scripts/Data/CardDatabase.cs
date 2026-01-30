using UnityEngine;

public static class CardDatabase
{
    #region 데미지 계산 함수들
    
    // 일반 데미지 계산
    private static float NormalDamageCalc(CardData cardData, TargetState target)
    {
        return cardData.m_baseDamage;
    }
    
    // 기절 상태 추가 피해 계산
    private static float StunBonusDamageCalc(CardData cardData, TargetState target)
    {
        if (target.m_isStunned)
        {
            return cardData.m_baseDamage * cardData.m_bonusDamage;
        }
        return cardData.m_baseDamage;
    }
    
    // 잃은 체력에 비례
    private static float FinisherDamageCalc(CardData cardData, TargetState target)
    {
        float healthLostPercent = target.GetHealthLostPercentage();
        float damage = cardData.m_baseDamage * (1f + healthLostPercent);
        Debug.Log($"피니쉬 데미지! 잃은 체력 {healthLostPercent:P0}에 비례 - 최종 데미지: {Mathf.RoundToInt(damage)}");
        return damage;
    }
    
    #endregion

    #region 특수 효과 함수들
    
    private static void NoAbility(CardData cardData, TargetState target)
    {
    }
    
    // 기절 효과 부여
    private static void StunAbility(CardData cardData, TargetState target)
    {
        target.m_isStunned = true;
    }
    
    #endregion

    #region 일반 카드
    public static CardData Chop = new CardData
    {
        m_name = "찹",
        m_baseDamage = 5,
        m_cost = 1,
        m_cardType = CardType.Normal,
        m_description = "손바닥으로 치는 기술",
        m_damageCalculator = NormalDamageCalc,
        m_abilityTrigger = NoAbility
    };
    
    public static CardData LowKick = new CardData
    {
        m_name = "로우킥",
        m_baseDamage = 5,
        m_cost = 1,
        m_cardType = CardType.Normal,
        m_description = "상대방 다리 안쪽 허벅지를 노려 차는 기술",
        m_damageCalculator = NormalDamageCalc,
        m_abilityTrigger = NoAbility
    };
    
    public static CardData Jab = new CardData
    {
        m_name = "잽",
        m_baseDamage = 4,
        m_cost = 1,
        m_cardType = CardType.Normal,
        m_description = "빠른 펀치",
        m_damageCalculator = NormalDamageCalc,
        m_abilityTrigger = NoAbility
    };
    
    public static CardData Headbutt = new CardData
    {
        m_name = "박치기",
        m_baseDamage = 6,
        m_cost = 1,
        m_cardType = CardType.Normal,
        m_description = "머리를 사용해서 상대방을 치는 기술",
        m_damageCalculator = NormalDamageCalc,
        m_abilityTrigger = NoAbility
    };
    #endregion

    #region 기절 관련 효과 카드
    public static CardData RearNakedChoke = new CardData
    {
        m_name = "리어 네이키드 초크",
        m_baseDamage = 8,
        m_cost = 2,
        m_cardType = CardType.Stun,
        m_description = "상대의 뒤에서 목을 팔로 감싸 조르는 기술. 기절 효과 부여",
        m_damageCalculator = NormalDamageCalc,
        m_abilityTrigger = StunAbility
    };
    
    public static CardData HeartPunch = new CardData
    {
        m_name = "하트 펀치",
        m_baseDamage = 9,
        m_cost = 2,
        m_cardType = CardType.Stun,
        m_description = " 상대의 가슴(심장 부근)을 주먹으로 강하게 정타로 가격하는 기술. 기절 효과 부여",
        m_damageCalculator = NormalDamageCalc,
        m_abilityTrigger = StunAbility
    };
    
    // 기절 상태 추가 피해 카드
    public static CardData Superkick = new CardData
    {
        m_name = "슈퍼킥",
        m_baseDamage = 10,
        m_bonusDamage = 1.6f,  // 기절 시 추가 데미지
        m_cost = 2,
        m_cardType = CardType.Stun,
        m_description = "서 있는 상태에서 상체의 반동을 이용해 상대의 턱이나 관자놀이를 발등으로 후려치는 기술. 기절 상태 시 추가 피해",
        m_damageCalculator = StunBonusDamageCalc,
        m_abilityTrigger = NoAbility 
    };
    #endregion

    #region 필살기 카드
    public static CardData RKO = new CardData
    {
        m_name = "RKO",
        m_baseDamage = 15,
        m_cost = 3,
        m_cardType = CardType.Finisher,
        m_description = "상대방의 잃은 체력에 비례하여 데미지가 높아지는 기술. 필살기 카드",
        m_damageCalculator = FinisherDamageCalc,
        m_abilityTrigger = NoAbility
    };
    #endregion
}
