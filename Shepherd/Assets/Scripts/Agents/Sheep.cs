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

    [Header("Goal Timers")]
    [SerializeField] float freezeTimer = 2f;
    public float FreezeTimer { get { return freezeTimer; } }
    [SerializeField] bool notInGoal = true;
    public bool NotInGoal { get { return notInGoal; } set { notInGoal = value; } }
    [Space(20)]

    [Header("Range")]
    [SerializeField] float radius = 3f;
    [SerializeField] List<Dog> nearbyDogs = new List<Dog>();
    [SerializeField] List<Dog> inactiveDogs = new List<Dog>();
    [SerializeField] List<Food> nearbyFood = new List<Food>();
    [SerializeField] List<Food> inactiveFood = new List<Food>();

    // Wander Variables
    [Header("Wander Variables")]
    [SerializeField] float wanderAngle = 15f;
    [SerializeField] float maxWanderAngle = 45f;
    [SerializeField] float maxWanderChangePerSecond = 3f;
    #endregion

    void Update()
    {
        if(!notInGoal)
        {
            freezeTimer -= Time.deltaTime;
        }

        if(freezeTimer >= 0 && notInGoal)
        {
            // Check if anything is in range
            AddToRange();

            // Calculate and apply steering forces
            CalcSteeringForces();
            totalForce = Vector3.ClampMagnitude(totalForce, maxForce);
            PhysicsObject.ApplyForce(totalForce);
            totalForce = Vector3.zero;

            // Remove anything thats out of range
            RemoveFromRange();
            RemoveInactiveDogs();
            RemoveInactiveFood();
        }
    }

    public override void CalcSteeringForces()
    {
        switch(currentState)
        {
            case SheepState.Wander:
                // Calculate steering forces
                totalForce += Wander(maxWanderChangePerSecond, wanderAngle, maxWanderAngle);
                totalForce += StayCohesive(agentManager.Sheep, 2f);
                totalForce += Align(agentManager.Sheep);
                totalForce += AvoidObstacles();
                totalForce += StayInBounds(PhysicsObject.WorldSize);
                totalForce += Separation(agentManager.Sheep);

                #region CHANGE STATE CASES
                // Dogs have 1st priority for changing states
                if (nearbyDogs.Count > 0)
                {
                    ChangeStateTo(SheepState.Flee);
                }

                // Food has 2nd priority for changing states
                if(nearbyFood.Count > 0)
                {
                    ChangeStateTo(SheepState.Seek);
                }
                #endregion
                break;

            case SheepState.Flee:
                totalForce += Flee(agentManager.Dogs);
                totalForce += StayCohesive(agentManager.Sheep, 2f);
                totalForce += Align(agentManager.Sheep);
                totalForce += AvoidObstacles();
                totalForce += StayInBounds(PhysicsObject.WorldSize);
                totalForce += Separation(agentManager.Sheep);

                #region CHANGE STATE CASES
                // If there are no more nearby dogs, go back to wandering
                if (nearbyDogs.Count == 0)
                {
                    ChangeStateTo(SheepState.Wander);
                }

                // If there are no more nearby dogs and there is nearby food, seek the food
                if(nearbyDogs.Count == 0 && nearbyFood.Count > 0)
                {
                    ChangeStateTo(SheepState.Seek);
                }
                #endregion
                break;

            case SheepState.Seek:
                totalForce += Seek(nearbyFood);
                totalForce += StayCohesive(agentManager.Sheep, 2f);
                totalForce += Align(agentManager.Sheep);
                totalForce += AvoidObstacles();
                totalForce += StayInBounds(PhysicsObject.WorldSize);
                totalForce += Separation(agentManager.Sheep);

                #region CHANGE STATE CASES
                // If there are nearby dogs, flee
                if(nearbyDogs.Count > 0)
                {
                    ChangeStateTo(SheepState.Flee);
                }

                if(nearbyDogs.Count == 0 && nearbyFood.Count == 0)
                {
                    ChangeStateTo(SheepState.Wander);
                }
                #endregion
                break;
        }
    }

    public void AddToRange()
    {
        // Check Dogs
        foreach (Dog dog in agentManager.Dogs)
        {
            bool addDog = true;

            // Check nearbyDogs to see if it is the same Dog object
            if(nearbyDogs.Count > 0)
            {
                foreach(Dog nearby in nearbyDogs)
                {
                    // If the Dog object in range is the same as one already detected,
                    // do not add the Dog to the nearbyDog list
                    if(dog == nearby)
                    {
                        addDog = false;
                    }
                }
            }

            // If the Dog is in range and not already in the list, add it to the nearbyDogs list
            if (Vector3.Distance(PhysicsObject.Position, dog.Position) <= radius && addDog)
            {
                nearbyDogs.Add(dog);
            }
        }

        // Check Food
        foreach(Food food in foodManager.Food)
        {
            bool addFood = true;

            // Check nearbyFood to see if it is the same Food object
            if(nearbyFood.Count > 0)
            {
                foreach (Food nearby in nearbyFood)
                {
                    // If the Food object in range is the same as one already detected,
                    // do not add the Food to the nearbyFood list
                    if (food == nearby)
                    {
                        addFood = false;
                    }
                }
            }

            // If the Food is in range and not already in the list, add it to the nearbyFood list
            if(Vector3.Distance(PhysicsObject.Position, food.Position) <= radius && addFood)
            {
                nearbyFood.Add(food);
            }
        }
    }

    public void RemoveFromRange()
    {
        foreach (Dog dog in nearbyDogs)
        {
            if (Vector3.Distance(PhysicsObject.Position, dog.Position) > radius|| dog.Duration <= 0)
            {
                inactiveDogs.Add(dog);
            }
        }

        foreach (Food food in nearbyFood)
        {
            if (Vector3.Distance(PhysicsObject.Position, food.Position) > radius || food.Duration <= 0)
            {
                inactiveFood.Add(food);
            }
        }
    }

    public void RemoveInactiveDogs()
    {
        // Check the list of inactive Dogs
        if (inactiveDogs.Count > 0)
        {
            // Compare the list of inactive Dogs with the total Dogs
            for (int i = 0; i < inactiveDogs.Count; i++)
            {
                for (int j = 0; j < nearbyDogs.Count; j++)
                {
                    // If any of the inactive Dogs are equal to any of the active Dogs
                    // in nearbyDogs, remove them from the list
                    if (inactiveDogs[i].Equals(nearbyDogs[j]))
                    {
                        nearbyDogs.Remove(inactiveDogs[i]);
                    }
                }
            }

            // Clear the inactiveDogs list
            inactiveDogs.Clear();
        }
    }

    public void RemoveInactiveFood()
    {
        // Check the list of inactive Food
        if (inactiveFood.Count > 0)
        {
            // Compare the list of inactive Food with the total Food
            for (int i = 0; i < inactiveFood.Count; i++)
            {
                for (int j = 0; j < nearbyFood.Count; j++)
                {
                    // If any of the inactive Food are equal to any of the placed Food
                    // in nearbyFood, remove them from the list
                    if (inactiveFood[i].Equals(nearbyFood[j]))
                    {
                        nearbyFood.Remove(inactiveFood[i]);
                    }
                }
            }

            // Clear the inactiveFoods list
            inactiveFood.Clear();
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
        #region RANGE GIZMOS
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.yellow;
        foreach(Dog dog in nearbyDogs)
        {
            Gizmos.DrawLine(PhysicsObject.Position, dog.Position);
        }

        Gizmos.color = Color.blue;
        foreach (Food food in nearbyFood)
        {
            Gizmos.DrawLine(PhysicsObject.Position, food.Position);
        }
        #endregion

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
