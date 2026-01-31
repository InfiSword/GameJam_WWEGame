# YaCht_StageDatabase 통합 - 코드 개선 요약

## ?? 개선된 파일 목록

### 1. ? YaCht_StageManager.cs
**개선 내용:**
- `CurrentStageData` 속성 추가 → 스테이지 리소스 정보 관리
- `YaCht_StageDatabase` 초기화 추가
- `IsCurrentEnemyBoss()` → `YaCht_StageDatabase.IsBossStage()` 활용
- 스테이지 로드 시 배경/BGM 경로 로그 출력
- 새로운 헬퍼 메서드 추가:
  - `GetCurrentStageDescription()`
  - `GetCurrentBackgroundPath()`
  - `GetCurrentBGMPath()`
  - `GetCurrentSpecialEffectPath()`

**Before:**
```csharp
public bool IsCurrentEnemyBoss()
{
    return CurrentEnemy.m_isBoss;
}
```

**After:**
```csharp
public bool IsCurrentEnemyBoss()
{
    return YaCht_StageDatabase.IsBossStage(CurrentStageNumber);
}
```

---

### 2. ? YaCht_GameScene.cs
**개선 내용:**
- 배경 이미지 로드 기능 추가
- BGM 재생 기능 추가
- 보스전 특수 효과 생성 기능 추가
- 리소스 정리 (`OnDestroy`)

**새로 추가된 SerializeField:**
```csharp
[Header("Stage Resources")]
[SerializeField] private Image backgroundImage;
[SerializeField] private AudioSource bgmAudioSource;
[SerializeField] private Transform specialEffectSpawnPoint;
```

**새로 추가된 메서드:**
- `LoadStageResources()` - 스테이지 리소스 통합 로드
- `LoadBackground()` - 배경 이미지 로드
- `PlayBGM()` - BGM 재생
- `SpawnSpecialEffect()` - 특수 효과 생성
- `OnDestroy()` - 리소스 정리

---

### 3. ? YaCht_GameManager.cs
**개선 내용:**
- `YaCht_StageDatabase` 초기화 추가
- 새로운 헬퍼 메서드 추가

**추가된 메서드:**
```csharp
/// <summary>
/// 전체 스테이지 수 가져오기
/// </summary>
public static int GetTotalStageCount()
{
    return YaCht_StageDatabase.GetTotalStageCount();
}

/// <summary>
/// 현재 스테이지가 보스전인지 확인
/// </summary>
public static bool IsCurrentStageBoss()
{
    if (StageManager != null)
    {
        return YaCht_StageDatabase.IsBossStage(StageManager.CurrentStageNumber);
    }
    return false;
}
```

---

### 4. ? YaCht_WWEMainGame.cs
**개선 내용:**
- `OnNextStageButtonClicked()` 메서드 간소화
- 보스전 확인을 `YaCht_GameManager.IsCurrentStageBoss()` 사용

**Before:**
```csharp
bool isBoss = m_currentEnemy != null ? m_currentEnemy.IsBoss : YaCht_GameManager.StageManager.IsCurrentEnemyBoss();
```

**After:**
```csharp
bool isBoss = YaCht_GameManager.IsCurrentStageBoss();
```

---

### 5. ? YaCht_EnemyDatabase.cs
**개선 내용:**
- `GetTotalStageCount()` → `YaCht_StageDatabase.GetTotalStageCount()` 위임

**Before:**
```csharp
public static int GetTotalStageCount()
{
    return 12;  // 하드코딩
}
```

**After:**
```csharp
public static int GetTotalStageCount()
{
    return YaCht_StageDatabase.GetTotalStageCount();  // 통합
}
```

---

## ?? 주요 개선 효과

### 1. 단일 책임 원칙 (SRP)
- **YaCht_EnemyDatabase**: 적 정보 관리
- **YaCht_StageDatabase**: 스테이지 환경 관리 (배경, BGM, 효과)
- **YaCht_StageManager**: 게임 진행 및 두 데이터베이스 통합

### 2. 중복 제거
- ? 하드코딩된 스테이지 수 제거
- ? 여러 곳에서 보스 확인 로직 중복 제거
- ? 단일 진실 공급원 (Single Source of Truth)

### 3. 확장성 향상
- 새로운 스테이지 추가 시 `YaCht_StageDatabase`만 수정
- 챕터 수/스테이지 수 변경 시 자동 반영

### 4. 유지보수성 향상
- 리소스 경로를 한 곳에서 관리
- 보스전 확인 로직 일원화

---

## ?? 코드 변경 전후 비교

### 보스전 확인 (Before)
```csharp
// YaCht_StageManager
if (CurrentEnemy.m_isBoss) { ... }

// YaCht_WWEMainGame
bool isBoss = m_currentEnemy != null ? m_currentEnemy.IsBoss : YaCht_GameManager.StageManager.IsCurrentEnemyBoss();

// YaCht_EnemyDatabase
// 보스 정보는 YaCht_EnemyData에만 존재
```

### 보스전 확인 (After)
```csharp
// 모든 곳에서 통일
bool isBoss = YaCht_GameManager.IsCurrentStageBoss();

// 또는
bool isBoss = YaCht_StageDatabase.IsBossStage(stageNumber);
```

---

## ?? Unity 에디터에서 설정할 항목

### YaCht_GameScene
인스펙터에서 다음 항목들을 할당해야 합니다:

```
[Header("Game Components")]
- wwe (YaCht_WWEMainGame)

[Header("Stage Resources")]
- backgroundImage (Image) - 배경을 표시할 UI Image
- bgmAudioSource (AudioSource) - BGM 재생용 AudioSource
- specialEffectSpawnPoint (Transform) - 특수 효과 생성 위치 (선택사항)
```

**설정 예시:**
1. Scene에 Canvas > Image 생성 → "BackgroundImage"로 이름 변경
2. Scene에 Empty GameObject 생성 → AudioSource 추가 → "BGMAudioSource"로 이름 변경
3. Scene에 Empty GameObject 생성 → "SpecialEffectPoint"로 이름 변경
4. YaCht_GameScene 스크립트에 모두 할당

---

## ?? 주의사항

### 1. Resources 폴더 구조
반드시 가이드에 명시된 폴더 구조를 따라야 합니다:
```
Resources/
├── Sprites/Backgrounds/Chapter1/...
├── Audio/BGM/Chapter1/...
└── Effects/Chapter1/...
```

### 2. 리소스가 없을 때
- **배경/BGM**: Warning 로그만 출력하고 게임 진행
- **특수 효과**: 빈 문자열이면 무시 (정상 동작)

### 3. AudioSource 설정
- **Loop**: true (BGM은 반복 재생)
- **Play On Awake**: false (코드에서 제어)
- **Volume**: 0.5~1.0 (적절히 조절)

---

## ?? 다음 단계

### 1. 리소스 준비
```
Resources/Sprites/Backgrounds/Chapter1/
  ├── Stage1.png (1920x1080 권장)
  ├── Stage2.png
  ├── Stage3.png
  └── BossStage.png

Resources/Audio/BGM/Chapter1/
  ├── Normal.mp3 (일반전)
  └── Boss.mp3 (보스전)
```

### 2. 특수 효과 Prefab 생성 (선택)
```csharp
Resources/Effects/Chapter1/
  └── BossAura.prefab
      - Particle System (오라, 불꽃 등)
      - Light (극적인 조명)
      - Animation (캐릭터 연출)
```

### 3. 테스트
1. 스테이지 1 → 일반 배경 + 일반 BGM
2. 스테이지 4 (보스) → 특별 배경 + 보스 BGM + 특수 효과
3. 다음 챕터 → 새로운 테마

---

## ?? 활용 예시

### 스테이지 정보 표시
```csharp
// UI에 스테이지 설명 표시
string description = YaCht_GameManager.StageManager.GetCurrentStageDescription();
stageDescriptionText.text = description;
```

### 스테이지별 연출 변경
```csharp
// 보스전이면 카메라 연출 변경
if (YaCht_GameManager.IsCurrentStageBoss())
{
    cinemachineCamera.m_Lens.FieldOfView = 50f;  // 줌인
    postProcessVolume.weight = 1.0f;             // 포스트 프로세싱 강화
}
```

### 챕터별 UI 테마 변경
```csharp
int chapter = YaCht_GameManager.StageManager.GetCurrentChapterNumber();
switch (chapter)
{
    case 1: uiThemeColor = Color.blue; break;
    case 2: uiThemeColor = Color.red; break;
    case 3: uiThemeColor = Color.purple; break;
}
```

---

## ?? 성능 최적화

### Resources.Load 캐싱
현재는 매번 로드하지만, 필요시 캐싱 가능:

```csharp
private Dictionary<int, Sprite> cachedBackgrounds = new Dictionary<int, Sprite>();
private Dictionary<int, AudioClip> cachedBGMs = new Dictionary<int, AudioClip>();

private Sprite LoadBackgroundCached(int stageNumber)
{
    if (!cachedBackgrounds.ContainsKey(stageNumber))
    {
        string path = YaCht_StageDatabase.GetBackgroundPath(stageNumber);
        cachedBackgrounds[stageNumber] = Resources.Load<Sprite>(path);
    }
    return cachedBackgrounds[stageNumber];
}
```

---

## ? 결론

이번 개선으로:
- ? 코드 중복 제거
- ? 스테이지 리소스 시스템 완성
- ? 확장성 및 유지보수성 향상
- ? 단일 책임 원칙 준수
- ? 일관된 API 제공

이제 각 스테이지마다 독특한 분위기와 경험을 제공할 수 있는 견고한 시스템이 완성되었습니다! ??
