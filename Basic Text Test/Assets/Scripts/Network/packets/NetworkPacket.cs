using System.Net;
using System.IO;

public enum PacketType
{
    HandShake,
    HandShake_OK, 
    Error,
    Ping,
    Pong,
    Message,
    Position,
}


public abstract class NetworkPacket<P> : ISerializePacket
{
    public P payload;

    public ushort packetType { get; set; }

    public NetworkPacket(PacketType packetType)
    {
        this.packetType = (ushort)packetType;
    }

    abstract public void Serialize(Stream stream);
    abstract public void Deserialize(Stream stream);
}


    /*public PacketType type;
    public int clientId;
    public IPEndPoint ipEndPoint;
    public float timeStamp;
    public byte[] payload;

    public NetworkPacket(PacketType type, byte[] data, float timeStamp, int clientId = -1, IPEndPoint ipEndPoint = null)
    {
        this.type = type;
        this.timeStamp = timeStamp;
        this.clientId = clientId;
        this.ipEndPoint = ipEndPoint;
        this.payload = data;
    }*/