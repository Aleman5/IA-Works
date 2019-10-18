using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public float speed;
    public float sightLenght;
    public float rotSpeed;

    void Update()
    {
        //Vector3 objDir = FlockingManager.Instance.CalculateDirectionObjective(this);
        //transform.forward = Vector3.Slerp(transform.forward, objDir, rotSpeed * Time.deltaTime);
    }
}
