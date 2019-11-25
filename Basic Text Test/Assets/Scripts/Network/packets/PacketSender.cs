using System.IO;
using System.Net;
using UnityEngine;

public struct AckData
{
    public uint sequence;
    public byte[] packetBytes;

    public void Reset()
    {
        sequence = 0;
        packetBytes = null;
    }
}

public struct PacketToProcess
{
    public IPEndPoint iPEndPoint;
    public uint sequence;
    public byte[] bytes;

    public void Reset()
    {
        iPEndPoint = null;
        sequence = 0;
        bytes = null;
    }
}

public class PacketSender : MBSingleton<PacketSender>
{
    const int aSize = 512;

    public uint actualSequence = 0;
    public uint lastSequenceReceived = 0;
    public uint lastSequenceProcessed = 0;

    AckData[] seqs = new AckData[aSize];
    bool[] acks = new bool[aSize];
    PacketToProcess[] packetsToProcess = new PacketToProcess[aSize];

    override protected void Initialize()
    {
        base.Initialize();
        NetworkManager.Instance.OnReceiveEvent += OnReceiveData;
    }

    public void SendGamePacket(byte[] packetBytes, ref uint _actualSequence, ref AckData[] _seqs, IPEndPoint iPEndPoint, bool reliable = false)
    {
        if (reliable)
        {
            if (!NetworkManager.Instance.isServer)
            {
                uint index = actualSequence % aSize;
                seqs[index].sequence = actualSequence;
                seqs[index].packetBytes = packetBytes;
            }
            else
            {
                //uint index = clientObjective.actualSequence % aSize;
                //clientObjective.seqs[index].sequence = clientObjective.actualSequence;
                //clientObjective.seqs[index].packetBytes = packetBytes;

                uint index = _actualSequence % aSize;
                _seqs[index].sequence = _actualSequence;
                _seqs[index].packetBytes = packetBytes;

                /*using (var iterator = ConnectionManager.Instance.clients.GetEnumerator())
                {
                    while (iterator.MoveNext())
                    {
                        Client client = iterator.Current.Value;

                        uint index = client.actualSequence % aSize;
                        client.seqs[index].sequence = client.actualSequence;
                        client.seqs[index].packetBytes = packetBytes;
                    }
                }*/
            }
        }

        if (!NetworkManager.Instance.isServer)
            SendPacketToServer(packetBytes);
        else
            //SendPacketToClient(packetBytes, clientObjective.ipEndPoint);
            SendPacketToClient(packetBytes, iPEndPoint);
    }

    public void SendPacketToServer(byte[] bytes)
    {
        NetworkManager.Instance.SendToServer(bytes);
    }

    public void SendPacketToClient(byte[] bytes, IPEndPoint iPEndPoint)
    {
        Debug.Log("Sending: " + bytes.GetLength(0) + " to " + iPEndPoint);
        NetworkManager.Instance.SendToClient(bytes, iPEndPoint);
    }

    public void Broadcast(byte[] data)
    {
        using (var iterator = ConnectionManager.Instance.clients.GetEnumerator())
        {
            while (iterator.MoveNext())
            {
                SendPacketToClient(data, iterator.Current.Value.ipEndPoint);
            }
        }
    }

    public void OnReceiveData(byte[] data, IPEndPoint iPEndPoint)
    {
        MemoryStream stream = new MemoryStream(data);
        AckHeader ackHeader = new AckHeader();
        if (!NetworkManager.Instance.isServer) Debug.Log("Packet Received from: " + iPEndPoint);
        ackHeader.Deserialize(stream);

        if (ackHeader.reliable)
        {
            if (!NetworkManager.Instance.isServer)
            {
                if (ackHeader.sequence > lastSequenceProcessed)
                {
                    ManageAcks(ackHeader, ref seqs, ref acks, ref lastSequenceReceived);
                    ManageReliablePacket(data, iPEndPoint, ref ackHeader, ref packetsToProcess, ref lastSequenceProcessed);
                }
            }
            else
            {
                using (var iterator = ConnectionManager.Instance.clients.GetEnumerator())
                {
                    while (iterator.MoveNext())
                    {
                        Client client = iterator.Current.Value;

                        if (ackHeader.sequence > client.lastSequenceProcessed)
                        {
                            ManageAcks(ackHeader, ref client.seqs, ref client.acks, ref client.lastSequenceReceived);
                            ManageReliablePacket(data, iPEndPoint, ref ackHeader, ref client.packetsToProcess, ref client.lastSequenceProcessed);
                        }
                    }
                }
            }
        }
        else
        {
            PacketManager.Instance.OnReceiveData(data, iPEndPoint);
        }
    }

    private void ManageAcks(AckHeader ackHeader, ref AckData[] _seqs, ref bool[] _acks, ref uint _lastSequenceReceived)
    {
        _acks[ackHeader.sequence % aSize] = true;

        if (_lastSequenceReceived < ackHeader.sequence)
            _lastSequenceReceived = ackHeader.sequence;

        _seqs[ackHeader.ack % aSize].Reset();

        for (int i = (int)Mathf.Min(31, ackHeader.ack); i >= 0; i--)
        {
            if ((ackHeader.ackBits & (1 << i)) != 0)
                _seqs[(ackHeader.ack - i - 1) % aSize].Reset();
        }
    }

    private void ManageReliablePacket(byte[] data, IPEndPoint iPEndPoint, ref AckHeader ackHeader, ref PacketToProcess[] _packetsToProcess, ref uint _lastSequenceProcessed)
    {
        if (ackHeader.sequence <= _lastSequenceProcessed)
            return;

        Debug.Log("AckHeader.sequence: " + ackHeader.sequence + ", LastSequenceProcessed: " + _lastSequenceProcessed); // Probar esto de lado del server
        
        if (ackHeader.sequence - _lastSequenceProcessed == 1)
        {
            PacketManager.Instance.OnReceiveData(data, iPEndPoint);

            _lastSequenceProcessed++;

            uint id = (_lastSequenceProcessed + 1) % aSize;

            while (_packetsToProcess[id].sequence != 0)
            {
                PacketManager.Instance.OnReceiveData(_packetsToProcess[id].bytes, _packetsToProcess[id].iPEndPoint);
                _packetsToProcess[id].Reset();
                
                _lastSequenceProcessed++;
                if (++id >= aSize)
                    id %= aSize;

            }
        }
        else
        {
            uint id = ackHeader.sequence % aSize;

            _packetsToProcess[id].iPEndPoint = iPEndPoint;
            _packetsToProcess[id].sequence   = ackHeader.sequence;
            _packetsToProcess[id].bytes      = data;
        }
    }

    public void SetAckHeaderData(ref AckHeader ackHeader, ref uint _actualSequence, uint _lastSequenceReceived, ref bool[] _acks, bool reliable = false)
    {
        if (reliable)
        {
            if (!NetworkManager.Instance.isServer)
                SetAckHeaderData(ref ackHeader, ref actualSequence, lastSequenceReceived, ref acks);
            else
                SetAckHeaderData(ref ackHeader, ref _actualSequence, _lastSequenceReceived, ref _acks);
        }
    }

    private void SetAckHeaderData(ref AckHeader ackHeader, ref uint _actualSequence, uint _lastSequenceReceived, ref bool[] _acks)
    {
            ackHeader.reliable = true;
            ackHeader.sequence = ++_actualSequence;
            ackHeader.ack = _lastSequenceReceived % aSize;

            for (int i = 31; i >= 0 && i < _lastSequenceReceived; i--)
            {
                int id = ((int)(_lastSequenceReceived - i - 1)) % aSize;
            
                if (_acks[id])
                {
                    _acks[id] = false;
                    ackHeader.ackBits |= (uint)(1 << i);
                }

                //if (_seqs[id].sequence != 0)
                //    ackHeader.ackBits |= (uint)(1 << i);
            }
    }

    /* ---------------  Packet bombardier  --------------- */
    float lastConnectionMsgTime;

    const float RESEND_REQUEST_RATE = 0.03f;

    bool NeedToResend()
    {
        return Time.realtimeSinceStartup - lastConnectionMsgTime >= RESEND_REQUEST_RATE;
    }

    void Update()
    {
        if (NeedToResend())
        {
            lastConnectionMsgTime = Time.realtimeSinceStartup;

            if (!NetworkManager.Instance.isServer)
                SendToServer();
            else
                SendToClients();
        }
    }

    void SendToServer()
    {
        uint index = 0;
        
        do
        {
            uint id = (actualSequence - index) % aSize;
            if (seqs[id].sequence != 0)
                NetworkManager.Instance.SendToServer(seqs[id].packetBytes);
        } while (++index < 64);
    }

    void SendToClients()
    {
        using (var iterator = ConnectionManager.Instance.clients.GetEnumerator())
        {
            while (iterator.MoveNext())
            {
                uint index = 0;

                do
                {
                    Client client = iterator.Current.Value;
                    uint id = (client.actualSequence - index) % aSize;
                    if (client.seqs[id].sequence != 0)
                        NetworkManager.Instance.SendToClient(client.seqs[id].packetBytes, client.ipEndPoint);
                } while (++index < 64);
            }
        }
    }
}