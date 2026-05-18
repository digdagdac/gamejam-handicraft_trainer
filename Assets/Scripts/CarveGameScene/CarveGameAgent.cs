using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;
using System.Text;

public class CarveGameAgent : Agent
{
    public CarveGameScene gameScene; // 게임 씬 참조
    private Dictionary<int, KeyCode> actionToKey; // 행동을 키 입력으로 매핑
    private const int MAX_INIT_ATTEMPTS = 3;
    private int initAttempts = 0;

    public override void Initialize()
    {
        gameScene = GetComponent<CarveGameScene>();
        InitializeActionMapping();
    }

    private void InitializeActionMapping()
    {
        actionToKey = new Dictionary<int, KeyCode>()
        {
         
            {0, KeyCode.Q},  // ㅂ
            {1, KeyCode.W},  // ㅈ
            {2, KeyCode.E},  // ㄷ
            {3, KeyCode.R},  // ㄱ
            {4, KeyCode.T},  // ㅅ
            {5, KeyCode.Y},  // ㅛ
            {6, KeyCode.U},  // ㅕ
            {7, KeyCode.I},  // ㅑ
            {8, KeyCode.O},  // ㅐ
            {9, KeyCode.P},  // ㅔ
            {10, KeyCode.A}, // ㅁ
            {11, KeyCode.S}, // ㄴ
            {12, KeyCode.D}, // ㅇ
            {13, KeyCode.F}, // ㄹ
            {14, KeyCode.G}, // ㅎ
            {15, KeyCode.H}, // ㅗ
            {16, KeyCode.J}, // ㅓ
            {17, KeyCode.K}, // ㅏ
            {18, KeyCode.L}, // ㅣ
            {19, KeyCode.Z}, // ㅋ
            {20, KeyCode.X}, // ㅌ
            {21, KeyCode.C}, // ㅊ
            {22, KeyCode.V}, // ㅍ
            {23, KeyCode.B}, // ㅠ
            {24, KeyCode.N}, // ㅜ
            {25, KeyCode.M}, // ㅡ

        
         
        };
    }
    private void Update()
    {
        // 에이전트의 행동을 수동으로 테스트하기 위한 입력 처리
        if (Input.anyKeyDown)
        {
            foreach (var keyMapping in actionToKey)
            {
                if (Input.GetKeyDown(keyMapping.Value))
                {
                    gameScene.CheckInput(keyMapping.Value);
                    break;
                }
            }
        }
    }
    public override void OnEpisodeBegin()
    {
        Debug.Log("에피소드 시작");
        
        if (!TryInitializeGameScene())
        {
            Debug.LogError($"GameScene 초기화 실패 (시도 횟수: {initAttempts})");
            return;
        }

        try
        {
            // 게임 상태 초기화
            gameScene.Clear();
            gameScene.InitializeGameState();
            gameScene.Init();
            
            // 게임 영역 초기화를 위한 초기 문자열 생성
            StringBuilder initialStr = new StringBuilder(5);
            for (int i = 0; i < 5; ++i)
            {
                initialStr.Append(gameScene.GetCurrentChar());
            }
            
            // 게임 영역 초기화
            gameScene._area.Init(initialStr.ToString());
            
            initAttempts = 0;
            Debug.Log("게임 씬 초기화 성공");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"게임 재시작 중 오류: {e.Message}");
            EndEpisode();
        }
    }

    private bool TryInitializeGameScene()
    {
        if (gameScene != null) return true;
        
        initAttempts++;
        if (initAttempts > MAX_INIT_ATTEMPTS)
        {
            enabled = false;
            return false;
        }

        gameScene = GetComponent<CarveGameScene>();
        if (gameScene == null)
        {
            gameScene = FindObjectOfType<CarveGameScene>();
        }

        return gameScene != null;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
       
        sensor.AddObservation(gameScene.GetScore());
        
        
        char currentChar = gameScene.GetCurrentChar();
        float normalizedChar = (float)((int)currentChar - 0xAC00) / (0xD7A3 - 0xAC00); // 한글 유니코드 범위 정규화
        sensor.AddObservation(normalizedChar);


       
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (gameScene == null) return;

        // actions.DiscreteActions[0]는 0~25 사이의 값
        int action = actions.DiscreteActions[0];  // 26가지 키보드 입력 중 하나 선택
        
        // 액션을 키보드 입력으로 변환
        KeyCode selectedKey = GetKeyCodeFromAction(action);
        if (selectedKey != KeyCode.None)
        {
            gameScene.CheckInput(selectedKey);
        }

        // 보상 계산
        float reward = CalculateReward();
        AddReward(reward);

        // 타임오버 체크
        if (!gameScene.UpdateGame())
        {
            Debug.Log("에피소드 종료");
            gameScene.FinishGame(false);
            EndEpisode();
        }
    }

    private KeyCode GetKeyCodeFromAction(int action)
    {
        // 26가지 키보드 입력에 대한 매핑
        return action switch
        {
            0 => KeyCode.Q,   // ㅂ
            1 => KeyCode.W,   // ㅈ
            2 => KeyCode.E,   // ㄷ
            3 => KeyCode.R,   // ㄱ
            4 => KeyCode.T,   // ㅅ
            5 => KeyCode.Y,   // ㅛ
            6 => KeyCode.U,   // ㅕ
            7 => KeyCode.I,   // ㅑ
            8 => KeyCode.O,   // ㅐ
            9 => KeyCode.P,   // ㅔ
            10 => KeyCode.A,  // ㅁ
            11 => KeyCode.S,  // ㄴ
            12 => KeyCode.D,  // ㅇ
            13 => KeyCode.F,  // ㄹ
            14 => KeyCode.G,  // ㅎ
            15 => KeyCode.H,  // ㅗ
            16 => KeyCode.J,  // ㅓ
            17 => KeyCode.K,  // ㅏ
            18 => KeyCode.L,  // ㅣ
            19 => KeyCode.Z,  // ㅋ
            20 => KeyCode.X,  // ㅌ
            21 => KeyCode.C,  // ㅊ
            22 => KeyCode.V,  // ㅍ
            23 => KeyCode.B,  // ㅠ
            24 => KeyCode.N,  // ㅜ
            25 => KeyCode.M,  // ㅡ
            _ => KeyCode.None
        };
    }

    private float CalculateReward()
    {
        float reward = 0f;
        
        // 1. 기본 점수 기반 보상
        reward += gameScene.GetScore() * 0.5f;
        
       

        return reward;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 수동 테스트를 위한 휴리스틱 구현
        var discreteActionsOut = actionsOut.DiscreteActions;
        
        // 키보드 입력을 행동으로 변환하는 로직
        if (Input.GetKeyDown(KeyCode.Q)) discreteActionsOut[0] = 0;
        else if (Input.GetKeyDown(KeyCode.W)) discreteActionsOut[0] = 1;
        else if (Input.GetKeyDown(KeyCode.E)) discreteActionsOut[0] = 2;
        else if (Input.GetKeyDown(KeyCode.R)) discreteActionsOut[0] = 3;
        else if (Input.GetKeyDown(KeyCode.T)) discreteActionsOut[0] = 4;
        else if (Input.GetKeyDown(KeyCode.Y)) discreteActionsOut[0] = 5;
        else if (Input.GetKeyDown(KeyCode.U)) discreteActionsOut[0] = 6;
        else if (Input.GetKeyDown(KeyCode.I)) discreteActionsOut[0] = 7;
        else if (Input.GetKeyDown(KeyCode.O)) discreteActionsOut[0] = 8;
        else if (Input.GetKeyDown(KeyCode.P)) discreteActionsOut[0] = 9;
        else if (Input.GetKeyDown(KeyCode.A)) discreteActionsOut[0] = 10;
        else if (Input.GetKeyDown(KeyCode.S)) discreteActionsOut[0] = 11;
        else if (Input.GetKeyDown(KeyCode.D)) discreteActionsOut[0] = 12;
        else if (Input.GetKeyDown(KeyCode.F)) discreteActionsOut[0] = 13;
        else if (Input.GetKeyDown(KeyCode.G)) discreteActionsOut[0] = 14;
        else if (Input.GetKeyDown(KeyCode.H)) discreteActionsOut[0] = 15;
        else if (Input.GetKeyDown(KeyCode.J)) discreteActionsOut[0] = 16;
        else if (Input.GetKeyDown(KeyCode.K)) discreteActionsOut[0] = 17;
        else if (Input.GetKeyDown(KeyCode.L)) discreteActionsOut[0] = 18;
        else if (Input.GetKeyDown(KeyCode.Z)) discreteActionsOut[0] = 19;
        else if (Input.GetKeyDown(KeyCode.X)) discreteActionsOut[0] = 20;
        else if (Input.GetKeyDown(KeyCode.C)) discreteActionsOut[0] = 21;
        else if (Input.GetKeyDown(KeyCode.V)) discreteActionsOut[0] = 22;
        else if (Input.GetKeyDown(KeyCode.B)) discreteActionsOut[0] = 23;
        else if (Input.GetKeyDown(KeyCode.N)) discreteActionsOut[0] = 24;
        else if (Input.GetKeyDown(KeyCode.M)) discreteActionsOut[0] = 25;
    }
}
