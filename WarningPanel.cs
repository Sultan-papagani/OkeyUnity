using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WarningPanel : MonoBehaviour
{
    public TextMeshProUGUI error_code;

    private void Awake() {
        error_code.text = "";
    }

    private void Start() {
        error_code.text = "";
    }

    public void CloseFromUI()
    {
        transform.SetAsFirstSibling(); // en arkaya at.
    }

    public void ClientDisconnect()
    {
        transform.SetAsLastSibling(); // en öne getir.
        error_code.text = "Sunucu Kapalı, atıldın veya oda dolu";
    }

    public void GeneralError(string error)
    {
        transform.SetAsLastSibling(); // en öne getir.
        error_code.text = error;
    }
}
