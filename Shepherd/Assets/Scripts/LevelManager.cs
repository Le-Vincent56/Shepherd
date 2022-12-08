using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum Level
{
    Title,
    One,
    Two,
    Three,
    End
}

public class LevelManager : MonoBehaviour
{
    [Header("Level")]
    public Level currentLevel = Level.Title;
    [SerializeField] bool changeLevel = false;
    public bool ChangeLevel { get { return changeLevel; } set { changeLevel = value; } }

    private void Awake()
    {
        // Find all Level Managers
        LevelManager[] objs = FindObjectsOfType<LevelManager>();

        // Destroy this if there is more than one Level Manager
        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }

        // Keep the LevelManager consistent
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentLevel)
        {
            case Level.Title:
                // If changeLevel is true
                if (changeLevel)
                {
                    // Load the first level
                    SceneManager.LoadScene("LevelOne");
                    currentLevel = Level.One;
                    changeLevel = false;
                }
                break;

            case Level.One:
                // If changeLevel is true
                if (changeLevel)
                {
                    // Load the second level
                    SceneManager.LoadScene("LevelTwo");
                    currentLevel = Level.Two;
                    changeLevel = false;
                }
                break;

            case Level.Two:
                // If changeLevel is true
                if (changeLevel)
                {
                    // Load the third level
                    SceneManager.LoadScene("LevelThree");
                    currentLevel = Level.Three;
                    changeLevel = false;
                }
                break;

            case Level.Three:
                // If changeLevel is true
                if (changeLevel)
                {
                    // Load the end screen
                    SceneManager.LoadScene("EndScreen");
                    currentLevel = Level.End;
                    changeLevel = false;
                }
                break;

            case Level.End:
                // If changeLevel is true
                if(changeLevel)
                {
                    // Load  the title screen
                    SceneManager.LoadScene("TitleScreen");
                    currentLevel = Level.Title;
                    changeLevel = false;
                }
                break;
        }
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        // Get the active scene
        Scene currentScene = SceneManager.GetActiveScene();

        // Set changeLevel to false
        changeLevel = false;

        // Set the current level
        switch (currentScene.name)
        {
            case "LevelOne":
                currentLevel = Level.One;
                break;

            case "LevelTwo":
                currentLevel = Level.Two;
                break;

            case "LevelThree":
                currentLevel = Level.Three;
                break;
        }

        // Reload the scene
        SceneManager.LoadScene(currentScene.name);
    }
}
