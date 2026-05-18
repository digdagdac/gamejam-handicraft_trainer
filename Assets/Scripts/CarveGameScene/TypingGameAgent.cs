// using Unity.MLAgents;
// using Unity.MLAgents.Actuators;
// using Unity.MLAgents.Sensors;
// using UnityEngine;
// using Unity.MLAgents.Policies;
// using System.Collections.Generic;

// public class TypingGameAgent : Agent
// {
//     [SerializeField] 
//     private CarveGameScene gameScene;  // Inspector에서 할당
//     private float previousScore = 0f;
//     private int previousCombo = 0;
//     private bool isInitialized = false;
//     private Dictionary<KeyCode, char> _keyCodeNCharPair;

//     private void Awake()
//     {
//         InitializeGameScene();
//     }

//     private void InitializeGameScene()
//     {
//         Debug.Log("1. 초기화 시작");
        
//         if (gameScene == null)
//         {
//             gameScene = GetComponent<CarveGameScene>();
//         }
//         if (gameScene == null)
//         {
//             gameScene = FindObjectOfType<CarveGameScene>();
//         }
//         if (gameScene == null)
//         {
//             Debug.LogError("CarveGameScene을 찾을 수 없음!");
//             enabled = false;
//             return;
//         }

//         isInitialized = gameScene != null;
//         Debug.Log($"2. 초기화 완료: {(isInitialized ? "성공" : "실패")}");
//     }

//     protected override void OnEnable()
//     {
//         base.OnEnable();
//         if (!isInitialized)
//         {
//             InitializeGameScene();
//         }
//         MakeKeyCodeNCharTable();
//     }

//     private void Start()
//     {
//         // Start에서 한 번 더 초기화 시도
//         if (gameScene == null)
//         {
//             gameScene = GetComponent<CarveGameScene>();
//             if (gameScene == null)
//             {
//                 gameScene = FindObjectOfType<CarveGameScene>();
//             }
//             if (gameScene == null)
//             {
//                 Debug.LogError("CarveGameScene을 찾을 수 없습니다!");
//                 enabled = false;
//                 return;
//             }
//         }
//         isInitialized = true;
//     }

//     public override void OnEpisodeBegin()
//     {
//         if(gameScene != null)
//         {
//             gameScene.RestartGame();
//         }
//     }

//     public override void CollectObservations(VectorSensor sensor)
//     {
//         if (gameScene == null) return;

//         // 현재 입력 가능한 한글 자모음 관찰
//         var currentWord = gameScene.GetCurrentWord();
//         if (!string.IsNullOrEmpty(currentWord))
//         {
//             sensor.AddObservation((int)currentWord[0]); // 한글 자모음 코드값 관찰
//         }
//         else
//         {
//             sensor.AddObservation(0);
//         }

//         // 게임 상태 관찰
//         sensor.AddObservation(gameScene.GetIsPlaying());
//         sensor.AddObservation(gameScene.GetScore());
//         sensor.AddObservation(gameScene.GetRemainingTime());
//     }

//     public override void OnActionReceived(ActionBuffers actions)
//     {
//         if (gameScene == null) return;

//         int action = actions.DiscreteActions[0];
//         if (action > 0)  // 0은 아무것도 하지 않음
//         {
//             char inputChar = (char)('a' + action - 1); // 1-26은 a-z에 매핑
//             gameScene.ProcessInput(inputChar);
            
//             // 보상 계산
//             float reward = CalculateReward();
//             AddReward(reward);
//         }
//     }

//     private float CalculateReward()
//     {
//         float reward = 0;
        
//         // 올바른 입력에 대한 보상
//         if (gameScene.IsLastInputCorrect())
//         {
//             reward += 0.1f;
//         }
//         else
//         {
//             reward -= 0.05f;
//         }

//         // 점수 변화에 따른 보상
//         reward += (gameScene.GetScore() - lastScore) * 0.01f;
//         lastScore = gameScene.GetScore();

//         return reward;
//     }

//     private float lastScore = 0;

//     public override void Heuristic(in ActionBuffers actionsOut)
//     {
//         // 사람이 직접 플레이할 때의 로직
//         var discreteActionsOut = actionsOut.DiscreteActions;
        
//         // 키보드 입력을 받아서 액션으로 변환
//         if(Input.anyKeyDown)
//         {
//             for(KeyCode key = KeyCode.A; key <= KeyCode.Z; key++)
//             {
//                 if(Input.GetKeyDown(key))
//                 {
//                     discreteActionsOut[0] = (int)key - (int)KeyCode.A;
//                     break;
//                 }
//             }
//         }
//     }

//     private void AddKeyCodeNCharPair(KeyCode key, char c)
//     {
//         _keyCodeNCharPair.Add(key, c);
//     }

//     private void MakeKeyCodeNCharTable()
//     {
//         _keyCodeNCharPair = new Dictionary<KeyCode, char>();
//         AddKeyCodeNCharPair(KeyCode.Q, 'ㅂ');
//         AddKeyCodeNCharPair(KeyCode.W, 'ㅈ');
//         AddKeyCodeNCharPair(KeyCode.E, 'ㄷ'); 
//         AddKeyCodeNCharPair(KeyCode.R, 'ㄱ');
//         AddKeyCodeNCharPair(KeyCode.T, 'ㅅ');
//         AddKeyCodeNCharPair(KeyCode.Y, 'ㅛ');
//         AddKeyCodeNCharPair(KeyCode.U, 'ㅕ');
//         AddKeyCodeNCharPair(KeyCode.I, 'ㅑ');
//         AddKeyCodeNCharPair(KeyCode.O, 'ㅐ');
//         AddKeyCodeNCharPair(KeyCode.P, 'ㅔ');
//         AddKeyCodeNCharPair(KeyCode.A, 'ㅁ');
//         AddKeyCodeNCharPair(KeyCode.S, 'ㄴ');
//         AddKeyCodeNCharPair(KeyCode.D, 'ㅇ');
//         AddKeyCodeNCharPair(KeyCode.F, 'ㄹ');
//         AddKeyCodeNCharPair(KeyCode.G, 'ㅎ');
//         AddKeyCodeNCharPair(KeyCode.H, 'ㅗ');
//         AddKeyCodeNCharPair(KeyCode.J, 'ㅓ');
//         AddKeyCodeNCharPair(KeyCode.K, 'ㅏ');
//         AddKeyCodeNCharPair(KeyCode.L, 'ㅣ');
//         AddKeyCodeNCharPair(KeyCode.Z, 'ㅋ');
//         AddKeyCodeNCharPair(KeyCode.X, 'ㅌ');
//         AddKeyCodeNCharPair(KeyCode.C, 'ㅊ');
//         AddKeyCodeNCharPair(KeyCode.V, 'ㅍ');
//         AddKeyCodeNCharPair(KeyCode.B, 'ㅠ');
//         AddKeyCodeNCharPair(KeyCode.N, 'ㅜ');
//         AddKeyCodeNCharPair(KeyCode.M, 'ㅡ');
//     }
// }
