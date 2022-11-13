using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wanderer : Agent
{
    #region FIELDS
    // Wander Variables
    [SerializeField] Vector3 wanderForce;
    [SerializeField] float wanderAngle = 0f;
    [SerializeField] float maxWanderAngle = 45f;
    [SerializeField] float maxWanderChangePerSecond = 3f;

    // Bounds Variables
    [SerializeField] Vector3 boundsForce;

    private Vector2 worldSize;
    #endregion

    private void Awake()
    {
        worldSize.y = Camera.main.orthographicSize;
        worldSize.x = Camera.main.aspect * worldSize.y;
    }

    public override void CalcSteeringForces()
    {
        wanderForce = Wander(maxWanderChangePerSecond, wanderAngle, maxWanderAngle);
        totalForce += wanderForce;

        boundsForce = StayInBounds(PhysicsObject.WorldSize);
        totalForce += boundsForce;

        PhysicsObject.ApplyForce(totalForce);
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(GetFuturePosition(wanderTime), wanderRadius);

        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(wanderPos, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, wanderForce * MaxSpeed);

        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, boundsForce * MaxSpeed);
    }
}
