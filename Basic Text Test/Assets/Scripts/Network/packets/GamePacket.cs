using UnityEngine;
using System.IO;

public enum UserPacketType
{
    Message,
    Position,
    Shoot,
    Reload,
    Hit,
    ResetPosition,
    Score,
    Destroy,
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
    public Quaternion bodyRot;
    public bool killer;
    public uint clientdIdKilled;
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
        binaryWriter.Write(payload.bodyRot.x);
        binaryWriter.Write(payload.bodyRot.y);
        binaryWriter.Write(payload.bodyRot.z);
        binaryWriter.Write(payload.bodyRot.w);
        binaryWriter.Write(payload.killer);
        if (payload.killer)
            binaryWriter.Write(payload.clientdIdKilled);
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
        payload.bodyRot.x = binaryReader.ReadSingle();
        payload.bodyRot.y = binaryReader.ReadSingle();
        payload.bodyRot.z = binaryReader.ReadSingle();
        payload.bodyRot.w = binaryReader.ReadSingle();
        payload.killer = binaryReader.ReadBoolean();
        if (payload.killer)
            payload.clientdIdKilled = binaryReader.ReadUInt32();
    }
}

public struct ShootInfo
{
    public Vector3 pos;
    public Vector3 fwd;
    public byte damage;
}

public class ShootPacket : GamePacket<ShootInfo>
{
    public ShootPacket(uint senderId = 0) : base((ushort)PacketType.User, (ushort)UserPacketType.Shoot, senderId) { }
    
    public override void OnSerialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(payload.pos.x);
        binaryWriter.Write(payload.pos.y);
        binaryWriter.Write(payload.pos.z);
        binaryWriter.Write(payload.fwd.x);
        binaryWriter.Write(payload.fwd.y);
        binaryWriter.Write(payload.fwd.z);
        binaryWriter.Write(payload.damage);
    }

    public override void OnDeserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);
        payload.pos.x = binaryReader.ReadSingle();
        payload.pos.y = binaryReader.ReadSingle();
        payload.pos.z = binaryReader.ReadSingle();
        payload.fwd.x = binaryReader.ReadSingle();
        payload.fwd.y = binaryReader.ReadSingle();
        payload.fwd.z = binaryReader.ReadSingle();
        payload.damage = binaryReader.ReadByte();
    }
}

public class ReloadPacket : GamePacket<bool>
{
    public ReloadPacket(uint senderId = 0) : base((ushort)PacketType.User, (ushort)UserPacketType.Reload, senderId) { }
    
    public override void OnSerialize(Stream stream)
    {
    }

    public override void OnDeserialize(Stream stream)
    {
    }
}

public struct HitInfo
{
    public byte damage;
}

public class HitPacket : GamePacket<HitInfo>
{
    public HitPacket(uint senderId = 0) : base((ushort)PacketType.User, (ushort)UserPacketType.Hit, senderId) { }
    
    public override void OnSerialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(payload.damage);
    }

    public override void OnDeserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);
        payload.damage = binaryReader.ReadByte();
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