using UnityEngine;

/// <summary>
/// 적 정보를 담는 데이터 구조체
/// </summary>
[System.Serializable]
public struct YaCht_EnemyData
{
    public string m_name;           // 적 이름
    public int m_maxHealth;         // 최대 체력
    public int m_stageNumber;       // 스테이지 번호 (1~12)
    public int m_chapterNumber;     // 챕터 번호 (1~3)
    public bool m_isBoss;           // 보스 여부
    public string m_spriteResourcePath; // 스프라이트 리소스 경로 (Resources 폴더 기준)

    public YaCht_EnemyData(string name, int maxHealth, int stageNumber, int chapterNumber, bool isBoss, string spriteResourcePath = "")
    {
        m_name = name;
        m_maxHealth = maxHealth;
        m_stageNumber = stageNumber;
        m_chapterNumber = chapterNumber;
        m_isBoss = isBoss;
        m_spriteResourcePath = spriteResourcePath;
    }
}
