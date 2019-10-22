using System.Collections.Generic;
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

public class PacketSender : MBSingleton<PacketSender>
{
    Queue<byte[]> bytesToSend = new Queue<byte[]>();



    /* ---------------  Packet bombardier  --------------- */
    float lastConnectionMsgTime;

    const float RESEND_REQUEST_RATE = 0.1f;

    bool NeedToResend()
    {
        return Time.realtimeSinceStartup - lastConnectionMsgTime >= RESEND_REQUEST_RATE;
    }

    void Update()
    {
        if (!NetworkManager.Instance.isServer)
        {
            if (NeedToResend())
            {
                lastConnectionMsgTime = Time.realtimeSinceStartup;

                foreach (byte[] bytes in bytesToSend)
                {
                    NetworkManager.Instance.SendToServer(bytes);
                }
            }
        }
    }
}