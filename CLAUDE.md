# CLAUDE.md

이 파일은 이 저장소에서 작업할 때 Claude Code(claude.ai/code)에게 안내를 제공합니다.

## 프로젝트 개요

**Unity 6000.3.11f1** + Universal Render Pipeline(URP)으로 제작한 3D 액션 보스 전투 게임. 스태미나 관리, 구르기 무적, 소비 아이템, 2페이즈 보스 전투를 갖춘 싱글플레이어 근접 전투 게임입니다.

## 빌드 명령어

Unity Editor(버전 6000.3.11f1)에서 열고 빌드합니다. 커맨드라인 빌드:

```
Unity.exe -projectPath "C:\Users\jokh9\TeamProject" -buildTarget StandaloneWindows64 -executeMethod BuildScript.Build -quit
```

패키지: New Input System, Cinemachine, TextMesh Pro, AI Navigation(NavMesh), Newtonsoft.Json — 모두 Unity Package Manager로 관리.

## 씬 목록

씬 파일 위치: `Assets/Scenes/`

| 씬 | 용도 |
|---|---|
| `MainScene.unity` | 주요 게임플레이 (NavMesh 베이크 완료) |
| `BossTest.unity` | 보스 AI 단독 테스트 |
| `CharacterSampleScene.unity` | 플레이어 이동/전투 테스트 |
| `MapTest 2.unity` | 맵 레이아웃 테스트 |

## 스크립트 구조

모든 스크립트는 `Assets/Script/` 하위 4개 디렉토리에 위치합니다:

- **`Character/`** — 플레이어 상태 머신 (`CharacterState.cs`), 이동 및 입력 (`CharacterMove.cs`), 공격 충돌 (`CharacterAttackZone.cs`)
- **`Boss/`** — 보스 AI 및 FSM (`Boss.cs`), 설정 ScriptableObject (`BossData.cs`), 보스 공격 충돌 (`NormalAttackZone.cs`)
- **`Common/`** — `IDamageable` 인터페이스, `DamageVO` 데이터 구조체, `InputManager` (키 리바인딩 정적 헬퍼)
- **`ScriptsUi/`** — 모든 UI: `UIController.cs` (메인 상태 머신), `PlayerHUDController.cs` / `BossHUDController.cs` (HUD), `UIState.cs` (구르기/힐 아이콘 오버레이), `KeyConfigPanel.cs` / `KeyConfigRow.cs` (키 설정 UI), `SettingsController.cs`, `SaveManager.cs`, `AudioManager.cs`, `ConsumableSlotUI.cs`, `StaminaBar.cs`, `DualSliderBar.cs`

## 핵심 시스템

### 상태 머신
- **플레이어** (`CharacterState.cs`): 체력, 스태미나, 상태(`currentState: StateType`)를 관리. 무적 여부는 `IsInvincible()` 메서드, 사망 여부는 `IsDead` 프로퍼티로 확인. 스태미나는 공격/구르기 시 소모되고 일정 딜레이 후 회복.
- **보스** (`Boss.cs`): 4개 상태 — `Idle`, `Move`, `Attack`, `Death`. HP 50% 시 2페이즈 진입, 이동속도와 공격 데미지 증가. NavMesh로 이동.

### 데미지 흐름
`DamageVO`는 `amount`(int)와 `damageType`(`noDamage/soft/normal/hard/veryHard/instantKill` 중 하나)을 담습니다. 충돌 존에서 `IDamageable.GetDamage(DamageVO)`를 호출합니다. `CharacterState`와 `Boss` 모두 `IDamageable`을 구현합니다. 플레이어는 구르기 중(`StateType.Dodge`) 무적 판정이 있습니다.

### 입력 및 키 리바인딩
Unity New Input System(`Assets/InputSystem_Actions.inputactions`) 사용. `InputManager.cs`가 런타임 키 리바인딩 정적 헬퍼를 제공합니다. 바인딩은 `SaveManager`를 통해 저장/불러오기(`Application.persistentDataPath`의 JSON).

### 세이브 시스템
`SaveManager.cs`는 Newtonsoft.Json을 사용해 `SaveData.cs`를 JSON으로 직렬화하는 싱글톤입니다. 저장 항목: 그래픽 설정, 오디오 볼륨, FOV, 마우스 감도, 키 바인딩, 플레이 시간. 30초마다 자동 저장, 설정 변경 및 종료 시에도 저장.

### UI 상태 머신
`UIController.cs`가 패널 전환을 관리합니다: `StartMenu → Game → (Pause | Settings | KeyConfig | GameOver | Clear)`. 이벤트 구독: `characterState.Damaged`(피격 통계), `boss.OnClear`(클리어). 플레이어 사망은 `Update()`의 `CheckDeath()` 폴링으로 감지. 게임 진행 중 `Time.timeScale = 0f`이 되는 구간(일시정지, GameOver, Clear)에서 UI 코루틴은 `WaitForSecondsRealtime` / `Time.unscaledDeltaTime` 사용 필요.

### 오디오
`AudioManager.cs` 싱글톤. BGM과 SE 각각 별도 `AudioSource` 채널을 `AudioMixer`로 라우팅. 볼륨 값은 `SaveManager`로 유지됩니다.

### 락온 시스템
`CharacterMove.cs`가 락온 타게팅을 처리합니다. 활성화 시 Cinemachine 카메라와 플레이어 회전이 보스를 추적합니다. 보스 사망 또는 범위 이탈 시 자동으로 락온이 해제됩니다.

## 주요 데이터 흐름

```
입력 (New Input System)
  → CharacterMove (이동, 구르기, 공격 트리거)
    → CharacterAttackZone (충돌 → DamageVO → Boss.GetDamage)
    → CharacterState (스태미나/체력 변경)

보스 FSM (NavMesh + 타이머 기반)
  → NormalAttackZone (충돌 → DamageVO → CharacterState.GetDamage)
  → HP 50%에서 2페이즈 진입 (속도/데미지 배율 적용)

UIController (characterState.Damaged 구독, boss.OnClear 구독, Update 폴링으로 IsDead 확인)
  → GameOver 또는 Clear 패널 활성화
```

## 허용/제한 파일 조작

`.claude/settings.json.txt` 기준:
- **허용**: 읽기, git 명령어, CSharpier 포매터
- **금지**: `Library/`, Unity 바이너리 파일, `.meta` 파일

`.meta` 파일과 `Library/` 하위 파일은 절대 편집하지 마십시오 — Unity가 자동으로 관리합니다.
