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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 어디서 왔는지 확인
        m_isFromDeckSelection = YaCht_GameManager.IsRelicSceneFromDeckSelection();

        // 선택 가능한 유물 3개 랜덤 생성
        GenerateRandomRelics();

        // 버튼 이벤트
        if (m_confirmButton != null)
        {
            m_confirmButton.onClick.AddListener(OnConfirmClicked);
            m_confirmButton.interactable = false; // 선택 전까지 비활성화
        }

        if (m_skipButton != null)
        {
            m_skipButton.onClick.AddListener(OnSkipClicked);
        }

        // 타이틀 및 정보 설정
        UpdateUIBasedOnContext();
    }

    // 컨텍스트에 따라 UI 업데이트
    private void UpdateUIBasedOnContext()
    {
        if (m_isFromDeckSelection)
        {
            // 덱 선택 후 첫 유물 선택
            if (m_titleText != null)
            {
                m_titleText.text = "시작 유물을 선택하세요";
            }

            if (m_stageInfoText != null)
            {
                m_stageInfoText.text = "모험을 시작합니다!";
            }
        }
        else
        {
            // 보스 처치 후 유물 선택
            if (m_titleText != null)
            {
                m_titleText.text = "보스 처치! 유물을 선택하세요";
            }

            if (m_stageInfoText != null && YaCht_GameManager.StageManager != null)
            {
                int currentChapter = YaCht_GameManager.StageManager.GetCurrentChapterNumber();
                int currentStage = YaCht_GameManager.StageManager.CurrentStageNumber;
                m_stageInfoText.text = $"챕터 {currentChapter} 클리어! (스테이지 {currentStage})";
            }
        }
    }

    // 랜덤 유물 3개 생성
    private void GenerateRandomRelics()
    {
        YaCht_RelicData[] allRelics = YaCht_RelicDatabase.GetAllRelics();
        YaCht_WrestlerType currentWrestler = YaCht_GameManager.nowPlayerData.wrestlerType;

        // 현재 캐릭터가 사용 가능한 유물 필터링
        List<YaCht_RelicData> availableRelicData = new List<YaCht_RelicData>();
        foreach (var relic in allRelics)
        {
            // 공용 유물이거나 현재 캐릭터 전용 유물
            if (relic.requiredWrestler == YaCht_WrestlerType.None || 
                relic.requiredWrestler == currentWrestler)
            {
                // 이미 보유하지 않은 유물만
                if (!YaCht_GameManager.RelicManager.HasRelic(relic.relicType))
                {
                    availableRelicData.Add(relic);
                }
            }
        }

        // 선택 가능한 유물이 없으면 경고
        if (availableRelicData.Count == 0)
        {
            Debug.LogWarning("[RelicScene] 선택 가능한 유물이 없습니다! 모든 유물을 이미 보유 중입니다.");
            
            // 건너뛰기로 자동 진행
            if (m_titleText != null)
            {
                m_titleText.text = "획득 가능한 유물이 없습니다";
            }
            
            // 2초 후 자동으로 다음으로 진행
            Invoke(nameof(OnSkipClicked), 2f);
            return;
        }

        // 랜덤하게 3개 선택
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
            Debug.LogError("[RelicScene] Container 또는 Prefab이 없습니다!");
            return;
        }

        foreach (var relicType in m_availableRelics)
        {
            GameObject itemObj = Instantiate(m_relicItemPrefab, m_relicContainer);
            YaCht_RelicData relicData = YaCht_RelicDatabase.GetRelicData(relicType);

            // UI 설정 (RelicItem 컴포넌트 필요)
            var itemComponent = itemObj.GetComponent<YaCht_RelicItem>();
            if (itemComponent != null)
            {
                itemComponent.Init(relicData, () => OnRelicSelected(relicType));
            }
        }
    }

    // 유물 선택 시
    private void OnRelicSelected(YaCht_RelicType relicType)
    {
        m_selectedRelic = relicType;
        
        if (m_confirmButton != null)
        {
            m_confirmButton.interactable = true;
        }

        Debug.Log($"[RelicScene] 선택: {YaCht_RelicDatabase.GetRelicData(relicType).name}");
    }

    // 확인 버튼 클릭
    private void OnConfirmClicked()
    {
        // 유물 추가
        YaCht_GameManager.RelicManager.AddRelic(m_selectedRelic);
        
        Debug.Log($"[RelicScene] 유물 획득: {YaCht_RelicDatabase.GetRelicData(m_selectedRelic).name}");

        // 다음 씬으로 이동
        ProceedToNextScene();
    }

    // 건너뛰기 버튼 클릭
    private void OnSkipClicked()
    {
        Debug.Log("[RelicScene] 유물 선택 건너뛰기");
        ProceedToNextScene();
    }

    // 다음 씬으로 진행
    private void ProceedToNextScene()
    {
        if (m_isFromDeckSelection)
        {
            // 덱 선택 후 -> 게임 시작 (첫 스테이지)
            Debug.Log("[RelicScene] 게임 시작! 스테이지 1로 이동");
            YaCht_GameManager.StartNewStage(1);
            SceneManager.LoadScene("YaCht_GameScene");
        }
        else
        {
            // 보스 처치 후 -> 다음 스테이지로
            MoveToNextStage();
        }
    }

    // 다음 스테이지로 이동 (보스 처치 후만 사용)
    private void MoveToNextStage()
    {
        // 다음 스테이지가 있는지 확인
        if (YaCht_GameManager.StageManager.CurrentStageNumber >= YaCht_EnemyDatabase.GetTotalStageCount())
        {
            // 모든 스테이지 클리어!
            Debug.Log("[RelicScene] ★★★ 모든 스테이지 클리어! 게임 완료! ★★★");
            // TODO: 엔딩 씬으로 이동
            SceneManager.LoadScene("YaCht_TitleScene"); // 임시로 타이틀로
            return;
        }

        // 다음 스테이지 로드
        bool success = YaCht_GameManager.MoveToNextStage();
        
        if (success)
        {
            Debug.Log($"[RelicScene] 다음 스테이지 {YaCht_GameManager.StageManager.CurrentStageNumber}로 이동");
            SceneManager.LoadScene("YaCht_GameScene");
        }
        else
        {
            Debug.LogError("[RelicScene] 다음 스테이지로 이동 실패!");
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
