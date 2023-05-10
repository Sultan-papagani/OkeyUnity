using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// * server side.

public class OkeyGameManager : MonoBehaviour
{
    public TileStack tileStack;

    // * server
    public void StartGameEvent()
    {
        tileStack.InitTileStack();
    }

    public List<string> RequestStartTilePayloadForOnePlayer(bool starter = false)
    {
        List<string> tiles = new List<string>();

        for (int i=0; i<14; i++)
        {
            tiles.Add(tileStack.PullTileFromStackString());
        }

        // 15 ver
        if (starter){tiles.Add(tileStack.PullTileFromStackString());}

        return tiles;
    }
}
