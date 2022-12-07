using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CounterType
{
    Food,
    Dog,
    Marker
}

public class Counter : MonoBehaviour
{
    private AgentManager agentManager;
    private FoodManager foodManager;
    private MarkerManager markerManager;
    private Text text;

    [Header("Counter State")]
    public CounterType counterType;

    // Start is called before the first frame update
    void Start()
    {
        agentManager = GameObject.Find("Agent Manager").GetComponent<AgentManager>();
        foodManager = GameObject.Find("Food Manager").GetComponent<FoodManager>();
        markerManager = GameObject.Find("Marker Manager").GetComponent<MarkerManager>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(counterType)
        {
            case CounterType.Food:
                text.text = "Food: " + foodManager.PlacedFood + "/" + foodManager.MaxFood;
                break;

            case CounterType.Dog:
                text.text = "Dogs: " + agentManager.PlacedDogs + "/" + agentManager.MaxDogs;
                break;

            case CounterType.Marker:
                text.text = "Markers: " + markerManager.PlacedMarkers + "/" + markerManager.MaxMarkers;
                break;
        }
    }
}
