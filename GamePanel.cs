using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
    bunu yine client odaklı yazmalıyız. 
*/

public class GamePanel : MonoBehaviour
{
    public NetworkClient client;

    public List<TextMeshProUGUI> oyuncu_textleri = new List<TextMeshProUGUI>();
    
    // * start yerine, oyun başlama eventi Client dan gelsin.
    // beynimin bütün hücreleri yok oldu
    public void GameStartEvent()
    {
        // bizim ismin indexi
        int our_index = 0;

        List<string> playerisimleri = new List<string>();
        playerisimleri = client.player_list;

        foreach(string player in playerisimleri)
        {
            if (player == PlayerData.singleton.player_name)
            {
                playerisimleri.Remove(player);
                break;
            }
            our_index++;
        }

        int textselector = 0;
        for (int i =0; i<playerisimleri.Count; i++)
        {
            if (our_index > playerisimleri.Count - 1){our_index = 0;}

            oyuncu_textleri[textselector].text = playerisimleri[our_index];

            our_index++;
            textselector++;
        }

        
        /*foreach(TextMeshProUGUI pt in oyuncu_textleri)
        {
            if (counter > client.player_list.Count - 1) {counter = 0;}
            if (counter == our_index) {break;}
            pt.text = client.player_list[counter];
        }*/

        /*
        // listede ismimizin yerini bul
        int our_index = 0;
        for (int i=0; i<4; i++){if (player_isimleri[i] == PlayerData.singleton.player_name){our_index = i; break;}}

        // ismimizden başlayarak listede ilerle
        // sona gelirsek başa sar.
        int track_head = our_index + 1;
        foreach(TextMeshProUGUI pt in oyuncu_textleri)
        {
            //if (track_head > 4){track_head = 0;}
            if (track_head > player_isimleri.Count-1){track_head = 0;}
            pt.text = player_isimleri[track_head];
            track_head++;
        }
        */

    }

    
}
