using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using ParrelSync;
#endif

public class DebugText : MonoBehaviour
{
    public TextMeshProUGUI debugtext;

#if UNITY_EDITOR
    private void OnValidate() {
        if (ClonesManager.IsClone()){debugtext.text = "CLONE";}
        else{debugtext.text = "MAIN PROJECT";}
    }
#endif

    private void Start() {
        debugtext.text = "OKEY 1.1.5\nNetwork:LiteNetLib 1.0.1.1\nGamePanel Denemesi";
    }
}
