using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    private Item item;

    public void SetItem(Item item)
    {
        this.item = item;
        UpdateSlot();
    }

    public Item GetItem()
    {
        return item;
    }

    public void UpdateSlot()
    {
        transform.Find("Image").GetComponent<Image>().sprite = item.sprite;
    }

    void Start()
    {
        UpdateSlot();
    }

    void Update()
    {
        
    }
}