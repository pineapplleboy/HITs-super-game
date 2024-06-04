using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    [SerializeField] public string name;
    [SerializeField] public Sprite sprite;
    [SerializeField] public int maxCount;
}

public class Inventory : MonoBehaviour
{
    public Item[] items;
    public InventoryItem[] slots;

    private void Start()
    {
        slots[3].SetItem(items[0]);
    }
}