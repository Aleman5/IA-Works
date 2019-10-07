using UnityEngine;
using System.Net;
using System;

public class NetworkManager : MBSingleton<NetworkManager>, IReceiveData
{
    public IPAddress ipAddress { get; private set; }
    public int port { get; private set; }
    public bool isServer { get; private set; }

    public int TimeOut = 30;
    public Action<byte[], IPEndPoint> OnReceiveEvent;

    UdpConnection connection;

    public bool StartServer(int port)
    {
        try
        {
            isServer = true;
            this.port = port;
            connection = new UdpConnection(port, this);
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
        return true;
    }

    public bool StartClient(IPAddress ip, int port)
    {
        try
        {
            isServer = false;
            this.port = port;
            this.ipAddress = ip;
        
            connection = new UdpConnection(ip, port, this);
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
        return true;
    }

    public void OnReceiveData(byte[] data, IPEndPoint ip)
    {
        OnReceiveEvent?.Invoke(data, ip);
    }

    public void SendToServer(byte[] data)
    {
        connection.Send(data);
    }

    public void SendToClient(byte[] data, IPEndPoint ip)
    {
        connection.Send(data, ip);
    }

    void Update()
    {
        // We flush the data in main thread
        connection?.FlushReceiveData();
    }
}