using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
    !  Canvasdaki istakadaki Tile Slotları
    !  Tile bırakıldığında trandformunu, bu objeninkine ayarla.
    !  childCount'a göre dolu mu değil mi karar verir.
*/

public class TileSlot : MonoBehaviour, IDropHandler
{
    public int index;

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            eventData.pointerDrag.GetComponent<DragDrop>().parentAfterDrag = transform;
        }
    }

    public bool DoluMu()
    {
        return transform.childCount != 0;
    }
}
