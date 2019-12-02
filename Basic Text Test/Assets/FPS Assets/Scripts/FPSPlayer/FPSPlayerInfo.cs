using System.IO;
using UnityEngine;

public class FPSPlayerInfo : MonoBehaviour
{
    public Transform bodyTransform;

    uint objectId = 40;

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
        switch ((UserPacketType)type)
        {
            case UserPacketType.Position:
                PositionPacket positionPacket = new PositionPacket();
                positionPacket.Deserialize(stream);

                if (NetworkManager.Instance.isServer)
                    MessageManager.Instance.SendEntityInfo(positionPacket.payload.pos, positionPacket.payload.rot, positionPacket.payload.bodyRot, false, 0, objectId, positionPacket.senderId);

                transform.position = positionPacket.payload.pos;
                transform.rotation = positionPacket.payload.rot;
                transform.rotation = positionPacket.payload.bodyRot;

                if (positionPacket.payload.killer)
                    FPSGameManager.Instance.OnKill();
            break;

            case UserPacketType.Score:
                ScorePacket scorePacket = new ScorePacket();
            break;
        }
    }
}