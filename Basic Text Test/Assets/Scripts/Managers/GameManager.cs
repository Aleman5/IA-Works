using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MBSingleton<GameManager>
{
    [Header("Initialize data")]
    public GameObject coin;
    public Transform coinsParent;
    public Transform ring;
    public int offset = 2;

    [Header("In-game elements")]
    public GameObject myPlayer;
    public GameObject enemyPlayer;

    List<GameObject> coins = new List<GameObject>();

    public void StartGame(bool isServer = true)
    {
        ActivateElements();
        AddComponents(isServer);
        InitializeCoins();
        UIManager.Instance.OnGameStart();

        //StartCoroutine("InitMatch");
    }

    public void UserConnected()
    {
        StartGame(false);
    }

    void ActivateElements()
    {
        ring.gameObject.SetActive(true);
        myPlayer.SetActive(true);
        enemyPlayer.SetActive(true);
    }

    void InitializeCoins()
    {
        while (coins.Count > 0)
        {
            GameObject coin = coins[0];
            coins.Remove(coin);
            Destroy(coin);
        }

        int height = (int)ring.localScale.x * 10;
        int widht = (int)ring.localScale.z * 10;

        for (int i = 2; i < height - offset; i++)
        {
            for (int j = 2; j < widht - offset; j++)
            {
                Vector3 pos = new Vector3(i + 0.5f, 0.5f, j + 0.5f);
                GameObject go = Instantiate(coin, pos, transform.rotation, coinsParent);
                go.GetComponent<Coin>().SetObjectId((uint)coins.Count + 1);
                coins.Add(go);
            }   
        }
    }

    void AddComponents(bool isServer)
    {
        if (isServer)
        {
            myPlayer.AddComponent<PlayerMovement>();
            myPlayer.AddComponent<PlayerTrigger>();

            enemyPlayer.AddComponent<PlayerUDP>();
        }
        else
        {
            enemyPlayer.AddComponent<PlayerMovement>();
            enemyPlayer.AddComponent<PlayerTrigger>();

            myPlayer.AddComponent<PlayerUDP>();
        }
    }
}