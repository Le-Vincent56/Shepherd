using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum DMState
{
    Idle,
    Selected
}

public class AgentManager : MonoBehaviour
{
    #region FIELDS
    private Camera cam;

    [Header("Sheep Management")]
    [SerializeField] Sheep sheepPrefab;
    private List<Sheep> sheep = new List<Sheep>();
    public List<Sheep> Sheep { get { return sheep; } }
    [SerializeField] int sheepSpawnCount;
    [Space(20)]

    [Header("Dog Management")]
    [SerializeField] Dog dogPrefab;
    private List<Dog> dogs = new List<Dog>();
    private List<Dog> inactiveDogs = new List<Dog>();
    public List<Dog> Dogs { get { return dogs; } }
    [SerializeField] int dogSpawnCount;
    private DMState currentDMState;
    Dog tempDog = null;
    private bool prefabSelected = false;
    private bool finalizedPlacement = false;
    [Space(20)]

    [Header("Obstacle Management")]
    [SerializeField] Obstacle obstaclePrefab;
    [SerializeField] List<Obstacle> obstacles = new List<Obstacle>();
    public List<Obstacle> Obstacles { get { return obstacles; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;

        #region SHEEP SPAWNING
        for (int i = 0; i < sheepSpawnCount; i++)
        {
            Vector3 randPos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            sheep.Add(Instantiate(sheepPrefab, randPos, Quaternion.identity));
            sheep[i].Init(this);
            sheep[i].ChangeStateTo(SheepState.Wander);
        }
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        #region DOG MANAGEMENT
        switch (currentDMState)
        {
            case DMState.Idle:
                break;

            case DMState.Selected:
                if (!prefabSelected)
                {
                    tempDog = Instantiate(dogPrefab, new Vector3(cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0)).x,
                                                                    cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0)).y, 0),
                                                                    Quaternion.identity);
                    prefabSelected = true;
                }

                if (finalizedPlacement)
                {
                    // Instantiate the food and add it to the list
                    dogs.Add(Instantiate(dogPrefab, new Vector3(cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0)).x,
                                                                    cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0)).y, 0),
                                                                    Quaternion.identity));

                    // Set the food to the Placed state
                    dogs[dogs.Count - 1].ChangePlacingStateTo(DogPlacedState.Placed);

                    // Set finalizedPlacement to false
                    finalizedPlacement = false;

                    // Set prefabSelected to false
                    prefabSelected = false;

                    // Reset tempFood
                    Destroy(tempDog.gameObject);
                    tempDog = null;

                    // Return currentState to Idle
                    currentDMState = DMState.Idle;
                }
                break;
        }

        // Check for inactive foods and add them to the inactive list
        foreach (Dog dog in dogs)
        {
            if (dog.Duration <= 0)
            {
                inactiveDogs.Add(dog);
            }
        }

        // Remove inactive foods
        RemoveInactiveDogs();
        #endregion
    }

    public void RemoveInactiveDogs()
    {
        // Check the list of inactive dogs
        if (inactiveDogs.Count > 0)
        {
            // Compare the list of inactive dogs with the total dogs
            for (int i = 0; i < inactiveDogs.Count; i++)
            {
                for (int j = 0; j < dogs.Count; j++)
                {
                    // If any of the inactive dogs are equal to any of the active dogs
                    // in dogs, remove them from the list
                    if (inactiveDogs[i].Equals(dogs[j]))
                    {
                        Destroy(dogs[j].gameObject);
                        dogs.Remove(inactiveDogs[i]);
                    }
                }
            }

            // Clear the inactive dogs list
            inactiveDogs.Clear();
        }
    }

    public void OnClick()
    {
        switch (currentDMState)
        {
            case DMState.Idle:
                // Change currentState to Selected
                currentDMState = DMState.Selected;
                break;

            case DMState.Selected:
                // Change currentState to Idle
                currentDMState = DMState.Idle;
                break;
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        switch (currentDMState)
        {
            case DMState.Idle:
                break;

            case DMState.Selected:
                // Set finalized placement to true
                finalizedPlacement = true;
                break;
        }
    }
}
