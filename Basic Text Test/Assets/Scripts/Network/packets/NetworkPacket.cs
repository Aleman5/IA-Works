using System.Net;

public enum PacketType
{
    HandShake,
    HandShake_OK, 
    Error,
    Ping,
    Pong,
    Message,
}


public abstract class NetworkPacket<P>
{
    public P payload;

    public NetworkPacket(PacketType packetType)
    {

    }
}


    /*public PacketType type;
    public int clientId;
    public IPEndPoint ipEndPoint;
    public float timeStamp;
    public byte[] payload;

    public NetworkPacket(PacketType type, byte[] data, float timeStamp, int clientId = -1, IPEndPoint ipEndPoint = null)
    {
        this.type = type;
        this.timeStamp = timeStamp;
        this.clientId = clientId;
        this.ipEndPoint = ipEndPoint;
        this.payload = data;
    }*/