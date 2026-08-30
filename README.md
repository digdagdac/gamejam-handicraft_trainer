# 금속활자장 (Handicraft) - ML-Agents Trainer

> Unity 미니게임 3종으로 구성된 게임잼 작품과, 그 타자 미니게임을 ML-Agents 학습 환경으로 재구성한 실험을 함께 담은 저장소입니다.

| 항목 | 내용 |
|---|---|
| 엔진 | Unity 2021.3.45f2 |
| 언어 | C# |
| 학습 | ML-Agents 0.26.0 / PPO / PyTorch 1.8.1 |
| 개인 작성 스크립트 | `Assets/Scripts/` 48개 |
| 역할 | 클라이언트 프로그래밍, 게임 루프 구조, 학습 환경 재구성 |

---

## 1. 이 저장소를 보는 순서

시간이 없다면 아래 3개만 보셔도 됩니다.

| 순서 | 파일 | 무엇을 볼 수 있는지 |
|---|---|---|
| 1 | [`Assets/Scripts/Scene/BaseGame.cs`](Assets/Scripts/Scene/BaseGame.cs) | 미니게임 3종을 하나의 추상 클래스로 공통화한 구조 |
| 2 | [`Assets/Scripts/ML/TypingAgent.cs`](Assets/Scripts/ML/TypingAgent.cs) | 게임 루프를 학습 가능한 계약으로 바꾼 Agent 구현 |
| 3 | [`Assets/Scripts/CarveGameScene/TypingActionMap.cs`](Assets/Scripts/CarveGameScene/TypingActionMap.cs) | 게임 입력 결과를 학습용 관측 데이터로 변환하는 경계 |

더 보고 싶다면 [`docs/ML_EXPERIMENT.md`](docs/ML_EXPERIMENT.md)에 학습 실패와 재조정 과정이 로그와 함께 정리되어 있습니다.

---

## 2. 게임 구조

한 판의 흐름은 아래와 같습니다.

```
0.Intro  ->  1.Lobby  ->  2.Game1 / 2.Game2 / 2.Game3  ->  99.Ending
                                    |
                          BaseGame (abstract)
                                    |
          +-------------------------+-------------------------+
          |                         |                         |
   TypingStyleGame          TimingStyleGame          FluidTimingStyleGame
   (타자)                    (타이밍)                   (유체 타이밍)
```

미니게임 3종은 규칙이 서로 다르지만, 시간 제한과 점수 집계와 성공·실패 판정은 동일하게 필요했습니다.
그래서 공통 흐름을 [`BaseGame`](Assets/Scripts/Scene/BaseGame.cs)에 두고 각 미니게임은 필요한 부분만 재정의합니다.

| 계층 | 책임 | 파일 |
|---|---|---|
| BaseGame | 시간 관리, 점수 집계, 시작·종료 판정 | `Scene/BaseGame.cs` |
| 개별 미니게임 | 각 게임 고유 규칙 | `Scene/TypingStyleGame.cs` 외 |
| GameManager | 게임 모드 전환, 전역 이벤트 | `Core/GameManager.cs` |
| DataManager / AccountManager | 스테이지 데이터, 진행 상태 저장 | `Core/` |
| Popup | 성공·실패·수집·설정 UI | `Popup/` 13개 |

---

## 3. ML-Agents 실험: 게임 루프를 학습 가능하게 만들기

### 문제

타자 미니게임에 강화학습을 붙이려 했지만, 모델을 손대기 전에 게임 자체가 학습에 부적합했습니다.

- 로비에서 미니게임 3종으로 이어지는 전체 흐름과 학습 경계가 섞여 있었습니다.
- Episode가 시작될 때마다 씬을 다시 불러오면서 무한 루프에 빠졌습니다.

### 해결

학습 범위를 타자 씬으로 좁히고, 씬 리로드 대신 상태 값만 초기화하도록 다시 설계했습니다.

```csharp
// TypingAgent.cs
public override void OnEpisodeBegin()
{
    currentCombo = 0;
    previousScore = 0;
    isInFeverMode = false;

    if (gameScene != null)
    {
        gameScene.RestartGame();
    }
}
```

게임 쪽에는 학습이 필요한 상태만 노출하는 메서드를 추가했습니다.
`InitializeGameState()`, `RestartGame()`, `GetRemainingTime()`, `GetScore()`가 그 경계입니다.

### 환경 계약

| 구분 | 설계 | 근거 |
|---|---|---|
| Observation | 보이는 문자별 자모 one-hot + 콤보/10 + 점수/1000 + 피버 여부 | `TypingAgent.cs` `CollectObservations` |
| Action | Discrete index를 실제 자모 `KeyCode`로 변환 | `ConvertActionToKeyCode` |
| Reward | 정답 +1 / 오답 -0.5 / 콤보 +0.2 / 피버 +0.5 / 시간초과 -1 | `TypingAgent.cs` 보상 필드 |
| Episode | 콤보·점수·피버 초기화, 시간 종료로 판정 | `OnEpisodeBegin` |

### 결과

| 실행 | steps | 최종 reward | 의미 |
|---|---:|---:|---|
| `test15` | 29,744 | -26.98 | 최고 checkpoint |
| `test15` | 32,080 | **-1085.44** | 학습 발산 |
| `typing_reward_v2` | 100,004 | **+24.71** | 재조정 후 수렴 |

첫 실행은 중간까지 개선되다가 발산했습니다. 원인을 학습 설정에서 찾아 아래를 바꿨습니다.

| 설정 | test15 | typing_reward_v2 |
|---|---|---|
| `buffer_size` | 12000 | 4096 |
| `batch_size` | 64 | 128 |
| `time_horizon` | 128 | 64 |
| `time_scale` | 0.1 | 20 |
| `no_graphics` | false | true |

수치는 저장소에 커밋된 로그에서 직접 확인할 수 있습니다.

- [`Assets/results/test15/run_logs/training_status.json`](Assets/results/test15/run_logs/training_status.json)
- [`Assets/results/typing_reward_v2/run_logs/training_status.json`](Assets/results/typing_reward_v2/run_logs/training_status.json)
- [`Assets/results/typing_reward_v2/configuration.yaml`](Assets/results/typing_reward_v2/configuration.yaml)

### 한계

이 결과는 **학습 지표 개선**입니다. 실제 게임의 재미나 레벨 디자인이 좋아졌다는 증거는 아닙니다.
에이전트가 사람보다 잘 친다는 의미도 아닙니다. 검증한 것은 "게임 루프를 반복 실행 가능한 학습 환경으로 바꿀 수 있는가"까지입니다.

---

## 4. 실행 방법

### 게임 실행

1. Unity 2021.3.45f2로 이 저장소를 엽니다.
2. `Assets/Scenes/0.Intro.unity`를 열고 Play를 누릅니다.

### 학습 재현

```bash
mlagents-learn Assets/results/typing_reward_v2/configuration.yaml --run-id=typing_reward_v3
```

학습 전용 빌드는 [`Assets/Editor/TrainingBuild.cs`](Assets/Editor/TrainingBuild.cs)로 생성합니다.

---

## 5. 담당 범위

| 구분 | 내용 |
|---|---|
| 직접 작성 | `Assets/Scripts/` 48개 스크립트, 게임 루프 구조, 미니게임 3종 구현, 학습 환경 재구성 |
| 직접 작성 아님 | `Assets/Plugins/` (DOTween, UniRx), `LiquidSimulation-Unity-Games`, 아트 리소스 |

아트 리소스와 서드파티 플러그인은 원저작자에게 권리가 있습니다.
