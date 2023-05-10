using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    public TextMeshProUGUI text;

    int totaltime = 0;

    private void Start() {
        text.text = "";
    }

    public void SetTimer(int time)
    {
        totaltime = time;
        InvokeRepeating(nameof(Tick), 1f, 1f);
    }

    public void Tick()
    {
        if (totaltime > 0){text.text = totaltime.ToString(); totaltime--;}
        else
        {
            text.text = "";
            CancelInvoke(nameof(Tick));
        }
    }
}
