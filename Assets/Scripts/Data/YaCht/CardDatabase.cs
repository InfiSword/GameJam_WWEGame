using UnityEngine;

public static class YaCht_CardDatabase
{
    #region 데미지 계산 함수들
    
    // 일반 데미지 계산
    private static float NormalDamageCalc(YaCht_CardData cardData, YaCht_TargetState target)
    {
        return cardData.m_baseDamage;
    }
    
    // 기절 상태 추가 데미지 계산
    private static float StunBonusDamageCalc(YaCht_CardData cardData, YaCht_TargetState target)
    {
        if (target.m_isStunned)
        {
            return cardData.m_baseDamage * cardData.m_bonusDamage;
        }
        return cardData.m_baseDamage;
    }
    
    // 남은 체력에 비례
    private static float FinisherDamageCalc(YaCht_CardData cardData, YaCht_TargetState target)
    {
        float healthLostPercent = target.GetHealthLostPercentage();
        float damage = cardData.m_baseDamage * (1f + healthLostPercent);
        Debug.Log($"피니셔 발동! 잃은 체력 {healthLostPercent:P0}만큼 추가 - 최종 데미지: {Mathf.RoundToInt(damage)}");
        return damage;
    }
    
    #endregion

    #region 특수 효과 함수들
    
    private static void NoAbility(YaCht_CardData cardData, YaCht_TargetState target)
    {
    }
    
    // 기절 효과 부여
    private static void StunAbility(YaCht_CardData cardData, YaCht_TargetState target)
    {
        target.m_isStunned = true;
    }
    
    #endregion

    #region 일반 카드
    public static YaCht_CardData Chop = new YaCht_CardData
    {
        m_name = "촙",
        m_baseDamage = 5,
        m_cost = 1,
        m_cardType = YaCht_CardType.Normal,
        m_description = "손바닥으로 치는 기술",
        m_damageCalculator = NormalDamageCalc,
        m_abilityTrigger = NoAbility
    };
    
    public static YaCht_CardData LowKick = new YaCht_CardData
    {
        m_name = "로킥",
        m_baseDamage = 5,
        m_cost = 1,
        m_cardType = YaCht_CardType.Normal,
        m_description = "낮은 다리 높이 정도로 차는 발차기 기술",
        m_damageCalculator = NormalDamageCalc,
        m_abilityTrigger = NoAbility
    };
    
    public static YaCht_CardData Jab = new YaCht_CardData
    {
        m_name = "잽",
        m_baseDamage = 4,
        m_cost = 1,
        m_cardType = YaCht_CardType.Normal,
        m_description = "빠른 펀치",
        m_damageCalculator = NormalDamageCalc,
        m_abilityTrigger = NoAbility
    };
    
    public static YaCht_CardData Headbutt = new YaCht_CardData
    {
        m_name = "박치기",
        m_baseDamage = 6,
        m_cost = 1,
        m_cardType = YaCht_CardType.Normal,
        m_description = "머리를 사용해서 상대를 치는 기술",
        m_damageCalculator = NormalDamageCalc,
        m_abilityTrigger = NoAbility
    };
    #endregion

    #region 기절 상태 효과 카드
    public static YaCht_CardData RearNakedChoke = new YaCht_CardData
    {
        m_name = "리어 네이키드 초크",
        m_baseDamage = 8,
        m_cost = 2,
        m_cardType = YaCht_CardType.Stun,
        m_description = "상대의 뒤에서 팔로 목을 감아 조이는 기술. 기절 효과 부여",
        m_damageCalculator = NormalDamageCalc,
        m_abilityTrigger = StunAbility
    };
    
    public static YaCht_CardData HeartPunch = new YaCht_CardData
    {
        m_name = "하트 펀치",
        m_baseDamage = 9,
        m_cost = 2,
        m_cardType = YaCht_CardType.Stun,
        m_description = " 상대의 심장(왼쪽 가슴)을 주먹으로 강하게 타격을 가하는 기술. 기절 효과 부여",
        m_damageCalculator = NormalDamageCalc,
        m_abilityTrigger = StunAbility
    };
    
    // 기절 상태 추가 데미지 카드
    public static YaCht_CardData Superkick = new YaCht_CardData
    {
        m_name = "슈퍼킥",
        m_baseDamage = 10,
        m_bonusDamage = 1.6f,  // 기절 시 추가 데미지
        m_cost = 2,
        m_cardType = YaCht_CardType.Stun,
        m_description = "한 발을 상태에서 몸체를 반대로 이용해 상대의 턱이나 목덜미에 중돌하듯 킥하치는 기술. 기절 상태 시 추가 데미지",
        m_damageCalculator = StunBonusDamageCalc,
        m_abilityTrigger = NoAbility 
    };
    #endregion

    #region 피니셔 카드
    public static YaCht_CardData RKO = new YaCht_CardData
    {
        m_name = "RKO",
        m_baseDamage = 15,
        m_cost = 3,
        m_cardType = YaCht_CardType.Finisher,
        m_description = "상대의 잃은 체력에 비례하여 데미지가 증가하는 기술. 피니셔 카드",
        m_damageCalculator = FinisherDamageCalc,
        m_abilityTrigger = NoAbility
    };
    #endregion
}
