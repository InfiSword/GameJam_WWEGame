using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YaCht_ComboGuideUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private TextMeshProUGUI m_titleText;
    [SerializeField] private Transform m_comboListParent;
    [SerializeField] private GameObject m_comboItemPrefab;
    [SerializeField] private Scrollbar m_Scrollbar;

    [Header("Settings")]
    [SerializeField] private KeyCode m_toggleKey = KeyCode.Tab;
    
    private bool m_isVisible = false;
    private bool m_isInitialized = false;
    
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
    public void Initialize(YaCht_WrestlerType wrestlerType)
    {
        if (m_isInitialized) return;
        
        BuildComboList(wrestlerType);
        m_isInitialized = true;
    }
    
    public void ToggleGuide()
    {
        m_isVisible = !m_isVisible;
        SetVisibility(m_isVisible);
    }
   
    private void SetVisibility(bool visible)
    {
        if (m_canvasGroup != null)
        {
            m_canvasGroup.alpha = visible ? 1f : 0f;
            m_canvasGroup.interactable = visible;
            m_canvasGroup.blocksRaycasts = visible;
        }
    }
    
    private void BuildComboList(YaCht_WrestlerType wrestlerType)
    {
        if (m_comboListParent == null) return;
                
        if (m_titleText != null)
        {
            string wrestlerName = wrestlerType == YaCht_WrestlerType.JohnCena ? "존 시나" :
                                  wrestlerType == YaCht_WrestlerType.Undertaker ? "언더테이커" : "알 수 없음";
            m_titleText.text = $"{wrestlerName} 야추 조합표";
        }
        
        YaCht_ComboData[] combos = YaCht_ComboDatabase.GetCombosByWrestler(wrestlerType);
                
        foreach (var combo in combos)
        {
            CreateComboItem(combo);
        }
        m_Scrollbar.value = 1f;
    }
    
    private void CreateComboItem(YaCht_ComboData comboData)
    {
        if (m_comboItemPrefab == null || m_comboListParent == null) return;
        
        GameObject itemObj = Instantiate(m_comboItemPrefab, m_comboListParent);
        YaCht_ComboGuideItem item = itemObj.GetComponent<YaCht_ComboGuideItem>();
        
        if (item != null)
        {
            item.SetComboData(comboData);
        }
    }
}
