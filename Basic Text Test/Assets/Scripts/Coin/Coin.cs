using System.IO;
using UnityEngine;

public class Coin : MonoBehaviour
{
    uint objectId = 20;

    public void SetObjectId(uint objectIdIncrement)
    {
        if (objectId != 20)
            PacketManager.Instance.RemoveListenerByObjectId(objectId);

        objectId += objectIdIncrement;
        PacketManager.Instance.AddListenerByObjectId(objectId, OnReceivePacket);
    }

    void OnDisable()
    {
        PacketManager.Instance.RemoveListenerByObjectId(objectId);
    }

    void OnReceivePacket(uint packetId, ushort type, Stream stream)
    {
        if (type == (ushort)UserPacketType.Destroy)
        {
            DestroyPacket destroyPacket = new DestroyPacket();
            destroyPacket.Deserialize(stream);

            if (NetworkManager.Instance.isServer)
                MessageManager.Instance.SendDestroyInfo(objectId, destroyPacket.senderId);

            Destroy(gameObject);
        }
    }

    public void Touched()
    {
        MessageManager.Instance.SendDestroyInfo(objectId, ConnectionManager.Instance.clientId);
    }
}
