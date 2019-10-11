using UnityEngine;
using System.IO;

public enum UserPacketType
{
    Message,
    Position,
    Score,
    Destroy,
    GameEvent,
    Count
}

public abstract class GamePacket<P> : NetworkPacket<P>
{
    public GamePacket(ushort packetType, ushort userPacketType, uint senderId) : base(packetType, userPacketType, senderId) { }
}

public class MessagePacket : GamePacket<string>
{
    public MessagePacket(uint senderId = 0) : base((ushort)PacketType.User, (ushort)UserPacketType.Message, senderId) { }

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
    public PositionPacket(uint senderId = 0) : base((ushort)PacketType.User, (ushort)UserPacketType.Position, senderId) { }
    
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
    public ScorePacket(uint senderId = 0) : base((ushort)PacketType.User, (ushort)UserPacketType.Score, senderId) { }
    
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

public class DestroyPacket : GamePacket<bool>
{
    public DestroyPacket(uint senderId = 0) : base((ushort)PacketType.User, (ushort)UserPacketType.Destroy, senderId) { }
    
    public override void OnSerialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(payload);
    }

    public override void OnDeserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);
        payload = binaryReader.ReadBoolean();
    }
}

public struct GameEvent
{
    public ushort gameEvent;
}

public class GameEventPacket : GamePacket<GameEvent>
{
    public GameEventPacket(uint senderId = 0) : base((ushort)PacketType.User, (ushort)UserPacketType.GameEvent, senderId) { }
    
    public override void OnSerialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(payload.gameEvent);
    }

    public override void OnDeserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);
        payload.gameEvent = binaryReader.ReadUInt16();
    }
}