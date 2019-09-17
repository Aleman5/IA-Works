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

    public void Serialize(Stream stream)
    {
        OnSerialize(stream);
    }

    public void Deserialize(Stream stream)
    {
        OnDeserialize(stream);
    }

    abstract public void OnSerialize(Stream stream);
    abstract public void OnDeserialize(Stream stream);
}