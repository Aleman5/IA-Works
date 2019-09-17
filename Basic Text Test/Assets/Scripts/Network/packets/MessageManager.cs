using UnityEngine;

public class MessageManager : Singleton<MessageManager>
{
    protected override void Initialize()
    {
        base.Initialize();
    }

    public void SendString(string message, uint objectId)
    {
        MessagePacket packet = new MessagePacket();

        packet.payload = message;

        PacketManager.Instance.SendPacket(packet, objectId);
    }
}
