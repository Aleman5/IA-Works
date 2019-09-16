using System.IO;
using System.Net;
using UnityEngine;

public class PacketHeader : MonoBehaviour, ISerializePacket
{
    public uint packetId;
    public uint clientId;
    public uint objectId;

    public ushort packetType { get; set; }

    public void Serialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        
        binaryWriter.Write(packetId);
        binaryWriter.Write(clientId);
        binaryWriter.Write(objectId);
        binaryWriter.Write(packetType);
    }

    public void Deserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);

        packetId   = binaryReader.ReadUInt32();        
        clientId   = binaryReader.ReadUInt32();        
        objectId   = binaryReader.ReadUInt32();        
        packetType = binaryReader.ReadUInt16();        
    }
}