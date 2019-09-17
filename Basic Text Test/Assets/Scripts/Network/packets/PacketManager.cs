using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class PacketManager : Singleton<PacketManager>, IReceiveData
{
    Dictionary<uint, System.Action<uint, ushort, Stream>> onPacketReceived = new Dictionary<uint, System.Action<uint, ushort, Stream>>();
    uint currentPacketId = 0;

    void Awake()
    {
        NetworkManager.Instance.OnReceiveEvent += OnReceiveData;
    }

    public void AddListener(uint ownerId, System.Action<uint, ushort, Stream> callback)
    {
        if (onPacketReceived.ContainsKey(ownerId))
            onPacketReceived.Add(ownerId, callback);
    }

    public void RemoveListener(uint ownerId)
    {
        if (onPacketReceived.ContainsKey(ownerId))
            onPacketReceived.Remove(ownerId);
    }

    public void SendPacket(ISerializePacket packet, uint objectId, bool reliable = false)
    {
        byte[] bytes = Serialize(packet, objectId);

        if (NetworkManager.Instance.isServer)
            NetworkManager.Instance.Broadcast(bytes);
        else
            NetworkManager.Instance.SendToServer(bytes);
    }

    byte[] Serialize(ISerializePacket packet, uint objectId)
    {
        PacketHeader header = new PacketHeader();
        MemoryStream stream = new MemoryStream();

        header.packetId = currentPacketId++;
        header.senderId = NetworkManager.Instance.clientId;
        header.objectId = objectId;
        header.packetType = packet.packetType;

        header.Serialize(stream);
        packet.Serialize(stream);

        stream.Close();

        return stream.ToArray();
    }

    public void OnReceiveData(byte[] data, IPEndPoint ipEndpoint)
    {
        PacketHeader header = new PacketHeader();
        MemoryStream stream = new MemoryStream(data);

        header.Deserialize(stream);

        InvokeCallback(header.objectId, header.packetId, header.packetType, stream);

        stream.Close();
    }

    void InvokeCallback(uint objectId, uint packetId, ushort packetType, Stream stream)
    {
        if (onPacketReceived.ContainsKey(objectId))
            onPacketReceived[objectId].Invoke(packetId, packetType, stream);
    }
}
