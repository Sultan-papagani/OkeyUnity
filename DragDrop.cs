using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
    ! Bu komponent oyundaki Tile ları haraket ettirmemiz için
    ! Bırakıldıklarında parentAfterDrag'ı değiştiricek TileHolder yok ise
    ! eski pozisyonuna döner.
*/

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IInitializePotentialDragHandler
{
    Canvas canvas;
    RectTransform rectTransform;
    Image image;
    Color _alphaColor = new Color(0,0,0,0.4f);

    [HideInInspector] public Transform parentAfterDrag;

    [HideInInspector] public Transform actualParent; // not root

    void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanDrag()){return;}

        actualParent = transform.parent;

        // solumuzdaki tileslottan aldıysak TasAtmali Durumuna geç.
        if (eventData.pointerDrag.transform.parent.TryGetComponent(out TileSlotKenar kenar))
        {
            if (kenar.pickable && PlayerData.singleton.playState == PlayState.TasCekmeli)
            {
                PlayerData.singleton.playState = PlayState.TasAtmali;
                //client.KenardanTasCektik(alma_yeri.GetTopTileAsStr(), alma_yeri.id);
                PlayerData.singleton.KenardanCekilenTasEvent?.Invoke(GetComponent<TileActor>().tileData);
            }
            else
            {
                //return;
            }
        }

        image.color -= _alphaColor;
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CanDrag()){return;}

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!CanDrag()){return;}

        image.color += _alphaColor;
        image.raycastTarget = true;

        transform.SetParent(parentAfterDrag);
        transform.localPosition = Vector3.zero;

        // bırakıldığımız yer tileslot ise
        if (transform.parent.TryGetComponent(out TileSlotKenar k))
        {
            if (k.droppable)
            {
                // aynı TileHoldere geri dönmüşüz, yani bir yere atılamadık.
                if (transform.parent.Equals(actualParent))
                {
                    PlayerData.singleton.playState = PlayState.TasCekmeli;
                    //PlayerData.singleton.playState = PlayState.TasAtmali; // hala taşı atmalıyız
                }
                else // tileholdere konulduk.
                {
                    PlayerData.singleton.playState = PlayState.Bos; // işimiz bitti o zman
                }
            }
            else
            {
                // attığımız yer atabileceğimiz bir yer diil.
            }
        }

        actualParent = transform.parent;
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (!CanDrag()){return;}
        
        eventData.useDragThreshold = false;
    }


    // * taşları aldığımız yer, taş çekmeye izin vermiyorsa taşıma.
    public bool CanDrag()
    {

        // kenarlardaki yerlerde isek kurala göre alabiliriz.
        if (transform.parent.TryGetComponent(out TileSlotKenar kenar))
        {
            if (PlayerData.singleton.playState == PlayState.Bos) {return false;}
            return kenar.pickable;
        }

        // normal tileslotlarda istediğimiz gibi
        return true;
    }
    
}
