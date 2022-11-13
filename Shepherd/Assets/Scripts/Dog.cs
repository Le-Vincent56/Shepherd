using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DogState
{
    Seek,
    Flee
}

public class Dog : Agent
{
    private DogState currentState;

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
