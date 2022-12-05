using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [Header("Obstacle List")]
    [SerializeField] List<Obstacle> obstacles = new List<Obstacle>();
    public List<Obstacle> Obstacles { get { return obstacles; } set { obstacles = value; } }

    // Start is called before the first frame update
    void Start()
    {
        // Add all Obstacles to the list
        obstacles.AddRange(FindObjectsOfType<Obstacle>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
