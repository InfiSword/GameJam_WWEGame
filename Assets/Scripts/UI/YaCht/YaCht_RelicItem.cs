using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

// 유물 아이템 UI 컴포넌트
public class YaCht_RelicItem : MonoBehaviour
{
    [SerializeField] private Image m_iconImage;              // 유물 아이콘 이미지
    [SerializeField] private Button m_selectButton;

    private YaCht_RelicData m_relicData;
    private Action m_onSelected;

    public void Init(YaCht_RelicData relicData, Action onSelected)
    {
        m_relicData = relicData;
        m_onSelected = onSelected;

        // 유물 이미지 로드
        LoadRelicIcon();
       
        // 버튼 이벤트
        if (m_selectButton != null)
        {
            m_selectButton.onClick.AddListener(OnClicked);
        }
    }

    /// <summary>
    /// 유물 아이콘 이미지를 Resources에서 로드
    /// </summary>
    private void LoadRelicIcon()
    {
        if (m_iconImage == null)
        {
            Debug.LogWarning("[RelicItem] Icon Image가 할당되지 않았습니다!");
            return;
        }

        // 이미지 경로가 지정되어 있으면 로드
        if (!string.IsNullOrEmpty(m_relicData.imageResourcePath))
        {
            Sprite loadedSprite = Resources.Load<Sprite>(m_relicData.imageResourcePath);

            if (loadedSprite != null)
            {
                m_iconImage.sprite = loadedSprite;
                m_iconImage.enabled = true;
                Debug.Log($"[RelicItem] 이미지 로드 성공: {m_relicData.imageResourcePath}");
            }
            else
            {
                Debug.LogWarning($"[RelicItem] 이미지를 찾을 수 없음: {m_relicData.imageResourcePath}");
                m_iconImage.enabled = false;
            }
        }
        else
        {
            Debug.LogWarning($"[RelicItem] {m_relicData.name}의 이미지 경로가 지정되지 않았습니다.");
            m_iconImage.enabled = false;
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
