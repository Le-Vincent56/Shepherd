using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SheepState
{
    Wander,
    Flee,
    Seek
}

public class Sheep : Agent
{
    #region FIELDS
    // Sheep State
    [SerializeField] SheepState currentState;

    // Wander Variables
    [SerializeField] float wanderAngle = 15f;
    [SerializeField] float maxWanderAngle = 45f;
    [SerializeField] float maxWanderChangePerSecond = 3f;
    #endregion

    public override void CalcSteeringForces()
    {
        switch(currentState)
        {
            case SheepState.Wander:
                totalForce += Wander(maxWanderChangePerSecond, wanderAngle, maxWanderAngle);
                totalForce += StayCohesive(agentManager.Sheep, 2);
                totalForce += Align(agentManager.Sheep);
                totalForce += StayInBounds(PhysicsObject.WorldSize);
                Separation(agentManager.Sheep);
                Separation(agentManager.Dogs);
                break;

            case SheepState.Flee:
                break;

            case SheepState.Seek:
                break;
        }
    }

    public void ChangeStateTo(SheepState newState)
    {
        switch (newState)
        {
            case SheepState.Wander:
                break;

            case SheepState.Flee:
                break;

            case SheepState.Seek:
                break;

        }

        currentState = newState;
    }
}
