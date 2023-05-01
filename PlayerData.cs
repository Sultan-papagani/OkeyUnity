using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Net.NetworkInformation;

public class PlayerData : MonoBehaviour
{
    public string player_name;
    public bool isHost;

    public string hostip = "localhost";
    public int hostport = 5000;

    public static PlayerData singleton;

    private void Start() 
    {
        DontDestroyOnLoad(gameObject);
        singleton = this;
    }

    public string GetIP()
    {
        string ip = GetLocalIPAddress();
        if (ip == "127.0.0.1")
        {
            ip = getIpadressSecondTime();
        }

        return ip;
    }

    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "127.0.0.1";
    }

    public string getIpadressSecondTime()
    {
        string localIP;
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("8.8.8.8", 65530);
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            localIP = endPoint.Address.ToString();
        }
        return localIP;
    }

    public bool IsNetworkAvailable(long minimumSpeed)
    {
        if (!NetworkInterface.GetIsNetworkAvailable())
            return false;

        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            // discard because of standard reasons
            if ((ni.OperationalStatus != OperationalStatus.Up) ||
                (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
                (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel))
                continue;

            // this allow to filter modems, serial, etc.
            // I use 10000000 as a minimum speed for most cases
            if (ni.Speed < minimumSpeed)
                continue;

            // discard virtual cards (virtual box, virtual pc, etc.)
            if ((ni.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0) ||
                (ni.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0))
                continue;

            // discard "Microsoft Loopback Adapter", it will not show as NetworkInterfaceType.Loopback but as Ethernet Card.
            if (ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
                continue;

            return true;
        }
        return false;
    }
}
