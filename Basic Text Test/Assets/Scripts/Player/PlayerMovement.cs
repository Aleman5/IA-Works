using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Varialbles")]
    public float speed;
    public int points = 0;

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0.0f, speed, 0.0f);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0.0f, -speed, 0.0f);
        }
    }

    public void UpdateScore(int amount)
    {
        points += amount;
    }
}
