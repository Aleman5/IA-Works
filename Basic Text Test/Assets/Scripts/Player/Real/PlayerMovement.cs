using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Variables")]
    public float speed = 3;
    public int points = 0;

    uint playerObjectId = 1;
    uint pointsObjectId = 2;

    void Update()
    {
        bool dataChanged = false;

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0.0f, speed, 0.0f);
            dataChanged = true;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0.0f, -speed, 0.0f);

            dataChanged = !dataChanged;
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            dataChanged = true;
        }

        if (dataChanged)
            MessageManager.Instance.SendEntityInfo(transform.position, transform.rotation, transform.rotation, false, 0, playerObjectId, ConnectionManager.Instance.clientId);
    }

    public void UpdateScore(int amount)
    {
        points += amount;
        UIManager.Instance.OnScoreChange(points);
        MessageManager.Instance.SendScore(points, pointsObjectId, ConnectionManager.Instance.clientId);
    }
}