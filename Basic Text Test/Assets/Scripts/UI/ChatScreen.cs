using UnityEngine.UI;
using System.IO;
using UnityEngine;

/* Esta clase envía los packets recibidos a las clases que requieran esa informacion */
public class ChatScreen : MBSingleton<ChatScreen>
{
    public Text messages;
    public InputField inputMessage;

    override protected void Awake()
    {
        base.Awake();

        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        PacketManager.Instance.AddListenerById(ConnectionManager.Instance.clientId, OnReceivePacket);
    }

    void OnDisable()
    {
        PacketManager.Instance.RemoveListenerById(ConnectionManager.Instance.clientId);
    }

    void OnReceivePacket(uint packetId, ushort type, Stream stream)
    {
        switch ((UserPacketType)type)
        {
            case UserPacketType.Message:
                MessagePacket messagePacket = new MessagePacket();
                messagePacket.Deserialize(stream);

                if (NetworkManager.Instance.isServer)
                    MessageManager.Instance.SendString(messagePacket.payload, 0);

                messages.text += messagePacket.payload + System.Environment.NewLine;
                break;

            case UserPacketType.Position:
                PositionPacket positionPacket = new PositionPacket();
                positionPacket.Deserialize(stream);

                if (NetworkManager.Instance.isServer)
                    MessageManager.Instance.SendPosition(positionPacket.payload, 0);

                // Aca enviaria el payload a donde sea necesario
            break;
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
