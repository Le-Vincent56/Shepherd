using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum DogState
{
    Seek,
    Flee,
    Freeze
}

public enum DogPlacedState
{
    Placing,
    Placed
}

public class Dog : Agent
{
    private Camera cam;

    [Header("Dog States")]
    [SerializeField] private DogState currentState;
    [SerializeField] private DogPlacedState currentPlacingState;
    [Space(20)]

    private Vector3 position;
    public Vector3 Position { get { return position; } }

    [SerializeField] float duration = 8f;
    public float Duration { get { return duration; } }

    [Header("Range")]
    [SerializeField] float radius = 3f;
    [SerializeField] List<Marker> nearbyMarkers = new List<Marker>();
    [SerializeField] List<Marker> inactiveMarkers = new List<Marker>();
    [SerializeField] List<Obstacle> nearbyObstacles = new List<Obstacle>();
    [SerializeField] List<Obstacle> inactiveObstacles = new List<Obstacle>();

    void Start()
    {
        cam = Camera.main;
        PhysicsObject = GetComponent<PhysicsObject>();
        agentManager = GameObject.Find("Agent Manager").GetComponent<AgentManager>();
        markerManager = GameObject.Find("Marker Manager").GetComponent<MarkerManager>();
        obstacleManager = GameObject.Find("Obstacles").GetComponent<ObstacleManager>();
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.back, Vector3.up) * Quaternion.Euler(0f, -180f, 0f);

        switch (currentPlacingState)
        {
            case DogPlacedState.Placing:
                // Show where the Dog will be placed depending on cursor movement
                position = new Vector3(cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0)).x,
                                                                    cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0)).y, 0);
                transform.position = position;
                break;

            case DogPlacedState.Placed:
                // Set position and start countdown
                position = transform.position;
                duration -= Time.deltaTime;

                // Add anything in range
                AddToRange();

                // Calculate and apply steering forces
                CalcSteeringForces();
                totalForce = Vector3.ClampMagnitude(totalForce, maxForce);
                PhysicsObject.ApplyForce(totalForce);
                totalForce = Vector3.zero;

                // Remove anything thats out of range
                RemoveFromRange();
                RemoveInactiveMarkers();
                RemoveInactiveObstacles();
                break;
        }
    }

    public override void CalcSteeringForces()
    {
        switch(currentState)
        {
            case DogState.Seek:
                PhysicsObject.ApplyPhysics = true;
                totalForce += Seek(nearbyMarkers);
                totalForce += StayInBounds(PhysicsObject.WorldSize);
                totalForce += Separation(agentManager.Sheep);

                #region CHANGE STATES CASES
                // Obstacles will cause the Dog to flee
                if (nearbyObstacles.Count > 0)
                {
                    ChangeStateTo(DogState.Flee);
                }

                // If there are no nearby obstacles or nearby markers, freeze
                if (nearbyObstacles.Count <= 0 && nearbyMarkers.Count <= 0)
                {
                    ChangeStateTo(DogState.Freeze);
                }
                #endregion
                break;

            case DogState.Flee:
                PhysicsObject.ApplyPhysics = true;
                totalForce += Flee(nearbyObstacles);
                totalForce += StayInBounds(PhysicsObject.WorldSize);
                totalForce += Separation(agentManager.Sheep);

                #region CHANGE STATE CASES
                // If there are nearby Markers and no nearbyObstacles, seek the Markers
                if (nearbyMarkers.Count > 0 && nearbyObstacles.Count <= 0)
                {
                    ChangeStateTo(DogState.Seek);
                }

                // If there are no nearby obstacles or nearby markers, freeze
                if (nearbyObstacles.Count <= 0 && nearbyMarkers.Count <= 0)
                {
                    ChangeStateTo(DogState.Freeze);
                }
                #endregion
                break;

            case DogState.Freeze:
                PhysicsObject.ApplyPhysics = false;

                #region CHANGE STATE CASES
                // Obstacles will cause the Dog to flee
                if (nearbyObstacles.Count > 0)
                {
                    ChangeStateTo(DogState.Flee);
                }

                // If there are nearby Markers and no nearbyObstacles, seek the Markers
                if (nearbyMarkers.Count > 0 && nearbyObstacles.Count <= 0)
                {
                    ChangeStateTo(DogState.Seek);
                }
                #endregion
                break;
        }
    }

    public void AddToRange()
    {
        // Check Markers
        foreach (Marker marker in markerManager.Markers)
        {
            bool addMarker = true;

            // Check nearbyMarkers to see if it is the same Marker object
            if (nearbyMarkers.Count > 0)
            {
                foreach (Marker nearby in nearbyMarkers)
                {
                    // If the Marker object in range is the same as one already detected,
                    // do not add the Marker to the nearbyMarker list
                    if (marker == nearby)
                    {
                        addMarker = false;
                    }
                }
            }

            // If the Marker is in range and not already in the list, add it to the nearbyMarkers list
            if (Vector3.Distance(PhysicsObject.Position, marker.Position) <= radius && addMarker)
            {
                nearbyMarkers.Add(marker);
            }
        }

        // Check Obstacles
        foreach (Obstacle obstacle in obstacleManager.Obstacles)
        {
            bool addObstacle = true;

            // Check nearbyFood to see if it is the same Food object
            if (nearbyObstacles.Count > 0)
            {
                foreach (Obstacle nearby in nearbyObstacles)
                {
                    // If the Food object in range is the same as one already detected,
                    // do not add the Food to the nearbyFood list
                    if (obstacle == nearby)
                    {
                        addObstacle = false;
                    }
                }
            }

            // If the Food is in range and not already in the list, add it to the nearbyFood list
            if (Vector3.Distance(PhysicsObject.Position, obstacle.Position) <= radius && addObstacle)
            {
                nearbyObstacles.Add(obstacle);
            }
        }
    }

    public void RemoveFromRange()
    {
        // Check each nearby marker
        foreach (Marker marker in nearbyMarkers)
        {
            // If they are no longer in range or their duration has run out, add them to inactiveMarkers
            if (Vector3.Distance(PhysicsObject.Position, marker.Position) > radius || marker.Duration <= 0)
            {
                inactiveMarkers.Add(marker);
            }
        }

        // Check each nearby obstacle
        foreach (Obstacle obstacle in nearbyObstacles)
        {
            // If they are no longer in range, add them to inativeObstacles
            if (Vector3.Distance(PhysicsObject.Position, obstacle.Position) > radius)
            {
                inactiveObstacles.Add(obstacle);
            }
        }
    }

    public void RemoveInactiveMarkers()
    {
        // Check the list of inactive Markers
        if (inactiveMarkers.Count > 0)
        {
            // Compare the list of inactive Markers with the nearby Markers
            for (int i = 0; i < inactiveMarkers.Count; i++)
            {
                for (int j = 0; j < nearbyMarkers.Count; j++)
                {
                    // If any of the inactive Markers are equal to any of the active Markers
                    // in nearbyMarkers, remove them from the list
                    if (inactiveMarkers[i].Equals(nearbyMarkers[j]))
                    {
                        nearbyMarkers.Remove(inactiveMarkers[i]);
                    }
                }
            }

            // Clear the inactiveMarkers list
            inactiveMarkers.Clear();
        }
    }

    public void RemoveInactiveObstacles()
    {
        // Check the list of inactive Obstacles
        if (inactiveObstacles.Count > 0)
        {
            // Compare the list of inactive Obstacles with the total Obstacles
            for (int i = 0; i < inactiveObstacles.Count; i++)
            {
                for (int j = 0; j < nearbyObstacles.Count; j++)
                {
                    // If any of the inactive Obstacles are equal to any of the placed Obstacles
                    // in nearbyObstacles, remove them from the list
                    if (inactiveObstacles[i].Equals(nearbyObstacles[j]))
                    {
                        nearbyObstacles.Remove(inactiveObstacles[i]);
                    }
                }
            }

            // Clear the inactiveObstacles list
            inactiveObstacles.Clear();
        }
    }

    public void ChangePlacingStateTo(DogPlacedState newState)
    {
        currentPlacingState = newState;
    }

    public void ChangeStateTo(DogState newState)
    {
        switch (newState)
        {
            case DogState.Seek:
                break;

            case DogState.Flee:
                break;
        }

        currentState = newState;
    }
}
