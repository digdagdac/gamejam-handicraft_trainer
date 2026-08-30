using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CarveGameAgent : Agent
{
    [SerializeField]
    private CarveGameScene gameScene;

    private BehaviorParameters behaviorParameters;

    private const int ObservationSize = TypingActionMap.ActionCount + 3;
    private const float StepPenalty = -0.001f;
    private const float CorrectReward = 1f;
    private const float WrongPenalty = -0.2f;
    private const float CompleteReward = 5f;
    private const float TimeoutPenalty = -1f;

    public override void Initialize()
    {
        behaviorParameters = GetComponent<BehaviorParameters>();
        ResolveGameScene();
    }

    public override void OnEpisodeBegin()
    {
        if (!ResolveGameScene())
        {
            AddReward(TimeoutPenalty);
            EndEpisode();
            return;
        }

        gameScene.Clear();
        gameScene.InitializeGameState();
        gameScene.Init();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (gameScene == null)
        {
            AddEmptyObservations(sensor);
            return;
        }

        TypingActionMap.AddOneHotObservation(sensor, gameScene.GetCurrentChar());
        sensor.AddObservation(gameScene.GetTrainingProgress01());
        sensor.AddObservation(Mathf.Clamp01(gameScene.GetCombo() / 5f));
        sensor.AddObservation(gameScene.GetRemainingTimeRatio());
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Manual play is handled once by CarveGameScene.CheckInput. In a
        // model-less Default/Heuristic policy, ignore the cleared action buffer
        // so its valid default value (0) cannot become repeated keyboard input.
        if (behaviorParameters != null && behaviorParameters.IsInHeuristicMode())
        {
            return;
        }

        if (gameScene == null && !ResolveGameScene())
        {
            AddReward(TimeoutPenalty);
            EndEpisode();
            return;
        }

        int actionIndex = actions.DiscreteActions[0];
        AddReward(StepPenalty);
        TypingActionResult result = gameScene.ApplyAgentAction(actionIndex);

        if (!result.IsValidAction)
        {
            AddReward(WrongPenalty);
            return;
        }

        AddReward(result.IsCorrect ? CorrectReward : WrongPenalty);

        if (result.IsComplete)
        {
            AddReward(CompleteReward);
            EndEpisode();
            return;
        }

        if (gameScene.IsTimeOver() || gameScene.GetRemainingTime() <= 0f)
        {
            AddReward(TimeoutPenalty);
            gameScene.FinishGame(false);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Keep the action inside the configured 0..25 branch. It is ignored by
        // OnActionReceived while the effective policy is heuristic.
        ActionSegment<int> discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0;
    }

    private bool ResolveGameScene()
    {
        if (gameScene != null) return true;

        gameScene = GetComponent<CarveGameScene>();
        if (gameScene == null)
        {
            gameScene = FindObjectOfType<CarveGameScene>();
        }

        return gameScene != null;
    }

    private static void AddEmptyObservations(VectorSensor sensor)
    {
        for (int i = 0; i < ObservationSize; i++)
        {
            sensor.AddObservation(0f);
        }
    }
}
