using System.Collections.Generic;
using UnityEngine;

public class FlockingManager : MBSingleton<FlockingManager>
{
    public Vector3 CalculateDirectionObjective(Boid thisBoid)
    {
        Vector3 dir = thisBoid.transform.forward;

        List<Transform> adyBoids = FlockingLogic.GetBoidsInRange(thisBoid.transform.position, thisBoid.sightLenght);

        if (adyBoids.Count > 0)
            dir = FlockingLogic.GetDirectionObjective(thisBoid.transform, adyBoids);

        return dir;
    }
}