using UnityEngine;

public class MessageManager : Singleton<MessageManager>
{
    override protected void Initialize()
    {
        base.Initialize();
    }

    public void SendString(string message, uint objectId)
    {
        MessagePacket packet = new MessagePacket();

        packet.payload = message;

        PacketManager.Instance.SendGamePacket(packet, objectId);
    }

    public void SendEntityInfo(Vector3 position, Quaternion rotation, uint objectId)
    {
        PositionPacket packet = new PositionPacket();

        packet.payload.pos = position;
        packet.payload.rot = rotation;

        PacketManager.Instance.SendGamePacket(packet, objectId);
    }

    public void SendScore(int score, uint objectId)
    {
        ScorePacket packet = new ScorePacket();

        packet.payload = score;

        PacketManager.Instance.SendGamePacket(packet, objectId);
    }
}