using System.Collections.Generic;
using UnityEngine;

public class FPSGameManager : MBSingleton<FPSGameManager>
{
    [Header("Match Settings")]
    public float matchTime;
    public float closeTime;

    [Header("In-game elements")]
    public GameObject myPlayerGO;
    public GameObject enemyPlayerGO;
    public Transform serverSpawnPoint;
    public Transform clientSpawnPoint;
    public List<Transform> spawnPoints;

    [HideInInspector] public GameObject myPlayerInstance;
    [HideInInspector] public GameObject enemyPlayerInstance;
    
    float matchTimeLeft = 9999;
    int kills = 0;
    int enemyKills = 0;
    bool gameRunning = false;

    public void StartGame(bool isServer = true)
    {
        gameRunning = true;

        if (isServer)
        {
            myPlayerInstance    = Instantiate(myPlayerGO,    serverSpawnPoint.position, serverSpawnPoint.rotation);
            enemyPlayerInstance = Instantiate(enemyPlayerGO, clientSpawnPoint.position, clientSpawnPoint.rotation);
        }
        else
        {
            myPlayerInstance    = Instantiate(myPlayerGO,    clientSpawnPoint.position, clientSpawnPoint.rotation);
            enemyPlayerInstance = Instantiate(enemyPlayerGO, serverSpawnPoint.position, serverSpawnPoint.rotation);
        }

        matchTimeLeft = matchTime;
        FPSUIManager.Instance.OnGameStart();
        FPSUIManager.Instance.OnTimeChange(matchTimeLeft);
    }

    void Update()
    {
        if (gameRunning)
        {
            matchTimeLeft -= Time.deltaTime;

            if (matchTimeLeft <= 0)
            {
                gameRunning = false;

                matchTimeLeft = 0;

                bool finishState = kills == enemyKills ? (NetworkManager.Instance.isServer == true ? true : false) :
                                                         (kills > enemyKills ? true : false);
                
                FPSUIManager.Instance.OnMatchFinished(finishState);

                DisablePlayers();
                ExitGame();
            }
            FPSUIManager.Instance.OnTimeChange(matchTimeLeft);
        }
    }

    public void UserConnected()
    {
        StartGame(false);
    }

    void DisablePlayers()
    {
        myPlayerInstance.GetComponent<PlayerMove>().enabled = false;
        myPlayerInstance.GetComponent<Footsteps>().enabled = false;
        myPlayerInstance.GetComponent<PlayerJump>().enabled = false;
        myPlayerInstance.GetComponent<CharacterController>().enabled = false;
        myPlayerInstance.GetComponentInChildren<HeadBob>().enabled = false;
        myPlayerInstance.GetComponentInChildren<PlayerLook>().enabled = false;
        myPlayerInstance.GetComponentInChildren<WeaponSway>().enabled = false;
        myPlayerInstance.GetComponentInChildren<Weapon>().enabled = false;
    }

    void ExitGame()
    {
        Invoke("Exit", closeTime);
    }

    void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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