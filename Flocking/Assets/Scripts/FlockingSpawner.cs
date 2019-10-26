using UnityEngine;

public class FlockingSpawner : MonoBehaviour
{
    public GameObject boidPrefab;
    public Vector3 origin;

    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Vector3 pos = new Vector3(origin.x + i, origin.y + j, origin.z);
                Instantiate(boidPrefab, pos, transform.rotation);
            }
        }
    }
}