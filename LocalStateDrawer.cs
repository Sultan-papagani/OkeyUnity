using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
    ! Oyundaki UI elementlerini ayarla
    ! okey taşını göster, kalan taş sayısını güncelle vs
*/

public class LocalStateDrawer : MonoBehaviour
{
    public Image okey_tasi;
    public TextMeshProUGUI ortada_kalan_tas_sayisi;

    public void OkeyTasiniAyarla(int sayi, TileColor color)
    {
        okey_tasi.sprite = StaticHelper.GetSpriteFromData(sayi, color);
    }

    public void KalanTasSayisiUI(int kalan_sayi)
    {
        ortada_kalan_tas_sayisi.text = kalan_sayi.ToString();
    }
}
