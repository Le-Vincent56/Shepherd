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
    private Goal goal;

    [Header("Sheep Management")]
    [SerializeField] Sheep sheepPrefab;
    [SerializeField] private List<Sheep> sheep = new List<Sheep>();
    public List<Sheep> Sheep { get { return sheep; } }
    [SerializeField] int sheepSpawnCount;
    [SerializeField] float spawnMinX = -1f;
    [SerializeField] float spawnMaxX = 1f;
    [SerializeField] float spawnMinY = -1f;
    [SerializeField] float spawnMaxY = 1f;
    [Space(20)]

    [Header("Dog Management")]
    [SerializeField] Dog dogPrefab;
    [SerializeField] private List<Dog> dogs = new List<Dog>();
    private List<Dog> inactiveDogs = new List<Dog>();
    public List<Dog> Dogs { get { return dogs; } }
    [SerializeField] int dogSpawnCount;
    private DMState currentDMState;
    Dog tempDog = null;
    private bool prefabSelected = false;
    private bool finalizedPlacement = false;
    [SerializeField] float dogMax = 5;
    [SerializeField] float dogsPlaced = 0;
    [Space(20)]

    [Header("Obstacle Management")]
    [SerializeField] Obstacle obstaclePrefab;
    [SerializeField] List<Obstacle> obstacles = new List<Obstacle>();
    public List<Obstacle> Obstacles { get { return obstacles; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        goal = GameObject.Find("Goal").GetComponent<Goal>();
        cam = Camera.main;

        #region SHEEP SPAWNING
        for (int i = 0; i < sheepSpawnCount; i++)
        {
            Vector3 randPos = new Vector3(Random.Range(spawnMinX, spawnMaxX), Random.Range(spawnMinY, spawnMaxY), 0);
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
                    // Instantiate the Dog and add it to the list
                    dogs.Add(Instantiate(dogPrefab, new Vector3(cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0)).x,
                                                                    cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0)).y, 0),
                                                                    Quaternion.identity));

                    // Increment the total amount of Dogs placed
                    dogsPlaced++;

                    // Set the Dog to the Placed state
                    dogs[dogs.Count - 1].ChangePlacingStateTo(DogPlacedState.Placed);

                    // Set finalizedPlacement to false
                    finalizedPlacement = false;

                    // Set prefabSelected to false
                    prefabSelected = false;

                    // Reset tempDog
                    Destroy(tempDog.gameObject);
                    tempDog = null;

                    // Return currentState to Idle
                    currentDMState = DMState.Idle;
                }
                break;
        }

        // Check for inactive dogs and add them to the inactive list
        foreach (Dog dog in dogs)
        {
            if (dog.Duration <= 0)
            {
                inactiveDogs.Add(dog);
            }
        }

        // Remove inactive dogs
        RemoveInactiveDogs();
        #endregion

        #region Penned Sheep
        RemovePennedSheep();
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

    public void RemovePennedSheep()
    {
        // Check the list of inactive dogs
        if (goal.PennedSheep.Count > 0)
        {
            // Compare the list of inactive dogs with the total dogs
            for (int i = 0; i < goal.PennedSheep.Count; i++)
            {
                for (int j = 0; j < Sheep.Count; j++)
                {
                    // If any of the inactive dogs are equal to any of the active dogs
                    // in dogs, remove them from the list
                    if (goal.PennedSheep[i].Equals(Sheep[j]))
                    {
                        Sheep.Remove(goal.PennedSheep[i]);
                    }
                }
            }
        }
    }

    public void OnClick()
    {
        // Check if the player can place a Dog
        if(dogsPlaced < dogMax)
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
