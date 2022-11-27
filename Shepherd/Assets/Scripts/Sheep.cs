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
    [Header("Sheep State")]
    [SerializeField] SheepState currentState;
    [Space(20)]

    // Wander Variables
    [Header("Wander Variables")]
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
                totalForce += StayCohesive(agentManager.Sheep, 2f);
                totalForce += Align(agentManager.Sheep);
                totalForce += AvoidObstacles();
                totalForce += StayInBounds(PhysicsObject.WorldSize);
                totalForce += Separation(agentManager.Sheep);
                totalForce += Separation(agentManager.Dogs);
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

    private void OnDrawGizmosSelected()
    {
        #region OBSTACLE AVOIDANCE
        Gizmos.color = Color.red;
        foreach(Vector3 pos in tempObsPos)
        {
            Gizmos.DrawLine(PhysicsObject.Position, pos);
        }

        // Draw safe zone box
        Gizmos.color = Color.green;

        // Set the center of the agent's future position
        Vector3 futurePos = GetFuturePosition(avoidMaxRange);
        float avoidMaxDist = Vector3.Magnitude(futurePos - PhysicsObject.Position);
        avoidMaxDist += avoidRadius;

        // Transition to LocalSpace
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(new Vector3(0, avoidMaxDist / 2f, 0), 
                            new Vector3(avoidRadius * 2f, avoidMaxDist, avoidRadius));

        // Return back to WorldSpace
        Gizmos.matrix = Matrix4x4.identity;
        #endregion
    }
}
