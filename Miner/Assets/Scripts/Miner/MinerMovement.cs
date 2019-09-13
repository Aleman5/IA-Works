using UnityEngine;

public class MinerMovement : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    public float maxMovSpeedReduced = 0.3f;

    [HideInInspector] public float percReduced = 0.0f;

    public bool Move(Vector3 objective)
    {
        objective.y = 0;
        transform.LookAt(objective, Vector3.up);

        float finalMovementSpeed = movementSpeed - maxMovSpeedReduced * percReduced;

        transform.Translate(Vector3.forward * finalMovementSpeed * Time.deltaTime);

        float dist = Vector3.Distance(transform.position, objective);

        if (dist <= 0.08f)
            return true;
        return false;
    }
}
