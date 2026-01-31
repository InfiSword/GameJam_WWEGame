using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

// 엠블럼 아이템 UI 전체 관리
public class YaCht_RelicItem : MonoBehaviour
{
    [SerializeField] private Image m_iconImage;              // 엠블럼 아이콘 이미지
    [SerializeField] private Button m_selectButton;

    [Header("Selection Animation Settings")]
    [SerializeField] private float m_selectedScale = 1.15f;    // 선택 시 크기 배율
    [SerializeField] private float m_animationDuration = 0.3f;  // 애니메이션 지속 시간
    [SerializeField] private AnimationCurve m_scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private YaCht_RelicData m_relicData;
    private Action m_onSelected;
    private bool m_isSelected = false;
    private Vector3 m_originalScale;
    private Coroutine m_scaleCoroutine;

    private void Awake()
    {
        m_originalScale = transform.localScale;
    }

    public void Init(YaCht_RelicData relicData, Action onSelected)
    {
        m_relicData = relicData;
        m_onSelected = onSelected;

        // 엠블럼 아이콘 이미지 로드
        LoadRelicIcon();
       
        // 선택 버튼 클릭 이벤트 추가
        if (m_selectButton != null)
        {
            m_selectButton.onClick.AddListener(OnClicked);
        }
    }

    /// <summary>
    /// 유물 타입 반환
    /// </summary>
    public YaCht_RelicType GetRelicType()
    {
        return m_relicData.relicType;
    }

    /// <summary>
    /// 엠블럼 아이콘 이미지 로드
    /// </summary>
    private void LoadRelicIcon()
    {
        if (m_iconImage == null)
        {
            Debug.LogWarning("[RelicItem] Icon Image 초기화 실패!");
            return;
        }

        // 엠블럼 아이콘 이미지 로드
        if (!string.IsNullOrEmpty(m_relicData.imageResourcePath))
        {
            Sprite loadedSprite = Resources.Load<Sprite>(m_relicData.imageResourcePath);

            if (loadedSprite != null)
            {
                m_iconImage.sprite = loadedSprite;
                m_iconImage.enabled = true;
                Debug.Log($"[RelicItem] 엠블럼 아이콘 이미지 로드: {m_relicData.imageResourcePath}");
            }
        }
    }

    private void OnClicked()
    {
        m_onSelected?.Invoke();
    }

    /// <summary>
    /// 유물 선택 상태 설정
    /// </summary>
    /// <param name="selected">선택 여부</param>
    public void SetSelected(bool selected)
    {
        if (m_isSelected == selected) return;
        
        m_isSelected = selected;
        
        if (selected)
        {
            // 선택 애니메이션 시작
            if (m_scaleCoroutine != null)
            {
                StopCoroutine(m_scaleCoroutine);
            }
            m_scaleCoroutine = StartCoroutine(ScaleAnimationCoroutine(m_selectedScale));         
           
        }
        else
        {
            // 선택 해제 애니메이션
            if (m_scaleCoroutine != null)
            {
                StopCoroutine(m_scaleCoroutine);
            }
            m_scaleCoroutine = StartCoroutine(ScaleAnimationCoroutine(1f));
            
        }
    }

    /// <summary>
    /// 크기 애니메이션 코루틴
    /// </summary>
    private IEnumerator ScaleAnimationCoroutine(float targetScale)
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = m_originalScale * targetScale;
        float elapsed = 0f;

        while (elapsed < m_animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / m_animationDuration;
            float curveValue = m_scaleCurve.Evaluate(t);
            
            transform.localScale = Vector3.Lerp(startScale, endScale, curveValue);
            yield return null;
        }

        transform.localScale = endScale;
        m_scaleCoroutine = null;
    }


    private void OnDestroy()
    {
        if (m_selectButton != null)
        {
            m_selectButton.onClick.RemoveListener(OnClicked);
        }
        
        if (m_scaleCoroutine != null)
        {
            StopCoroutine(m_scaleCoroutine);
        }
    }
}
