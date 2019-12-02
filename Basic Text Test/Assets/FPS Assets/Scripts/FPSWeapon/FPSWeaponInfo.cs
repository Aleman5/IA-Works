using System.IO;
using UnityEngine;

public class FPSWeaponInfo : MonoBehaviour
{
    PlayerHealth playerHealth;

    uint objectId = 42;

    void Start()
    {
        playerHealth = FPSGameManager.Instance.myPlayerInstance.GetComponent<PlayerHealth>();
    }

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
            case UserPacketType.Shoot:
                ShootPacket shootPacket = new ShootPacket();
                shootPacket.Deserialize(stream);

                if (NetworkManager.Instance.isServer)
                {
                    MessageManager.Instance.SendShootInfo(shootPacket.payload.pos, shootPacket.payload.fwd, shootPacket.payload.damage, objectId, shootPacket.senderId);

                    RaycastHit hit;                                                    

                    if (Physics.Raycast(shootPacket.payload.pos, shootPacket.payload.fwd, out hit))
                    {
                        if (hit.transform.tag == "Player")
                        {
                            playerHealth.TakeDamage(shootPacket.payload.damage, shootPacket.senderId);
                        }
                    }
                }
            break;
            case UserPacketType.Reload:

            break;
        }
    }
}