using System.IO;

public class PacketHeader : ISerializePacket
{
    public uint protocolId;
    public ushort packetType { get; set; }

    public void Serialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        
        binaryWriter.Write(protocolId);
        binaryWriter.Write(packetType);

        OnSerialize(stream);
    }

    public void Deserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);

        protocolId = binaryReader.ReadUInt32();
        packetType = binaryReader.ReadUInt16();

        OnDeserialize(stream);
    }

    virtual protected void OnSerialize(Stream stream) { }
    virtual protected void OnDeserialize(Stream stream) { }
}

public class AckHeader : ISerializePacket
{
    public bool reliable = false;
    public uint sequence = 0;
    public uint ack = 0;
    public uint ackBits = 0;

    public void Serialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);

        binaryWriter.Write(reliable);

        if (reliable)
        {
            binaryWriter.Write(sequence);
            binaryWriter.Write(ack);
            binaryWriter.Write(ackBits);
        }

        OnSerialize(stream);
    }

    public void Deserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);

        reliable = binaryReader.ReadBoolean();

        if (reliable)
        {
            sequence = binaryReader.ReadUInt32();
            ack = binaryReader.ReadUInt32();
            ackBits = binaryReader.ReadUInt32();
        }

        OnDeserialize(stream);
    }

    virtual protected void OnSerialize(Stream stream) { }
    virtual protected void OnDeserialize(Stream stream) { }
}

public class ProtocolHeader : ISerializePacket
{
    public uint myCRC;
    public int acksBytesCount = 0;
    public int packetBytesCount = 0;
    public byte[] acksBytes;
    public byte[] packetBytes;

    public void Serialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);

        binaryWriter.Write(myCRC);
        binaryWriter.Write(acksBytesCount);
        binaryWriter.Write(packetBytesCount);
        binaryWriter.Write(acksBytes);
        binaryWriter.Write(packetBytes);

        OnSerialize(stream);
    }

    public void Deserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);

        myCRC = binaryReader.ReadUInt32();
        acksBytesCount = binaryReader.ReadInt32();
        packetBytesCount = binaryReader.ReadInt32();
        if (acksBytesCount != 0)
            acksBytes = binaryReader.ReadBytes(acksBytesCount);
        packetBytes = binaryReader.ReadBytes(packetBytesCount);

        OnDeserialize(stream);
    }

    virtual protected void OnSerialize(Stream stream) { }
    virtual protected void OnDeserialize(Stream stream) { }
}