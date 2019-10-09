using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MBSingleton<UIManager>
{
    [Header("Texts")]
    public Text waitingTxt;
    public Text scoreTxt;
    public Text enemyScoreTxt;

    uint objectId = 2;

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

    public void OnScoreChange(int score)
    {
        scoreTxt.text = "My score: " + score;
    }
}
