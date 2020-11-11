using UnityEngine;

namespace GWLPXL.ARPGCore.Movement.com
{

    public interface IMover
    {
        void SetUpMover();
        GameObject GetGameObject();
        void SetVelocity(Vector3 newVel);
        void ResetState();
        void SetDesiredDestination(Vector3 newDestination, float stoppingDistance);
        void StopAgentMovement(bool isStopped);
        void SetNewSpeed(float newTopSpeed, float newAcceleration);
        void ResetSpeed();

    }
}