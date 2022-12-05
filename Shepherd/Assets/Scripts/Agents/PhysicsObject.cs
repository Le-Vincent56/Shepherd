using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    #region FIELDS
    [Header("Camera")]
    [SerializeField] Camera cam;
    public float camHeight;
    public float camWidth;
    public Vector2 WorldSize;
    [Space(20)]

    [Header("Physics Vectors and Mass")]
    [SerializeField] Vector3 position = Vector3.zero;
    [SerializeField] Vector3 direction = Vector3.right;
    [SerializeField] Vector3 velocity = Vector3.zero;
    [SerializeField] Vector3 acceleration = Vector3.zero;
    public float mass = 1f;
    [Space(20)]

    [Header("Friction")]
    public bool applyFriction = false;
    public float coeffOfFriction = 0.05f;
    [Space(20)]

    [Header("Gravity")]
    public bool applyGravity = false;
    public float strengthOfGravity = -1f;
    [Space(20)]

    [Header("Collision Radius")]
    [SerializeField] float radius;

    [Header("Apply Physics")]
    [SerializeField] bool applyPhysics = true;

    // Physics Properties
    public Vector3 Position { get { return position; } }
    public Vector3 Velocity { get { return velocity; } }
    public Vector3 Direction { get { return direction; } }

    // Collision Properties
    public float Radius { get { return radius; } }

    // Apply Physics Properties
    public bool ApplyPhysics { get { return applyPhysics; } set { applyPhysics = value; } }
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
        if(applyPhysics)
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

            position += velocity * Time.deltaTime;

            if (velocity.sqrMagnitude > Mathf.Epsilon)
            {
                // Calculate current direction from velocity
                direction = velocity.normalized;
            }

            // Draw movement
            transform.position = position;

            // Rotate towards the velocity
            transform.rotation = Quaternion.LookRotation(Vector3.back, direction);

            // Zero out acceleration
            acceleration = Vector3.zero;
        }
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
