using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class modemstatus : MonoBehaviour
{
    public TextMeshProUGUI text;

    void Start()
    {
        bool x = PlayerData.singleton.IsNetworkAvailable(0);
        if (x){text.text = "Modeme Bağlı";}
        else{text.text = "Bağlantı Yok";}
    }

}
