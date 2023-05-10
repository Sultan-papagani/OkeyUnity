using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

using DG.Tweening;

public class TileSlotKenar : MonoBehaviour, IDropHandler
{
    public int id;

    public bool droppable;
    public bool pickable;

    public GameObject Gelen;
    public GameObject Giden;

    public GameObject TilePrefab;
    public Transform AnimasyonBaslaPoz;
    public Transform AnimasyonBitirPoz;

    [HideInInspector]
    public UnityEvent<Tile> OnTileDropped; // gamepanele düşen taşı bildir ki yollayam


    private void Start() {
        ResetArrow();
    }


    public void EnUsttekiTileyiCek()
    {
        if (transform.childCount >0)
        {
            Transform x = transform.GetChild(transform.childCount - 1);
            x.transform.position = transform.position;
            x.DOMove(AnimasyonBaslaPoz.position, 2f, false);
            
            StartCoroutine(DestroyTile(x));
        }
    }

    public IEnumerator DestroyTile(Transform t)
    {
        yield return new WaitForSecondsRealtime(2f);

        t.DOKill(true);
        Destroy(t.gameObject);
    }

    public string GetTopTileAsStr()
    {
        string tile = "";
        if (transform.childCount >0)
        {
            tile = transform.GetChild(transform.childCount - 1).GetComponent<TileActor>().tileData.getasstring();
        }
        
        return tile;
    }

    // * diğer clientlerin synclanması için
    public void TasEkle(string tas)
    {
        GameObject spawnedTile = Instantiate(TilePrefab, 
                                             Vector3.zero, 
                                             Quaternion.identity,
                                             transform);
        

        spawnedTile.transform.position = AnimasyonBaslaPoz.position;
        //spawnedTile.transform.localPosition = Vector3.zero;


        Tile tile = new Tile();
        tile.setfromstring(tas);

        TileActor spawnedActor = spawnedTile.GetComponent<TileActor>();
        spawnedActor.tileData.number = tile.number;
        spawnedActor.tileData.color = tile.color;

        spawnedActor.transform.SetAsLastSibling(); // en öne at

        spawnedTile.transform.DOMove(transform.position, 1f, false);
        //TweenObject.singleton.Tween(spawnedTile.transform, AnimasyonBaslaPoz, AnimasyonBitirPoz);

        spawnedActor.TileSprite.sprite = StaticHelper.GetSpriteFromData(tile);
    }





    // * taş atıyoruz buraya
    public void OnDrop(PointerEventData eventData)
    {
        // buraya taş konulmuyorsa dön
        if (!droppable){return;}

        // taş atmalı isek.
        if (PlayerData.singleton.playState == PlayState.TasAtmali)
        {
            
            // yerleştir.
            eventData.pointerDrag.GetComponent<DragDrop>().parentAfterDrag = transform;
            // internet üzerinden gönder.
            OnTileDropped?.Invoke(eventData.pointerDrag.GetComponent<TileActor>().tileData);
        }

    }


    public void SetArrowGraphic(bool gelen = true)
    {
        Gelen.SetActive(gelen);
        Giden.SetActive(!gelen);
    }

    public void ResetArrow()
    {
        Gelen.SetActive(false);
        Giden.SetActive(false);
    }

    
}
