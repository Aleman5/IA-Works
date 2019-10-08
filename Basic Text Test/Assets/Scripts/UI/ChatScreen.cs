using UnityEngine.UI;
using System.IO;
using UnityEngine;

/* Esta clase envía los packets recibidos a las clases que requieran esa informacion */
public class ChatScreen : MBSingleton<ChatScreen>
{
    public Text messages;
    public InputField inputMessage;

    uint objectId = 3;

    override protected void Awake()
    {
        base.Awake();

        gameObject.SetActive(false);
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
        if (type == (ushort)UserPacketType.Message)
        {
            MessagePacket messagePacket = new MessagePacket();
            messagePacket.Deserialize(stream);

            if (NetworkManager.Instance.isServer)
                MessageManager.Instance.SendString(messagePacket.payload, 0);

            messages.text += messagePacket.payload + System.Environment.NewLine;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (inputMessage && inputMessage.text != "")
            {
                if (NetworkManager.IsAvailable() && NetworkManager.Instance.isServer)
                    messages.text += inputMessage.text + System.Environment.NewLine;

                MessageManager.Instance.SendString(inputMessage.text, 0);

                inputMessage.ActivateInputField();
                inputMessage.Select();
                inputMessage.text = "";
            }
        }
    }
}
