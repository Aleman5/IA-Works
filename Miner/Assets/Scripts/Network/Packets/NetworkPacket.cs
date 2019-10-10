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
    public uint senderId;
    public P payload;
    public ushort userPacketType { get; set; }
    public ushort packetType { get; set; }

    public NetworkPacket(ushort packetType, ushort userPacketType = ushort.MaxValue, uint senderId = 0)
    {
        this.packetType = packetType;

        if (userPacketType != ushort.MaxValue)
            this.userPacketType = userPacketType;
        
        if (senderId != 0)
            this.senderId = senderId;
    }

    public void Serialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(senderId);
        OnSerialize(stream);
    }

    public void Deserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);
        senderId = binaryReader.ReadUInt32();
        OnDeserialize(stream);
    }

    abstract public void OnSerialize(Stream stream);
    abstract public void OnDeserialize(Stream stream);
}