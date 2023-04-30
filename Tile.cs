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
}

public enum TileColor
{
    black,
    blue,
    yellow,
    red,
    fake_okey
}
