using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public GameObject Panel;
    public Transform InventoryPanel;
    public Transform QuickslotPanel;
    public List<InventorySlot> slots = new List<InventorySlot>();
    public List<InventorySlot> quickSlots = new List<InventorySlot>();
    public bool isOpened;
    void Start()
    {
        for (int i = 0; i < InventoryPanel.childCount; i++)
        {
            if (InventoryPanel.GetChild(i).GetComponent<InventorySlot>() != null)
            {
                slots.Add(InventoryPanel.GetChild(i).GetComponent<InventorySlot>());
            }
        }
        for (int i = 0; i < QuickslotPanel.childCount; i++)
        {
            if (QuickslotPanel.GetChild(i).GetComponent<InventorySlot>() != null)
            {
                quickSlots.Add(QuickslotPanel.GetChild(i).GetComponent<InventorySlot>());
            }
        }
        Panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isOpened = !isOpened;
            if (isOpened)
            {
                Panel.SetActive(true);
            }
            else
            {
                Panel.SetActive(false);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Item"))
        {
            Item itemPickup = other.gameObject.GetComponent<Item>();
            if (itemPickup != null)
            {
                AddItem(itemPickup.item, itemPickup.amount);
                Destroy(other.gameObject);
            }
        }
    }
    public void AddItem(ItemScriptableObject _item, int _amount)
    {
        if (_item == null)
        {
            return;
        }
        foreach (InventorySlot quickSlot in quickSlots)
        {
            if (quickSlot.item == _item && quickSlot.amount < _item.maximumAmount)
            {
                quickSlot.amount += _amount;
                if (quickSlot.itemAmountText != null)
                {
                    quickSlot.itemAmountText.text = quickSlot.amount.ToString();
                }
                else
                {
                    Debug.LogError("Item amount text is null in slot.");
                }
                return;
            }
        }

        foreach (InventorySlot quickSlot in quickSlots)
        {
            if (quickSlot.isEmpty)
            {
                quickSlot.item = _item;
                quickSlot.amount = _amount;
                quickSlot.isEmpty = false;
                Debug.Log("Item added to empty slot");
                if (quickSlot.itemAmountText != null)
                {
                    quickSlot.itemAmountText.text = _amount.ToString();
                }
                else
                {
                    Debug.LogError("Item amount text is null in slot.");
                }
                if (_item.icon != null)
                {
                    quickSlot.SetIcon(_item.icon);
                }
                else
                {
                    Debug.LogError("Item icon is null.");
                }
                return;
            }
        }

        foreach (InventorySlot slot in slots)
        {
            if (slot.item == _item && slot.amount < _item.maximumAmount)
            {
                slot.amount += _amount;
                if (slot.itemAmountText != null)
                {
                    slot.itemAmountText.text = slot.amount.ToString();
                }
                else
                {
                    Debug.LogError("Item amount text is null in slot.");
                }
                return;
            }
        }
        
        foreach (InventorySlot slot in slots)
        {
            if (slot.isEmpty)
            {
                slot.item = _item;
                slot.amount = _amount;
                slot.isEmpty = false;
                Debug.Log("Item added to empty slot");
                if (slot.itemAmountText != null)
                {
                    slot.itemAmountText.text = _amount.ToString();
                }
                else
                {
                    Debug.LogError("Item amount text is null in slot.");
                }
                if (_item.icon != null)
                {
                    slot.SetIcon(_item.icon);
                }
                else
                {
                    Debug.LogError("Item icon is null.");
                }
                return;
            }
        }
    }
}
