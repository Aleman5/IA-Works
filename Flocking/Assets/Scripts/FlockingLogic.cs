using System.Collections.Generic;
using UnityEngine;

public static class FlockingLogic
{
    static public List<Transform> GetBoidsInRange(Vector3 origin, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(origin, radius);

        List<Transform> boids = new List<Transform>();

        foreach (Collider col in hitColliders)
        {
            if (col.tag == "Boid")
                boids.Add(col.transform);
        }

        return boids;
    }

    static public Vector3 GetDirectionObjective(Transform thisBoid, List<Transform> adyBoids)
    {
        Vector3 alignment  = Vector3.zero;
        Vector3 cohesion   = Vector3.zero;
        Vector3 separation = Vector3.zero;

        foreach (Transform adyBoid in adyBoids)
        {
            alignment  += adyBoid.forward;
            cohesion   += adyBoid.position;
            separation += adyBoid.position - thisBoid.position;
        }

        cohesion /= adyBoids.Count;
        separation /= adyBoids.Count;

        return (alignment.normalized + cohesion.normalized + separation.normalized).normalized;
    }
}