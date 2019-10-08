using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject coin;
    public Transform coinsParent;
    public Transform ring;
    public int offset = 2;

    List<GameObject> coins = new List<GameObject>();

    void Awake()
    {
        StartGame();
    }

    public void StartGame()
    {
        while (coins.Count > 0)
            Destroy(coins[0]);

        int height = (int)ring.localScale.x * 10;
        int widht = (int)ring.localScale.z * 10;

        for (int i = 2; i < height - offset; i++)
        {
            for (int j = 2; j < widht - offset; j++)
            {
                Vector3 pos = new Vector3(i + 0.5f, 0.5f, j + 0.5f);
                coins.Add(Instantiate(coin, pos, transform.rotation, coinsParent));
            }   
        }
    }
}
