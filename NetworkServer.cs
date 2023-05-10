using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using System;

public class NetworkServer : MonoBehaviour, INetEventListener, INetLogger
{
    public List<Player> peer_list = new List<Player>();
    //public List<NetPeer> peer_list = new List<NetPeer>();

    private NetPacketProcessor _packetProcessor;
    private NetManager _netServer;
    private NetDataWriter _dataWriter;
    private readonly NetDataWriter _cachedWriter = new NetDataWriter();

    public OkeyGameManager okeyGameManager;

    public bool GameRunning;
    public int PlayersTurnIndex = 0; // sırası olan kişinin indexi.

    // * sunucuyu başlatır
    public void StartServer()
    {
        _dataWriter = new NetDataWriter();
        _netServer = new NetManager(this);

        NetDebug.Logger = this;

        _netServer.IPv6Mode = IPv6Mode.Disabled;
        _netServer.Start(PlayerData.singleton.hostip, PlayerData.singleton.hostip, PlayerData.singleton.hostport);

        _netServer.BroadcastReceiveEnabled = true;
        _netServer.UpdateTime = 15;
        _packetProcessor = new NetPacketProcessor();
    }



    // * sunucuyu durdur
    public void StopServer()
    {
        _netServer.Stop();
        NetDebug.Logger = null;

        StopAllCoroutines();
        CancelInvoke();
        
        peer_list.Clear();
        gameObject.SetActive(false);
    }


    
    void Update()
    {
        _netServer.PollEvents();
    }




    // * Bir peer sunucuya bağlandı
    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[SERVER] bir peer bağlandı " + peer.EndPoint);
        Player new_player = new Player();
        new_player.peer = peer;
        peer_list.Add(new_player);
    }




    // * Bağlantısız paket aldık
    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
        UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.Broadcast)
        {
            NetDataWriter resp = new NetDataWriter();
            DiscoveryResponse response = new DiscoveryResponse();

            response.ServerName = PlayerData.singleton.player_name;
            response.playerCount = peer_list.Count;
            resp.Put<DiscoveryResponse>(response);

            _netServer.SendUnconnectedMessage(resp, remoteEndPoint);
        }

        if (messageType == UnconnectedMessageType.BasicMessage)
        {

        }

    }




    // * peer bağlantı isteği
    public void OnConnectionRequest(ConnectionRequest request)
    {
        // * 4 kişi ise reddet.
        if (peer_list.Count >= 4)
        {
            request.RejectForce();
            return;
        }
        request.AcceptIfKey("sample_app");
    }




    // * peer sunucudan ayrıldı
    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[SERVER] peer bağlantisi koptu " + peer.EndPoint + ", bilgi: " + disconnectInfo.Reason);

        RemovePlayer(peer);
        UpdateClientsPlayerList();
    }




    // * oyuncuyu listeden kaldır
    public void RemovePlayer(NetPeer peer)
    {
        foreach(Player player in peer_list)
        {
            if (player.peer.Equals(peer))
            {
                peer_list.Remove(player);
                return;
            }
        }
    }




    // * peer'den Paket aldık
    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        byte packetType = reader.GetByte();
        PacketType pt = (PacketType) packetType;
        switch (pt)
        {
            case PacketType.ts_PlayerLoginPacket:
                PlayerLoginPacketRecived(reader, peer);
                break;

            case PacketType.ts_ClientTasAtti:
                OnClientTasAtti(reader);
                break;

            case PacketType.ts_ClientTasCekti:
                OnClientKenardanTasCekti(reader);
                break;

        }
    }

    // * çekilen taşı göster
    public void OnClientKenardanTasCekti(NetPacketReader reader)
    {
        TasPacket tas = new TasPacket();
        tas.Deserialize(reader);

        foreach(Player player in peer_list)
        {
            player.peer.Send(WriteSerializable(PacketType.tc_CekilenTasiGoster, tas), DeliveryMethod.ReliableOrdered);
        }
    }



    // * client taş attı. diğer clientlerde göster ve ilerlet
    public void OnClientTasAtti(NetPacketReader reader)
    {
        TasPacket tas = new TasPacket();
        tas.Deserialize(reader);

        foreach(Player player in peer_list)
        {
            player.peer.Send(WriteSerializable(PacketType.tc_AtilanTasiGoster, tas), DeliveryMethod.ReliableOrdered);
        }

        // belli bir süre sonra bir sonraki kişiye sırayı ver.
        Invoke(nameof(GivePlayerTurn), 1f);
    }




    // * peer kullanıcı bilgisini yolladı.
    public void PlayerLoginPacketRecived(NetDataReader reader, NetPeer peer)
    {
        PlayerLoginPacket p = new PlayerLoginPacket();
        p.Deserialize(reader);

        for (int i=0; i<peer_list.Count; i++)
        {
            if (peer_list[i].peer.Equals(peer))
            {
                peer_list[i].PlayerName = p.PlayerName;
            }
        }

        UpdateClientsPlayerList();
    }



    // * Wait Panelden oyunu başlatıyoruz. herkesi oyun paneline geçir ve süre sonra taşları dağıt
    public void StartGameFromWaitLobby()
    {
        StartGamePacket packet = new StartGamePacket();
        packet.test = true;

        foreach (Player player in peer_list)
        {
            player.peer.Send(WriteSerializable(PacketType.tc_StartGamePacket, packet), DeliveryMethod.ReliableOrdered);
        }

        okeyGameManager.StartGameEvent();

        Invoke(nameof(HerkesinTaslariVer), 6f);
    }




    // * oyunculara başlangıç taşlarını ver 14-15 olan
    public void HerkesinTaslariVer()
    {
        bool firstStart = true;

        foreach (Player player in peer_list)
        {
            string[] arr = okeyGameManager.RequestStartTilePayloadForOnePlayer(firstStart).ToArray();
            PlayerStarterTiles newtiles = new PlayerStarterTiles();
            newtiles.tiles = arr;

            player.peer.Send(WriteSerializable(PacketType.tc_PlayerStarterTiles, newtiles), DeliveryMethod.ReliableOrdered);

            firstStart = false;
        }

        //Invoke(nameof(GivePlayerTurn), 19f);
        GivePlayerTurn(); // şimdilik direk başlat
    }





    // * oyunu ilerlet, bir sonrakine sıra ver.
    public void GivePlayerTurn()
    {
        PlayerOynamaSirasi sira = new PlayerOynamaSirasi();
        sira.playername = peer_list[PlayersTurnIndex].PlayerName;

        peer_list[PlayersTurnIndex].peer.Send(WriteSerializable(PacketType.tc_PlayerOynamaSirasi, sira), DeliveryMethod.ReliableOrdered);

        PlayersTurnIndex++;

        if (PlayersTurnIndex > 3)
        {
            PlayersTurnIndex = 0;
        }
    }




    // * tüm clientlerin oyuncu listelerini güncelle
    public void UpdateClientsPlayerList()
    {
        UpdatePlayerList list = new UpdatePlayerList();
        
        List<string> player_namelist = new List<string>();
        for (int i=0; i<peer_list.Count; i++)
        {
            player_namelist.Add(peer_list[i].PlayerName);
        }

        list.player_list = player_namelist.ToArray();

        foreach(Player player in peer_list)
        {
            player.peer.Send(WriteSerializable(PacketType.tc_UpdatePlayerList, list), DeliveryMethod.ReliableOrdered);
        }
    }



    #region Gereksizler

    public void WriteNet(NetLogLevel level, string str, params object[] args)
    {
        Debug.LogFormat(str, args);
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
    {
        Debug.Log("[SERVER] hata " + socketErrorCode);
    }

    void OnDestroy()
    {
        NetDebug.Logger = null;
        if (_netServer != null) {_netServer.Stop();}
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
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


// TasPaketi
public struct TasPacket : INetSerializable
{
    public string atilan_tas{get; set;}
    public int atma_yer_id{get; set;}

    public void Deserialize(NetDataReader reader)
    {
        atilan_tas = reader.GetString();
        atma_yer_id = reader.GetInt();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(atilan_tas);
        writer.Put(atma_yer_id);
    }
}



public struct PlayerOynamaSirasi : INetSerializable
{
    public string playername {get; set;}

    public void Deserialize(NetDataReader reader)
    {
        playername = reader.GetString();
    }

    public void Serialize(NetDataWriter writer)
    {
        
        writer.Put(playername);
    }
}


public struct PlayerStarterTiles : INetSerializable
{
    public string[] tiles {get; set;}

    public void Deserialize(NetDataReader reader)
    {
        tiles = reader.GetStringArray();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.PutArray(tiles);
    }
}


// * peer oyuna katıldığında bilgisini sunucuya yollaması için
public struct PlayerLoginPacket : INetSerializable
{
    public string PlayerName { get; set; }
    public bool isHost { get; set; }

    public void Deserialize(NetDataReader reader)
    {
        PlayerName = reader.GetString();
        isHost = reader.GetBool();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(PlayerName);
        writer.Put(isHost);
    }
}


// * sunucunun discovery cevabi
public struct DiscoveryResponse : INetSerializable
{
    public string ServerName { get; set; }
    public int playerCount { get; set; }

    public void Deserialize(NetDataReader reader)
    {
        ServerName = reader.GetString();
        playerCount = reader.GetInt();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(ServerName);
        writer.Put(playerCount);
    }
}


public struct StartGamePacket : INetSerializable
{
    public bool test;

    public void Deserialize(NetDataReader reader)
    {
       test = reader.GetBool();
    }

    public void Serialize(NetDataWriter writer)
    {
       writer.Put(test);
    }
}




// * sunucunun, peerlere oyuncu listesini vermesi için
public struct UpdatePlayerList : INetSerializable
{
    public string[] player_list;

    public void Deserialize(NetDataReader reader)
    {
        player_list = reader.GetStringArray();
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.PutArray(player_list);
    }
}

[System.Serializable]
public class Player
{
    public NetPeer peer;
    public string PlayerName;
}

// * tüm paket tipleri
public enum PacketType : byte
{
    ts_PlayerLoginPacket,
    tc_DiscoveryResponse,
    tc_UpdatePlayerList,
    tc_StartGamePacket,
    tc_PlayerStarterTiles,
    tc_PlayerOynamaSirasi,
    ts_ClientTasAtti,
    tc_AtilanTasiGoster,
    ts_ClientTasCekti,
    tc_CekilenTasiGoster
}


// * client'da sunucu listesini göstermek için
public class DiscoveryResultObject
{
    public IPEndPoint ip;
    public string ServerName;
    public int playerCount;
}