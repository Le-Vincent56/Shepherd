using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum DogState
{
    Seek,
    Flee
}

public enum DogPlacedState
{
    Placing,
    Placed
}

public class Dog : Agent
{
    private DogState currentState;
    private DogPlacedState currentPlacingState;
    private Camera cam;

    private Vector3 position;
    public Vector3 Position { get { return position; } }

    [SerializeField] float duration = 8f;
    public float Duration { get { return duration; } }

    void Start()
    {
        cam = Camera.main;
        PhysicsObject = GetComponent<PhysicsObject>();
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
                // Start countdown
                duration -= Time.deltaTime;

                // Calculate steering forces
                CalcSteeringForces();
                totalForce = Vector3.ClampMagnitude(totalForce, maxForce);

                PhysicsObject.ApplyForce(totalForce);

                totalForce = Vector3.zero;
                break;
        }
    }

    public override void CalcSteeringForces()
    {
        switch(currentState)
        {
            case DogState.Seek:
                
                break;

            case DogState.Flee:
                break;
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
