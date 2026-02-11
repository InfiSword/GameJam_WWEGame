using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 유물 정보를 표시하는 UI 컴포넌트
/// Scroll View의 각 항목으로 사용됩니다.
/// </summary>
public class YaCht_RelicInfoItem : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI m_relicNameText;
    [SerializeField] private TextMeshProUGUI m_relicDescriptionText;
    [SerializeField] private TextMeshProUGUI m_statusInfoText;
    [SerializeField] private Image m_relicIconImage;

    private YaCht_RelicData m_relicData;
    private Dictionary<string, string> m_statusInfo;

    /// <summary>
    /// 유물 정보 초기화
    /// </summary>
    public void Init(YaCht_RelicData relicData, Dictionary<string, string> statusInfo)
    {
        m_relicData = relicData;
        m_statusInfo = statusInfo;

        UpdateUI();
    }

    /// <summary>
    /// UI 업데이트
    /// </summary>
    private void UpdateUI()
    {
        // 유물 이름 표시
        if (m_relicNameText != null)
        {
            m_relicNameText.text = m_relicData.name;
        }

        // 유물 설명 표시
        if (m_relicDescriptionText != null)
        {
            m_relicDescriptionText.text = m_relicData.description;
        }

        // 상태 정보 표시
        if (m_statusInfoText != null)
        {
            string statusText = "";
            if (m_statusInfo != null && m_statusInfo.Count > 0)
            {
                foreach (var kvp in m_statusInfo)
                {
                    if (!string.IsNullOrEmpty(statusText))
                    {
                        statusText += "\n";
                    }
                    statusText += $"{kvp.Key}: {kvp.Value}";
                }
            }
            else
            {
                statusText = "현재 활성 상태 정보 없음";
            }

            m_statusInfoText.text = statusText;
        }

        // 유물 아이콘 로드
        LoadRelicIcon();
    }

    /// <summary>
    /// 유물 아이콘 이미지 로드
    /// </summary>
    private void LoadRelicIcon()
    {
        if (m_relicIconImage == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(m_relicData.imageIconResourcePath))
        {
            Sprite loadedSprite = Resources.Load<Sprite>(m_relicData.imageIconResourcePath);

            if (loadedSprite != null)
            {
                m_relicIconImage.sprite = loadedSprite;
                m_relicIconImage.enabled = true;
            }
            else
            {
                m_relicIconImage.enabled = false;
            }
        }
        else
        {
            m_relicIconImage.enabled = false;
        }
    }

    /// <summary>
    /// 상태 정보 업데이트
    /// </summary>
    public void UpdateStatusInfo(Dictionary<string, string> statusInfo)
    {
        m_statusInfo = statusInfo;
        UpdateUI();
    }
}
