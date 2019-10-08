using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public struct Client
{
    public enum ClientState
    {
        NotConnected,
        Connected
    }

    public uint id;
    public ClientState state;
    public long clientSalt;
    public long serverSalt;
    public float timeStamp;
    public IPEndPoint ipEndPoint;

    public Client(IPEndPoint ipEndPoint, uint id, long clientSalt, long serverSalt, float timeStamp)
    {
        this.ipEndPoint = ipEndPoint;
        this.id = id;
        this.clientSalt = clientSalt;
        this.serverSalt = serverSalt;
        this.timeStamp = timeStamp;

        state = ClientState.NotConnected;
    }
}

/* Hace la parte de conexion del NetworkManager para alivianar su responsabilidad */
public class ConnectionManager : MBSingleton<ConnectionManager>
{
    public readonly Dictionary<uint, Client> clients = new Dictionary<uint, Client>();
    private readonly Dictionary<IPEndPoint, uint> ipToId = new Dictionary<IPEndPoint, uint>();
    private System.Action<bool> onConnect;
    private const float RESEND_REQUEST_RATE = 0.15f;

    public enum State
    {
        Disconnected,
        RequestingConnect,
        AnsweringChallenge,
        Connected
    }

    public State state { get; private set; }
    public uint clientId { get; private set; }
    public long clientSalt { get; private set; }
    public long serverSalt { get; private set; }

    override protected void Awake()
    {
        base.Awake();
        
        clientSalt = serverSalt = -1;
        state = State.Disconnected;
        PacketManager.Instance.onInternalPacketReceived += OnInternalPacketReceived;
    }

    public bool StartServer(int port)
    {
        if (NetworkManager.Instance.StartServer(port))
        {
            state = State.Connected;
            
            return true;
        }

        return false;
    }

    public void ConnectToServer(IPAddress ip, int port, System.Action<bool> onConnectCallback)
    {
        if (!NetworkManager.Instance.StartClient(ip, port))
        {
            if (onConnectCallback != null)
                onConnectCallback(false);
            
            return;
        }

        if (onConnectCallback != null)
            onConnect += onConnectCallback;

        state = State.RequestingConnect;
        clientSalt = (long)Random.Range(0, float.MaxValue);

        SendConnectionRequest();
    }

    uint AddClient(long clientSalt, long serverSalt, IPEndPoint ip)
    {
        if (!ipToId.ContainsKey(ip))
        {
            uint id = 0;
            do
            {
                id = (uint)Random.Range(1, uint.MaxValue);
            } while (clients.ContainsKey(id));

            Debug.Log("Adding client, ipAdress: " + ip.Address + " id: " + id);

            ipToId.Add(ip, id);

            clients.Add(id, new Client(ip, id, clientSalt, serverSalt, Time.realtimeSinceStartup));

            return id;
        }
        return 0;
    }

    void RemoveClient(IPEndPoint ip)
    {
        if (ipToId.ContainsKey(ip))
        {
            Debug.Log("Removing client: " + ip.Address);
            clients.Remove(ipToId[ip]);
        }
    }

    void SendConnectionRequest()
    {
        ConnectionRequestPacket packet = new ConnectionRequestPacket();
        packet.payload.clientSalt = clientSalt;
        PacketManager.Instance.SendPacketToServer(packet);
    }

    void SendChallengeRequest(uint clientId, long clientSalt, long serverSalt, IPEndPoint ipEndPoint)
    {
        ChallengeRequestPacket packet = new ChallengeRequestPacket();
        packet.payload.clientId = clientId;
        packet.payload.clientSalt = clientSalt;
        packet.payload.serverSalt = serverSalt;
        PacketManager.Instance.SendPacketToClient(packet, ipEndPoint);
    }

    void SendChallengeResponse(long clientSalt, long serverSalt)
    {
        ChallengeResponsePacket packet = new ChallengeResponsePacket();
        packet.payload.result = clientSalt ^ serverSalt; // Example: 110011 ^ 000011 = 110000
        PacketManager.Instance.SendPacketToServer(packet);
    }

    void SendConnected(uint clientId, IPEndPoint iPEndPoint)
    {
        ConnectedPacket packet = new ConnectedPacket();
        packet.payload.clientId = clientId;
        PacketManager.Instance.SendPacketToClient(packet, iPEndPoint);
    }

    void OnInternalPacketReceived(ushort packetType, IPEndPoint ipEndPoint, Stream stream)
    {
        switch ((PacketType)packetType)
        {
            case PacketType.ConnectionRequest:
                OnConnectionRequest(stream, ipEndPoint);
                break;
            case PacketType.ChallengeRequest:
                OnChallenge(stream, ipEndPoint);
                break;
            case PacketType.ChallengeResponse:
                OnChallengeResponse(stream, ipEndPoint);
                break;
            case PacketType.Connected:
                OnConnected(stream, ipEndPoint);
                break;
        }
    }

    void OnConnectionRequest(Stream stream, IPEndPoint ipEndPoint)
    {
        if (NetworkManager.Instance.isServer)
        {
            ConnectionRequestPacket packet = new ConnectionRequestPacket();
            packet.Deserialize(stream);

            long clientSalt = packet.payload.clientSalt;
            long serverSalt = -1;
            uint id = 0;
            
            if (ipToId.ContainsKey(ipEndPoint))
            {
                id = ipToId[ipEndPoint];
                serverSalt = clients[id].serverSalt;
            }
            else
            {
                serverSalt = (long)Random.Range(0, float.MaxValue);
                id = AddClient(clientSalt, serverSalt, ipEndPoint);
            }

            SendChallengeRequest(id, clientSalt, serverSalt, ipEndPoint);
        }
    }

    void OnChallenge(Stream stream, IPEndPoint iPEndPoint)
    {
        if (!NetworkManager.Instance.isServer)
        {
            state = State.AnsweringChallenge;
            
            ChallengeRequestPacket packet = new ChallengeRequestPacket();
            packet.Deserialize(stream);
            clientId = packet.payload.clientId;
            serverSalt = packet.payload.serverSalt;

            SendChallengeResponse(packet.payload.clientSalt, serverSalt);
        }
    }

    void OnChallengeResponse(Stream stream, IPEndPoint iPEndPoint)
    {
        if (NetworkManager.Instance.isServer)
        {
            ChallengeResponsePacket packet = new ChallengeResponsePacket();
            packet.Deserialize(stream);
            
            if (ipToId.ContainsKey(iPEndPoint))
            {
                Client client = clients[ipToId[iPEndPoint]];

                long result = client.clientSalt ^ client.serverSalt;

                if (result == packet.payload.result)
                {
                    client.state = Client.ClientState.Connected;
                    SendConnected(client.id, iPEndPoint);
                }
            }
        }
    }

    void OnConnected(Stream stream, IPEndPoint iPEndPoint)
    {
        if (!NetworkManager.Instance.isServer && state != State.Connected)
        {
            ConnectedPacket packet = new ConnectedPacket();
            packet.Deserialize(stream);

            if (packet.payload.clientId == clientId)
            {
                state = State.Connected;
                if (onConnect != null)
                {
                    onConnect(true);
                    onConnect = null;
                }
            }
        }
    }


    /* -----------------------  This is the Packet Sender Bombardment in case it didn´t reach objective  ----------------------- */
    float lastConnectionMsgTime;

    bool NeedToResend()
    {
        return state != State.Connected && state != State.Disconnected && Time.realtimeSinceStartup - lastConnectionMsgTime >= RESEND_REQUEST_RATE;
    }

    void Update()
    {
        if (!NetworkManager.Instance.isServer)
        {
            if (NeedToResend())
            {
                lastConnectionMsgTime = Time.realtimeSinceStartup;

                switch (state)
                {
                    case State.RequestingConnect:
                        SendConnectionRequest();
                        break;
                    case State.AnsweringChallenge:
                        SendChallengeResponse(clientSalt, serverSalt);
                        break;
                }
            }
        }
    }
}