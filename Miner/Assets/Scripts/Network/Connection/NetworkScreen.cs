using UnityEngine;
using UnityEngine.UI;
using System.Net;

public class NetworkScreen : MBSingleton<NetworkScreen>
{
    public Button connectBtn;
    public Button startServerBtn;
    public InputField addressInputField;
    public InputField portInputField;

    override protected void Awake()
    {
        base.Awake();

        connectBtn.onClick.AddListener(OnConnectBtnClick);
        startServerBtn.onClick.AddListener(OnStartServerBtnClick);
    }

    void OnConnectBtnClick()
    {
        IPAddress ipAddress = IPAddress.Parse(addressInputField.text);
        int port = System.Convert.ToInt32(portInputField.text);

        ConnectionManager.Instance.ConnectToServer(ipAddress, port, OnConnect);
    }

    void OnConnect(bool state)
    {
        Debug.Log("Connected: " + state);
        SwitchToNextScreen();
        //GameManager.Instance.UserConnected();
    }

    void OnStartServerBtnClick()
    {
        int port = System.Convert.ToInt32(portInputField.text);
        //if (ConnectionManager.Instance.StartServer(port, GameManager.Instance.StartGame))
        //{
        //    SwitchToNextScreen();
        //    UIManager.Instance.OnStartWaiting();
        //}
    }

    void SwitchToNextScreen()
    {
        //ChatScreen.Instance.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}