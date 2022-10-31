using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Agent : MonoBehaviour
{
    #region FIELDS
    [SerializeField] PhysicsObject physObj;

    public PhysicsObject PhysicsObject { get { return physObj; } }

    [SerializeField] float maxSpeed;
    [SerializeField] float maxForce;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        physObj = GetComponent<PhysicsObject>();
    }

    // Update is called once per frame
    void Update()
    {
        CalcSteeringForces();
    }

    public abstract void CalcSteeringForces();

    public Vector3 Seek(Agent target)
    {
        // Calculate desired velocity and scale by max speed
        Vector3 desiredVelocity = target.PhysicsObject.Position - PhysicsObject.Position;
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        // Calculate and return steering force
        Vector3 seekingForce = desiredVelocity - PhysicsObject.Velocity;
        return seekingForce;
    }

    public Vector3 Flee(Agent target)
    {
        // Calculate desired velocity and scale by max speed
        Vector3 desiredVelocity = PhysicsObject.Position - target.PhysicsObject.Position;
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        // Calculate and return steering force
        Vector3 fleeingForce = desiredVelocity - PhysicsObject.Velocity;
        return fleeingForce;
    }
}
