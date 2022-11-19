using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    #region FIELDS
    // Physics Fields
    [SerializeField] Vector3 position = Vector3.zero;
    [SerializeField] Vector3 direction = Vector3.right;
    [SerializeField] Vector3 velocity = Vector3.zero;
    [SerializeField] Vector3 acceleration;
    public float mass = 1f;

    // Physics Properties
    public Vector3 Position { get { return position; } }
    public Vector3 Velocity { get { return velocity; } }
    public Vector3 Direction { get { return direction; } }

    // Friction Fields
    public bool applyFriction = false;
    public float coeffOfFriction = 0.05f;

    // Gravity Fields
    public bool applyGravity = false;
    public float strengthOfGravity = -1f;

    // Collision Fields
    float radius;

    // Collision Properties
    public float Radius { get { return radius; } }

    // Camera
    [SerializeField] Camera cam;
    public float camHeight;
    public float camWidth;
    public Vector2 WorldSize;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Set position
        position = transform.position;

        // Set radius
        radius = (GetComponent<SpriteRenderer>().bounds.max - GetComponent<SpriteRenderer>().bounds.center).magnitude;

        // Set Camera bounds
        cam = Camera.main;
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;
        WorldSize = new Vector2(camWidth, camHeight);
    }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;

        if (applyFriction)
        {
            ApplyFriction();
        }

        if (applyGravity)
        {
            ApplyGravity();
        }

        // Calculate the velocity for this frame
        velocity += acceleration * Time.deltaTime;

        // Bounce();

        position += velocity * Time.deltaTime;

        if (velocity.sqrMagnitude > Mathf.Epsilon)
        {
            // Calculate current direction from velocity
            direction = velocity.normalized;
        }

        // Rotate towards the velocity
        transform.rotation = Quaternion.LookRotation(Vector3.back, velocity);

        // Draw movement
        transform.position = position;

        // Zero out acceleration
        acceleration = Vector3.zero;
    }

    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    public void ApplyFriction()
    {
        Vector3 friction = velocity * -1;
        friction.Normalize();
        friction = friction * coeffOfFriction;
        ApplyForce(friction);
    }

    public void ApplyGravity()
    {
        Vector3 gravity = new Vector3(0, strengthOfGravity * mass, 0);
        ApplyForce(gravity);
    }

    public bool CircleCollision(Agent target)
    {
        // Combine the radii
        float unitedRadiiSquared = Mathf.Pow(Radius + target.GetComponent<PhysicsObject>().Radius, 2);

        // Get distances between the centers of the two GameObjects
        Vector3 distance = target.GetComponent<SpriteRenderer>().bounds.center - GetComponent<SpriteRenderer>().bounds.center;
        float distanceF = distance.magnitude;
        float distanceSquared = Mathf.Pow(distanceF, 2);

        // If the distance between the two centers are less than the sum of their radii, return true
        // otherwise, return false
        if (distanceSquared < unitedRadiiSquared)
        {
            return true;
        }
        else return false;
    }

    public void Bounce()
    {
        // Check X bounds and apply necessary bounce forces
        if(position.x > camWidth)
        {
            position.x = camWidth;
            velocity.x *= -1;
        } else if(position.x < -camWidth)
        {
            position.x = -camWidth;
            velocity.x *= -1;
        }

        if(position.y > camHeight)
        {
            position.y = camHeight;
            velocity.y *= -1;
        } else if(position.y < -camHeight)
        {
            position.y = -camHeight;
            velocity.y *= -1;
        }
    }
}
