using System.Collections.Generic;
using UnityEngine;

public class FlockingManager : MonoBehaviour
{
    // Hacerlo Singleton

    public Vector3 CalculateDirectionObjective(Boid thisBoid)
    {
        Vector3 dir = new Vector3();

        List<Transform> adyBoids = FlockingLogic.GetBoidsInRange(thisBoid.transform.position, thisBoid.sightLenght);

        dir = FlockingLogic.GetDirectionObjective(thisBoid.transform, adyBoids);

        return dir;
    }
}