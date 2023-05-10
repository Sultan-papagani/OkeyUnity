using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameWaitPanel : MonoBehaviour
{
    public NetworkClient client;
    public NetworkServer server;
    public TextMeshProUGUI sunucu_kapat_butonu;

    public GameObject baslat_butonu_transformu;
    public GameObject client_Wait_text;

    //public List<TextMeshProUGUI> player_ui_tabla = new List<TextMeshProUGUI>();
    public List<PlayerWaitSlot> player_ui_tabla = new List<PlayerWaitSlot>();

    public WarningPanel warningPanel;

    void Start() 
    {
        updateUI();
    }

    void Awake() 
    {
        updateUI();
    }

    public void updateUI()
    {
        bool host = PlayerData.singleton.isHost;

        if (host)
        {sunucu_kapat_butonu.text = "ODAYI\nKAPAT";} 
        else{sunucu_kapat_butonu.text = "ODADAN\nAYRIL";}

        baslat_butonu_transformu.SetActive(host);
        client_Wait_text.SetActive(!host);

        foreach(PlayerWaitSlot slot in player_ui_tabla){slot.SetStatus("");}

        int i = 0;
        foreach(string playername in client.player_list)
        {
            player_ui_tabla[i].SetStatus(playername);
            i++;
        }
    }

    public void StartGameAsHost()
    {
        // kozmik ışıma bitleri çevirebilir
        if (PlayerData.singleton.isHost)
        {
            // kontrolü burda yazıyoz ki hata ekranının her yere referansını vermeyelim
            if (server.peer_list.Count > 0) // BUNU DÜZEELT
            {
                // masayı aç.
                // buranın client olmasına gerek yok ZATEN her şekilde sunucu bilgisayarındayız
                server.StartGameFromWaitLobby();
            }
            else
            {
                warningPanel.GeneralError("Oyuna başlamak için 4 kişi olmalı");
            }
        }
    }
}
