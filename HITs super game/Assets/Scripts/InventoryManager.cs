using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using UnityEngine.UI;
using TMPro;
using System;
using System.Transactions;

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
    public GameObject stoneBricks;
    public GameObject aluminiumBricks;
    public GameObject leadBricks;
    private bool isNear = false;

    private string saveKey = "mainSaveInventory";

    public ItemScriptableObject[] items;

    private ItemScriptableObject FindItemByName(string name)
    {
        foreach (var item in items)
        {
            if (item.itemName == name)
                return item;
        }

        return null;
    }

    private SaveData.Inventory GetSaveSnapshot()
    {
        List<SingleSlot> newSlots = new List<SingleSlot>();
        foreach(InventorySlot slot in slots)
        {
            newSlots.Add(new SingleSlot(slot));
        }

        List<SingleSlot> newQuickSlots = new List<SingleSlot>();
        foreach (InventorySlot slot in quickSlots)
        {
            newQuickSlots.Add(new SingleSlot(slot));
        }

        var data = new SaveData.Inventory()
        {
            slots = newSlots,
            quickSlots = newQuickSlots,
        };

        return data;
    }
    public void Save()
    {
        SaveManager.Save(saveKey, GetSaveSnapshot());
    }

    public void Load()
    {
        var data = SaveManager.Load<SaveData.Inventory>(saveKey);

        for(int i = 0; i < slots.Count; ++i)
        {
            if (data.slots[i] != null && data.slots[i].item != null)
            {
                slots[i].item = FindItemByName(data.slots[i].item);
                slots[i].amount = data.slots[i].amount;
                slots[i].isEmpty = false;
                slots[i].iconGO.GetComponent<Image>().sprite = FindItemByName(data.slots[i].item).icon;
                slots[i].itemAmountText.text = slots[i].amount.ToString();
                slots[i].iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
        }

        for (int i = 0; i < quickSlots.Count; ++i)
        {
            if (data.quickSlots[i] != null && data.quickSlots[i].item != null)
            {
                quickSlots[i].iconGO.GetComponent<Image>().sprite = FindItemByName(data.quickSlots[i].item).icon;
                quickSlots[i].item = FindItemByName(data.quickSlots[i].item);
                quickSlots[i].amount = data.quickSlots[i].amount;
                quickSlots[i].isEmpty = false;
                quickSlots[i].itemAmountText.text = quickSlots[i].amount.ToString();
                quickSlots[i].iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            else
            {
                quickSlots[i].iconGO.GetComponent<Image>().sprite = null;
                quickSlots[i].item = null;
                quickSlots[i].amount = 0;
                quickSlots[i].isEmpty = true;
                quickSlots[i].itemAmountText.text = "";
                quickSlots[i].iconGO.GetComponent<Image>().color = new Color(0, 0, 0, 255);
            }
        }
    }

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
        Load();
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
        if (isNear)
        {
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
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("COMPUTER"))
        {
            isNear = false;
            ShopPanel.SetActive(false);
            shopIsOpened = false;
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
        if (other.gameObject.CompareTag("COMPUTER"))
        {
            isNear = true;
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
            Debug.Log("�� ������");
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
            Debug.Log("�� ������");
        }
    }
    public void BuyStoneBricks()
    {
        if (player.GetComponent<PlayerStats>().money >= 1)
        {
            player.GetComponent<PlayerStats>().money -= 1;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            Item block = stoneBricks.GetComponent<Item>();
            AddItem(block.item, block.amount);
        }
        else
        {
            Debug.Log("�� ������");
        }
    }
    public void BuyAluminiumBricks()
    {
        if (player.GetComponent<PlayerStats>().money >= 1)
        {
            player.GetComponent<PlayerStats>().money -= 1;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            Item block = aluminiumBricks.GetComponent<Item>();
            AddItem(block.item, block.amount);
        }
        else
        {
            Debug.Log("�� ������");
        }
    }
    public void BuyLeadBricks()
    {
        if (player.GetComponent<PlayerStats>().money >= 1)
        {
            player.GetComponent<PlayerStats>().money -= 1;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            Item block = leadBricks.GetComponent<Item>();
            AddItem(block.item, block.amount);
        }
        else
        {
            Debug.Log("�� ������");
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
            Debug.Log("�� ������");
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
