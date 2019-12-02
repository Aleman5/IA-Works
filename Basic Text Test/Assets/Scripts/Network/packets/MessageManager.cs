using UnityEngine;

public class MessageManager : Singleton<MessageManager>
{
    override protected void Initialize()
    {
        base.Initialize();
    }

    public void SendString(string message, uint objectId, uint senderId)
    {
        MessagePacket packet = new MessagePacket(senderId);

        packet.payload = message;

        PacketManager.Instance.SendGamePacket(packet, objectId, senderId, true);
    }

    public void SendEntityInfo(Vector3 position, Quaternion rotation, Quaternion bodyRotation, bool killer, uint clientId, uint objectId, uint senderId)
    {
        PositionPacket packet = new PositionPacket(senderId);

        packet.payload.pos = position;
        packet.payload.rot = rotation;
        packet.payload.bodyRot = bodyRotation;
        packet.payload.killer = killer;
        packet.payload.clientdIdKilled = clientId;

        PacketManager.Instance.SendGamePacket(packet, objectId, senderId, true);
    }

    public void SendShootInfo(Vector3 position, Vector3 forward, byte damage, uint objectId, uint senderId)
    {
        ShootPacket packet = new ShootPacket(senderId);

        packet.payload.pos = position;
        packet.payload.fwd = forward;
        packet.payload.damage = damage;

        PacketManager.Instance.SendGamePacket(packet, objectId, senderId, true);
    }

    public void SendHitInfo(byte damage, uint objectId, uint senderId)
    {
        HitPacket packet = new HitPacket(senderId);

        packet.payload.damage = damage;

        PacketManager.Instance.SendGamePacket(packet, objectId, senderId, true);
    }

    public void SendReload(uint objectId, uint senderId)
    {
        ReloadPacket packet = new ReloadPacket(senderId);

        packet.payload = false;

        PacketManager.Instance.SendGamePacket(packet, objectId, senderId, true);
    }

    public void SendScore(int score, uint objectId, uint senderId)
    {
        ScorePacket packet = new ScorePacket(senderId);

        packet.payload = score;

        PacketManager.Instance.SendGamePacket(packet, objectId, senderId, true);
    }

    public void SendDestroyInfo(uint objectId, uint senderId)
    {
        DestroyPacket packet = new DestroyPacket(senderId);

        packet.payload = true;

        PacketManager.Instance.SendGamePacket(packet, objectId, senderId, true);
    }
}