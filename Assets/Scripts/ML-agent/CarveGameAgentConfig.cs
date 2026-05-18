using UnityEngine;

public class CarveGameAgentConfig : MonoBehaviour
{
    [SerializeField]
    private CarveGameAgent agent;

    [Header("Training Configuration")]
    public float maxEpisodeSteps = 100;
    public int trainingBatchSize = 64;
    public float learningRate = 3e-4f;

    void Start()
    {
        // Training configuration can be set here
        var behaviorParams = agent.GetComponent<Unity.MLAgents.Policies.BehaviorParameters>();
        behaviorParams.BehaviorType = Unity.MLAgents.Policies.BehaviorType.Default;
    }
}