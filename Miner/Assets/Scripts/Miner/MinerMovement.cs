using UnityEngine;

public class MinerMovement : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    public float maxMovementSpeedReduced = 0.5f;
    public float percReduced = 0.0f;

    public void Move(Vector3 dir)
    {
        transform.LookAt(dir, Vector3.up);

        float finalMovementSpeed = movementSpeed - maxMovementSpeedReduced * percReduced;

        transform.Translate(transform.forward * finalMovementSpeed * Time.deltaTime);
    }
}
