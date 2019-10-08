using System.IO;

public abstract class InternalPacket<P> : NetworkPacket<P>
{
    public InternalPacket(ushort packetType) : base(packetType) { }
}


public struct ConnectionRequestData
{
    public long clientSalt;
}
public class ConnectionRequestPacket : InternalPacket<ConnectionRequestData>
{
    public ConnectionRequestPacket() : base((ushort)PacketType.ConnectionRequest) { }

    public override void OnSerialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(payload.clientSalt);
    }

    public override void OnDeserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);
        payload.clientSalt = binaryReader.ReadInt64();
    }
}


public struct DeclinedData
{
    public string reason;
}
public class DeclinedPacket : InternalPacket<DeclinedData>
{
    public DeclinedPacket() : base((ushort)PacketType.DeclinedRequest) { }

    public override void OnSerialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(payload.reason);
    }

    public override void OnDeserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);
        payload.reason = binaryReader.ReadString();
    }
}


public struct ChallengeRequestData
{
    public uint clientId;
    public long clientSalt;
    public long serverSalt;
}
public class ChallengeRequestPacket : InternalPacket<ChallengeRequestData>
{
    public ChallengeRequestPacket() : base((ushort)PacketType.ChallengeRequest) { }

    public override void OnSerialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(payload.clientId);
        binaryWriter.Write(payload.clientSalt);
        binaryWriter.Write(payload.serverSalt);
    }

    public override void OnDeserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);
        payload.clientId   = binaryReader.ReadUInt32();
        payload.clientSalt = binaryReader.ReadInt64();
        payload.serverSalt = binaryReader.ReadInt64();
    }
}


public struct ChallengeResponseData
{
    public long result;
}
public class ChallengeResponsePacket : InternalPacket<ChallengeResponseData>
{
    public ChallengeResponsePacket() : base((ushort)PacketType.ChallengeResponse) { }

    public override void OnSerialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(payload.result);
    }

    public override void OnDeserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);
        payload.result = binaryReader.ReadInt64();
    }
}


public struct ConnectedData
{
    public uint clientId;
}
public class ConnectedPacket : InternalPacket<ConnectedData>
{
    public ConnectedPacket() : base((ushort)PacketType.Connected) { }

    public override void OnSerialize(Stream stream)
    {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(payload.clientId);
    }

    public override void OnDeserialize(Stream stream)
    {
        BinaryReader binaryReader = new BinaryReader(stream);
        payload.clientId = binaryReader.ReadUInt32();
    }
}