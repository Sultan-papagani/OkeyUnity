using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [HideInInspector]
    public List<Tile> tile_stack_middle = new List<Tile>();

    /// <summary>
    /// Oyundaki tüm taşları oluştur
    /// </summary>
    public void InitTileStack()
    {
        // tüm renklerden
        for (int i = 0; i < 4; i++)
        {
            // tüm sayılardan iki tane
            for (int s = 1; s < 13; s++)
            {
                Tile tile = new Tile();
                tile.color = (TileColor)i;
                tile.number = s;
                tile_stack_middle.Add(tile);
            }
        }

        // 2 tane joker
        Tile joker_tile = new Tile();
        joker_tile.color = TileColor.fake_okey;
        joker_tile.number = 0;
        tile_stack_middle.Add(joker_tile);
        tile_stack_middle.Add(joker_tile);
    }

    /// <summary>
    /// Tüm taşların listesinden taş çek.
    /// "Siyah 0" geliyorsa boş demektir
    /// </summary>
    public Tile PullTileFromStack()
    {
        if (tile_stack_middle.Count == 0)
        {
            // 0 "siyah" dönen boş oluyor
            Tile tile_empty = new Tile();
            return tile_empty;
        }

        int index = Random.Range(0, tile_stack_middle.Count - 1);
        Tile card = tile_stack_middle[index];
        tile_stack_middle.RemoveAt(index);

        Debug.Log($"num:{card.number} color:{card.color} kart ortadan çekildi, kalan taş: {tile_stack_middle.Count}");

        return card;
    }
}
