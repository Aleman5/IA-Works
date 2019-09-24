using System.Net;
using System.IO;

public enum PacketType
{
    ConnectionRequest,
    ChallengeRequest,
    ChallengeResponse,
    Connected,
    User,
    Count
}


public abstract class NetworkPacket<P> : ISerializePacket
{
    public P payload;

    public ushort userPacketType { get; set; }
    public ushort packetType { get; set; }

    public NetworkPacket(ushort packetType)
    {
        this.packetType = packetType;
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