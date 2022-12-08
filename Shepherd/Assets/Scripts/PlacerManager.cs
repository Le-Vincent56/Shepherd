using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlaceType
{
    None,
    Food,
    Dog,
    Marker
}

public class PlacerManager : MonoBehaviour
{
    #region
    private FoodManager foodManager;
    private AgentManager agentManager;
    private MarkerManager markerManager;

    public PlaceType currentType = PlaceType.None;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        foodManager = GameObject.Find("Food Manager").GetComponent<FoodManager>();
        agentManager = GameObject.Find("Agent Manager").GetComponent<AgentManager>();
        markerManager = GameObject.Find("Marker Manager").GetComponent<MarkerManager>();
    }

    /// <summary>
    /// Allow the player to place their desired object
    /// </summary>
    /// <param name="context">Controller context</param>
    public void OnFire(InputAction.CallbackContext context)
    {
        switch(currentType)
        {
            case PlaceType.None:
                // Do nothing
                break;

            case PlaceType.Food:
                switch (foodManager.CurrentState)
                {
                    case FMState.Idle:
                        break;

                    case FMState.Selected:
                        // Set finalized placement to true
                        foodManager.FinalizedPlacement = true;
                        break;
                }
                break;

            case PlaceType.Dog:
                switch (agentManager.CurrentDMState)
                {
                    case DMState.Idle:
                        break;

                    case DMState.Selected:
                        // Set finalized placement to true
                        agentManager.FinalizedPlacement = true;
                        break;
                }
                break;

            case PlaceType.Marker:
                switch (markerManager.CurrentState)
                {
                    case MMState.Idle:
                        break;

                    case MMState.Selected:
                        // Set finalized placement to true
                        markerManager.FinalizedPlacement = true;
                        break;
                }
                break;
        }
    }

    /// <summary>
    /// Allow the player to escape their selection of placement
    /// </summary>
    /// <param name="context">Controller context</param>
    public void OnEscape(InputAction.CallbackContext context)
    {
        switch (currentType)
        {
            case PlaceType.None:
                // Do nothing
                break;

            case PlaceType.Food:
                switch (foodManager.CurrentState)
                {
                    case FMState.Idle:
                        break;

                    case FMState.Selected:
                        // Set prefabSelected to false
                        foodManager.PrefabSelected = false;

                        // Set finalizedPlacement to false
                        foodManager.FinalizedPlacement = false;

                        // Reset tempFood
                        Destroy(foodManager.tempFood.gameObject);
                        foodManager.tempFood = null;

                        // Change the currentState back to FMState.Idle
                        foodManager.CurrentState = FMState.Idle;
                        break;
                }
                break;

            case PlaceType.Dog:
                switch (agentManager.CurrentDMState)
                {
                    case DMState.Idle:
                        break;

                    case DMState.Selected:
                        // Set prefabSelected to false
                        agentManager.PrefabSelected = false;

                        // Set finalizedPlacement to false
                        agentManager.FinalizedPlacement = false;

                        // Reset tempDog
                        Destroy(agentManager.tempDog.gameObject);
                        agentManager.tempDog = null;

                        // Change the currentDMState back to DMState.Idle
                        agentManager.CurrentDMState = DMState.Idle;
                        break;
                }
                break;

            case PlaceType.Marker:
                switch (markerManager.CurrentState)
                {
                    case MMState.Idle:
                        break;

                    case MMState.Selected:
                        // Set prefabSelected to false
                        markerManager.PrefabSelected = false;

                        // Set finalizedPlacement to false
                        markerManager.FinalizedPlacement = false;

                        // Reset tempMarker
                        Destroy(markerManager.tempMarker.gameObject);
                        markerManager.tempMarker = null;

                        // Change the currentState back to MMState.Idle
                        markerManager.CurrentState = MMState.Idle;
                        break;
                }
                break;
        }
    }
}
