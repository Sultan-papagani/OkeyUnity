using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    ! Tile Bilgisi
*/

public struct Tile
{
    public int number;
    public TileColor color;
    public GameObject GraphicObject;

    public string getasstring()
    {
        string data = "";
        if (color == TileColor.black){data += "B";}
        if (color == TileColor.blue){data += "M";}
        if (color == TileColor.yellow){data += "Y";}
        if (color == TileColor.red){data += "R";}
        if (color == TileColor.fake_okey){data += "J";}

        data += number.ToString();

        return data;
    }

    public void setfromstring(string x)
    {
        char ccolor = x[0];
        
        number = int.Parse(x.Substring(1));

        if (ccolor == 'B'){color = TileColor.black;}
        if (ccolor == 'M'){color = TileColor.blue;}
        if (ccolor == 'Y'){color = TileColor.yellow;}
        if (ccolor == 'R'){color = TileColor.red;}
        if (ccolor == 'J'){color = TileColor.fake_okey;}
    }
}

public enum TileColor
{
    black,
    blue,
    yellow,
    red,
    fake_okey
}
