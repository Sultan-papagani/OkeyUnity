using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerWaitSlot : MonoBehaviour
{
    public TextMeshProUGUI txt_player;
    public Image status_sprite;

    public Sprite ok_sprite;
    public Sprite red_sprite;

    public void SetStatus(string playername)
    {
        if (playername == "")
        {
            // kimse yok
            txt_player.text = "BOŞ";
            status_sprite.sprite = ok_sprite;
        }
        else
        {
            txt_player.text = playername;
            status_sprite.sprite = red_sprite;
        }
    }
}
