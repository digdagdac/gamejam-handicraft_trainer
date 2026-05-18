using UnityEngine;

namespace LegacyTraining
{
    public class CarveGameAgentConfig : MonoBehaviour
    {
        [SerializeField]
        private LegacyCarveGameAgent agent;

        [Header("Training Configuration")]
        public float maxEpisodeSteps = 100;
        public int trainingBatchSize = 64;
        public float learningRate = 3e-4f;

        void Start()
        {
            if (agent == null) return;

            var behaviorParams = agent.GetComponent<Unity.MLAgents.Policies.BehaviorParameters>();
            behaviorParams.BehaviorType = Unity.MLAgents.Policies.BehaviorType.Default;
        }
    }
}
