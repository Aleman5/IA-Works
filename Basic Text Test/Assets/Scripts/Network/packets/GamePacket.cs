using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public abstract class GamePacket<P> : NetworkPacket<P>
{
    protected NetworkManager nM = NetworkManager.Instance;
    public GamePacket(PacketType packetType) : base(packetType) { }
    public override void Serialize(Stream stream) { }
    public override void Deserialize(Stream stream) { }
}

[System.Serializable]
public class PositionPacket : GamePacket<Vector3>
{
    /*PositionPacket(Queue<Element> elements) // This will be for the RTS
    {

    }*/

    Queue<Vector2> positions;

    PositionPacket(Queue<Vector2> positions) : base(PacketType.Position)
    {
        //payload = positions;
    }
    
    public override void Serialize(Stream stream)
    {
        PacketHeader header = new PacketHeader();
        header.packetId = 0;
        header.clientId = nM.clientId;
        header.objectId = 0;
        header.packetType = (ushort)PacketType.Position;
        header.Serialize(stream);

        BinaryWriter binaryWriter = new BinaryWriter(stream);
        //binaryWriter.Write(payload);
    }

    public override void Deserialize(Stream stream)
    {
        
    }
}