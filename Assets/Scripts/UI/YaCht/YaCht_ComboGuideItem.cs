using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YaCht_ComboGuideItem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI m_comboLevelText;
    [SerializeField] private TextMeshProUGUI m_comboNameText;
    [SerializeField] private TextMeshProUGUI m_descriptionText;
    [SerializeField] private TextMeshProUGUI m_cardsText;
    [SerializeField] private TextMeshProUGUI m_effectText;
    [SerializeField] private Image m_backgroundImage;
    
    [Header("Color Settings")]
    [SerializeField] private Color m_combo3Color = new Color(0.8f, 0.9f, 1f);
    [SerializeField] private Color m_combo4Color = new Color(0.8f, 1f, 0.8f);
    [SerializeField] private Color m_combo5Color = new Color(1f, 0.9f, 0.6f);
    [SerializeField] private Color m_combo6Color = new Color(1f, 0.7f, 0.7f);
    
    public void SetComboData(YaCht_ComboData comboData)
    {
        if (comboData == null)
        {
            Debug.LogError("YaCht_ComboGuideItem: comboData is null!");
            return;
        }
        
        Debug.Log($"YaCht_ComboGuideItem: Setting data for {comboData.comboName}");
        
        // 콤보 레벨 표시
        if (m_comboLevelText != null)
        {
            m_comboLevelText.text = $"{(int)comboData.comboLevel}콤보";
        }
        else
        {
            Debug.LogWarning("YaCht_ComboGuideItem: m_comboLevelText is null!");
        }
        
        // 콤보 이름
        if (m_comboNameText != null)
        {
            m_comboNameText.text = comboData.comboName;
        }
        else
        {
            Debug.LogWarning("YaCht_ComboGuideItem: m_comboNameText is null!");
        }
        
        // 설명
        if (m_descriptionText != null)
        {
            m_descriptionText.text = comboData.description;
        }
        else
        {
            Debug.LogWarning("YaCht_ComboGuideItem: m_descriptionText is null!");
        }
        
        // 필요한 카드 조합
        if (m_cardsText != null && comboData.requiredCards != null)
        {
            string cardsStr = "카드: ";
            for (int i = 0; i < comboData.requiredCards.Length; i++)
            {
                cardsStr += comboData.requiredCards[i];
                if (i < comboData.requiredCards.Length - 1)
                {
                    cardsStr += " + ";
                }
            }
            m_cardsText.text = cardsStr;
        }
        else if (m_cardsText == null)
        {
            Debug.LogWarning("YaCht_ComboGuideItem: m_cardsText is null!");
        }
        
        // 효과
        if (m_effectText != null)
        {
            m_effectText.text = $"점수 x{comboData.scoreMultiplier} | 데미지 x{comboData.damageMultiplier:F1}";
        }
        else
        {
            Debug.LogWarning("YaCht_ComboGuideItem: m_effectText is null!");
        }
        
        // 배경색 설정
        if (m_backgroundImage != null)
        {
            switch (comboData.comboLevel)
            {
                case YaCht_ComboLevel.Combo3:
                    m_backgroundImage.color = m_combo3Color;
                    break;
                case YaCht_ComboLevel.Combo4:
                    m_backgroundImage.color = m_combo4Color;
                    break;
                case YaCht_ComboLevel.Combo5:
                    m_backgroundImage.color = m_combo5Color;
                    break;
                case YaCht_ComboLevel.Combo6:
                    m_backgroundImage.color = m_combo6Color;
                    break;
            }
        }
        else
        {
            Debug.LogWarning("YaCht_ComboGuideItem: m_backgroundImage is null!");
        }
    }
}
