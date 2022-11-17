using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    #region FIELDS
    [Header("Sheep Management")]
    [SerializeField] Sheep sheepPrefab;
    private List<Sheep> sheep = new List<Sheep>();
    public List<Sheep> Sheep { get { return sheep; } }
    [SerializeField] int sheepSpawnCount;
    [Space(20)]

    [Header("Dog Management")]
    [SerializeField] Dog dogPrefab;
    private List<Dog> dogs = new List<Dog>();
    public List<Dog> Dogs { get { return dogs; } }
    [SerializeField] int dogSpawnCount;
    [Space(20)]

    [Header("Obstacle Management")]
    [SerializeField] Obstacle obstaclePrefab;
    [SerializeField] List<Obstacle> obstacles = new List<Obstacle>();
    public List<Obstacle> Obstacles { get { return obstacles; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < sheepSpawnCount; i++)
        {
            Vector3 randPos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            sheep.Add(Instantiate(sheepPrefab, randPos, Quaternion.identity));
            sheep[i].Init(this);
            sheep[i].ChangeStateTo(SheepState.Wander);
        }

        for(int i = 0; i < dogSpawnCount; i++)
        {
            Vector3 randPos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            dogs.Add(Instantiate(dogPrefab, randPos, Quaternion.identity));
            dogs[i].Init(this);
            dogs[i].ChangeStateTo(DogState.Seek);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
