using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Item
{
    [SerializeField] public string name;
    [SerializeField] public Sprite sprite;
    [SerializeField] public int maxCount;
    [SerializeField] public string type;

    public void LaunchAction(WorldGeneration wg)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = wg.tilemap.WorldToCell(worldPoint);
        if (type == "block")
        {
            wg.SetBlock(wg.world, wg.blocks.GetBlock(name), cellPosition.x, cellPosition.y);
        }
        if(type == "breaking_tool")
        {
            wg.DetectTilePressed(10);
        }
    }
}

public class Inventory : MonoBehaviour
{
    public Item[] items;
    public InventoryItem[] slots;
    private int currentSLot = 0;

    private void Start()
    {
        slots[3].SetItem(items[0]);
        slots[0].SetItem(items[1]);
        slots[1].SetItem(items[2]);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1)) currentSLot = 1;
        else if (Input.GetKey(KeyCode.Alpha2)) currentSLot = 2;
        else if (Input.GetKey(KeyCode.Alpha3)) currentSLot = 3;
        else if (Input.GetKey(KeyCode.Alpha4)) currentSLot = 4;
        else if (Input.GetKey(KeyCode.Alpha5)) currentSLot = 5;
        else if (Input.GetKey(KeyCode.Alpha6)) currentSLot = 6;
        else if (Input.GetKey(KeyCode.Alpha7)) currentSLot = 7;
        else if (Input.GetKey(KeyCode.Alpha8)) currentSLot = 8;
        else if (Input.GetKey(KeyCode.Alpha9)) currentSLot = 9;
        else if (Input.GetKey(KeyCode.Alpha0)) currentSLot = 0;

        if (Input.GetKey(KeyCode.Mouse0))
        {
            //slots[currentSLot].GetItem().LaunchAction(GameObject.FindGameObjectWithTag("World").GetComponent<WorldGeneration>());
        }
    }
}