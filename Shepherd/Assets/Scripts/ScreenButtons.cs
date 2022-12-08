using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenButtons : MonoBehaviour
{
    private LevelManager levelManager;

    public void Start()
    {
        levelManager = GameObject.Find("Level Manager").GetComponent<LevelManager>();
    }

    public void OnClick()
    {
        levelManager.ChangeLevel = true;
    }
}
