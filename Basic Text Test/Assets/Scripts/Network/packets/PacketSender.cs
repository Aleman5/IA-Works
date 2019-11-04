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
}

public class PacketSender : MBSingleton<PacketSender>
{
    AckData[] ackDatas = new AckData[intSize];

    uint actualSequence = 0;

    const int intSize = 32;


    public void SendGamePacket(byte[] packetBytes, bool reliable = false)
    {
        if (reliable)
        {
            int index = (int)(++actualSequence % intSize);
            ackDatas[index].sequence = actualSequence;
            ackDatas[index].packetBytes = packetBytes;
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

    public void OnAcknowledgesReceived(uint lastPickedUp, int acks)
    {
        /*if (packetsWithoutAckId.Contains(lastPickedUp))
        {
            packetsWithoutAck.Remove(lastPickedUp);
            packetsWithoutAckId.Remove(lastPickedUp);
        }*/

        /* Aca deberia hacer lo del shifting leyendo 'acks' */

        /*do
        {
            if (acks)
            {

            }
        } while ();*/
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
            if (!NetworkManager.Instance.isServer)
                SendToServer();
            else
                SendToClients();
        }
    }

    void SendToServer()
    {
        lastConnectionMsgTime = Time.realtimeSinceStartup;

        int index = 0;
        
        do
        {
            if (ackDatas[index].sequence != 0)
                NetworkManager.Instance.SendToServer(ackDatas[index].packetBytes);
        } while (++index < intSize);
    }

    void SendToClients()
    {
        using (var iterator = ConnectionManager.Instance.clients.GetEnumerator())
        {
            while (iterator.MoveNext())
            {
                

                //SendPacketToClient(data, iterator.Current.Value.ipEndPoint);
            }
        }
    }
}