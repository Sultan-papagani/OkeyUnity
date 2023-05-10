using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;

/*
    ! Oyundaki Tüm Taşların Listesi
    ! Taş PullTileFromStack ile bir taş alabilirsin.
    ! bu oyundaki ortada kalan taşlar değil TÜM taşlardır.
    ! bu sayede masada hep toplamda 106 taş kalmasını sağlayabiliriz.
*/

public class TileStack : MonoBehaviour
{
    /// <summary>
    /// Oyundaki Tüm taşların listesi.
    /// </summary>

    public List<Tile> tile_stack_middle = new List<Tile>();

    public int getTileStackCount()
    {
        return tile_stack_middle.Count;
    }

    /// <summary>
    /// Oyundaki tüm taşları oluştur
    /// </summary>
    public void InitTileStack()
    {
        // tüm renklerden
        for (int i = 0; i < 4; i++)
        {
            // tüm sayılardan iki tane
            for (int s = 1; s < 14; s++)
            {
                Tile tile = new Tile();
                tile.color = (TileColor)i;
                tile.number = s;
                tile_stack_middle.Add(tile);
                tile_stack_middle.Add(tile);
            }
        }

        // 2 tane joker
        Tile joker_tile = new Tile();
        joker_tile.color = TileColor.fake_okey;
        joker_tile.number = 0;
        tile_stack_middle.Add(joker_tile);
        tile_stack_middle.Add(joker_tile);

        Debug.Log($"dolduruldu: {tile_stack_middle.Count}");
    }

    /// <summary>
    /// Tüm taşların listesinden taş çek.
    /// "Siyah 0" geliyorsa boş demektir
    /// </summary>
    public Tile PullTileFromStack()
    {
        if (tile_stack_middle.Count == 0)
        {
   
            Tile tile_empty = new Tile();
            return tile_empty;
        }

        int index = RandomNumberGenerator.GetInt32(0, tile_stack_middle.Count - 1);
        Tile card = tile_stack_middle[index];
        tile_stack_middle.RemoveAt(index);

        return card;
    }



    /// <summary>
    /// Tüm taşların listesinden taş çek.
    /// "" geliyorsa boş demektir
    /// </summary>
    public string PullTileFromStackString()
    {

        if (tile_stack_middle.Count == 0)
        {
            // boş dönen boş
            return "";
        }

        int index = Random.Range(0, tile_stack_middle.Count - 1);
        Tile card = tile_stack_middle[index];
        tile_stack_middle.RemoveAt(index);

        return card.getasstring();
    }
}
