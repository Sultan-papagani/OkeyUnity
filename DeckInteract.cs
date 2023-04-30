using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    ! istakaya tile spawnla.
    ! tüm istakaya tile_slot_list ile eriş.
*/

public class DeckInteract : MonoBehaviour
{
    public static DeckInteract singleton;
    public GameObject TilePrefab;
    public List<TileSlot> tile_slot_list = new List<TileSlot>();

    void Start()
    {
        singleton = this;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // spawn test tile
            if (SpawnTileWithCheck(10, TileColor.yellow, 14))
            {
                Debug.Log("başarili");
            }
            else
            {
                Debug.Log("yer yok");
            }
        }
    }

    public bool SpawnTileWithCheck(int number, TileColor color, int real_index)
    {
        if (real_index < 23)
        {
            if (tile_slot_list[real_index].transform.childCount == 0)
            {
                SpawnTile(number, color, real_index);
                return true;
            } 
        }
        return false;
    }

    public void SpawnTile(int number, TileColor color, int real_index)
    {
        GameObject spawnedTile = Instantiate(TilePrefab, 
                                             Vector3.zero, 
                                             Quaternion.identity, tile_slot_list[real_index].transform);
        
        spawnedTile.transform.localPosition = Vector3.zero;

        TileActor spawnedActor = spawnedTile.GetComponent<TileActor>();
        spawnedActor.tileData.number = number;
        spawnedActor.tileData.color = color;

        /*
        string cardColor = TileColor.GetName(typeof(TileColor), color);
        Sprite resource_sprite;
        if (number != 0)
        {
            resource_sprite = Resources.Load<Sprite>($"Textures/{cardColor+"_textures"}/{cardColor}_{number}");
        }
        else
        {   
            resource_sprite = Resources.Load<Sprite>("Textures/fake_joker");
        }
        */

        spawnedActor.TileSprite.sprite = StaticHelper.GetSpriteFromData(number, color);
    }
}
