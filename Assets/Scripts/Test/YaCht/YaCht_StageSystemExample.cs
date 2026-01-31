using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스테이지 시스템 사용 예시 및 테스트 스크립트
/// </summary>
public class YaCht_StageSystemExample : MonoBehaviour
{
    [Header("UI References (Optional)")]
    public Text stageInfoText;
    public Text enemyHealthText;
    public Slider enemyHealthBar;

    private void Start()
    {
        // 게임 시작 시 첫 스테이지 로드
        YaCht_GameManager.StartNewStage(1);
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();

        // 테스트용 단축키
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestDamage(50); // 적에게 50 데미지
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TestDamage(500); // 적에게 500 데미지
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            TestNextStage(); // 다음 스테이지
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            TestReset(); // 게임 리셋
        }
    }

    private void UpdateUI()
    {
        if (YaCht_GameManager.StageManager == null)
            return;

        // 스테이지 정보 업데이트
        if (stageInfoText != null)
        {
            stageInfoText.text = YaCht_GameManager.StageManager.GetStageInfoString();
        }

        // 적 체력 바 업데이트
        if (enemyHealthBar != null)
        {
            enemyHealthBar.value = YaCht_GameManager.StageManager.GetEnemyHealthPercent();
        }

        // 적 체력 텍스트 업데이트
        if (enemyHealthText != null)
        {
            float currentHP = YaCht_GameManager.StageManager.CurrentEnemyHealth;
            float maxHP = YaCht_GameManager.StageManager.CurrentEnemy.m_maxHealth;
            enemyHealthText.text = $"HP: {currentHP:F0} / {maxHP}";
        }
    }

    // 테스트: 적에게 데미지 입히기
    private void TestDamage(float damage)
    {
        Debug.Log($"[Test] 적에게 {damage} 데미지!");
        YaCht_GameManager.DamageEnemy(damage);
    }

    // 테스트: 다음 스테이지로 이동
    private void TestNextStage()
    {
        bool success = YaCht_GameManager.MoveToNextStage();
        if (success)
        {
            Debug.Log("[Test] 다음 스테이지로 이동 성공!");
        }
        else
        {
            Debug.LogWarning("[Test] 다음 스테이지로 이동 실패!");
        }
    }

    // 테스트: 게임 리셋
    private void TestReset()
    {
        Debug.Log("[Test] 게임 리셋!");
        YaCht_GameManager.Clear();
    }

    // UI 버튼용 메서드들
    public void OnNextStageButtonClick()
    {
        TestNextStage();
    }

    public void OnResetButtonClick()
    {
        TestReset();
    }

    // 특정 스테이지로 직접 이동 (치트/테스트용)
    public void LoadStage(int stageNumber)
    {
        if (stageNumber >= 1 && stageNumber <= YaCht_EnemyDatabase.GetTotalStageCount())
        {
            YaCht_GameManager.StartNewStage(stageNumber);
            Debug.Log($"[Test] 스테이지 {stageNumber} 로드!");
        }
        else
        {
            Debug.LogError($"[Test] 잘못된 스테이지 번호: {stageNumber}");
        }
    }
}
