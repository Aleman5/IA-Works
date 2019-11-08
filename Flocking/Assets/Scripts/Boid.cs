using UnityEngine;

public class Boid : MonoBehaviour
{
    public float speed;
    public float sightLenght;
    public float rotSpeed;
    
    Vector3 objDir;

    void Update()
    {
        objDir = FlockingManager.Instance.CalculateDirectionObjective(this);
    }

    void LateUpdate()
    {
        transform.forward = Vector3.Slerp(transform.forward, objDir, rotSpeed * Time.deltaTime);
        transform.Translate(transform.forward * speed * Time.deltaTime);
    }
}