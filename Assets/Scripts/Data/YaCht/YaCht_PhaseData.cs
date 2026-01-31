using UnityEngine;

/// <summary>
/// 보스 페이즈별 데이터 (배경, BGM, 효과, 체력 임계값)
/// </summary>
[System.Serializable]
public struct YaCht_PhaseData
{
    public int m_phaseNumber;                   // 페이즈 번호 (1, 2, 3...)
    public float m_healthThreshold;             // 페이즈 전환 체력 임계값 (0~1, 예: 0.5 = 50%)
    public string m_backgroundResourcePath;     // 페이즈별 배경 이미지 경로
    public string m_bgmResourcePath;            // 페이즈별 BGM 경로 (선택사항, 빈 문자열이면 변경 안 함)
    public string m_specialEffectResourcePath;  // 페이즈별 특수 효과 경로 (선택사항)
    public string m_phaseDescription;           // 페이즈 설명
    
    public YaCht_PhaseData(
        int phaseNumber,
        float healthThreshold,
        string backgroundPath,
        string description = "",
        string bgmPath = "",
        string effectPath = "")
    {
        m_phaseNumber = phaseNumber;
        m_healthThreshold = healthThreshold;
        m_backgroundResourcePath = backgroundPath;
        m_bgmResourcePath = bgmPath;
        m_specialEffectResourcePath = effectPath;
        m_phaseDescription = description;
    }
}
