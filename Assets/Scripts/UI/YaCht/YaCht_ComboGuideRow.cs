using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class YaCht_ComboGuideRow : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject m_itemPrefab; // YaCht_ComboGuideItem 프리팹
    
    private YaCht_ComboData m_comboData;
    private List<YaCht_ComboGuideItem> m_items = new List<YaCht_ComboGuideItem>();
    private YaCht_CardRarity[] m_requiredRarities; // 필요한 등급 배열
    
    /// <summary>
    /// 초기화: 콤보 데이터 설정
    /// </summary>
    public void Initialize(YaCht_ComboData comboData)
    {
        m_comboData = comboData;               
        
        // 필요한 등급 파싱 (예: "BBB" -> [B, B, B])
        m_requiredRarities = ParseRequiredRarities(comboData.requiredPattern);
        
        // 등급 아이템 생성
        CreateItems();
        
        Debug.Log($"[ComboGuideRow] {comboData.comboName} 초기화 완료 - 필요한 등급: {comboData.requiredPattern}");
    }
    
    /// <summary>
    /// 필요한 등급 파싱 (예: "BBB" -> [B, B, B])
    /// </summary>
    private YaCht_CardRarity[] ParseRequiredRarities(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            return new YaCht_CardRarity[0];
        }
        
        List<YaCht_CardRarity> rarities = new List<YaCht_CardRarity>();
        
        foreach (char c in pattern.ToUpper())
        {
            switch (c)
            {
                case 'S':
                    rarities.Add(YaCht_CardRarity.S);
                    break;
                case 'A':
                    rarities.Add(YaCht_CardRarity.A);
                    break;
                case 'B':
                    rarities.Add(YaCht_CardRarity.B);
                    break;
                case 'C':
                    rarities.Add(YaCht_CardRarity.C);
                    break;
                case 'D':
                    rarities.Add(YaCht_CardRarity.D);
                    break;
            }
        }
        
        return rarities.ToArray();
    }
    
    /// <summary>
    /// 등급 아이템 생성
    /// </summary>
    private void CreateItems()
    {
        // 등급 아이템 초기화
        ClearItems();
        
        if (m_itemPrefab == null)
        {
            Debug.LogWarning("[ComboGuideRow] Item Prefab 초기화 실패!");
            return;
        }        
        
        // 필요한 등급별로 등급 아이템 생성
        foreach (var rarity in m_requiredRarities)
        {
            GameObject itemObj = Instantiate(m_itemPrefab, transform);
            YaCht_ComboGuideItem item = itemObj.GetComponent<YaCht_ComboGuideItem>();
            
            if (item != null)
            {
                item.Initialize(rarity);
                m_items.Add(item);
            }
            else
            {
                Debug.LogWarning($"[ComboGuideRow] {itemObj.name} YaCht_ComboGuideItem 생성 실패!");
            }
        }
        
        Debug.Log($"[ComboGuideRow] {m_requiredRarities.Length}개 등급 아이템 생성 완료 (Container: {transform.name})");
    }
    
    /// <summary>
    /// 등급 아이템 초기화
    /// </summary>
    private void ClearItems()
    {
        foreach (var item in m_items)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
        m_items.Clear();
    }
    
    public void UpdateHighlights(List<YaCht_CardRarity> setupRarities)
    {
        // 셋업 카드가 없으면 모든 하이라이트 끄기
        if (setupRarities == null || setupRarities.Count == 0)
        {
            ResetHighlights();
            return;
        }
        
        // 매칭에 사용할 셋업 카드 복사본 생성
        List<YaCht_CardRarity> remainingRarities = new List<YaCht_CardRarity>(setupRarities);
        int highlightedCount = 0;
        
        // 각 아이템별로 매칭 확인
        for (int i = 0; i < m_items.Count; i++)
        {
            if (m_items[i] == null) continue;
            
            YaCht_CardRarity requiredRarity = m_requiredRarities[i];
            
            // 남은 셋업 카드 중에 필요한 등급이 있는지 확인
            if (remainingRarities.Contains(requiredRarity))
            {
                // 매칭 성공 -> 하이라이트 켜기
                if (!m_items[i].IsHighlighted)
                {
                    m_items[i].Highlight();
                }
                
                // 사용한 카드는 제거 (중복 매칭 방지)
                remainingRarities.Remove(requiredRarity);
                highlightedCount++;
            }
            else
            {
                // 매칭 실패 -> 하이라이트 끄기
                if (m_items[i].IsHighlighted)
                {
                    m_items[i].ResetHighlight();
                }
            }
        }
        
        if (highlightedCount > 0)
        {
            Debug.Log($"[ComboGuideRow] {m_comboData.comboName} - {highlightedCount}개 하이라이트");
        }
    }
    
    /// <summary>
    /// 모든 하이라이트 초기화
    /// </summary>
    public void ResetHighlights()
    {
        foreach (var item in m_items)
        {
            if (item != null)
            {
                item.ResetHighlight();
            }
        }
    }
    
    /// <summary>
    /// 콤보 데이터 반환
    /// </summary>
    public YaCht_ComboData GetComboData()
    {
        return m_comboData;
    }
    
    private void OnDestroy()
    {
        ClearItems();
    }
}
