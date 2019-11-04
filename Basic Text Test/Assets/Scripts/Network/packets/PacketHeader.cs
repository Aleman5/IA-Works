using System.IO;

public class PacketHeader : ISerializePacket
{
    public bool reliable;
    public uint protocolId;
    public ushort packetType { get; set; }

    public void Serialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        
        binaryWriter.Write(reliable);
        binaryWriter.Write(protocolId);
        binaryWriter.Write(packetType);

        OnSerialize(stream);
    }

    public void Deserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);

        reliable   = binaryReader.ReadBoolean();
        protocolId = binaryReader.ReadUInt32();
        packetType = binaryReader.ReadUInt16();

        OnDeserialize(stream);
    }

    virtual protected void OnSerialize(Stream stream) { }
    virtual protected void OnDeserialize(Stream stream) { }
}

public class AckHeader : ISerializePacket
{
    public uint sequence;
    public uint ack;
    public int  ackBits;

    public void Serialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);

        binaryWriter.Write(sequence);
        binaryWriter.Write(ack);
        binaryWriter.Write(ackBits);

        OnSerialize(stream);
    }

    public void Deserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);

        sequence = binaryReader.ReadUInt32();
        ack = binaryReader.ReadUInt32();
        ackBits = binaryReader.ReadInt32();

        OnDeserialize(stream);
    }

    virtual protected void OnSerialize(Stream stream) { }
    virtual protected void OnDeserialize(Stream stream) { }
}