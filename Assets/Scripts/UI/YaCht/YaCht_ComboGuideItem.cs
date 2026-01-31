using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YaCht_ComboGuideItem : MonoBehaviour
{
    [Header("Fade Effect")]
    [SerializeField] private MultiColorFadeUI m_colorFader;

    [Header("Rarity Image")]
    [SerializeField] private Image m_rarityImage; // 알파벳 등급 이미지

    private YaCht_CardRarity m_rarity;
    private bool m_isHighlighted = false;

    /// <summary>
    /// 초기화: 등급 설정
    /// </summary>
    public void Initialize(YaCht_CardRarity rarity)
    {
        m_rarity = rarity;

        string imagePath = GetImagePathFromRarity(rarity);
        Sprite sprite = Resources.Load<Sprite>(imagePath);
        if (sprite != null)
        {
            m_rarityImage.sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"[ComboGuideItem] 이미지를 찾을 수 없음: {imagePath}");
        }
            
        m_colorFader.Init();

        // 하이라이트 초기화
        ResetHighlight();
}

/// <summary>
/// 하이라이트 설정 (하이라이트 ON)
/// </summary>
public void Highlight()
{
    if (m_isHighlighted) return;

    m_isHighlighted = true;

    // MultiColorFadeUI 설정 후 하이라이트 설정
    if (m_colorFader != null)
    {
        MultiColorFadeUI.ColorType colorType = GetColorTypeFromRarity(m_rarity);
        m_colorFader.StartFadeEffect(colorType);
    }

    Debug.Log($"[ComboGuideItem] {m_rarity} 하이라이트 설정");
}

/// <summary>
/// 하이라이트 초기화 (하이라이트 OFF - 하이라이트 초기화)
/// </summary>
public void ResetHighlight()
{
    if (!m_isHighlighted && m_colorFader != null)
    {
        // 하이라이트 초기화
        m_colorFader.ResetFade();
        return;
    }

    m_isHighlighted = false;

    // ColorFader 설정 후 하이라이트 초기화
    if (m_colorFader != null)
    {
        m_colorFader.StartFadeOutEffect();
    }

    Debug.Log($"[ComboGuideItem] {m_rarity} 하이라이트 초기화 (하이라이트 초기화)");
}

private MultiColorFadeUI.ColorType GetColorTypeFromRarity(YaCht_CardRarity rarity)
{
    switch (rarity)
    {
        case YaCht_CardRarity.D:
            return MultiColorFadeUI.ColorType.Grey;
        case YaCht_CardRarity.C:
            return MultiColorFadeUI.ColorType.Green;
        case YaCht_CardRarity.B:
            return MultiColorFadeUI.ColorType.SkyBlue;
        case YaCht_CardRarity.A:
            return MultiColorFadeUI.ColorType.Red;
        case YaCht_CardRarity.S:
            return MultiColorFadeUI.ColorType.Yellow;
        default:
            return MultiColorFadeUI.ColorType.Green;
    }
}

/// <summary>
/// 등급 비교
/// </summary>
public bool IsRarity(YaCht_CardRarity rarity)
{
    return m_rarity == rarity;
}

/// <summary>
/// 하이라이트 여부 반환
/// </summary>
public bool IsHighlighted => m_isHighlighted;

/// <summary>
/// 등급 반환
/// </summary>
public YaCht_CardRarity GetRarity()
{
    return m_rarity;
}

/// <summary>
/// 등급에 따른 이미지 경로 반환
/// </summary>
private string GetImagePathFromRarity(YaCht_CardRarity rarity)
{
    switch (rarity)
    {
        case YaCht_CardRarity.S:
            return "Sprites/UI/S1";
        case YaCht_CardRarity.A:
            return "Sprites/UI/A1";
        case YaCht_CardRarity.B:
            return "Sprites/UI/B1";
        case YaCht_CardRarity.C:
            return "Sprites/UI/C1";
        case YaCht_CardRarity.D:
            return "Sprites/UI/D1";
        default:
            return "Sprites/UI/C1";
    }
}
}
