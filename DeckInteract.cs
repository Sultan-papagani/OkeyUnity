using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    ! istakaya tile spawnla.
    ! tüm istakaya tile_slot_list ile eriş.
*/

public class DeckInteract : MonoBehaviour
{
   // public static DeckInteract singleton;
    public GameObject TilePrefab;
    public List<TileSlot> tile_slot_list = new List<TileSlot>();

    void Start()
    {
        //singleton = this;
    }

    public int GetTileCount()
    {
        int i=0;
        foreach (TileSlot s in tile_slot_list)
        {
            if (s.transform.childCount > 0){i++;}
        }
        return i;
    }

    public void SpawnFromStarterTiles(PlayerStarterTiles newtiles)
    {
        int real_index = 0;
        foreach (string tile in newtiles.tiles)
        {
            
            TileColor tilecolor = TileColor.black;

            int number = 0;
            char color = tile[0];

            if (color == 'J'){tilecolor = TileColor.fake_okey;} // joker
            if (color == 'B'){tilecolor = TileColor.black;} // black
            if (color == 'M'){tilecolor = TileColor.blue;} // mavi blue
            if (color == 'Y'){tilecolor = TileColor.yellow;} // yellow
            if (color == 'R'){tilecolor = TileColor.red;} // red

            number = int.Parse(tile.Substring(1));
            

            SpawnTileWithCheck(number, tilecolor, real_index);
            real_index++;

            
        }
    }

    public bool SpawnTileWithCheck(int number, TileColor color, int real_index)
    {
        if (real_index < 23)
        {
            if (tile_slot_list[real_index].transform.childCount == 0)
            {
                SpawnTile(number, color, real_index);
                return true;
            } 
        }
        return false;
    }

    public void SpawnTile(int number, TileColor color, int real_index)
    {
        GameObject spawnedTile = Instantiate(TilePrefab, 
                                             Vector3.zero, 
                                             Quaternion.identity, tile_slot_list[real_index].transform);
        
        spawnedTile.transform.localPosition = Vector3.zero;

        TileActor spawnedActor = spawnedTile.GetComponent<TileActor>();
        spawnedActor.tileData.number = number;
        spawnedActor.tileData.color = color;


        spawnedActor.TileSprite.sprite = StaticHelper.GetSpriteFromData(number, color);
    }
}
