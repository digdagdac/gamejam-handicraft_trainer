using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Unity.MLAgents.Policies;
using System.Collections.Generic;

public class TypingAgent : Agent
{
    private bool isInitialized = false;
    [SerializeField] private CarveGameScene gameScene;
    
    [Header("Observation Settings")]
    [SerializeField] private bool enableVisualObservations = true;
    [SerializeField] private Camera observationCamera;
    [SerializeField] private int visualObservationWidth = 84;
    [SerializeField] private int visualObservationHeight = 84;
    
    [Header("Reward Settings")]
    [SerializeField] private float correctCharacterReward = 1.0f;
    [SerializeField] private float wrongCharacterPenalty = -0.5f;
    [SerializeField] private float comboReward = 0.2f;
    [SerializeField] private float timeoutPenalty = -1.0f;
    [SerializeField] private float feverModeReward = 0.5f;
    
    private int currentCombo = 0;
    private int previousScore = 0;
    private Dictionary<char, int> charToActionMap;
    private char[] currentVisibleChars = new char[5];
    private bool isInFeverMode = false;
    
    public override void Initialize()
    {
        if (gameScene == null)
        {
            gameScene = FindObjectOfType<CarveGameScene>();
        }

        InitializeCharacterMapping();
        SetupSensors();
    }

    private void InitializeCharacterMapping()
    {
        // 자음/모음을 액션 인덱스에 매핑
        charToActionMap = new Dictionary<char, int>()
        {
            {'ㄱ', 0}, {'ㄴ', 1}, {'ㄷ', 2}, {'ㄹ', 3}, {'ㅁ', 4},
            {'ㅂ', 5}, {'ㅅ', 6}, {'ㅇ', 7}, {'ㅈ', 8}, {'ㅊ', 9},
            {'ㅋ', 10}, {'ㅌ', 11}, {'ㅍ', 12}, {'ㅎ', 13},
            {'ㅏ', 14}, {'ㅐ', 15}, {'ㅑ', 16}, {'ㅓ', 17}, {'ㅔ', 18},
            {'ㅕ', 19}, {'ㅗ', 20}, {'ㅛ', 21}, {'ㅜ', 22}, {'ㅠ', 23},
            {'ㅡ', 24}, {'ㅣ', 25}
        };
    }

    private void SetupSensors()
    {
        // 시각적 관찰 설정
        if (enableVisualObservations && observationCamera != null)
        {
            var cameraSensor = gameObject.AddComponent<CameraSensorComponent>();
            cameraSensor.Camera = observationCamera;
            cameraSensor.Width = visualObservationWidth;
            cameraSensor.Height = visualObservationHeight;
            cameraSensor.Grayscale = true;
            cameraSensor.ObservationType = ObservationType.Default;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (gameScene == null) return;

        // 현재 보이는 문자들의 상태 관찰
        foreach (char c in currentVisibleChars)
        {
            // 각 문자에 대한 one-hot 인코딩
            foreach (var kvp in charToActionMap)
            {
                sensor.AddObservation(c == kvp.Key ? 1.0f : 0.0f);
            }
        }

        // 게임 상태 관찰
        sensor.AddObservation(currentCombo / 10.0f); // 정규화된 콤보 수
        sensor.AddObservation(gameScene.GetScore() / 1000.0f); // 정규화된 점수
        sensor.AddObservation(isInFeverMode ? 1.0f : 0.0f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (gameScene == null) return;

        int actionIndex = actions.DiscreteActions[0];
        KeyCode selectedKey = ConvertActionToKeyCode(actionIndex);
        
        // 입력 처리 및 결과 평가
        int previousCombo = currentCombo;
        int previousScore = gameScene.GetScore();
        bool wasInFeverMode = isInFeverMode;
        
        // 게임에 입력 전달
        gameScene.CheckInput(selectedKey);
        
        // 결과 평가 및 보상 계산
        int newScore = gameScene.GetScore();
        float reward = CalculateReward(previousScore, newScore, previousCombo, wasInFeverMode);
        AddReward(reward);
        
        // 에피소드 종료 조건 체크
        if (gameScene.IsTimeOver())
        {
            AddReward(timeoutPenalty);
            EndEpisode();
        }
    }

    private float CalculateReward(int prevScore, int newScore, int prevCombo, bool prevFeverMode)
    {
        float totalReward = 0f;
        
        // 점수 기반 보상
        if (newScore > prevScore)
        {
            totalReward += correctCharacterReward;
            
            // 콤보 보상
            if (currentCombo > prevCombo)
            {
                totalReward += comboReward * (currentCombo / 5.0f);
            }
            
            // Fever 모드 보상
            if (!prevFeverMode && isInFeverMode)
            {
                totalReward += feverModeReward;
            }
        }
        else
        {
            totalReward += wrongCharacterPenalty;
        }
        
        return totalReward;
    }

    private KeyCode ConvertActionToKeyCode(int action)
    {
        // 액션 인덱스를 키코드로 변환
        switch(action)
        {
            case 0: return KeyCode.R;  // ㄱ
            case 1: return KeyCode.S;  // ㄴ
            case 2: return KeyCode.E;  // ㄷ
            case 3: return KeyCode.F;  // ㄹ
            case 4: return KeyCode.A;  // ㅁ
            case 5: return KeyCode.Q;  // ㅂ
            case 6: return KeyCode.T;  // ㅅ
            case 7: return KeyCode.D;  // ㅇ
            case 8: return KeyCode.W;  // ㅈ
            case 9: return KeyCode.C;  // ㅊ
            case 10: return KeyCode.Z; // ㅋ
            case 11: return KeyCode.X; // ㅌ
            case 12: return KeyCode.V; // ㅍ
            case 13: return KeyCode.G; // ㅎ
            case 14: return KeyCode.K; // ㅏ
            case 15: return KeyCode.O; // ㅐ
            case 16: return KeyCode.I; // ㅑ
            case 17: return KeyCode.J; // ㅓ
            case 18: return KeyCode.P; // ㅔ
            case 19: return KeyCode.U; // ㅕ
            case 20: return KeyCode.H; // ㅗ
            case 21: return KeyCode.Y; // ㅛ
            case 22: return KeyCode.N; // ㅜ
            case 23: return KeyCode.B; // ㅠ
            case 24: return KeyCode.M; // ㅡ
            case 25: return KeyCode.L; // ㅣ
            default: return KeyCode.None;
        }
    }

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

    // 휴리스틱 모드 (수동 테스트용)
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = 0;  // 기본값

        // 키보드 입력을 액션으로 변환
        foreach (var kvp in charToActionMap)
        {
            KeyCode key = ConvertActionToKeyCode(kvp.Value);
            if (Input.GetKeyDown(key))
            {
                discreteActions[0] = kvp.Value;
                break;
            }
        }
    }

    // 현재 보이는 문자들 업데이트 (게임 씬에서 호출)
    public void UpdateVisibleCharacters(char[] chars)
    {
        if (chars.Length != 5) return;
        currentVisibleChars = (char[])chars.Clone();
    }

    // Fever 모드 상태 업데이트 (게임 씬에서 호출)
    public void UpdateFeverMode(bool isFever)
    {
        isInFeverMode = isFever;
    }

    // 콤보 상태 업데이트 (게임 씬에서 호출)
    public void UpdateCombo(int combo)
    {
        currentCombo = combo;
    }
}