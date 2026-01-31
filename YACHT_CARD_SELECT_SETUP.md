# 야추 카드 선택 씬 구현 가이드

## 구현된 기능
1. **카드 세트 시스템**: 플레이어가 선택할 수 있는 카드 세트 (현재 Chop 10개 세트)
2. **카드 선택 씬**: 버튼을 통해 카드 세트를 선택하고 게임씬으로 이동
3. **플레이어 덱 설정**: 선택한 카드 세트가 플레이어의 덱으로 설정됨

## 새로 생성된 파일
1. `Assets\Scripts\Data\YaCht\YaCht_CardSetData.cs` - 카드 세트 데이터 및 데이터베이스
2. `Assets\Scripts\Scene\YaCht\YaCht_CardSelectScene.cs` - 카드 선택 씬 스크립트

## 수정된 파일
1. `Assets\Scripts\Data\YaCht\YaCht_PlayerData.cs` - SetPlayerDeck() 메서드 추가

## Unity에서 설정해야 할 것들

### 1. YaCht_CardSelectScene 생성
- 새로운 씬 생성: `YaCht_CardSelectScene`
- Build Settings에 씬 추가

### 2. UI 구성 (YaCht_CardSelectScene)
씬에 다음 UI 오브젝트들을 생성:

```
Canvas
├── SetInfoPanel
│   ├── SetNameText (Text)
│   └── SetDescriptionText (Text)
└── CardSetButtons
    └── ChopSetButton (Button)
        └── Text (버튼 텍스트: "찹 마스터 세트 선택")
```

### 3. YaCht_CardSelectScene 스크립트 연결
- 빈 GameObject 생성 후 `YaCht_CardSelectScene` 스크립트 추가
- Inspector에서 다음 항목들을 연결:
  - `m_chopSetButton`: ChopSetButton 연결
  - `m_setNameText`: SetNameText 연결
  - `m_setDescriptionText`: SetDescriptionText 연결

### 4. 씬 전환 확인
- Title Scene에서 Start 버튼 → `YaCht_CardSelectScene` 이동
- CardSelect Scene에서 세트 선택 버튼 → `YaCht_GameScene` 이동

## 작동 방식
1. 플레이어가 타이틀 씬에서 시작 버튼을 누르면 `YaCht_CardSelectScene`으로 이동
2. 카드 선택 씬에서 원하는 카드 세트 버튼을 클릭
3. 선택한 카드 세트가 `YaCht_GameManager.nowPlayerData.playerDeck`에 설정됨
4. 자동으로 `YaCht_GameScene`으로 이동하여 게임 시작
5. 게임 씬에서 설정된 10장의 카드로 게임 진행

## 추가 카드 세트 만들기
`YaCht_CardSetData.cs`에서 주석 처리된 예시를 참고하여 새로운 카드 세트 추가 가능:

```csharp
public static YaCht_CardSetData NewSet = new YaCht_CardSetData(
    "세트 이름",
    "세트 설명",
    new List<YaCht_CardData>
    {
        // 10장의 카드 추가
        YaCht_CardDatabase.Card1,
        YaCht_CardDatabase.Card2,
        // ...
    }
);
```

그리고 `GetAllCardSets()` 메서드에 추가하면 됩니다.
