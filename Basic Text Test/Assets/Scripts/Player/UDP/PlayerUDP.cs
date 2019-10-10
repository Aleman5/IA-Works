﻿using System.IO;
using UnityEngine;

public class PlayerUDP : MonoBehaviour
{
    uint objectId = 1;

    void OnEnable()
    {
        PacketManager.Instance.AddListenerByObjectId(objectId, OnReceivePacket);
    }

    void OnDisable()
    {
        PacketManager.Instance.RemoveListenerByObjectId(objectId);
    }

    void OnReceivePacket(uint packetId, ushort type, Stream stream)
    {
        if (type == (ushort)UserPacketType.Position)
        {
            PositionPacket positionPacket = new PositionPacket();
            positionPacket.Deserialize(stream);

            if (NetworkManager.Instance.isServer)
                MessageManager.Instance.SendEntityInfo(positionPacket.payload.pos, positionPacket.payload.rot, objectId, positionPacket.senderId);

            transform.position = positionPacket.payload.pos;
            transform.rotation = positionPacket.payload.rot;
        }
    }
}