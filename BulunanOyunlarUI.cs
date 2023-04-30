using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulunanOyunlarUI : MonoBehaviour
{
    public NetworkClient client;

    public Transform ListObjectUI;
    public GameObject ServerButtonInstance;

    public void onServerFound()
    {
        for (int i=0; i<ListObjectUI.childCount; i++)
        {
            Destroy(ListObjectUI.GetChild(i).gameObject);
        }

        foreach(DiscoveryResultObject server in client.foundedServers)
        {
            DiscoveryResultObject cache = server;

            GameObject button = Instantiate(ServerButtonInstance, Vector3.zero, Quaternion.identity);

            button.transform.SetParent(ListObjectUI);
            button.transform.localScale = Vector3.one;

            ServerFoundButton btn = button.GetComponent<ServerFoundButton>();
            btn.Setup(server.ServerName, server.playerCount);
            btn.button.onClick.AddListener(() => HostaBaglan(cache));
        }
    }

    public void SunucuListesiniYenile()
    {
        for (int i=0; i<ListObjectUI.childCount; i++)
        {
            Destroy(ListObjectUI.GetChild(i).gameObject);
        }
        client.foundedServers.Clear();
    }

    public void HostaBaglan(DiscoveryResultObject s)
    {
        client.ConnectToHost(s);
    }
}
