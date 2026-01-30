using UnityEngine;
using System;

// 카드 타입
public enum CardType
{
    Normal,      // 일반 카드
    Stun,        // 기절 적용 카드 
    Floating,    // 띄움
    Fall,        // 넘어짐
    Finisher     // 피니쉬 기술 
}

// 카드 데이터 구조체
public struct CardData
{
    public string m_name;
    public float m_baseDamage;    
    public float m_bonusDamage;
    public int m_cost;
    public CardType m_cardType;
    public string m_description;
    
    // 데미지 계산 함수
    public Func<CardData, TargetState, float> m_damageCalculator;
    public Action<CardData, TargetState> m_abilityTrigger;
}

// 대상 레슬러 상태 (데미지 계산에 필요한 정보)
public class TargetState
{
    public bool m_isStunned;          // 기절 상태 여부
    public float m_maxHealth;           // 최대 체력
    public float m_currentHealth;       // 현재 체력
    
    public float GetHealthLostPercentage()
    {
        return 1f - (m_currentHealth / m_maxHealth);
    }
}