using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class CarveGameAgent : Agent
{
    [SerializeField]
    private CarveGameScene gameScene;

    private const int ObservationSize = TypingActionMap.ActionCount + 3;
    private const float StepPenalty = -0.001f;
    private const float CorrectReward = 1f;
    private const float WrongPenalty = -0.2f;
    private const float CompleteReward = 5f;
    private const float TimeoutPenalty = -1f;

    public override void Initialize()
    {
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
        if (gameScene == null && !ResolveGameScene())
        {
            AddReward(TimeoutPenalty);
            EndEpisode();
            return;
        }

        AddReward(StepPenalty);

        int actionIndex = actions.DiscreteActions[0];
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
        ActionSegment<int> discreteActionsOut = actionsOut.DiscreteActions;

        if (TypingActionMap.TryGetPressedAction(out int actionIndex))
        {
            discreteActionsOut[0] = actionIndex;
        }
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
