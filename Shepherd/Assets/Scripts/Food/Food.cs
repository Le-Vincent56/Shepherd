using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum FoodState
{
    None,
    Placing,
    Placed
}

public class Food : MonoBehaviour
{
    #region FIELDS
    private FoodState currentState = FoodState.Placing;
    private Camera cam;

    private Vector3 position;
    public Vector3 Position { get { return position; } }

    [SerializeField] float duration = 3f;
    public float Duration { get { return duration; } }
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
            case FoodState.None:
                break;

            case FoodState.Placing:
                // Show where the Food will be placed depending on cursor movement
                position = new Vector3(cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0)).x,
                                                                    cam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0)).y, 0);
                transform.position = position;
                break;

            case FoodState.Placed:
                // Set position and start countdown
                position = transform.position;
                duration -= Time.deltaTime;
                break;
        }
    }

    public void ChangeStateTo(FoodState newState)
    {
        currentState = newState;
    }
}
