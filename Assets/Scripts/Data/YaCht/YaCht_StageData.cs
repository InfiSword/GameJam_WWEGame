using UnityEngine;

/// <summary>
/// 스테이지별 리소스 및 효과 데이터 구조체
/// </summary>
[System.Serializable]
public struct YaCht_StageData
{
    public int m_stageNumber;                   // 스테이지 번호 (1~12)
    public int m_chapterNumber;                 // 챕터 번호 (1~3)
    public string m_backgroundResourcePath;     // 배경 이미지 리소스 경로 (Resources 폴더 기준)
    public string m_bgmResourcePath;            // BGM 리소스 경로 (Resources 폴더 기준)
    public string m_specialEffectResourcePath;  // 특수 효과 리소스 경로 (Resources 폴더 기준, 선택사항)
    public bool m_isBossStage;                  // 보스 스테이지 여부
    public string m_stageDescription;           // 스테이지 설명
    public YaCht_PhaseData[] m_phases;          // 페이즈별 데이터 (null이면 단일 페이즈)
    
    public YaCht_StageData(
        int stageNumber,
        int chapterNumber,
        string backgroundPath,
        string bgmPath,
        bool isBoss,
        string description = "",
        string effectPath = "",
        YaCht_PhaseData[] phases = null)
    {
        m_stageNumber = stageNumber;
        m_chapterNumber = chapterNumber;
        m_backgroundResourcePath = backgroundPath;
        m_bgmResourcePath = bgmPath;
        m_specialEffectResourcePath = effectPath;
        m_isBossStage = isBoss;
        m_stageDescription = description;
        m_phases = phases;
    }
    
    /// <summary>
    /// 다중 페이즈 스테이지인지 확인
    /// </summary>
    public bool HasMultiplePhases => m_phases != null && m_phases.Length > 0;
    
    /// <summary>
    /// 체력 퍼센트에 따른 현재 페이즈 가져오기
    /// </summary>
    public YaCht_PhaseData? GetPhaseByHealthPercent(float healthPercent)
    {
        if (!HasMultiplePhases)
            return null;
        
        // 체력 퍼센트에 맞는 페이즈 찾기 (내림차순 정렬 가정)
        for (int i = 0; i < m_phases.Length; i++)
        {
            if (healthPercent <= m_phases[i].m_healthThreshold)
            {
                return m_phases[i];
            }
        }
        
        // 첫 번째 페이즈 반환 (체력이 가장 높을 때)
        return m_phases[0];
    }
    
    /// <summary>
    /// 페이즈 번호로 페이즈 데이터 가져오기
    /// </summary>
    public YaCht_PhaseData? GetPhaseByNumber(int phaseNumber)
    {
        if (!HasMultiplePhases)
            return null;
        
        foreach (var phase in m_phases)
        {
            if (phase.m_phaseNumber == phaseNumber)
            {
                return phase;
            }
        }
        
        return null;
    }
}
