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

    // Connection variables
    public uint id;
    public ClientState state;
    public long clientSalt;
    public long serverSalt;
    public float timeStamp;
    public IPEndPoint ipEndPoint;

    // Packets variables
    public AckData[] seqs;
    public bool[] acks;
    public PacketToProcess[] packetsToProcess;
    public uint actualSequence;
    public uint lastSequenceReceived;
    public uint lastSequenceProcessed;

    public Client(IPEndPoint ipEndPoint, uint id, long clientSalt, long serverSalt, float timeStamp)
    {
        this.ipEndPoint = ipEndPoint;
        this.id = id;
        this.clientSalt = clientSalt;
        this.serverSalt = serverSalt;
        this.timeStamp  = timeStamp;

        this.seqs = new AckData[512];
        this.acks = new bool[512];
        this.packetsToProcess = new PacketToProcess[512];

        this.actualSequence = 0;
        this.lastSequenceReceived = 1;
        this.lastSequenceProcessed = 0;

        state = ClientState.NotConnected;
    }
}

/* Hace la parte de conexion del NetworkManager para alivianar su responsabilidad */
public class ConnectionManager : MBSingleton<ConnectionManager>
{
    public readonly Dictionary<uint, Client> clients = new Dictionary<uint, Client>();
    readonly Dictionary<IPEndPoint, uint> ipToId = new Dictionary<IPEndPoint, uint>();
    System.Action<bool> onConnect;
    const float RESEND_REQUEST_RATE = 0.03f;
    const int DECLINED = 0;

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
    public const int MAXUSERS = 2;

    override protected void Awake()
    {
        base.Awake();
        
        clientSalt = serverSalt = -1;
        state = State.Disconnected;
        PacketManager.Instance.onInternalPacketReceived += OnInternalPacketReceived;
    }

    public bool StartServer(int port, System.Action<bool> onConnectCallback)
    {
        if (NetworkManager.Instance.StartServer(port))
        {
            state = State.Connected;
            clientId = 0;
            
            if (onConnectCallback != null)
                onConnect += onConnectCallback;

            return true;
        }

        return false;
    }

    public bool ConnectToServer(IPAddress ip, int port, System.Action<bool> onConnectCallback)
    {
        if (!NetworkManager.Instance.StartClient(ip, port))
        {
            if (onConnectCallback != null)
                onConnectCallback(false);
            
            return false;
        }

        if (onConnectCallback != null)
            onConnect += onConnectCallback;

        state = State.RequestingConnect;
        clientSalt = (long)Random.Range(0, float.MaxValue);

        SendConnectionRequest();

        return true;
    }

    uint AddClient(long clientSalt, long serverSalt, IPEndPoint ip)
    {
        if (clients.Count < MAXUSERS - 1 && !ipToId.ContainsKey(ip))
        {
            uint id = 0;
            do
            {
                id = (uint)Random.Range(1, uint.MaxValue);
            } while (clients.ContainsKey(id));

            Debug.Log("Adding client, ipAdress: " + ip.Address + " id: " + id);

            ipToId.Add(ip, id);

            clients.Add(id, new Client(ip, id, clientSalt, serverSalt, Time.realtimeSinceStartup));

            onConnect(true); // It's here to know when a Client is trying to connect

            return id;
        }
        return DECLINED;
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

    void SendDeclinedRequest(IPEndPoint iPEndPoint)
    {
        DeclinedPacket packet = new DeclinedPacket();
        packet.payload.reason = "Max clients reached";
        PacketManager.Instance.SendPacketToClient(packet, iPEndPoint);
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
            case PacketType.DeclinedRequest:
                OnDeclinedRequest(stream, ipEndPoint);
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

            if (id == DECLINED)
            {
                SendDeclinedRequest(ipEndPoint);
            }
            else
            {
                SendChallengeRequest(id, clientSalt, serverSalt, ipEndPoint);
            }
        }
    }

    void OnDeclinedRequest(Stream stream, IPEndPoint iPEndPoint)
    {
        if (!NetworkManager.Instance.isServer)
        {
            state = State.Disconnected;
            clientSalt = serverSalt = -1;

            DeclinedPacket packet = new DeclinedPacket();
            packet.Deserialize(stream);

            Debug.LogError(packet.payload.reason);
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
        if (!NetworkManager.Instance.isServer && state != State.Connected)
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