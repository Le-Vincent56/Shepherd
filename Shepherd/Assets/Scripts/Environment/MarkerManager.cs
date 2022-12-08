using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MMState
{
    Idle,
    Selected
}

public class MarkerManager : MonoBehaviour
{
    #region FIELDS
    private Camera cam;
    private PlacerManager placerManager;
    private MMState currentState = MMState.Idle;
    public MMState CurrentState { get { return currentState; } set { currentState = value; } }
    [SerializeField] Marker markerPrefab;
    [SerializeField] List<Marker> markers = new List<Marker>();
    [SerializeField] List<Marker> inactiveMarkers = new List<Marker>();
    [SerializeField] float markerMax = 5;
    public float MaxMarkers { get { return markerMax; } }
    [SerializeField] float markersPlaced = 0;
    public float PlacedMarkers { get { return markersPlaced; } }
    public List<Marker> Markers { get { return markers; } }
    public Marker tempMarker = null;

    private bool prefabSelected = false;
    public bool PrefabSelected { get { return prefabSelected; } set { prefabSelected = value; } }
    private bool finalizedPlacement = false;
    public bool FinalizedPlacement { get { return finalizedPlacement; } set { finalizedPlacement = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        placerManager = GameObject.Find("Placer Manager").GetComponent<PlacerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case MMState.Idle:
                break;

            case MMState.Selected:
                // Check if the prefab is selected, if so, then instantiate a new marker as tempMarker
                if (!prefabSelected)
                {
                    tempMarker = Instantiate(markerPrefab, new Vector3(cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0)).x,
                                                                    cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0)).y, 0), Quaternion.identity);
                    prefabSelected = true;
                }

                // Check if placement if finalized
                if (finalizedPlacement)
                {
                    // Instantiate the Marker and add it to the list
                    markers.Add(Instantiate(markerPrefab, new Vector3(cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0)).x,
                                                                    cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0)).y, 0), Quaternion.identity));

                    // Increment the total amount of Markers placed
                    markersPlaced++;

                    // Set the food to the Placed state
                    markers[markers.Count - 1].ChangeStateTo(MarkerState.Placed);

                    // Set finalizedPlacement to false
                    finalizedPlacement = false;

                    // Set prefabSelected to false
                    prefabSelected = false;

                    // Reset tempMarker
                    Destroy(tempMarker.gameObject);
                    tempMarker = null;

                    // Return currentState to Idle
                    currentState = MMState.Idle;
                }
                break;
        }

        // Check for inactive Markers and add them to the inactive list
        foreach (Marker placedMarker in markers)
        {
            if (placedMarker.Duration <= 0)
            {
                inactiveMarkers.Add(placedMarker);
            }
        }

        // Remove inactive Markers
        RemoveInactiveMarkers();
    }

    public void RemoveInactiveMarkers()
    {
        // Check the list of inactive food
        if (inactiveMarkers.Count > 0)
        {
            // Compare the list of inactive foods with the total foods
            for (int i = 0; i < inactiveMarkers.Count; i++)
            {
                for (int j = 0; j < markers.Count; j++)
                {
                    // If any of the inactive foods are equal to any of the placed foods
                    // in food, remove them from the list
                    if (inactiveMarkers[i].Equals(markers[j]))
                    {
                        Destroy(markers[j].gameObject);
                        markers.Remove(inactiveMarkers[i]);
                    }
                }
            }

            // Clear the inactive foods list
            inactiveMarkers.Clear();
        }
    }

    public void OnClick()
    {
        placerManager.currentType = PlaceType.Marker;

        // Check if the player can place any more food
        if (markersPlaced < markerMax)
        {
            switch (currentState)
            {
                case MMState.Idle:
                    // Change currentState to Selected
                    currentState = MMState.Selected;
                    break;

                case MMState.Selected:
                    // Change currentState to Idle
                    currentState = MMState.Idle;
                    break;
            }
        }
    }
}
