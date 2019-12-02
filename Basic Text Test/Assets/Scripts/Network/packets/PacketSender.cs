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

    public void SendGamePacket(byte[] packetBytes, bool reliable = false, Client clientObjective = null)
    {
        MemoryStream stream = new MemoryStream();

        ProtocolHeader protocolHeader = new ProtocolHeader();

        protocolHeader.packetCRC = CRC32.ComputeChecksum(packetBytes);
        protocolHeader.packetBytes = packetBytes;
        protocolHeader.packetBytesCount = packetBytes.Length;

        if (reliable)
        {
            byte[] acksBytes = MakeAckBytes(clientObjective);

            protocolHeader.acksCRC = CRC32.ComputeChecksum(acksBytes);
            protocolHeader.acksBytes = acksBytes;
            protocolHeader.acksBytesCount = acksBytes.Length;
        }

        protocolHeader.Serialize(stream);

        stream.Close();

        byte[] finalBytes = stream.ToArray();

        if (reliable)
        {
            if (!NetworkManager.Instance.isServer)
            {
                uint index = actualSequence % aSize;
                seqs[index].sequence = actualSequence;
                seqs[index].packetBytes = finalBytes;
            }
            else
            {
                uint index = clientObjective.actualSequence % aSize;
                clientObjective.seqs[index].sequence = clientObjective.actualSequence;
                clientObjective.seqs[index].packetBytes = finalBytes;
            }
        }

        if (!NetworkManager.Instance.isServer)
            NetworkManager.Instance.SendToServer(finalBytes);
        else
            NetworkManager.Instance.SendToClient(finalBytes, clientObjective.ipEndPoint);
    }

    public void SendPacketToServer(byte[] packetBytes)
    {
        MemoryStream stream = new MemoryStream();

        ProtocolHeader protocolHeader = new ProtocolHeader();

        protocolHeader.packetCRC = CRC32.ComputeChecksum(packetBytes);
        protocolHeader.packetBytes = packetBytes;
        protocolHeader.packetBytesCount = packetBytes.Length;
        
        protocolHeader.Serialize(stream);

        stream.Close();

        NetworkManager.Instance.SendToServer(stream.ToArray());
    }

    public void SendPacketToClient(byte[] packetBytes, IPEndPoint iPEndPoint)
    {
        MemoryStream stream = new MemoryStream();

        ProtocolHeader protocolHeader = new ProtocolHeader();

        protocolHeader.packetCRC = CRC32.ComputeChecksum(packetBytes);
        protocolHeader.packetBytes = packetBytes;
        protocolHeader.packetBytesCount = packetBytes.Length;
        
        protocolHeader.Serialize(stream);

        stream.Close();

        NetworkManager.Instance.SendToClient(stream.ToArray(), iPEndPoint);
    }

    public void OnReceiveData(byte[] data, IPEndPoint iPEndPoint)
    {
        MemoryStream stream = new MemoryStream(data);

        ProtocolHeader protocolHeader = new ProtocolHeader();
        protocolHeader.Deserialize(stream);

        if (protocolHeader.packetCRC != CRC32.ComputeChecksum(protocolHeader.packetBytes))
            return;
        
        if (protocolHeader.acksCRC != 0 && protocolHeader.acksCRC == CRC32.ComputeChecksum(protocolHeader.acksBytes))
        {
            MemoryStream ackStream = new MemoryStream(protocolHeader.acksBytes);

            AckHeader ackHeader = new AckHeader();
        
            ackHeader.Deserialize(ackStream);
            
            if (ackHeader.reliable)
            {
                if (!NetworkManager.Instance.isServer)
                {
                    if (ackHeader.sequence > lastSequenceProcessed)
                    {
                        ManageAcks(ackHeader, ref seqs, ref acks, ref lastSequenceReceived);
                        ManageReliablePacket(protocolHeader.packetBytes, iPEndPoint, ref ackHeader, ref packetsToProcess, ref lastSequenceProcessed);
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
                                ManageReliablePacket(protocolHeader.packetBytes, iPEndPoint, ref ackHeader, ref client.packetsToProcess, ref client.lastSequenceProcessed);
                            }
                        }
                    }
                }
            }
            else
            {
                PacketManager.Instance.OnReceiveData(protocolHeader.packetBytes, iPEndPoint);
            }
        }
        else
        {
            PacketManager.Instance.OnReceiveData(protocolHeader.packetBytes, iPEndPoint);
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

        if (ackHeader.sequence - _lastSequenceProcessed == 1)
        {
            PacketManager.Instance.OnReceiveData(data, iPEndPoint);

            _lastSequenceProcessed++;

            uint id = (_lastSequenceProcessed + 1) % aSize;

            while (_packetsToProcess[id].sequence != 0)
            {
                PacketManager.Instance.OnReceiveData(_packetsToProcess[id].bytes, _packetsToProcess[id].iPEndPoint);
                _packetsToProcess[id].Reset();

                ++_lastSequenceProcessed;

                if (++id >= aSize)
                    id = (_lastSequenceProcessed + 1) % aSize;
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

    private byte[] MakeAckBytes(Client clientObjective)
    {
        MemoryStream stream = new MemoryStream();

        AckHeader ackHeader = new AckHeader();

        if (!NetworkManager.Instance.isServer)
            SetAckHeaderData(ref ackHeader, ref actualSequence, lastSequenceReceived, ref acks);
        else
            SetAckHeaderData(ref ackHeader, ref clientObjective.actualSequence, clientObjective.lastSequenceReceived, ref clientObjective.acks);

        ackHeader.Serialize(stream);

        stream.Close();

        return stream.ToArray();
    }

    private void SetAckHeaderData(ref AckHeader ackHeader, ref uint _actualSequence, uint _lastSequenceReceived, ref bool[] _acks)
    {
        ackHeader.reliable = true;
        ackHeader.sequence = ++_actualSequence;
        ackHeader.ack = _lastSequenceReceived;

        for (int i = 31; i >= 0 && i < _lastSequenceReceived; i--)
        {
            int id = ((int)(_lastSequenceReceived - i - 1)) % aSize;
            
            if (_acks[id])
            {
                _acks[id] = false;
                ackHeader.ackBits |= (uint)(1 << i);
            }
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