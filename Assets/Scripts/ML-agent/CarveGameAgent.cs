using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using System.Text;
using System.Collections.Generic;

public class CarveGameAgent : Agent
{
    [SerializeField]
    private CarveGameScene gameScene;

    private int currentPosition = 0;
    private StringBuilder currentSequence;

    [SerializeField]
    private float wrongPenalty = -1.0f;
    [SerializeField]
    private float correctReward = 1.0f;

    [SerializeField]
    private AreaPooling area;
    [SerializeField]
    private BlockPooling block;

    private Dictionary<char, int> charToAction;
    private const string CONSONANTS = "ㄱㄴㄷㄹㅁㅂㅅㅇㅈㅊㅋㅌㅍㅎ";
    private const string VOWELS = "ㅏㅐㅑㅓㅔㅕㅗㅛㅜㅠㅡㅣ";
    private string ALL_VALID_CHARS;

    [SerializeField]
    private float minInputInterval = 1f; // 각 입력 사이의 최소 시간 간격
    [SerializeField]
    private float minGroupInterval = 1f; // 5개 그룹 사이의 최소 시간 간격
    
    private float lastInputTime;
    private float lastGroupTime;
    private int inputCountInGroup;

    private void Awake()
    {
        ALL_VALID_CHARS = CONSONANTS + VOWELS;

        if (gameScene == null)
        {
            gameScene = GetComponent<CarveGameScene>();
            if (gameScene == null)
            {
                Debug.LogError("CarveGameScene component is missing!");
            }
        }

        ValidateComponents();
    }

    private void ValidateComponents()
    {
        if (area == null) Debug.LogError("AreaPooling is not assigned!");
        if (block == null) Debug.LogError("BlockPooling is not assigned!");
    }

    public override void Initialize()
    {
        Debug.Log("Initializing CarveGameAgent...");
        InitializeActionMapping();
        InitializeGameState();
        ResetTimers();
    }

    private void ResetTimers()
    {
        lastInputTime = 0f;
        lastGroupTime = 0f;
        inputCountInGroup = 0;
    }

    private void InitializeActionMapping()
    {
        Debug.Log("Initializing Action Mapping...");
        charToAction = new Dictionary<char, int>();
        int idx = 0;

        // 자음
        foreach (char c in CONSONANTS)
        {
            charToAction[c] = idx++;
        }

        // 모음
        foreach (char c in VOWELS)
        {
            charToAction[c] = idx++;
        }

        Debug.Log($"Action mapping initialized with {charToAction.Count} characters (자음: {CONSONANTS.Length}, 모음: {VOWELS.Length})");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 현재 입력해야 할 문자의 one-hot encoding만 관찰
        char currentChar = currentSequence[currentPosition];
        foreach (char c in charToAction.Keys)
        {
            sensor.AddObservation(c == currentChar ? 1.0f : 0.0f);
        }
    }

    private void InitializeGameState()
    {
        Debug.Log("Initializing Game State...");

        currentPosition = 0;
        GenerateNewSequence();
        InitializeUI();

        Debug.Log("Game State Initialized Successfully");
    }

    private void GenerateNewSequence()
    {
        currentSequence = new StringBuilder();

        for (int i = 0; i < 50; i++)
        {
            currentSequence.Append(ALL_VALID_CHARS[Random.Range(0, ALL_VALID_CHARS.Length)]);
        }

        Debug.Log($"New sequence generated: {currentSequence.ToString().Substring(0, Mathf.Min(5, currentSequence.Length))}...");
    }

    private void InitializeUI()
    {
        try
        {
            if (area != null)
            {
                StringBuilder firstFiveChars = new StringBuilder(5);
                for (int i = 0; i < 5 && i < currentSequence.Length; i++)
                {
                    firstFiveChars.Append(currentSequence[i]);
                }
                area.Init(firstFiveChars.ToString());
            }

            if (block != null)
            {
                block.Init(currentSequence.Length);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error initializing UI: {e.Message}");
        }
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("Starting new episode...");
        InitializeGameState();
        ResetTimers();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float currentTime = Time.time;

        // 입력 간격 체크
        if (currentTime - lastInputTime < minInputInterval)
        {
            return;
        }

        // 그룹 간격 체크 (5개 입력마다)
        if (inputCountInGroup == 0 && currentTime - lastGroupTime < minGroupInterval)
        {
            return;
        }

        int actionIndex = actions.DiscreteActions[0];
        char inputChar = GetCharFromAction(actionIndex);
        char correctChar = currentSequence[currentPosition];

        if (inputChar == correctChar)
        {
            if (area != null)
            {
                area.ActiveImageEffect(currentPosition % 5);
            }
            if (block != null)
            {
                block.SetBlock(currentPosition);
            }

            AddReward(correctReward);
            currentPosition++;
            inputCountInGroup = (currentPosition % 5);
            
            // 입력 시간 갱신
            lastInputTime = currentTime;

            // 5개 입력 완료시 그룹 시간 갱신
            if (inputCountInGroup == 0)
            {
                lastGroupTime = currentTime;
            }

            if (currentPosition % 5 == 0 && currentPosition < currentSequence.Length)
            {
                StringBuilder nextFiveChars = new StringBuilder(5);
                for (int i = 0; i < 5 && (currentPosition + i) < currentSequence.Length; i++)
                {
                    nextFiveChars.Append(currentSequence[currentPosition + i]);
                }
                if (area != null)
                {
                    area.ChangeArea(nextFiveChars.ToString());
                }
            }

            if (currentPosition >= currentSequence.Length)
            {
                AddReward(5.0f);
                EndEpisode();
            }
        }
        else
        {
            AddReward(wrongPenalty);
            EndEpisode();
        }
    }

    private char GetCharFromAction(int actionIndex)
    {
        foreach (var kvp in charToAction)
        {
            if (kvp.Value == actionIndex)
                return kvp.Key;
        }
        return '\0';
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;

        if (Input.anyKeyDown)
        {
            char currentInput = '\0';

            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    if (gameScene._keyCodeNCharPair.TryGetValue(key, out char value))
                    {
                        currentInput = value;
                        break;
                    }
                }
            }

            if (currentInput != '\0' && charToAction.TryGetValue(currentInput, out int actionIndex))
            {
                discreteActionsOut[0] = actionIndex;
            }
        }
    }
}