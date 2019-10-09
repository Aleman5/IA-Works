using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    PlayerMovement playerMovement;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Coin")
        {
            playerMovement.UpdateScore(10);
            other.GetComponent<Coin>().Touched();
            Destroy(other.gameObject);
        }
    }
}
