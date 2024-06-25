using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using UnityEngine.UI;
using TMPro;
using System;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public GameObject Panel;
    public Transform InventoryPanel;
    public Transform QuickslotPanel;
    public List<InventorySlot> slots = new List<InventorySlot>();
    public List<InventorySlot> quickSlots = new List<InventorySlot>();
    public bool isOpened = false;
    public GameObject ShopPanel;
    public bool shopIsOpened = false;
    public GameObject sword;
    public GameObject gun;
    public GameObject laserGun;
    private GameObject player;
    public TMP_Text wallet;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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
        ShopPanel.SetActive(false);
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
        if (Input.GetKeyDown(KeyCode.B))
        {
            shopIsOpened = !shopIsOpened;
            if (shopIsOpened)
            {
                ShopPanel.SetActive(true);
            }
            else
            {
                ShopPanel.SetActive(false);
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
    public void BuySword()
    {
        if (player.GetComponent<PlayerStats>().money >= 1)
        {
            player.GetComponent<PlayerStats>().money -= 1;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            Item weapon = sword.GetComponent<Item>();
            AddItem(weapon.item, weapon.amount);
        }
        else
        {
            Debug.Log("¬€ ¡≈ƒÕ€…");
        }
    }
    public void BuyGun()
    {
        if (player.GetComponent<PlayerStats>().money >= 1)
        {
            player.GetComponent<PlayerStats>().money -= 1;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            Item weapon = gun.GetComponent<Item>();
            AddItem(weapon.item, weapon.amount);
        }
        else
        {
            Debug.Log("¬€ ¡≈ƒÕ€…");
        }
    }
    public void BuyLaserGun()
    {
        if (player.GetComponent<PlayerStats>().money >= 1)
        {
            player.GetComponent<PlayerStats>().money -= 1;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            Item weapon = laserGun.GetComponent<Item>();
            AddItem(weapon.item, weapon.amount);
        }
        else
        {
            Debug.Log("¬€ ¡≈ƒÕ€…");
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
    public bool RemoveItem(ItemScriptableObject _item, int _amount = 1)
    {
        foreach (InventorySlot quickSlot in quickSlots)
        {
            if (quickSlot.item == _item && quickSlot.amount == _amount)
            {
                quickSlot.isEmpty = true;
                quickSlot.amount = 0;
                quickSlot.item = null;
                if (quickSlot.amount <= 0)
                {
                    quickSlot.item = null;
                    quickSlot.amount = 0;
                    quickSlot.isEmpty = true;
                    quickSlot.iconGO.GetComponent<Image>().color = new Color(0, 0, 0, 255);
                    quickSlot.iconGO.GetComponent<Image>().sprite = null;
                    quickSlot.itemAmountText.text = "";
                }
                quickSlot.itemAmountText.text = quickSlot.amount.ToString();
                return true;
            }

            else if (quickSlot.item == _item && quickSlot.amount > _amount)
            {
                quickSlot.isEmpty = false;
                quickSlot.amount -= _amount;
                if (quickSlot.amount <= 0)
                {
                    quickSlot.item = null;
                    quickSlot.amount = 0;
                    quickSlot.isEmpty = true;
                    quickSlot.iconGO.GetComponent<Image>().color = new Color(0, 0, 0, 255);
                    quickSlot.iconGO.GetComponent<Image>().sprite = null;
                    quickSlot.itemAmountText.text = "";
                }
                quickSlot.itemAmountText.text = quickSlot.amount.ToString();
                return true;
            }

        }

        foreach (InventorySlot slot in slots)
        {
            if (slot.item == _item && slot.amount == _amount)
            {
                slot.isEmpty = true;
                slot.amount = 0;
                slot.item = null;
                if (slot.amount <= 0)
                {
                    slot.item = null;
                    slot.amount = 0;
                    slot.isEmpty = true;
                    slot.iconGO.GetComponent<Image>().color = new Color(0, 0, 0, 255);
                    slot.iconGO.GetComponent<Image>().sprite = null;
                    slot.itemAmountText.text = "";
                }
                return true;
            }

            else if (slot.item == _item && slot.amount > _amount)
            {
                slot.isEmpty = false;
                slot.amount -= _amount;
                if (slot.amount <= 0)
                {
                    slot.item = null;
                    slot.amount = 0;
                    slot.isEmpty = true;
                    slot.iconGO.GetComponent<Image>().color = new Color(0, 0, 0, 255);
                    slot.iconGO.GetComponent<Image>().sprite = null;
                    slot.itemAmountText.text = "";
                }
                slot.itemAmountText.text = slot.amount.ToString();
                return true;
            }
        }
        return false;
    }

}
