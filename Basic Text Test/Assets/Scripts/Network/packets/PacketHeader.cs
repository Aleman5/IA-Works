using System.Net;
using UnityEngine;

public class PacketHeader : MonoBehaviour, ISerializePacket
{
    public int clientId;
    public uint objectId;
    public uint packetId;
    public byte[] payload;
    public IPEndPoint iPEndPoint;

    public ushort packetType { get; set; }

    public void Deserialize()
    {
        
    }

    public void Serialize()
    {
        
    }
}