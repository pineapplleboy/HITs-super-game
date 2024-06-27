using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class SingleSlot
{
    public string item;
    public int amount;

    public SingleSlot()
    {
        item = null;
        amount = 0;
    }

    public SingleSlot(InventorySlot slot)
    {
        if(slot.item != null)
        {
            this.item = slot.item.itemName;
            this.amount = slot.amount;
        }
        else
        {
            item = null;
            amount = 0;
        }
    }
}

public class InventorySlot : MonoBehaviour
{
    public bool isShop = false;
    public ItemScriptableObject item;
    public int amount;
    public bool isEmpty = true;
    public GameObject iconGO;
    public TMP_Text itemAmountText;
    private void Start()
    {
        iconGO = transform.GetChild(0).GetChild(0).gameObject;
        itemAmountText = transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
    }
    public void SetIcon(Sprite icon)
    {
        iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        iconGO.GetComponent<Image>().sprite = icon;
    }
}