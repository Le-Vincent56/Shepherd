using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    SpriteRenderer sprite;
    AgentManager agentManager;
    LevelManager levelManager;
    [SerializeField] List<Sheep> pennedSheep = new List<Sheep>();
    public List<Sheep> PennedSheep { get { return pennedSheep; } set { pennedSheep = value; } }

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        agentManager = GameObject.Find("Agent Manager").GetComponent<AgentManager>();
        levelManager = GameObject.Find("Level Manager").GetComponent<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if any sheep from the list have collided with the goal
        foreach(Sheep sheep in agentManager.Sheep)
        {
            bool sheepPenned = false;

            foreach(Sheep pennedSheep in pennedSheep)
            {
                if(sheep == pennedSheep)
                {
                    sheepPenned = true;
                }
            }

            // If any of the sheep collide with the goal, add a timer to the list
            // and state that they are in the goal
            if(AABBCollision(sheep))
            {
                sheep.NotInGoal = false;
            } else
            {
                sheep.NotInGoal = true;
            }

            // If the sheep's freeze timer is 0 and it is still in the goal
            // remove the sheep from the list of sheep
            if(sheep.FreezeTimer <= 0 && !sheep.NotInGoal && !sheepPenned)
            {
                pennedSheep.Add(sheep);
                sheep.PhysicsObject.ApplyPhysics = false;
            }
        }

        // If all the sheep are in the goal, then win
        if(agentManager.Sheep.Count == pennedSheep.Count)
        {
            levelManager.ChangeLevel = true;
        }
    }

    public bool AABBCollision(Sheep sheep)
    {
        float BMinX = sprite.bounds.min.x;
        float BMaxY = sprite.bounds.max.y;
        float BMinY = sprite.bounds.min.y;
        float BMaxX = sprite.bounds.max.x;

        float AMaxX = sheep.GetComponent<SpriteRenderer>().bounds.max.x;
        float AMinX = sheep.GetComponent<SpriteRenderer>().bounds.min.x;
        float AMaxY = sheep.GetComponent<SpriteRenderer>().bounds.max.y;
        float AMinY = sheep.GetComponent<SpriteRenderer>().bounds.min.y;

        // If all conditions are met, return true, otherwise return false
        if (BMinX < AMaxX && BMaxX > AMinX && BMaxY > AMinY && BMinY < AMaxY)
        {
            return true;
        }
        else return false;
    }
}
