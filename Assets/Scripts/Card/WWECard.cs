using UnityEngine;

public class WWECard : MonoBehaviour
{
    private CardData m_cardData;
    public CardData GetCardData => m_cardData;

    public void Init(CardData _cardData)
    {
        m_cardData = _cardData;
    }

    void Update()
    {

    }

    // 카드 사용 시 호출되는 메인 메서드
    public void UseCard(TargetState target)
    {
        float finalDamage = 0f;
        finalDamage = m_cardData.m_damageCalculator(m_cardData, target);
        target.m_currentHealth -= finalDamage;

        // 특수 효과 발동 
        m_cardData.m_abilityTrigger(m_cardData, target);
              
        Debug.Log($"{m_cardData.m_name} 사용! {finalDamage} 데미지!");
    }
}
