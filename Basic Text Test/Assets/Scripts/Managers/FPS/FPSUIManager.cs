using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FPSUIManager : MBSingleton<FPSUIManager>
{
    [Header("Texts")]
    public Text waitingTxt;
    public Text scoreTxt;
    public Text enemyScoreTxt;
    public Text timerTxt;
    public Text finalStateTxt;
    public Image emptyBarImg;
    public Image healthBarImg;

    uint objectId = 41;

    void OnEnable()
    {
        PacketManager.Instance.AddListenerByObjectId(objectId, OnReceivePacket);
    }

    void OnDisable()
    {
        PacketManager.Instance.RemoveListenerByObjectId(objectId);
    }

    void OnReceivePacket(uint packetId, ushort type, Stream stream)
    {
        if (type == (ushort)UserPacketType.Score)
        {
            ScorePacket scorePacket = new ScorePacket();
            scorePacket.Deserialize(stream);

            if (NetworkManager.Instance.isServer)
                MessageManager.Instance.SendScore(scorePacket.payload, objectId, scorePacket.senderId);
            
            enemyScoreTxt.text = "Enemy score: " + scorePacket.payload;
        }
    }

    public void OnGameStart()
    {
        waitingTxt.enabled = false;
        scoreTxt.enabled = true;
        enemyScoreTxt.enabled = true;
        timerTxt.enabled = true;
        emptyBarImg.enabled = true;
        healthBarImg.enabled = true;
    }

    public void OnStartWaiting()
    {
        waitingTxt.enabled = true;
    }

    public void OnEnemyKilled(int kills)
    {
        scoreTxt.text = "My kills: " + kills;
    }

    public void OnDeath(int kills) // In case of having multiple clients, here, I must add a clientId. Also, this function should be named OnEnemyKillsChanged
    {
        enemyScoreTxt.text = "Enemy kills: " + kills;
    }

    public void OnTimeChange(float timeLeft)
    {
        string minutes = Mathf.Floor(timeLeft * 0.0167f).ToString("00");
        string seconds = Mathf.Floor(timeLeft % 60).ToString("00");

        timerTxt.text = minutes + ":" + seconds;
    }

    public void OnHealthChange(int healthAmount)
    {
        healthBarImg.fillAmount = healthAmount * 0.01f;
    }

    public void OnMatchFinished(bool win)
    {
        finalStateTxt.enabled = true;

        if (!win)
            finalStateTxt.text = "You lose!! :(";
    }
}