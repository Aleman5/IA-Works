using UnityEngine;
using System.IO;

public abstract class GamePacket<P> : NetworkPacket<P>
{
    public GamePacket(ushort packetType, ushort userPacketType) : base(packetType, userPacketType) { }
}

public class MessagePacket : GamePacket<string>
{
    public MessagePacket() : base((ushort)PacketType.User, (ushort)UserPacketType.Message) { }

    public override void OnSerialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(payload);
    }

    public override void OnDeserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);
        payload = binaryReader.ReadString();
    }
}

public class PositionPacket : GamePacket<Vector3>
{
    public PositionPacket() : base((ushort)PacketType.User, (ushort)UserPacketType.Position) { }
    
    public override void OnSerialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(payload.x);
        binaryWriter.Write(payload.y);
        binaryWriter.Write(payload.z);
    }

    public override void OnDeserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);
        payload.x = binaryReader.ReadSingle();
        payload.y = binaryReader.ReadSingle();
        payload.z = binaryReader.ReadSingle();
    }
}