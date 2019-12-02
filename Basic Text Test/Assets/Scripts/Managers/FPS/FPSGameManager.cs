using System.Collections.Generic;
using UnityEngine;

public class FPSGameManager : MBSingleton<FPSGameManager>
{
    [Header("In-game elements")]
    public GameObject myPlayerGO;
    public GameObject enemyPlayerGO;
    public Transform serverSpawnPoint;
    public Transform clientSpawnPoint;
    public List<Transform> spawnPoints;

    [HideInInspector] public GameObject myPlayerInstance;
    [HideInInspector] public GameObject enemyPlayerInstance;


    int kills = 0;
    int enemyKills = 0;

    public void StartGame(bool isServer = true)
    {
        if (isServer)
        {
            myPlayerInstance    = Instantiate(myPlayerGO,    serverSpawnPoint.position, serverSpawnPoint.rotation);
            enemyPlayerInstance = Instantiate(enemyPlayerGO, clientSpawnPoint.position, clientSpawnPoint.rotation);
        }
        else
        {
            myPlayerInstance    = Instantiate(enemyPlayerGO, serverSpawnPoint.position, serverSpawnPoint.rotation);
            enemyPlayerInstance = Instantiate(myPlayerGO,    clientSpawnPoint.position, clientSpawnPoint.rotation);
        }

        FPSUIManager.Instance.OnGameStart();
    }

    public void UserConnected()
    {
        StartGame(false);
    }

    public void OnKill()
    {
        FPSUIManager.Instance.OnEnemyKilled(++kills);
    }

    public void OnDeath()
    {
        FPSUIManager.Instance.OnDeath(++enemyKills);
    }
}