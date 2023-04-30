using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyCanvasManager : MonoBehaviour
{
    public NetworkServer SunucuObjesi;
    public NetworkClient ClientObjesi;

    public GameObject SunucuBulPaneli; // sunucuların çıktığı küçük liste
    public Transform LobbyPanel;       // GameWaitPanel yani ara lobi
    public Transform AnaMenu;          // intro ana menü işte

    public GameObject GamePanel;       // asıl oyun paneli
    public GamePanel oyunPanel;        // panelin komponenti

    public TMP_InputField isim_girme; // isim girme yeri.
    public WarningPanel warningPanel; // uyarı paneli.

    private void Start() {
        SunucuBulPaneli.SetActive(false);
        LobbyPanel.gameObject.SetActive(false);
    }

    public bool NameValidate()
    {
        // isim iki karaktere eşit/küçükse girme.
        if (isim_girme.text.Length > 0)
        {
            if (isim_girme.text.Length <= 2)
            {
                warningPanel.GeneralError("ismin en az 3 karakter olmalı.");
            }
        }
        else
        {
            warningPanel.GeneralError("bir isim yazmalısın.");
        }
        
        PlayerData.singleton.player_name = isim_girme.text;

        return isim_girme.text.Length <= 2;
    }

    public void SunucuBaslatButonu()
    {
        if (NameValidate()){return;}

        // sunucu objesini aç ve başlat
        SunucuObjesi.gameObject.SetActive(true);
        SunucuObjesi.StartServer();

        // client objesini aç ve başlat
        ClientObjesi.gameObject.SetActive(true);
        ClientObjesi.StartClientAndDiscovery(true);

        // bekleme yerine gir
        LobbyPanel.gameObject.SetActive(true);

        // intro paneli kapat
        AnaMenu.gameObject.SetActive(false);

        PlayerData.singleton.isHost = true;
    }

    public void SunucuAraButonu()
    {
        if (NameValidate()){return;}

        // sunucu bulma panelini aç
        SunucuBulPaneli.SetActive(true);

        // client objesini aç ve başlat
        ClientObjesi.gameObject.SetActive(true);
        ClientObjesi.StartClientAndDiscovery();

        PlayerData.singleton.isHost = false;
    }

    public void DisconnectLocalClient()
    {
        // * client disconnect

        if (PlayerData.singleton.isHost)
        {
            SunucuObjesi.StopServer();
        }

        // clienti durdur
        ClientObjesi.StopClient();

        // bekleme yerini kapat
        LobbyPanel.gameObject.SetActive(false);

        // sunucu bulma panelini kapat
        SunucuBulPaneli.SetActive(false);

        // intro panelini aç
        AnaMenu.gameObject.SetActive(true);

        PlayerData.singleton.isHost = false;
    }

    public void ClientSucsessConnected()
    {
        // * bağlandık. bekleme panelini aç

        // bekleme panelini aç
        LobbyPanel.gameObject.SetActive(true);

        // sunucu bulma panelini kapat
        SunucuBulPaneli.SetActive(false);

        // intro panelini kapat
        AnaMenu.gameObject.SetActive(false);

    }

    public void ServerConnectionLost()
    {
        // * oyundan atıldık

        // bekleme yerini kapat
        LobbyPanel.gameObject.SetActive(false);

        // sunucu bulma panelini kapat
        SunucuBulPaneli.SetActive(false);

        // intro panelini aç
        AnaMenu.gameObject.SetActive(true);
    }

    // host sonunda oyunu başlattı. oyun görünümüne geç.
    public void OnGameStartEvent()
    {
        LobbyPanel.gameObject.SetActive(false);
        SunucuBulPaneli.SetActive(false);
        AnaMenu.gameObject.SetActive(false);
        GamePanel.SetActive(true);
        oyunPanel.GameStartEvent();
    }
}
