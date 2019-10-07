using UnityEngine;
using UnityEngine.UI;
using System.Net;

public class NetworkScreen : MBSingleton<NetworkScreen>
{
    public Button connectBtn;
    public Button startServerBtn;
    public InputField portInputField;
    public InputField addressInputField;

    protected override void Initialize()
    {
        connectBtn.onClick.AddListener(OnConnectBtnClick);
        startServerBtn.onClick.AddListener(OnStartServerBtnClick);
    }

    void OnConnectBtnClick()
    {
        IPAddress ipAddress = IPAddress.Parse(addressInputField.text);
        int port = System.Convert.ToInt32(portInputField.text);

        ConnectionManager.Instance.ConnectToServer(ipAddress, port, OnConnect);
        
        SwitchToNextScreen();
    }

    void OnConnect(bool state)
    {
        Debug.Log("Connected: " + state);
        SwitchToNextScreen();
    }

    void OnStartServerBtnClick()
    {
        int port = System.Convert.ToInt32(portInputField.text);
        if (ConnectionManager.Instance.StartServer(port))
            SwitchToNextScreen();
    }

    void SwitchToNextScreen()
    {
        ChatScreen.Instance.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}