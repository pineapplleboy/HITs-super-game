using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DragAndDropItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public InventorySlot oldSlot;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        oldSlot = transform.GetComponentInParent<InventorySlot>();
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (oldSlot.isEmpty)
            return;
        GetComponent<RectTransform>().position += new Vector3(eventData.delta.x, eventData.delta.y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (oldSlot.isEmpty)
            return;
        GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.75f);
        GetComponentInChildren<Image>().raycastTarget = false;
        transform.SetParent(transform.parent.parent);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (oldSlot.isEmpty)
            return;
        GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
        GetComponentInChildren<Image>().raycastTarget = true;

        transform.SetParent(oldSlot.transform);
        transform.position = oldSlot.transform.position;
        if (eventData.pointerCurrentRaycast.gameObject.name == "UIPanel")
        {
            GameObject itemObject = Instantiate(oldSlot.item.itemPrefab, player.position + Vector3.up + player.right * 5f, Quaternion.identity);
            itemObject.GetComponent<Item>().amount = oldSlot.amount;
            NullifySlotData();
        }
        else if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.GetComponent<InventorySlot>() != null)
        {
            ExchangeSlotData(eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.GetComponent<InventorySlot>());
        }

    }
    public void NullifySlotData()
    {
        oldSlot.item = null;
        oldSlot.amount = 0;
        oldSlot.isEmpty = true;
        oldSlot.iconGO.GetComponent<Image>().color = new Color(0, 0, 0, 255);
        oldSlot.iconGO.GetComponent<Image>().sprite = null;
        oldSlot.itemAmountText.text = "";
    }
    void ExchangeSlotData(InventorySlot newSlot)
    {
        ItemScriptableObject item = newSlot.item;
        int amount = newSlot.amount;
        bool isEmpty = newSlot.isEmpty;
        Sprite tempIcon = newSlot.iconGO.GetComponent<Image>().sprite;
        TMP_Text itemAmountText = newSlot.itemAmountText;

        newSlot.item = oldSlot.item;
        newSlot.amount = oldSlot.amount;
        if (oldSlot.isEmpty == false && oldSlot.item != item)
        {
            newSlot.SetIcon(oldSlot.iconGO.GetComponent<Image>().sprite);
            newSlot.itemAmountText.text = oldSlot.amount.ToString();
            newSlot.isEmpty = oldSlot.isEmpty;
        }
        else if (oldSlot.isEmpty == false && oldSlot.item == item)
        {
            newSlot.amount += amount;
            newSlot.itemAmountText.text = newSlot.amount.ToString();
            newSlot.SetIcon(oldSlot.iconGO.GetComponent<Image>().sprite);
            oldSlot.isEmpty = true;
            newSlot.isEmpty = false;
        }
        else 
        {
            newSlot.iconGO.GetComponent<Image>().color = new Color(0, 0, 0, 255);
            newSlot.iconGO.GetComponent<Image>().sprite = null;
            newSlot.itemAmountText.text = "";
            newSlot.isEmpty = oldSlot.isEmpty;
        }

        

        oldSlot.item = item;
        oldSlot.amount = amount;
        if (isEmpty == false && oldSlot.item.itemName != newSlot.item.itemName)
        {
            oldSlot.SetIcon(tempIcon);
            oldSlot.itemAmountText.text = amount.ToString();
            oldSlot.isEmpty = isEmpty;
        }
        else if (isEmpty == false && oldSlot.item.itemName == newSlot.item.itemName) 
        {
            oldSlot.item = null;
            oldSlot.amount = 0;
            oldSlot.iconGO.GetComponent<Image>().color = new Color(0, 0, 0, 255);
            oldSlot.iconGO.GetComponent<Image>().sprite = null;
            oldSlot.itemAmountText.text = "";
            oldSlot.isEmpty = true;
        }
        else
        {
            oldSlot.iconGO.GetComponent<Image>().color = new Color(0, 0, 0, 255);
            oldSlot.iconGO.GetComponent<Image>().sprite = null;
            oldSlot.itemAmountText.text = "";
            oldSlot.isEmpty = isEmpty;
        }

        
    }
}
