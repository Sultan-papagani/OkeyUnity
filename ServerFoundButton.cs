using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ServerFoundButton : MonoBehaviour
{
    public TextMeshProUGUI server_name;
    public TextMeshProUGUI server_player_count;
    public Button button;
    
    public void Setup(string playername, int count)
    {
        server_name.text = playername;
        server_player_count.text = count.ToString();
        if (count >= 4)
        {
            server_player_count.color = Color.red;
        }
        else
        {
            server_player_count.color = Color.green;
        }
    }
}
