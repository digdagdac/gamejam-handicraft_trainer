using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace LegacyTraining
{
    public class LegacyCarveGameAgent : Agent
    {
        [SerializeField]
        private CarveGameScene gameScene;

        public override void Initialize()
        {
            if (gameScene == null)
            {
                gameScene = GetComponent<CarveGameScene>();
            }
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(gameScene == null ? 0f : gameScene.GetTrainingProgress01());
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            AddReward(-0.001f);
        }
    }
}
