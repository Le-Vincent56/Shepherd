using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSheepManager : MonoBehaviour
{
    private Camera cam;
    private float camHeight;
    private float camWidth;

    [Header("Title Sheep Management")]
    [SerializeField] TitleSheep titleSheepPrefab;
    [SerializeField] private List<TitleSheep> titleSheep = new List<TitleSheep>();
    public List<TitleSheep> TitleSheep { get { return titleSheep; } }
    [SerializeField] int sheepSpawnCount;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;

        #region TITLE SHEEP SPAWNING
        for (int i = 0; i < sheepSpawnCount; i++)
        {
            Vector3 randPos = new Vector3(Random.Range(-camWidth, camWidth), Random.Range(-camHeight, camHeight), 95);
            titleSheep.Add(Instantiate(titleSheepPrefab, randPos, Quaternion.identity));
            titleSheep[i].Init(this);
        }
        #endregion
    }
}
