using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 콤보 가이드 UI 전체 관리
/// - Row 리스트별 콤보 표시
/// - 덱에 넣은 카드 정보로 자동 하이라이트 적용
/// - Tab 키로 토글 표시
/// </summary>
public class YaCht_ComboGuideUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private TextMeshProUGUI m_titleText;
    [SerializeField] private Transform m_comboListParent;
    [SerializeField] private GameObject m_comboRowPrefab; // YaCht_ComboGuideRow 프리팹

    [Header("Settings")]
    [SerializeField] private KeyCode m_toggleKey = KeyCode.Tab;
    [SerializeField] private bool m_autoUpdateOnSetup = true; // 카드 설정이 변경되면 자동 하이라이트 적용
    
    private bool m_isVisible = false;
    private bool m_isInitialized = false;
    private YaCht_WrestlerType m_currentWrestler;
    private List<YaCht_ComboGuideRow> m_comboRows = new List<YaCht_ComboGuideRow>();
    
    void Awake()
    {
        if (m_canvasGroup == null)
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
            if (m_canvasGroup == null)
            {
                m_canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
    }
    
    void Start()
    {
        m_isVisible = false;
        SetVisibility(false);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(m_toggleKey))
        {
            ToggleGuide();
        }
    }
    
    /// <summary>
    /// 초기화: 워스터 타입 설정
    /// </summary>
    public void Initialize(YaCht_WrestlerType wrestlerType)
    {
        if (m_isInitialized && m_currentWrestler == wrestlerType)
        {
            return;
        }
        
        m_currentWrestler = wrestlerType;
        BuildComboList(wrestlerType);
        m_isInitialized = true;
        
        Debug.Log($"[ComboGuideUI] {wrestlerType} 콤보 가이드 초기화 완료");
    }
    
    /// <summary>
    /// 토글 표시
    /// </summary>
    public void ToggleGuide()
    {
        m_isVisible = !m_isVisible;
        SetVisibility(m_isVisible);
        
        // 카드 설정이 변경되면 하이라이트 적용
        if (m_isVisible)
        {
            RefreshHighlights();
        }
        
        Debug.Log($"[ComboGuideUI] 콤보 가이드 {(m_isVisible ? "표시" : "숨김")}");
    }
    
    /// <summary>
    /// 콤보 가이드 UI 표시/숨김
    /// </summary>
    private void SetVisibility(bool visible)
    {
        if (m_canvasGroup != null)
        {
            m_canvasGroup.alpha = visible ? 1f : 0f;
            m_canvasGroup.interactable = visible;
            m_canvasGroup.blocksRaycasts = visible;
        }
    }
    
    /// <summary>
    /// 콤보 데이터 목록 생성
    /// </summary>
    private void BuildComboList(YaCht_WrestlerType wrestlerType)
    {
        if (m_comboListParent == null)
        {
            Debug.LogWarning("[ComboGuideUI] ComboListParent 초기화 실패!");
            return;
        }
        
        // 콤보 Row 초기화
        ClearComboRows();
        
        // 워스터 이름 설정
        if (m_titleText != null)
        {
            string wrestlerName = wrestlerType == YaCht_WrestlerType.JohnCena ? "존 세나" :
                                  wrestlerType == YaCht_WrestlerType.Undertaker ? "업퍼테이커" : "데이브 슈워츠";
            m_titleText.text = $"{wrestlerName} 콤보 가이드";
        }
        
        YaCht_ComboData[] combos = YaCht_ComboDatabase.GetCombosByWrestler(wrestlerType);
        var sortedCombos = new List<YaCht_ComboData>(combos);
        sortedCombos.Sort((a, b) => ((int)b.comboLevel).CompareTo((int)a.comboLevel));
        
        // 콤보 데이터 목록별로 콤보 Row 생성
        foreach (var combo in sortedCombos)
        {
            CreateComboRow(combo);
        }
        
        Debug.Log($"[ComboGuideUI] {sortedCombos.Count}개 콤보 Row 생성 완료");
    }
    
    /// <summary>
    /// 콤보 Row 생성
    /// </summary>
    private void CreateComboRow(YaCht_ComboData comboData)
    {
        if (m_comboRowPrefab == null || m_comboListParent == null)
        {
            Debug.LogWarning("[ComboGuideUI] RowPrefab 초기화 실패!");
            return;
        }
        
        GameObject rowObj = Instantiate(m_comboRowPrefab, m_comboListParent);
        YaCht_ComboGuideRow row = rowObj.GetComponent<YaCht_ComboGuideRow>();
        
        if (row != null)
        {
            row.Initialize(comboData);
            m_comboRows.Add(row);
        }
        else
        {
            Debug.LogWarning($"[ComboGuideUI] {rowObj.name} YaCht_ComboGuideRow 생성 실패!");
        }
    }
    
    /// <summary>
    /// 콤보 Row 초기화
    /// </summary>
    private void ClearComboRows()
    {
        foreach (var row in m_comboRows)
        {
            if (row != null)
            {
                Destroy(row.gameObject);
            }
        }
        m_comboRows.Clear();
    }
    
    /// <summary>
    /// 카드 설정이 변경되면 하이라이트 적용
    /// </summary>
    public void OnSetupChanged(List<YaCht_CardData> setupCards)
    {
        if (!m_autoUpdateOnSetup || setupCards == null)
        {
            return;
        }
        
        // 카드 설정 등급 목록 변환
        List<YaCht_CardRarity> setupRarities = new List<YaCht_CardRarity>();
        foreach (var card in setupCards)
        {
            setupRarities.Add(card.m_rarity);
        }
        
        // 콤보 Row 하이라이트 적용
        UpdateHighlights(setupRarities);
    }
    
    /// <summary>
    /// 카드 설정 등급 목록별로 콤보 Row 하이라이트 적용
    /// </summary>
    private void UpdateHighlights(List<YaCht_CardRarity> setupRarities)
    {
        foreach (var row in m_comboRows)
        {
            if (row != null)
            {
                // 카드 설정 등급 목록별로 콤보 Row 하이라이트 적용 (콤보 Row 제거 후 추가)
                List<YaCht_CardRarity> raritiesCopy = new List<YaCht_CardRarity>(setupRarities);
                row.UpdateHighlights(raritiesCopy);
            }
        }
        
        Debug.Log($"[ComboGuideUI] 카드 설정 등급 목록별로 콤보 Row 하이라이트 적용 - 카드 설정 등급: {setupRarities.Count}개");
    }
    
    /// <summary>
    /// 카드 설정 등급 목록별로 콤보 Row 하이라이트 적용 (Tab 키로 토글 표시)
    /// </summary>
    private void RefreshHighlights()
    {
        // YaCht_WWEMainGame에서 현재 업셋된 카드 정보 가져오기
        YaCht_WWEMainGame mainGame = FindFirstObjectByType<YaCht_WWEMainGame>();
        if (mainGame != null)
        {
            // 카드 설정 등급 목록 변환
            List<YaCht_CardData> setupCards = mainGame.GetSetupCards();
            List<YaCht_CardRarity> setupRarities = new List<YaCht_CardRarity>();
            foreach (var card in setupCards)
            {
                setupRarities.Add(card.m_rarity);
            }
            UpdateHighlights(setupRarities);
        }
    }
    
    /// <summary>
    /// 모든 하이라이트 초기화
    /// </summary>
    public void ResetAllHighlights()
    {
        foreach (var row in m_comboRows)
        {
            if (row != null)
            {
                row.ResetHighlights();
            }
        }
        
        Debug.Log("[ComboGuideUI] 모든 하이라이트 초기화");
    }
    
    private void OnDestroy()
    {
        ClearComboRows();
    }
}
