using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 유물 아이콘 버튼 UI 컴포넌트
/// ScrollRect의 GridLayoutGroup에서 사용되는 아이콘 버튼
/// </summary>
public class YaCht_RelicIconButton : MonoBehaviour
{
    [SerializeField] private Image m_iconImage;
    [SerializeField] private Button m_button;

    private YaCht_RelicData m_relicData;
    private Action<YaCht_RelicData> m_onClicked;

    /// <summary>
    /// 유물 아이콘 버튼 초기화
    /// </summary>
    public void Init(YaCht_RelicData relicData, Action<YaCht_RelicData> onClicked)
    {
        m_relicData = relicData;
        m_onClicked = onClicked;

        // 아이콘 이미지 로드
        LoadRelicIcon();

        // 버튼 클릭 이벤트 추가
        if (m_button != null)
        {
            m_button.onClick.RemoveAllListeners();
            m_button.onClick.AddListener(OnButtonClicked);
        }
    }

    /// <summary>
    /// 유물 아이콘 이미지 로드
    /// </summary>
    private void LoadRelicIcon()
    {
        if (m_iconImage == null)
        {
            Debug.LogWarning("[RelicIconButton] Icon Image가 없습니다!");
            return;
        }

        // 아이콘 이미지 경로 사용 (imageIconResourcePath)
        if (!string.IsNullOrEmpty(m_relicData.imageIconResourcePath))
        {
            Sprite loadedSprite = Resources.Load<Sprite>(m_relicData.imageIconResourcePath);

            if (loadedSprite != null)
            {
                m_iconImage.sprite = loadedSprite;
                m_iconImage.enabled = true;
                Debug.Log($"[RelicIconButton] 유물 아이콘 로드 성공: {m_relicData.imageIconResourcePath}");
            }
            else
            {
                Debug.LogWarning($"[RelicIconButton] 유물 아이콘 로드 실패: {m_relicData.imageIconResourcePath}");
                m_iconImage.enabled = false;
            }
        }
        else
        {
            Debug.LogWarning($"[RelicIconButton] {m_relicData.name} 아이콘 경로가 없습니다.");
            m_iconImage.enabled = false;
        }
    }

    /// <summary>
    /// 버튼 클릭 이벤트
    /// </summary>
    private void OnButtonClicked()
    {
        m_onClicked?.Invoke(m_relicData);
    }

    private void OnDestroy()
    {
        if (m_button != null)
        {
            m_button.onClick.RemoveAllListeners();
        }
    }
}
