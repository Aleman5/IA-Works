using System.IO;

public enum PacketType
{
    ConnectionRequest,
    DeclinedRequest,
    ChallengeRequest,
    ChallengeResponse,
    Connected,
    User,
}

public abstract class NetworkPacket<P> : ISerializePacket
{
    public uint id;
    public P payload;
    public ushort userPacketType { get; set; }
    public ushort packetType { get; set; }

    public NetworkPacket(ushort packetType, ushort userPacketType = ushort.MaxValue)
    {
        this.packetType = packetType;

        if (userPacketType != ushort.MaxValue)
            this.userPacketType = userPacketType;
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