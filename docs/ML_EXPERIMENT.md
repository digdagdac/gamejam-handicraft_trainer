# ML-Agents 실험 기록

타자 미니게임을 강화학습 환경으로 재구성한 과정을 기록한 문서입니다.
모든 수치는 이 저장소에 커밋된 로그에서 확인할 수 있습니다.

---

## 1. 왜 모델보다 게임 루프를 먼저 고쳤는가

처음에는 학습이 아예 진행되지 않았습니다. 원인은 하이퍼파라미터가 아니라 게임 구조였습니다.

| 증상 | 원인 |
|---|---|
| 학습 경계가 불분명 | 로비에서 미니게임 3종으로 이어지는 전체 흐름이 한 루프에 묶여 있었음 |
| Episode가 진행되지 않음 | Episode 시작마다 씬을 다시 로드하면서 무한 루프 발생 |

학습 범위를 타자 씬으로 좁히고, 씬 리로드 대신 상태 값만 초기화하도록 바꿨습니다.

```csharp
// Assets/Scripts/ML/TypingAgent.cs
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

게임 쪽에는 학습에 필요한 상태만 노출하는 경계를 만들었습니다.

| 메서드 | 역할 |
|---|---|
| `RestartGame()` | 씬 리로드 없이 플레이 상태 초기화 |
| `InitializeGameState()` | 점수, 콤보, 피버 초기화 |
| `GetScore()` | 보상 계산용 점수 조회 |
| `GetRemainingTime()` | Episode 종료 판정 |
| `CheckInput(KeyCode)` | 에이전트 액션을 게임 입력으로 전달 |

---

## 2. 환경 계약

### Observation

```csharp
// 보이는 문자 5칸에 대한 자모 one-hot
foreach (char c in currentVisibleChars)
{
    foreach (var kvp in charToActionMap)
    {
        sensor.AddObservation(c == kvp.Key ? 1.0f : 0.0f);
    }
}

sensor.AddObservation(currentCombo / 10.0f);
sensor.AddObservation(gameScene.GetScore() / 1000.0f);
sensor.AddObservation(isInFeverMode ? 1.0f : 0.0f);
```

콤보와 점수는 정규화했습니다. 스케일이 큰 값을 그대로 넣으면 학습이 불안정해지기 때문입니다.

### Action

Discrete action index를 실제 자모 `KeyCode`로 변환합니다.

```csharp
case 22: return KeyCode.N; // ㅜ
case 23: return KeyCode.B; // ㅠ
case 24: return KeyCode.M; // ㅡ
case 25: return KeyCode.L; // ㅣ
```

### Reward

```csharp
[SerializeField] private float correctCharacterReward = 1.0f;
[SerializeField] private float wrongCharacterPenalty = -0.5f;
[SerializeField] private float comboReward = 0.2f;
[SerializeField] private float timeoutPenalty = -1.0f;
[SerializeField] private float feverModeReward = 0.5f;
```

보상은 점수 증가를 기준으로 판정합니다. 게임 규칙과 학습 신호를 따로 관리하지 않기 위한 선택입니다.

---

## 3. 첫 실행: test15에서의 발산

`Assets/results/test15/run_logs/training_status.json`

| checkpoint | steps | reward |
|---|---:|---:|
| 1 | 29,396 | -72.03 |
| 2 | 29,658 | -66.02 |
| 3 | 29,744 | **-26.98** |
| 4 | 31,354 | -73.75 |
| final | 32,080 | **-1085.44** |

29,744 step에서 -26.98까지 개선되었지만, 이후 급격히 악화되어 최종 -1085.44로 발산했습니다.
중간 지점이 최고 성능이었다는 점이 문제의 성격을 보여줍니다. 정책이 수렴하지 못하고 무너진 것입니다.

---

## 4. 재조정: typing_reward_v2

발산 원인을 학습 설정에서 찾고 아래를 조정했습니다.

| 설정 | test15 | typing_reward_v2 | 의도 |
|---|---|---|---|
| `buffer_size` | 12000 | 4096 | 업데이트 주기를 짧게 |
| `batch_size` | 64 | 128 | 그래디언트 분산 감소 |
| `time_horizon` | 128 | 64 | 짧은 에피소드에 맞춤 |
| `max_steps` | 5000000 | 100000 | 실험 사이클 단축 |
| `time_scale` | 0.1 | 20 | 학습 속도 확보 |
| `no_graphics` | false | true | 렌더링 비용 제거 |
| `env_path` | null | 전용 빌드 경로 | 에디터 실행 대신 학습 빌드 사용 |

`Assets/results/typing_reward_v2/run_logs/training_status.json`

| checkpoint | steps | reward |
|---|---:|---:|
| final | 100,004 | **+24.71** |

음수에서 양수로 전환되었고, 발산 없이 종료되었습니다.

---

## 5. 보존 산출물

| 파일 | 내용 |
|---|---|
| `Assets/results/typing_reward_v2/TypingGame.onnx` | 최종 추론 모델 |
| `Assets/results/typing_reward_v2/TypingGame/TypingGame-100004.onnx` | 100,004 step checkpoint |
| `Assets/results/typing_reward_v2/TypingGame/checkpoint.pt` | PyTorch checkpoint |
| `Assets/results/typing_reward_v2/configuration.yaml` | PPO 설정 전문 |
| `Assets/results/test15/` | 발산 사례 원본 로그 |

실패한 실행을 지우지 않고 남겼습니다. 무엇이 안 되었는지가 함께 있어야 판단 근거가 됩니다.

---

## 6. 이 실험이 증명하는 것과 아닌 것

### 증명하는 것

- 기존 게임 루프를 반복 실행 가능한 학습 환경으로 재구성할 수 있다
- 게임 상태와 학습 신호 사이의 경계를 코드로 만들 수 있다
- 학습 실패를 로그로 진단하고 설정을 근거 있게 조정할 수 있다

### 증명하지 않는 것

- 실제 게임의 재미나 레벨 디자인 개선
- 에이전트가 사람보다 뛰어난 플레이를 한다는 것
- 상용 QA 파이프라인 수준의 자동화

reward 개선은 학습 지표입니다. 게임 품질 지표가 아닙니다.

---

## 7. 재현 방법

```bash
mlagents-learn Assets/results/typing_reward_v2/configuration.yaml --run-id=typing_reward_v3
```

학습 전용 빌드는 `Assets/Editor/TrainingBuild.cs`로 생성합니다.

| 환경 | 버전 |
|---|---|
| Unity | 2021.3.45f2 |
| ML-Agents | 0.26.0 |
| PyTorch | 1.8.1 |
| 알고리즘 | PPO |
