using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Agent : MonoBehaviour
{
    #region FIELDS
    private PhysicsObject physObj;
    protected AgentManager agentManager;
    public PhysicsObject PhysicsObject { get { return physObj; } }

    [SerializeField] protected Vector3 totalForce = Vector3.zero;

    [Header("Max Parameters")]
    [SerializeField] float maxSpeed = 5f;
    public float MaxSpeed { get { return maxSpeed; } }
    [SerializeField] float maxForce = 5f;
    public float MaxForce { get { return maxForce; } }
    [Space(20)]

    [Header("Separation")]
    [SerializeField] float personalSpace = 1f;
    [Space(20)]

    [Header("Obstacle Avoidance")]
    [SerializeField] protected List<Vector3> tempObsPos = new List<Vector3>();
    [SerializeField] protected float avoidMaxRange = 1.5f;
    [SerializeField] protected float avoidRadius = 0.05f;
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
        totalForce = Vector3.ClampMagnitude(totalForce, maxForce);

        PhysicsObject.ApplyForce(totalForce);

        totalForce = Vector3.zero;
    }

    public abstract void CalcSteeringForces();

    /// <summary>
    /// Seek a target Agent
    /// </summary>
    /// <param name="target">The Agent to seek</param>
    /// <returns>A seeking force</returns>
    protected Vector3 Seek(Vector3 target, float weight = 1f)
    {
        // Calculate desired velocity and scale by max speed
        Vector3 desiredVelocity = target - PhysicsObject.Position;
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        // Calculate and return steering force
        Vector3 seekingForce = desiredVelocity - PhysicsObject.Velocity;
        return seekingForce * weight;
    }

    public void Init(AgentManager agentManagerRef)
    {
        agentManager = agentManagerRef;
    }

    /// <summary>
    /// Flee from a target Agent
    /// </summary>
    /// <param name="target">The Agent to flee from</param>
    /// <returns>A fleeing force</returns>
    protected Vector3 Flee(Vector3 target, float weight = 1f)
    {
        // Calculate desired velocity and scale by max speed
        Vector3 desiredVelocity = PhysicsObject.Position - target;
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        // Calculate and return steering force
        Vector3 fleeingForce = desiredVelocity - PhysicsObject.Velocity;
        return fleeingForce * weight;
    }

    /// <summary>
    /// Wander around the screen by using random angles
    /// </summary>
    /// <param name="maxWanderChangePerSecond">How many times to change the wander per second</param>
    /// <param name="wanderAngle">The angle for wandering</param>
    /// <param name="maxWanderAngle">The maximum wander angle</param>
    /// <returns>A wander force</returns>
    protected Vector3 Wander(float maxWanderChangePerSecond, float wanderAngle, float maxWanderAngle)
    {
        float maxWanderChange = maxWanderChangePerSecond * Time.deltaTime;
        float randomAngle = Random.Range(-maxWanderChange, maxWanderChange);
        wanderAngle += randomAngle;

        wanderAngle = Mathf.Clamp(wanderAngle, -maxWanderAngle, maxWanderAngle);

        // Get a position that is defined by the wander angle
        Vector3 wanderTarget = Quaternion.Euler(0, 0, wanderAngle) * PhysicsObject.Direction.normalized + PhysicsObject.Position;

        return Seek(wanderTarget);
    }

    /// <summary>
    /// Separate from any neighbor agents that are too close
    /// </summary>
    /// <param name="neighbors">The list of agents</param>
    /// <returns>A separation force that steers the agent proportionally away from agents depending on how close they are</returns>
    protected Vector3 Separation<T>(List<T> neighbors) where T : Agent
    {
        Vector3 separateForce = Vector3.zero;
        float sqrPersonalSpace = Mathf.Pow(personalSpace, 2);

        // Loop hrough all the other agents
        foreach(T other in neighbors)
        {
            // Find the square distance between the two agents
            float sqrDist = Vector3.SqrMagnitude(other.PhysicsObject.Position - PhysicsObject.Position);
            if(sqrDist < float.Epsilon)
            {
                continue;
            }

            // If there is an agent within separation range, begin to separate
            if (sqrDist < sqrPersonalSpace)
            {
                float weight = sqrPersonalSpace / (sqrDist + 0.1f);
                separateForce += Flee(other.PhysicsObject.Position, weight);
            }
        }

        // Return the force
        return separateForce;
    }

    /// <summary>
    /// Stay cohesive within a flock of agents by tracking the centroid
    /// </summary>
    /// <param name="flock">The list of agents</param>
    /// <returns>A cohesion force that seeks the centroid</returns>
    protected Vector3 StayCohesive<T>(List<T> flock, float weight = 1f) where T : Agent
    {
        // Calculate centroid by taking average position
        Vector3 centroid = Vector3.zero;
        foreach (T agent in flock)
        {
            centroid += agent.PhysicsObject.Position;
        }
        centroid /= flock.Count;

        // Seek the centroid
        return Seek(centroid, weight);
    }

    /// <summary>
    /// Returns a steering force that aligns the agent with the other agents in the flock
    /// </summary>
    /// <param name="flock">The list of agents</param>
    /// <returns>An aligning force that keeps the agent aligned with the other agents in the flock</returns>
    protected Vector3 Align<T>(List<T> flock, float weight = 1f) where T : Agent
    {
        // Find the sum of all the forward vectors of
        // agents in the flock
        Vector3 desiredVelocity = Vector3.zero;
        foreach(T agent in flock)
        {
            desiredVelocity += agent.PhysicsObject.Direction;
        }

        // Compute desired velocity by normalizing and multiplying
        // by max speed
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        // Return the total aligning force
        Vector3 alignForce = desiredVelocity - PhysicsObject.Velocity;
        return alignForce * weight;
    }

    /// <summary>
    /// Get the future position of the Agent in a certain amount of time
    /// </summary>
    /// <param name="time">The amount of time in the future to predict a position</param>
    /// <returns>The agent's future position</returns>
    protected Vector3 GetFuturePosition(float time)
    {
        // Add the future velocity multiplied by time to the Agent's current position
        return physObj.Position + (physObj.Velocity * time);
    }

    /// <summary>
    /// Calculate if the object will be out of bounds in the near future, and, if so, steer it back into bounds
    /// </summary>
    /// <returns>A steering force that returns the Agent back to bounds</returns>
    protected Vector3 StayInBounds(Vector2 bounds, float time = 1f)
    {
        // Get position
        Vector3 position = GetFuturePosition(time);

        // Check that position
        if(position.x > bounds.x || position.x < -bounds.x 
            || position.y > bounds.y || position.y < -bounds.y)
        {
            // If out of bounds, seek the center
            return Seek(Vector3.zero);
        }

        // If not out of bounds, don't change force
        return Vector3.zero; 
    }

    protected Vector3 AvoidObstacles(float weight = 1f)
    {
        // Set an avoidance force
        Vector3 avoidForce = Vector3.zero;

        // Set a vector from the agent to the obstacle
        Vector3 vectorToObs = Vector3.zero;

        // Store dot products
        float dotForward;
        float dotRight;

        // Set the center of the agent's future position
        Vector3 futurePos = GetFuturePosition(avoidMaxRange);
        float avoidMaxSqrDist = Vector3.SqrMagnitude(futurePos - PhysicsObject.Position);
        avoidMaxSqrDist += avoidRadius;

        // Clear the list for the new frame
        tempObsPos.Clear();

        foreach (Obstacle obstacle in agentManager.Obstacles)
        {
            // Calculate the vector from the agent to the obstacle
            vectorToObs = obstacle.Position - PhysicsObject.Position;

            // Calculate the forward dot product
            dotForward = Vector3.Dot(PhysicsObject.Velocity.normalized, vectorToObs);

            Debug.Log("Forward Dot: " + dotForward);
            Debug.Log("Forward Dot Squared: " + Mathf.Pow(dotForward, 2));
            Debug.Log("Max Sqr Dist: " + avoidMaxSqrDist);

            // Check if the obstacle is within the forward-facing box
            if (dotForward > 0 && dotForward < avoidMaxSqrDist)
            {
                // Calculate the right dot product
                dotRight = Vector3.Dot(transform.right, vectorToObs);

                // Check if the obstacle is within the box bounds
                if (Mathf.Abs(dotRight) < avoidRadius + obstacle.radius)
                {
                    tempObsPos.Add(obstacle.Position);

                    if (dotRight > 0)
                    {
                        // Turn left
                        avoidForce += -transform.right * maxSpeed * (1f / dotForward);
                    }
                    else
                    {
                        // Turn right
                        avoidForce += transform.right * maxSpeed * (1f / dotForward);
                    }
                }
            }
        }

        return avoidForce * weight;
    }
}
