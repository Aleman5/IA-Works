using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FPSUIManager : MBSingleton<FPSUIManager>
{
    [Header("Texts")]
    public Text waitingTxt;
    public Text scoreTxt;
    public Text enemyScoreTxt;
    public Image barImg;

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
    }

    public void OnStartWaiting()
    {
        waitingTxt.enabled = true;
    }

    public void OnEnemyKilled(int kills)
    {
        scoreTxt.text = "My kills: " + kills;
    }

    public void OnEnemyKillsChanged(int kills) // In case of having multiple clients, here, I must add a clientId.
    {
        enemyScoreTxt.text = "Enemy kills: " + kills;
    }

    public void OnHealthChange(int healthAmount)
    {
        barImg.fillAmount = healthAmount * 0.01f;
    }
}