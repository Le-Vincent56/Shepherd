using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum FMState
{
    Idle,
    Selected
}

public class FoodManager : MonoBehaviour
{
    #region FIELDS
    private Camera cam;
    public FMState currentState = FMState.Idle;
    [SerializeField] Food foodPrefab;
    [SerializeField] List<Food> food = new List<Food>();
    [SerializeField] List<Food> inactiveFood = new List<Food>();
    [SerializeField] float foodMax = 5;
    [SerializeField] float foodPlaced = 0;
    public List<Food> Food { get { return food; } }
    public float MaxFood { get { return foodMax; } }
    public float PlacedFood { get { return foodPlaced; } }
    Food tempFood = null;

    private bool prefabSelected = false;
    private bool finalizedPlacement = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentState)
        {
            case FMState.Idle:
                break;

            case FMState.Selected:
                // Check if the prefab is selected, if so, then instantiate a new Food as tempFood
                if (!prefabSelected)
                {
                    tempFood = Instantiate(foodPrefab, new Vector3(cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0)).x,
                                                                    cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0)).y, 0), Quaternion.identity);
                    prefabSelected = true;
                }

                // Check if placement if finalized
                if (finalizedPlacement)
                {
                    // Instantiate the Food and add it to the list
                    food.Add(Instantiate(foodPrefab, new Vector3(cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0)).x, 
                                                                    cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0)).y, 0), Quaternion.identity));

                    // Increment the total amount of Food placed
                    foodPlaced++;

                    // Set the Food to the Placed state
                    food[food.Count - 1].ChangeStateTo(FoodState.Placed);

                    // Set finalizedPlacement to false
                    finalizedPlacement = false;

                    // Set prefabSelected to false
                    prefabSelected = false;

                    // Reset tempFood
                    Destroy(tempFood.gameObject);
                    tempFood = null;

                    // Return currentState to Idle
                    currentState = FMState.Idle;
                }
                break;
        }

        // Check for inactive foods and add them to the inactive list
        foreach(Food placedFood in food)
        {
            if(placedFood.Duration <= 0)
            {
                inactiveFood.Add(placedFood);
            }
        }

        // Remove inactive foods
        RemoveInactiveFood();
    }

    public void RemoveInactiveFood()
    {
        // Check the list of inactive food
        if (inactiveFood.Count > 0)
        {
            // Compare the list of inactive foods with the total foods
            for (int i = 0; i < inactiveFood.Count; i++)
            {
                for (int j = 0; j < food.Count; j++)
                {
                    // If any of the inactive foods are equal to any of the placed foods
                    // in food, remove them from the list
                    if (inactiveFood[i].Equals(food[j]))
                    {
                        Destroy(food[j].gameObject);
                        food.Remove(inactiveFood[i]);
                    }
                }
            }

            // Clear the inactive foods list
            inactiveFood.Clear();
        }
    }

    public void OnClick()
    {
        // Check if the player can place any more food
        if(foodPlaced < foodMax)
        {
            switch (currentState)
            {
                case FMState.Idle:
                    // Change currentState to Selected
                    currentState = FMState.Selected;
                    break;

                case FMState.Selected:
                    // Change currentState to Idle
                    currentState = FMState.Idle;
                    break;
            }
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        switch(currentState)
        {
            case FMState.Idle:
                break;

            case FMState.Selected:
                // Set finalized placement to true
                finalizedPlacement = true;
                break;
        }
    }
}
