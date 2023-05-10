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
    public TimerUI startCountDown;
    public List<TextMeshProUGUI> oyuncu_textleri = new List<TextMeshProUGUI>();

    public DeckInteract deckInteract;

    public List<TileSlotKenar> atma_yerleri = new List<TileSlotKenar>();
    public TileSlotKenar atma_yeri;
    public TileSlotKenar alma_yeri;

    public GameObject bizimSiraIndicator;
    
    // * WaitPanelden, oyun paneline geçiş yaptık.
    public void GameStartEvent()
    {
        atma_yeri.OnTileDropped.AddListener(TasAtmaYerineTasAttik);

        // isimleri diz
        int our_index = 0;
        foreach(string player in client.player_list)
        {
            if (player == PlayerData.singleton.player_name)
            {
                break;
            }
            our_index++;
        }
          

        int[] index_list = PlayerYerlesmeSirasi.getSeqfromIndex(our_index);

        for(int i=0; i<3; i++)
        {
            // oyunda daha az insan oluyor test ederken...
            try
            {
                oyuncu_textleri[i].text = client.player_list[index_list[i]];
            }
            catch
            {

            }
        }

        // atma yerleri indexleri diz

        int[] index_idlist = PlayerYerlesmeSirasi.getSeqfromIndexID(our_index);
        for (int s=0; s<4; s++)
        {
            atma_yerleri[s].id = index_idlist[s];
        }

        startCountDown.SetTimer(5);
    }




    // * başlangıç taşları geldi. hepsini diz
    public void OnStarterTilesRecived(PlayerStarterTiles newtiles)
    {
        startCountDown.SetTimer(5); // bu sunucuda da beklenmeli.
        deckInteract.SpawnFromStarterTiles(newtiles);
    } 





    // * oyunda bizim sıramız, ne yapacağımıza karar ver.
    public void OnOurTurn()
    {
        bizimSiraIndicator.SetActive(true);
        // oyuna ilk biz başlamışız.. fazlayı at.
        if(deckInteract.GetTileCount() == 15)
        {
            PlayerData.singleton.playState = PlayState.TasAtmali;
            atma_yeri.SetArrowGraphic(true);
        }
        else
        {
            // o zaman önce çeker, sonra atarız
            PlayerData.singleton.playState = PlayState.TasCekmeli;
            alma_yeri.SetArrowGraphic(false);
        }

    }


    public void KenardanTasiCektikEvent(Tile tile)
    {
        client.KenardanTasCektik(alma_yeri.GetTopTileAsStr(), alma_yeri.id);
    }



    // * tas attığımızı bildiriyoruz
    public void TasAtmaYerineTasAttik(Tile tile)
    {
        client.KenaraTasAttik(tile.getasstring(), atma_yeri.id);
        bizimSiraIndicator.SetActive(false);
        PlayerData.singleton.playState = PlayState.Bos; // artık taşı attık sıramız bitti

        alma_yeri.ResetArrow();
        atma_yeri.ResetArrow();
    }


    public void CekilenTasiGoster(TasPacket tas)
    {
        if (tas.atma_yer_id == alma_yeri.id){return;} // bizim şimdi çektiğimiz taş...

        foreach(TileSlotKenar atma in atma_yerleri)
        {
            if (atma.id == tas.atma_yer_id)
            {
                atma.EnUsttekiTileyiCek();
                return;
            }
        }
    }



    // * sunucu atılan taşları bize gönderiyo
    public void AtilanTasiGoster(TasPacket tas)
    {
        if (tas.atma_yer_id == atma_yeri.id){return;} // bizim şimdi attığımız taş...

        foreach(TileSlotKenar atma in atma_yerleri)
        {
            if (atma.id == tas.atma_yer_id)
            {
                atma.TasEkle(tas.atilan_tas);
                return;
            }
        }
    }
}




public static class PlayerYerlesmeSirasi
{
    public static int[] getSeqfromIndex(int index)
    {
        int[] index_array = new int[3] {1,2,3};
        switch (index)
        {
            case 1:
                index_array = new int[] {2,3,0};
                break;
            case 2:
                index_array = new int[] {3,0,1};
                break;
            case 3:
                index_array = new int[] {0,1,2};
                break;
        }

        return index_array;
    }

    public static int[] getSeqfromIndexID(int index)
    {
        int[] index_array = new int[4] {0,1,2,3};
        switch (index)
        {
            case 1:
                index_array = new int[] {1,2,3,0};
                break;
            case 2:
                index_array = new int[] {2,3,0,1};
                break;
            case 3:
                index_array = new int[] {3,0,1,2};
                break;
        }

        return index_array;
    }
}
