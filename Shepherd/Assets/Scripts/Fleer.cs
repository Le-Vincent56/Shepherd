using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fleer : Agent
{
    #region FIELDS
    [SerializeField] Agent toFlee;
    #endregion

    // Update is called once per frame
    void Update()
    {
        CalcSteeringForces();

        if (PhysicsObject.CircleCollision(toFlee))
        {
            Vector3 minSpawnPoint = new Vector3(-PhysicsObject.camWidth, -PhysicsObject.camHeight, 0);
            Vector3 maxSpawnPoint = new Vector3(PhysicsObject.camWidth, PhysicsObject.camHeight, 0);

            float randValX = Random.Range(minSpawnPoint.x, maxSpawnPoint.x);
            float randValY = Random.Range(minSpawnPoint.y, maxSpawnPoint.y);
            Vector3 randomSpawnPoint = new Vector3(randValX, randValY, 0);

            transform.position = randomSpawnPoint;
        }
    }

    public override void CalcSteeringForces()
    {
        PhysicsObject.ApplyForce(Flee(toFlee));
    }
}
