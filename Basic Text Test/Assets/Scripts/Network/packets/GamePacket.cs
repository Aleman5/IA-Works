using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public abstract class GamePacket<P> : NetworkPacket<P>
{
    public GamePacket(PacketType packetType) : base(packetType) { }
}

public class MessagePacket : GamePacket<string>
{
    public MessagePacket() : base(PacketType.Message) { }

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
    public PositionPacket() : base(PacketType.Position) { }
    
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