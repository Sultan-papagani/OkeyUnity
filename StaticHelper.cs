using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    ! MonoBehaviour'a bağlantısı olmayan statik yardımcı sınıfı
    ! herhangi bir yerden Tile Sprite'a erişmek için.
*/

public static class StaticHelper
{
    public static Sprite GetSpriteFromData(int tile_number, TileColor color)
    {
        string cardColor = TileColor.GetName(typeof(TileColor), color);
        Sprite resource_sprite;
        if (tile_number != 0)
        {
            resource_sprite = Resources.Load<Sprite>($"Textures/{cardColor+"_textures"}/{cardColor}_{tile_number}");
        }
        else
        {   
            resource_sprite = Resources.Load<Sprite>("Textures/fake_joker");
        }
        return resource_sprite;
    }


    public static Sprite GetSpriteFromData(Tile tile)
    {
        string cardColor = TileColor.GetName(typeof(TileColor), tile.color);
        Sprite resource_sprite;
        if (tile.number != 0)
        {
            resource_sprite = Resources.Load<Sprite>($"Textures/{cardColor+"_textures"}/{cardColor}_{tile.number}");
        }
        else
        {   
            resource_sprite = Resources.Load<Sprite>("Textures/fake_joker");
        }
        return resource_sprite;
    }

}
