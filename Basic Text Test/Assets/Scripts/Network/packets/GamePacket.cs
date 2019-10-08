using UnityEngine;
using System.IO;

public enum UserPacketType
{
    Message,
    Position,
    Score,
    Count
}

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

public struct EntityInfo
{
    public Vector3 pos;
    public Quaternion rot;
}

public class PositionPacket : GamePacket<EntityInfo>
{
    public PositionPacket() : base((ushort)PacketType.User, (ushort)UserPacketType.Position) { }
    
    public override void OnSerialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(payload.pos.x);
        binaryWriter.Write(payload.pos.y);
        binaryWriter.Write(payload.pos.z);
        binaryWriter.Write(payload.rot.x);
        binaryWriter.Write(payload.rot.y);
        binaryWriter.Write(payload.rot.z);
        binaryWriter.Write(payload.rot.w);
    }

    public override void OnDeserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);
        payload.pos.x = binaryReader.ReadSingle();
        payload.pos.y = binaryReader.ReadSingle();
        payload.pos.z = binaryReader.ReadSingle();
        payload.rot.x = binaryReader.ReadSingle();
        payload.rot.y = binaryReader.ReadSingle();
        payload.rot.z = binaryReader.ReadSingle();
        payload.rot.w = binaryReader.ReadSingle();
    }
}

public class ScorePacket : GamePacket<int>
{
    public ScorePacket() : base((ushort)PacketType.User, (ushort)UserPacketType.Score) { }
    
    public override void OnSerialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(payload);
    }

    public override void OnDeserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);
        payload = binaryReader.ReadInt32();
    }
}