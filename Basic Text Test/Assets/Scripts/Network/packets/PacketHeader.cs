using System.IO;
using UnityEngine;

public class PacketHeader : MonoBehaviour, ISerializePacket
{
    public uint packetId;
    public uint senderId;
    public uint objectId;

    public ushort packetType { get; set; }

    public void Serialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        
        binaryWriter.Write(packetId);
        binaryWriter.Write(senderId);
        binaryWriter.Write(packetType);

        OnSerialize(stream);
    }

    public void Deserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);

        packetId   = binaryReader.ReadUInt32();
        senderId   = binaryReader.ReadUInt32();
        packetType = binaryReader.ReadUInt16();

        OnDeserialize(stream);
    }

    virtual protected void OnSerialize(Stream stream) { }
    virtual protected void OnDeserialize(Stream stream) { }
}