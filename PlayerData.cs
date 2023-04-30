using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public string player_name;
    public bool isHost;

    public static PlayerData singleton;

    private void Start() 
    {
        singleton = this;
    }
}
