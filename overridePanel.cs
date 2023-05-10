using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class overridePanel : MonoBehaviour
{

    public TMP_InputField textipadress;
    public TMP_InputField textport;

    public GameObject panel;

    private void Start() 
    {
        textipadress.text = PlayerData.singleton.hostip;
        textport.text = PlayerData.singleton.hostport.ToString();
    }

    public void SetVaulesUI()
    {
        string ipadress = "localhost";
        int port = 5000;

        if (textipadress.text != "")
        {
            ipadress = textipadress.text;
        }
        if (textport.text != "")
        {
            if (!int.TryParse(textport.text, out port))
            {
                return; // port numarası hatalı
            }
        }

        PlayerData.singleton.hostip = ipadress;
        PlayerData.singleton.hostport = port;

        SetPanel();

    }

    public void SetPanel()
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void OtomatikDoldurUI()
    {
        string ip = PlayerData.singleton.GetIP();

        textipadress.text = ip;
        textport.text = "5000";

        PlayerData.singleton.hostip = ip;
        PlayerData.singleton.hostport = 5000;
    }

}


