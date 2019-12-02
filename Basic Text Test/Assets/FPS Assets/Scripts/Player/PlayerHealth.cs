using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int startingHealth = 100;
    [SerializeField] int currentHealth;
    [SerializeField] Slider healthSlider;

    AudioSource playerAudio;

    uint playerObjectId = 40;
    uint healthObjectId = 43;


    void Awake()
    {
        playerAudio = GetComponent<AudioSource>();
        currentHealth = startingHealth;
    }

    public void TakeDamage(int amount, uint senderId)
    {
        currentHealth -= amount;
        Debug.Log("On TakeDamage: " + ConnectionManager.Instance.clientId);

        playerAudio.Play();

        if (currentHealth <= 0)
            Death(senderId);

        FPSUIManager.Instance.OnHealthChange(currentHealth);
    }

    void Death(uint senderId)
    {
        currentHealth = startingHealth;

        Transform spawnPoint = FPSGameManager.Instance.spawnPoints[Random.Range(0, 4)];

        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        MessageManager.Instance.SendEntityInfo(transform.position, transform.rotation, transform.rotation, true, senderId, playerObjectId, ConnectionManager.Instance.clientId);
    }

    void OnEnable()
    {
        Debug.Log("Gola");
        PacketManager.Instance.AddListenerByObjectId(healthObjectId, OnReceivePacket);
    }

    void OnDisable()
    {
        PacketManager.Instance.RemoveListenerByObjectId(healthObjectId);
    }

    void OnReceivePacket(uint packetId, ushort type, Stream stream)
    {
        switch ((UserPacketType)type)
        {
            case UserPacketType.Hit:
                HitPacket hitPacket = new HitPacket();
                hitPacket.Deserialize(stream);
                Debug.Log("On HitPacket Received");
                TakeDamage(hitPacket.payload.damage, hitPacket.senderId);
            break;
        }
    }
}