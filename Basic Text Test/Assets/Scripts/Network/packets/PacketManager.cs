using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;

public class PacketManager : Singleton<PacketManager>, IReceiveData
{
    //                  Header | IpEndPoint | Stream
    public System.Action<ushort, IPEndPoint, Stream> onInternalPacketReceived;
    //       ObjectId         PacketId | PacketType | Stream
    Dictionary<uint, System.Action<uint, ushort, Stream>> onGamePacketReceived = new Dictionary<uint, System.Action<uint, ushort, Stream>>();
    uint currentPacketId = 0;

    public void AddListenerByObjectId(uint objectId, System.Action<uint, ushort, Stream> callback)
    {
        if (!onGamePacketReceived.ContainsKey(objectId))
            onGamePacketReceived.Add(objectId, callback);
    }

    public void RemoveListenerByObjectId(uint objectId)
    {
        if (onGamePacketReceived.ContainsKey(objectId))
            onGamePacketReceived.Remove(objectId);
    }

    public void SendGamePacket<T>(NetworkPacket<T> packet, uint objectId, uint senderId, bool reliable = false)
    {
        if (!NetworkManager.Instance.isServer)
        {
            byte[] bytes = Serialize(packet, objectId, senderId, reliable);

            AckData[] d = new AckData[0];

            PacketSender.Instance.SendGamePacket(bytes, reliable);
        }
        else
        {
            using (var iterator = ConnectionManager.Instance.clients.GetEnumerator())
            {
                while (iterator.MoveNext())
                {
                    Client client = iterator.Current.Value;

                    byte[] bytes = Serialize(packet, objectId, senderId, reliable, client);

                    //PacketSender.Instance.SendGamePacket(bytes, reliable, client);
                    PacketSender.Instance.SendGamePacket(bytes, reliable, client);
                }
            }
        }

        currentPacketId++;
    }

    public void SendPacketToServer<T>(NetworkPacket<T> packet)
    {
        byte[] bytes = Serialize(packet);

        PacketSender.Instance.SendPacketToServer(bytes);
    }

    public void SendPacketToClient<T>(NetworkPacket<T> packet, IPEndPoint iPEndPoint)
    {
        byte[] bytes = Serialize(packet);

        PacketSender.Instance.SendPacketToClient(bytes, iPEndPoint);
    }

    byte[] Serialize<T>(NetworkPacket<T> packet, uint objectId = 0, uint senderId = 0, bool reliable = false, Client clientObjective = null)
    {
        MemoryStream stream = new MemoryStream();
        AckHeader ackHeader = new AckHeader();

        PacketSender.Instance.SetAckHeaderData(ref ackHeader, reliable, clientObjective);
        ackHeader.Serialize(stream);

        PacketHeader header = new PacketHeader();

        header.protocolId = 0;
        header.packetType = packet.packetType;
        header.Serialize(stream);

        if (packet.packetType == (ushort)PacketType.User)
        {
            UserPacketHeader userHeader = new UserPacketHeader();

            userHeader.packetType = packet.userPacketType;
            userHeader.packetId   = currentPacketId;
            userHeader.senderId   = senderId;
            userHeader.objectId   = objectId;
            userHeader.Serialize(stream);
        }

        packet.Serialize(stream);

        stream.Close();

        return stream.ToArray();
    }

    public void OnReceiveData(byte[] data, IPEndPoint ipEndpoint)
    {
        MemoryStream stream = new MemoryStream(data);
        AckHeader ackHeader = new AckHeader();

        ackHeader.Deserialize(stream);

        PacketHeader header = new PacketHeader();

        header.Deserialize(stream);
        
        if ((PacketType)header.packetType == PacketType.User)
        {
            UserPacketHeader userHeader = new UserPacketHeader();
            
            userHeader.Deserialize(stream);
            
            if (userHeader.senderId != ConnectionManager.Instance.clientId && onGamePacketReceived.ContainsKey(userHeader.objectId))
                onGamePacketReceived[userHeader.objectId].Invoke(userHeader.packetId, userHeader.packetType, stream);
        }
        else
        {
            onInternalPacketReceived.Invoke(header.packetType, ipEndpoint, stream);
        }

        stream.Close();
    }
}