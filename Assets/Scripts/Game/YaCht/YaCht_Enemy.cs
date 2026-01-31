using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 적 개체를 관리하는 통합 클래스 (데이터, 로직, 시각화)
/// </summary>
public class YaCht_Enemy : MonoBehaviour
{
    #region Events
    /// <summary>
    /// 적이 데미지를 받았을 때 발생하는 이벤트
    /// </summary>
    public event Action<float> OnDamaged;

    /// <summary>
    /// 적이 사망했을 때 발생하는 이벤트
    /// </summary>
    public event Action OnDeath;

    /// <summary>
    /// 적의 HP가 변경되었을 때 발생하는 이벤트
    /// </summary>
    public event Action<float, float> OnHealthChanged;
    #endregion

    #region Serialized Fields
    [Header("Enemy Data")]
    [SerializeField] private YaCht_EnemyData m_enemyData;

    [Header("Enemy Visual")]
    [SerializeField] private SpriteRenderer m_enemySprite;

    [Header("Enemy UI (Canvas Space)")]
    [SerializeField] private Transform m_enemyUIRoot;
    [SerializeField] private TextMeshProUGUI m_enemyNameText;
    [SerializeField] private Slider m_hpBarSlider;
    [SerializeField] private Image m_hpBarFillImage;
    [SerializeField] private TextMeshProUGUI m_hpText;

    [Header("HP Bar Colors")]
    [SerializeField] private Color m_highHpColor = Color.green;
    [SerializeField] private Color m_mediumHpColor = Color.yellow;
    [SerializeField] private Color m_lowHpColor = Color.red;

    [Header("Animation Settings")]
    [SerializeField] private float m_damageShakeIntensity = 0.3f;      // 0.1 → 0.3 (3배)
    [SerializeField] private float m_damageShakeDuration = 0.5f;       // 0.2 → 0.5 (2.5배)
    [SerializeField] private float m_deathFadeDuration = 0.5f;
    [SerializeField] private Color m_damageFlashColor = Color.red;     // 데미지 플래시 색상
    [SerializeField] private float m_damageFlashDuration = 0.3f;       // 플래시 지속 시간

    [Header("Spawn Settings")]
    [SerializeField] private Vector3 m_spawnOffset = Vector3.zero;
    #endregion

    #region Private Fields
    private float m_currentHealth;
    private float m_maxHealth;
    private bool m_isDead = false;
    private Vector3 m_originalPosition;
    private Coroutine m_shakeCoroutine;
    private Coroutine m_flashCoroutine;
    private Color m_originalSpriteColor;
    #endregion

    #region Properties
    /// <summary>
    /// 현재 체력
    /// </summary>
    public float CurrentHealth => m_currentHealth;

    /// <summary>
    /// 최대 체력
    /// </summary>
    public float MaxHealth => m_maxHealth;

    /// <summary>
    /// 체력 비율 (0 ~ 1)
    /// </summary>
    public float HealthPercentage => m_maxHealth > 0 ? m_currentHealth / m_maxHealth : 0;

    /// <summary>
    /// 사망 여부
    /// </summary>
    public bool IsDead => m_isDead;

    /// <summary>
    /// 보스 여부
    /// </summary>
    public bool IsBoss => m_enemyData.m_isBoss;

    /// <summary>
    /// 적 이름
    /// </summary>
    public string EnemyName => m_enemyData.m_name;

    /// <summary>
    /// 적 데이터
    /// </summary>
    public YaCht_EnemyData EnemyData => m_enemyData;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (m_enemySprite != null)
        {
            m_originalPosition = m_enemySprite.transform.localPosition;
            m_originalSpriteColor = m_enemySprite.color;
        }
    }
    #endregion

    #region Initialization
    /// <summary>
    /// 초기화 + 스폰을 동시에 수행
    /// </summary>
    public void Initialize(YaCht_EnemyData enemyData, Transform spawnPosition)
    {
        m_enemyData = enemyData;
        m_currentHealth = enemyData.m_maxHealth;
        m_maxHealth = enemyData.m_maxHealth;
        m_isDead = false;

        if (m_enemyNameText != null)
        {
            string bossLabel = m_enemyData.m_isBoss ? " [보스]" : "";
            m_enemyNameText.text = $"{m_enemyData.m_name}{bossLabel}";
        }
        LoadEnemySprite();
        UpdateHealthBar();

        m_enemyUIRoot.GetComponent<Canvas>().worldCamera = Camera.main;

        transform.position = spawnPosition.position + m_spawnOffset;
        transform.rotation = spawnPosition.rotation;

        Debug.Log($"[Enemy] {enemyData.m_name} 스폰 완료 - 위치: {transform.position}");
    }

    /// <summary>
    /// 적 스프라이트를 리소스에서 로드
    /// </summary>
    private void LoadEnemySprite()
    {
        if (m_enemySprite == null)
        {
            Debug.LogWarning("[Enemy] SpriteRenderer가 없습니다!");
            return;
        }

        // 스프라이트 경로가 지정되어 있으면 로드
        if (!string.IsNullOrEmpty(m_enemyData.m_spriteResourcePath))
        {
            Sprite loadedSprite = Resources.Load<Sprite>(m_enemyData.m_spriteResourcePath);

            if (loadedSprite != null)
            {
                m_enemySprite.sprite = loadedSprite;
                Debug.Log($"[Enemy] 스프라이트 로드 성공: {m_enemyData.m_spriteResourcePath}");
            }
        }
    }

    /// <summary>
    /// 체력만 설정 (적 데이터는 유지)
    /// </summary>
    public void SetHealth(float currentHealth, float maxHealth)
    {
        m_currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        m_maxHealth = maxHealth;
        m_isDead = m_currentHealth <= 0;

        UpdateHealthBar();
        OnHealthChanged?.Invoke(m_currentHealth, m_maxHealth);
    }
    #endregion

    #region Damage & Death
    /// <summary>
    /// 데미지를 받음
    /// </summary>
    public void TakeDamage(float damage, bool playAnimation = true)
    {
        if (m_isDead)
        {
            Debug.LogWarning("[Enemy] 이미 사망한 적입니다.");
            return;
        }

        float previousHealth = m_currentHealth;
        m_currentHealth = Mathf.Max(0, m_currentHealth - damage);

        Debug.Log($"[Enemy] {m_enemyData.m_name} 데미지: {damage} (HP: {previousHealth} → {m_currentHealth})");

        UpdateHealthBar();

        if (playAnimation && !m_isDead)
        {
            PlayDamageAnimation();
        }

        OnDamaged?.Invoke(damage);
        OnHealthChanged?.Invoke(m_currentHealth, m_maxHealth);

        // 사망 체크
        if (m_currentHealth <= 0 && !m_isDead)
        {
            Die();
        }
    }

    /// <summary>
    /// 체력을 회복함
    /// </summary>
    public void Heal(float amount)
    {
        if (m_isDead)
        {
            Debug.LogWarning("[Enemy] 사망한 적은 회복할 수 없습니다.");
            return;
        }

        float previousHealth = m_currentHealth;
        m_currentHealth = Mathf.Min(m_maxHealth, m_currentHealth + amount);

        Debug.Log($"[Enemy] {m_enemyData.m_name} 회복: {amount} (HP: {previousHealth} → {m_currentHealth})");

        UpdateHealthBar();
        OnHealthChanged?.Invoke(m_currentHealth, m_maxHealth);
    }

    /// <summary>
    /// 즉시 사망
    /// </summary>
    public void Die()
    {
        if (m_isDead) return;

        m_isDead = true;
        m_currentHealth = 0;

        Debug.Log($"[Enemy] {m_enemyData.m_name} 사망!");

        UpdateHealthBar();
        OnDeath?.Invoke();

        PlayDeathAnimation();
    }
    #endregion

    #region UI Update
    /// <summary>
    /// HP바 UI 업데이트
    /// </summary>
    private void UpdateHealthBar()
    {
        if (m_hpBarSlider != null)
        {
            float hpPercentage = HealthPercentage;
            m_hpBarSlider.value = hpPercentage;

            // HP 비율에 따라 색상 변경
            if (m_hpBarFillImage != null)
            {
                m_hpBarFillImage.color = GetHpColor(hpPercentage);
            }
        }

        if (m_hpText != null)
        {
            m_hpText.text = $"{m_currentHealth:F0} / {m_maxHealth:F0}";
        }
    }

    /// <summary>
    /// HP 비율에 따른 색상 반환
    /// </summary>
    private Color GetHpColor(float hpPercentage)
    {
        if (hpPercentage > 0.6f)
        {
            return m_highHpColor;
        }
        else if (hpPercentage > 0.3f)
        {
            return Color.Lerp(m_mediumHpColor, m_highHpColor, (hpPercentage - 0.3f) / 0.3f);
        }
        else
        {
            return Color.Lerp(m_lowHpColor, m_mediumHpColor, hpPercentage / 0.3f);
        }
    }
    #endregion

    #region Animations
    /// <summary>
    /// 데미지 애니메이션 재생
    /// </summary>
    private void PlayDamageAnimation()
    {
        if (m_enemySprite == null) return;

        if (m_shakeCoroutine != null)
        {
            StopCoroutine(m_shakeCoroutine);
        }

        if (m_flashCoroutine != null)
        {
            StopCoroutine(m_flashCoroutine);
        }

        m_shakeCoroutine = StartCoroutine(ShakeCoroutine());
        m_flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    /// <summary>
    /// 흔들림 효과 코루틴
    /// </summary>
    private System.Collections.IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < m_damageShakeDuration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * m_damageShakeIntensity;
            float y = UnityEngine.Random.Range(-1f, 1f) * m_damageShakeIntensity;

            m_enemySprite.transform.localPosition = m_originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        m_enemySprite.transform.localPosition = m_originalPosition;
        m_shakeCoroutine = null;
    }

    /// <summary>
    /// 데미지 플래시 효과 코루틴 (빨간색 하이라이트)
    /// </summary>
    private System.Collections.IEnumerator FlashCoroutine()
    {
        if (m_enemySprite != null)
        {
            Color originalColor = m_originalSpriteColor;
            float elapsed = 0f;

            while (elapsed < m_damageFlashDuration)
            {
                // Ping-pong으로 빨간색↔원래색 전환
                float t = Mathf.PingPong(elapsed * 8f, 1f);
                m_enemySprite.color = Color.Lerp(m_damageFlashColor, originalColor, t);

                elapsed += Time.deltaTime;
                yield return null;
            }

            m_enemySprite.color = originalColor;
        }

        m_flashCoroutine = null;
    }

    /// <summary>
    /// 사망 애니메이션 (페이드아웃)
    /// </summary>
    private void PlayDeathAnimation()
    {
        if (m_enemySprite != null)
        {
            StartCoroutine(FadeOutCoroutine());
        }
    }

    /// <summary>
    /// 페이드아웃 코루틴
    /// </summary>
    private System.Collections.IEnumerator FadeOutCoroutine()
    {
        float elapsed = 0f;
        Color originalColor = m_enemySprite.color;

        while (elapsed < m_deathFadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / m_deathFadeDuration);
            m_enemySprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }
    #endregion

    #region Debug
    /// <summary>
    /// 디버그 정보 출력
    /// </summary>
    public void PrintDebugInfo()
    {
        Debug.Log($"=== {m_enemyData.m_name} 정보 ===");
        Debug.Log($"HP: {m_currentHealth}/{m_maxHealth} ({HealthPercentage * 100:F1}%)");
        Debug.Log($"보스: {(m_enemyData.m_isBoss ? "예" : "아니오")}");
        Debug.Log($"사망: {(m_isDead ? "예" : "아니오")}");
        Debug.Log($"스테이지: {m_enemyData.m_stageNumber}");
        Debug.Log($"챕터: {m_enemyData.m_chapterNumber}");
    }
    #endregion
}

