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
    public uint packetCRC = 0;
    public uint acksCRC = 0;
    public int acksBytesCount = 0;
    public int packetBytesCount = 0;
    public byte[] acksBytes;
    public byte[] packetBytes;

    public void Serialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);

        binaryWriter.Write(packetCRC);
        binaryWriter.Write(acksCRC);
        binaryWriter.Write(packetBytesCount);
        binaryWriter.Write(packetBytes);
        if (acksCRC != 0)
        {
            binaryWriter.Write(acksBytesCount);
            binaryWriter.Write(acksBytes);
        }

        OnSerialize(stream);
    }

    public void Deserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);

        packetCRC = binaryReader.ReadUInt32();
        acksCRC = binaryReader.ReadUInt32();
        packetBytesCount = binaryReader.ReadInt32();
        packetBytes = binaryReader.ReadBytes(packetBytesCount);
        if (acksCRC != 0)
        {
            acksBytesCount = binaryReader.ReadInt32();
            acksBytes = binaryReader.ReadBytes(acksBytesCount);
        }

        OnDeserialize(stream);
    }

    virtual protected void OnSerialize(Stream stream) { }
    virtual protected void OnDeserialize(Stream stream) { }
}