using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

// 유물 아이템 UI 컴포넌트
public class YaCht_RelicItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_nameText;
    [SerializeField] private TextMeshProUGUI m_descriptionText;
    [SerializeField] private Button m_selectButton;
    [SerializeField] private Image m_backgroundImage;

    private YaCht_RelicData m_relicData;
    private Action m_onSelected;

    public void Init(YaCht_RelicData relicData, Action onSelected)
    {
        m_relicData = relicData;
        m_onSelected = onSelected;

        // UI 업데이트
        if (m_nameText != null)
        {
            m_nameText.text = relicData.name;
        }

        if (m_descriptionText != null)
        {
            m_descriptionText.text = relicData.description;
        }

        // 등급에 따른 색상 (옵션)
        if (m_backgroundImage != null)
        {
            switch (relicData.rarity)
            {
                case YaCht_RelicRarity.Common:
                    m_backgroundImage.color = new Color(0.8f, 0.8f, 0.8f); // 회색
                    break;
                case YaCht_RelicRarity.Unique:
                    m_backgroundImage.color = new Color(1.0f, 0.84f, 0.0f); // 금색
                    break;
            }
        }

        // 버튼 이벤트
        if (m_selectButton != null)
        {
            m_selectButton.onClick.AddListener(OnClicked);
        }
    }

    private void OnClicked()
    {
        m_onSelected?.Invoke();
    }

    private void OnDestroy()
    {
        if (m_selectButton != null)
        {
            m_selectButton.onClick.RemoveListener(OnClicked);
        }
    }
}
