using System.IO;
using System.Net;
using UnityEngine;

/*
Data necesaria:
    - Cliente
        - Lista de packets que todavía no retornaron su acknowledge (estos se enviarán cada RESEND_REQUEST_RATE).
        - Diccionario de <PacketId, ListIndex> (para que, a la hora de recibir un acknowledge sepa qué packet eliminar de la lista).
    - Server
        - Lista de packets por cliente obtenidos ese frame.
        - Lista de packetId por cliente con el valor del packetId más alto.

Preguntas:
    - ¿El server de qué se encarga? Bettini había mencionado que el host debía ser el de mejor PC.
        Eso quiere decir que debe hacer procesos distintos a los clientes, aparte de enviar los packets a todos los clientes.
*/

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

public class PacketSender : MBSingleton<PacketSender>
{
    const int aSize = 512;

    public uint actualSequence = 0;
    public uint lastSequenceReceived = 1;

    AckData[] seqs = new AckData[aSize];
    bool[] acks = new bool[aSize];

    override protected void Initialize()
    {
        base.Initialize();
        NetworkManager.Instance.OnReceiveEvent += OnReceiveData;
    }

    public void SendGamePacket(byte[] packetBytes, bool reliable = false)
    {
        if (reliable)
        {
            int index = (int)(++actualSequence % aSize);
            seqs[index].sequence = actualSequence;
            seqs[index].packetBytes = packetBytes;
        }

        if (NetworkManager.Instance.isServer)
            Broadcast(packetBytes);
        else
            NetworkManager.Instance.SendToServer(packetBytes);
    }

    public void SendPacketToServer(byte[] bytes)
    {
        NetworkManager.Instance.SendToServer(bytes);
    }

    public void SendPacketToClient(byte[] bytes, IPEndPoint iPEndPoint)
    {
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

    public void OnAcknowledgesReceived(uint lastSeqReceived, int acks)
    {
        if (lastSeqReceived == 0)
            return;

        seqs[lastSeqReceived % aSize].Reset();
        
        int index = -1;
        int limit = aSize + (int)(lastSeqReceived - actualSequence);

        while (++index <= limit)
        {
            if (actualSequence - lastSeqReceived - index < 0)
                return;

            //if ((acks & index) != 0)

            if ((acks & (1 << index)) != 0)
                seqs[(lastSeqReceived - index - 1) % aSize].Reset();
        }
    }

    public void OnReceiveData(byte[] data, IPEndPoint ipEndpoint)
    {
        MemoryStream stream = new MemoryStream(data);
        AckHeader ackHeader = new AckHeader();

        ackHeader.Deserialize(stream);

        uint id = ackHeader.ack % aSize;

        if (ackHeader.reliable)
        {
            acks[ackHeader.sequence % aSize] = true;

            seqs[ackHeader.ack % aSize].Reset();

            for (int i = (int)Mathf.Min(31, ackHeader.ack); i >= 0; i--)
            {
                if ((ackHeader.ackBits & (1 << i)) != 0)
                    seqs[(ackHeader.ack - i - 1) % aSize].Reset();
            }
        }

        PacketManager.Instance.OnReceiveData(data, ipEndpoint);
    }

    public void SetAckHeaderData(ref AckHeader ackHeader, bool reliable)
    {
        ackHeader.reliable = reliable;
        if (reliable)
        {
            ackHeader.sequence = ++actualSequence;
        }
        
        ackHeader.ack = lastSequenceReceived % aSize;

        for (int i = 31; i >= 0 && i < lastSequenceReceived; i--)
        {
            int id = ((int)(lastSequenceReceived - i - 1)) % aSize;
           
            if (seqs[id].sequence != 0)
                ackHeader.ackBits |= (uint)(1 << i);
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
        int index = 0;
        
        do
        {
            if (seqs[index].sequence != 0)
                NetworkManager.Instance.SendToServer(seqs[index].packetBytes);
        } while (++index < aSize);
    }

    void SendToClients()
    {
        using (var iterator = ConnectionManager.Instance.clients.GetEnumerator())
        {
            while (iterator.MoveNext())
            {
                int index = 0;

                do
                {
                    Client client = iterator.Current.Value;
                    if (client.ackDatas[index].sequence != 0)
                        NetworkManager.Instance.SendToClient(client.ackDatas[index].packetBytes, client.ipEndPoint);
                } while (++index < aSize);
            }
        }
    }
}