using UnityEngine;

public class MessageManager : Singleton<MessageManager>
{
    override protected void Initialize()
    {
        base.Initialize();
    }

    public void SendString(string message, uint objectId, uint senderId)
    {
        MessagePacket packet = new MessagePacket(senderId);

        packet.payload = message;

        PacketManager.Instance.SendGamePacket(packet, objectId, senderId);
    }

    public void SendEntityInfo(Vector3 position, Quaternion rotation, uint objectId, uint senderId)
    {
        PositionPacket packet = new PositionPacket(senderId);

        packet.payload.pos = position;
        packet.payload.rot = rotation;

        PacketManager.Instance.SendGamePacket(packet, objectId, senderId);
    }

    public void SendScore(int score, uint objectId, uint senderId)
    {
        ScorePacket packet = new ScorePacket(senderId);

        packet.payload = score;

        PacketManager.Instance.SendGamePacket(packet, objectId, senderId);
    }

    public void SendDestroyInfo(uint objectId, uint senderId)
    {
        DestroyPacket packet = new DestroyPacket(senderId);

        packet.payload = true;

        PacketManager.Instance.SendGamePacket(packet, objectId, senderId);
    }
}