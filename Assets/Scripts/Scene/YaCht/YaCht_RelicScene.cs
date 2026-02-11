using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class YaCht_RelicScene : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform m_relicContainer;
    [SerializeField] private GameObject m_relicItemPrefab;
    [SerializeField] private Button m_confirmButton;
    [SerializeField] private Button m_skipButton;
    [SerializeField] private TextMeshProUGUI m_titleText;
    [SerializeField] private TextMeshProUGUI m_stageInfoText;

    private List<YaCht_RelicType> m_availableRelics = new List<YaCht_RelicType>();
    private YaCht_RelicType m_selectedRelic;
    private bool m_isFromDeckSelection;
    private List<YaCht_RelicItem> m_relicItems = new List<YaCht_RelicItem>(); // 생성된 유물 아이템들

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 덱에서 왔는지 확인
        m_isFromDeckSelection = YaCht_GameManager.IsRelicSceneFromDeckSelection();

        // 선택 가능한 유물을 3개 생성

        GenerateRandomRelics();

        // 확인 버튼 이벤트 추가
        if (m_confirmButton != null)
        {
            m_confirmButton.onClick.AddListener(OnConfirmClicked);
            m_confirmButton.interactable = false; // 선택된 유물이 없으면 비활성화
        }

        if (m_skipButton != null)
        {
            m_skipButton.onClick.AddListener(OnSkipClicked);
        }

        // 컨텍스트에 따라 UI 업데이트
        UpdateUIBasedOnContext();
    }

    // 컨텍스트에 따라 UI 업데이트
    private void UpdateUIBasedOnContext()
    {
        if (m_isFromDeckSelection)
        {
            // 덱에서 왔을 때
            m_titleText.text = "처음 유물을 선택해주세요";
                       
        }
        else
        {
            // 보스 스테이지에서 왔을 때
            if (m_titleText != null)
            {
                m_titleText.text = "보스 보상유물을 선택해주세요!";
            }

            if (m_stageInfoText != null && YaCht_GameManager.StageManager != null)
            {
                int currentChapter = YaCht_GameManager.StageManager.GetCurrentChapterNumber();
                int currentStage = YaCht_GameManager.StageManager.CurrentStageNumber;
                m_stageInfoText.text = $"보스 {currentChapter} 스테이지! (스테이지 {currentStage})";
            }
        }
    }

    // 선택 가능한 유물 3개 생성
    private void GenerateRandomRelics()
    {
        YaCht_RelicData[] allRelics = YaCht_RelicDatabase.GetAllRelics();
        YaCht_WrestlerType currentWrestler = YaCht_GameManager.nowPlayerData.wrestlerType;

        // 선택 가능한 유물 데이터 리스트 초기화
        List<YaCht_RelicData> availableRelicData = new List<YaCht_RelicData>();
        foreach (var relic in allRelics)
        {
            // 유물 필요 캐릭터가 없거나 현재 캐릭터와 일치하면 추가
            if (relic.requiredWrestler == YaCht_WrestlerType.None || 
                relic.requiredWrestler == currentWrestler)
            {
                // 이미 보스 보상으로 획득한 유물은 제외
                if (!YaCht_GameManager.RelicManager.HasRelic(relic.relicType))
                {
                    availableRelicData.Add(relic);
                }
            }
        }

        // 선택 가능한 유물이 없을 때
        if (availableRelicData.Count == 0)
        {
            Debug.LogWarning("[RelicScene] 선택 가능한 유물이 없습니다! 모든 유물이 이미 보스 보상으로 획득되었습니다.");
            
            // 스킵 버튼 클릭
            if (m_titleText != null)
            {
                m_titleText.text = "보스 보상유물을 선택해주세요!";
            }
            
            // 2초 후 스킵 버튼 클릭
            Invoke(nameof(OnSkipClicked), 2f);
            return;
        }

        // 선택 가능한 유물 3개 생성
        int count = Mathf.Min(3, availableRelicData.Count);
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, availableRelicData.Count);
            m_availableRelics.Add(availableRelicData[randomIndex].relicType);
            availableRelicData.RemoveAt(randomIndex);
        }

        // UI 생성
        CreateRelicUI();
    }

    // 유물 UI 생성
    private void CreateRelicUI()
    {
        if (m_relicContainer == null || m_relicItemPrefab == null)
        {
            Debug.LogError("[RelicScene] Container 없거나 Prefab이 없습니다!");
            return;
        }

        m_relicItems.Clear();

        foreach (var relicType in m_availableRelics)
        {
            GameObject itemObj = Instantiate(m_relicItemPrefab, m_relicContainer);
            YaCht_RelicData relicData = YaCht_RelicDatabase.GetRelicData(relicType);

            // UI 생성 (RelicItem 생성)  
            var itemComponent = itemObj.GetComponent<YaCht_RelicItem>();
            if (itemComponent != null)
            {
                itemComponent.Init(relicData, () => OnRelicSelected(relicType));
                m_relicItems.Add(itemComponent);
            }
        }
    }

    // 유물 선택 이벤트
    private void OnRelicSelected(YaCht_RelicType relicType)
    {
        // 이전 선택 해제
        foreach (var item in m_relicItems)
        {
            if (item != null)
            {
                item.SetSelected(false);
            }
        }

        m_selectedRelic = relicType;
        
        // 선택된 유물 시각적 효과 적용
        foreach (var item in m_relicItems)
        {
            if (item != null && item.GetRelicType() == relicType)
            {
                item.SetSelected(true);
                break;
            }
        }
        
        if (m_confirmButton != null)
        {
            m_confirmButton.interactable = true;
        }

        Debug.Log($"[RelicScene] 선택된 유물: {YaCht_RelicDatabase.GetRelicData(relicType).name}");
    }

    // 확인 버튼 클릭
    private void OnConfirmClicked()
    {
        // 유물 추가
        YaCht_GameManager.RelicManager.AddRelic(m_selectedRelic);
        
        Debug.Log($"[RelicScene] 선택된 유물: {YaCht_RelicDatabase.GetRelicData(m_selectedRelic).name}");

        // 다음 스테이지로 이동
        ProceedToNextScene();
    }

    // 스킵 버튼 클릭
    private void OnSkipClicked()
    {
        Debug.Log("[RelicScene] 스킵 버튼 클릭");
        ProceedToNextScene();
    }

    // 다음 스테이지로 이동
    private void ProceedToNextScene()
    {
        // S급 기술 사운드 중단
        YaCht_BGMManager.Instance.StopSSkillSound();
        
        // 모든 공격 이펙트 정리 (WWEMainGame이 있으면 호출)
        YaCht_WWEMainGame wweMainGame = FindFirstObjectByType<YaCht_WWEMainGame>();
        if (wweMainGame != null)
        {
            wweMainGame.ClearAllAttackEffects();
        }
        
        if (m_isFromDeckSelection)
        {
            // 덱에서 왔을 때 -> 보스 스테이지 (유물 1개 선택)
            Debug.Log("[RelicScene] 덱에서 왔습니다! 보스 스테이지 (유물 1개 선택)");
            YaCht_GameManager.StartNewStage(1);
            SceneManager.LoadScene("YaCht_GameScene");
        }
        else
        {
            // 보스 스테이지에서 왔을 때 -> 다음 스테이지로 이동
            MoveToNextStage();
        }
    }

    // 다음 스테이지로 이동 (보스 스테이지에서 왔을 때)
    private void MoveToNextStage()
    {
        // S급 기술 사운드 중단
        YaCht_BGMManager.Instance.StopSSkillSound();
        
        // 현재 스테이지가 총 스테이지 수보다 크거나 같을 때
        if (YaCht_GameManager.StageManager.CurrentStageNumber >= YaCht_EnemyDatabase.GetTotalStageCount())
        {
            // 최종 스테이지!
            Debug.Log("[RelicScene] 최종 스테이지! 게임 종료! 최종 스테이지!");
            
            // 모든 공격 이펙트 정리 (WWEMainGame이 있으면 호출)
            YaCht_WWEMainGame wweMainGame = FindFirstObjectByType<YaCht_WWEMainGame>();
            if (wweMainGame != null)
            {
                wweMainGame.ClearAllAttackEffects();
            }
            
            // 게임 클리어 시 유물 제거
            YaCht_GameManager.nowPlayerData.ClearRelics();
            
            // TODO: 게임 종료
            SceneManager.LoadScene("YaCht_TitleScene"); // 타이틀 씬으로 이동
            return;
        }

        // 다음 스테이지로 이동
        bool success = YaCht_GameManager.MoveToNextStage();
        
        if (success)
        {
            Debug.Log($"[RelicScene] 다음 스테이지로 이동: {YaCht_GameManager.StageManager.CurrentStageNumber}");
            SceneManager.LoadScene("YaCht_GameScene");
        }
        else
        {
            Debug.LogError("[RelicScene] 다음 스테이지로 이동 실패!");
            // 모든 공격 이펙트 정리 (WWEMainGame이 있으면 호출)
            YaCht_WWEMainGame wweMainGame = FindFirstObjectByType<YaCht_WWEMainGame>();
            if (wweMainGame != null)
            {
                wweMainGame.ClearAllAttackEffects();
            }
            SceneManager.LoadScene("YaCht_TitleScene");
        }
    }

    private void OnDestroy()
    {
        if (m_confirmButton != null)
        {
            m_confirmButton.onClick.RemoveListener(OnConfirmClicked);
        }

        if (m_skipButton != null)
        {
            m_skipButton.onClick.RemoveListener(OnSkipClicked);
        }
    }
}
