using System.Net;
using System.Net.Sockets;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;
using UnityEngine.Events;

//using ParrelSync;

public class NetworkClient : MonoBehaviour, INetEventListener
{
    private NetManager _netClient;
    private NetPacketProcessor _packetProcessor;
    private readonly NetDataWriter _cachedWriter = new NetDataWriter();

    // * bulunan sunucular
    public List<DiscoveryResultObject> foundedServers = new List<DiscoveryResultObject>();
    public UnityEvent ServerFoundEvent;
    public UnityEvent ConnectedToServerEvent;
    public UnityEvent ServerConnectionLost;
    public UnityEvent OnPlayerListUpdated;

    public UnityEvent GameStartEvent;

    public WarningPanel warningPanel;

    // * player isimleri
    public List<string> player_list = new List<string>();

    void Update()
    {
        _netClient.PollEvents();
    }


    public void StopClient()
    {
        _netClient.Stop();
        player_list.Clear();
        foundedServers.Clear();

        CancelInvoke();
        StopAllCoroutines();

        gameObject.SetActive(false);
    }


    // * client + discovery başlat
    public void StartClientAndDiscovery(bool localHost = false)
    {
        _netClient = new NetManager(this);
        _netClient.UnconnectedMessagesEnabled = true;
        _netClient.UpdateTime = 15;
        _packetProcessor = new NetPacketProcessor();

        _netClient.Start();

        if (localHost)
        {
            _netClient.Connect(PlayerData.singleton.hostip, PlayerData.singleton.hostport, "sample_app");
            //_netClient.Connect("localhost", 5000, "sample_app");
            return;
        }

        InvokeRepeating(nameof(FindServerRepeating), 0f, 0.5f);

    }


    public void ConnectToHost(DiscoveryResultObject p)
    {
        if (p.playerCount >= 4)
        {
            warningPanel.GeneralError("Girmeye çalıştığın oda dolu");
            return;
        }
        _netClient.Connect(p.ip, "sample_app");
        ConnectedToServerEvent?.Invoke();
        // * herkes katılamayabilir. ona göre oyun paneline geçişmeli
    }


    // * Discovery paketi yolla.
    public void FindServerRepeating()

    {
        var peer = _netClient.FirstPeer;
        _netClient.SendBroadcast(new byte[] {1}, 5000);

        if (peer != null && peer.ConnectionState == ConnectionState.Connected)
        {
            Debug.Log("[client] sunucu bulundu. discovery kapatiliyor.");
            CancelInvoke(nameof(FindServerRepeating));
        }
    }


    // HOCAM MERHABAAAAAAA bunu okulda size göstereceğimi bildiğim için yazıyorum
    // tarih 30.04.2023 saat 03:44 
    // daha yapılacak çoooooook iş var.
    // umarım oyun çalışır
    

    // * sunucuya bağlandık (sunucuya LoginPacket yolluyoruz)
    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[CLIENT] sunucuya bağlandik: " + peer.EndPoint);
        PlayerLoginPacket login_info = new PlayerLoginPacket();

        login_info.PlayerName = PlayerData.singleton.player_name;
        login_info.isHost = PlayerData.singleton.isHost;

        peer.Send(WriteSerializable(PacketType.ts_PlayerLoginPacket, login_info), DeliveryMethod.ReliableOrdered);
    }




    // * sunucuya bağlanmadan mesaj aldık (sunucu bağlanma iznimize karar verdi)
    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        // DiscoveryResponse aldık
        if (messageType == UnconnectedMessageType.BasicMessage && _netClient.ConnectedPeersCount == 0)
        {
            DiscoveryResponse x = reader.Get<DiscoveryResponse>();
            foreach(DiscoveryResultObject s in foundedServers)
            {
                if (s.ip.Equals(remoteEndPoint))
                {
                    // ip adresleri aynı olsa bile kişi sayısı değişmiş olabilir.
                    if (s.playerCount == x.playerCount)
                    {
                        return;
                    }
                    else
                    {
                        // aynı oda ama kişi sayısı değişmiş (güncelle.)
                        s.playerCount = x.playerCount;
                        // yinede tüm listeyi güncellemek zorundayız.
                        ServerFoundEvent?.Invoke();
                        return;
                    }
                }
            }
            DiscoveryResultObject data = new DiscoveryResultObject();
            data.ip = remoteEndPoint;
            data.ServerName = x.ServerName;
            data.playerCount = x.playerCount;
            foundedServers.Add(data);
            ServerFoundEvent?.Invoke();
        }
    }




    // * bağlantı koptu
    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[CLIENT] Bağlanti koptu: " + disconnectInfo.Reason);
        ServerConnectionLost?.Invoke();
        player_list.Clear();
        foundedServers.Clear();
        // UI listesinide güncelle. genelde atıldığımızda sunucu yok oluyor.
        ServerFoundEvent?.Invoke();
    }




    // * sunucudan paket aldık
    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        byte packetType = reader.GetByte();

        /*if (packetType >= 2)
            return;*/

        PacketType pt = (PacketType) packetType;

        switch (pt)
        {
            case PacketType.tc_UpdatePlayerList:
                OnPlayerListChange(reader);
                break;

            case PacketType.tc_StartGamePacket:
            OnGameStartPacket(reader);
                break;
        }

        print(pt);
    }

    
    public void OnGameStartPacket(NetPacketReader reader)
    {
        StartGamePacket pckt = new StartGamePacket();
        pckt.Deserialize(reader);

        // game start eventi LobbyCanvasManager de çağrılıyor
        // ordan doğru GamePanel çağrılıyor..
        GameStartEvent?.Invoke();

        Debug.Log("oyuna başlıyoruz...");
    }


    // * oyuncu listesi değişti.
    public void OnPlayerListChange(NetPacketReader reader)
    {
        UpdatePlayerList newplayerlist = new UpdatePlayerList();
        newplayerlist.Deserialize(reader);

        player_list.Clear();
        foreach (string name in newplayerlist.player_list)
        {
            player_list.Add(name);
        }

        OnPlayerListUpdated?.Invoke();
    }


    #region  Gereksizler

    void OnDestroy()
    {
        if (_netClient != null) {_netClient.Stop();}
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
    {
        Debug.Log("[CLIENT] hata: " + socketErrorCode);
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {

    }

    public void OnConnectionRequest(ConnectionRequest request)
    {

    }

    /// <summary>
    /// struct gönderimi
    /// </summary>
    private NetDataWriter WriteSerializable<T>(PacketType type, T packet) where T : struct, INetSerializable
    {
        _cachedWriter.Reset();
        _cachedWriter.Put((byte) type);
        packet.Serialize(_cachedWriter);
        return _cachedWriter;
    }

    /// <summary>
    /// class gönderimi
    /// </summary>
    private NetDataWriter WritePacket<T>(PacketType type, T packet) where T : class, new()
    {
        _cachedWriter.Reset();
        //_cachedWriter.Put((byte) PacketType.tc_updatePlayerList);
        _cachedWriter.Put((byte) type);
        _packetProcessor.Write(_cachedWriter, packet);
        return _cachedWriter;
    }

    #endregion
}