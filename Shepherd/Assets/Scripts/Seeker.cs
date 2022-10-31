using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeker : Agent
{
    #region FIELDS
    [SerializeField] Agent toSeek;
    #endregion

    public override void CalcSteeringForces()
    {
        PhysicsObject.ApplyForce(Seek(toSeek));
    }
}
