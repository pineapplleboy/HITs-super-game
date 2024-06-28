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
    public GameObject kirk;
    private bool isNear = false;
    public TMP_Text costText;
    public GameObject upgradeButton;
    public TMP_Text healthCost;
    private int healthLevel = 1;
    public TMP_Text intelCost;
    private int intelLevel = 1;
    public TMP_Text damageCost;
    private int damageLevel = 1;
    public TMP_Text meleeCost;
    private int meleeLevel = 1;
    public TMP_Text rangeCost;
    private int rangeLevel = 1;
    public TMP_Text regenCost;
    private int regenLevel = 1;
    public TMP_Text regenIntCost;
    private int regenIntLevel = 1;

    private string saveKey = "mainSaveInventory";
    private string saveKey2 = "mainSavePermLvls";

    public ItemScriptableObject[] items;
    private SaveData.PermStatsLvls GetSaveSnapshot2()
    {
        var data = new SaveData.PermStatsLvls()
        {
            healthLevel = healthLevel,
            intelLevel = intelLevel,
            damageLevel = damageLevel,
            meleeLevel = meleeLevel,
            rangeLevel = rangeLevel,
            regenLevel = regenLevel,
            regenIntLevel = regenIntLevel,
        };

        return data;
    }

    public void UpdateMoneyOnScreen()
    {
        wallet.text = player.GetComponent<PlayerStats>().money.ToString();
    }

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
        SaveManager.Save(saveKey2, GetSaveSnapshot2());
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

        var data2 = SaveManager.Load<SaveData.PermStatsLvls>(saveKey2);
        healthLevel = data2.healthLevel;
        intelLevel = data2.intelLevel;
        damageLevel = data2.damageLevel;
        meleeLevel = data2.meleeLevel;
        rangeLevel = data2.rangeLevel;
        regenLevel = data2.regenLevel;
        regenIntLevel = data2.regenIntLevel;

        if (healthLevel == 2)
        {
            healthCost.text = "600";
        }

        else if (healthLevel == 3)
        {
            healthCost.text = "1000";
        }

        else if (healthLevel == 4)
        {
            healthCost.text = "1500";
        }

        else if (healthLevel == 5)
        {
            healthCost.text = "MAX";
        }

        if (intelLevel == 2)
        {
            intelCost.text = "600";
        }

        else if (intelLevel == 3)
        {
            intelCost.text = "1000";
        }

        else if (intelLevel == 4)
        {
            intelCost.text = "1500";
        }

        else if (intelLevel == 5)
        {
            intelCost.text = "MAX";
        }

        if (damageLevel == 2)
        {
            damageCost.text = "600";
        }

        else if (damageLevel == 3)
        {
            damageCost.text = "1000";
        }

        else if (damageLevel == 4)
        {
            damageCost.text = "1500";
        }

        else if (damageLevel == 5)
        {
            damageCost.text = "MAX";
        }

        if (meleeLevel == 2)
        {
            meleeCost.text = "600";
        }

        else if (meleeLevel == 3)
        {
            meleeCost.text = "1000";
        }

        else if (meleeLevel == 4)
        {
            meleeCost.text = "1500";
        }

        else if (meleeLevel == 5)
        {
            meleeCost.text = "MAX";
        }

        if (rangeLevel == 2)
        {
            rangeCost.text = "600";
        }

        else if (rangeLevel == 3)
        {
            rangeCost.text = "1000";
        }

        else if (rangeLevel == 4)
        {
            rangeCost.text = "1500";
        }

        else if (rangeLevel == 5)
        {
            rangeCost.text = "MAX";
        }

        if (regenLevel == 2)
        {
            regenCost.text = "600";
        }

        if (regenLevel == 3)
        {
            regenCost.text = "1000";
        }

        if (regenLevel == 4)
        {
            regenCost.text = "1500";
        }

        if (regenLevel == 5)
        {
            regenCost.text = "MAX";
        }

        if (regenIntLevel == 2)
        {
            regenIntCost.text = "600";
        }

        if (regenIntLevel == 3)
        {
            regenIntCost.text = "1000";
        }

        if (regenIntLevel == 4)
        {
            regenIntCost.text = "1500";
        }

        if (regenIntLevel == 5)
        {
            regenIntCost.text = "MAX";
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
        if (PlayerStats.isDead)
            return;
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
            Guide.ShowMessage("Недостаточно денег");
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
            Guide.ShowMessage("Недостаточно денег");
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
            Guide.ShowMessage("Недостаточно денег");
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
            Guide.ShowMessage("Недостаточно денег");
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
            Guide.ShowMessage("Недостаточно денег");
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
            Guide.ShowMessage("Недостаточно денег");
        }
    }
    public void UpgradeKirk()
    {
        Item kirka = kirk.GetComponent<Item>();
        Debug.Log("YURA");
        if (kirka.item.level == 1)
        {
            if (player.GetComponent<PlayerStats>().money >= 300)
            {
                player.GetComponent<PlayerStats>().money -= 300;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                kirka.item.level += 1;
                costText.text = "1000";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }
        else if (kirka.item.level == 2)
        {
            if (player.GetComponent<PlayerStats>().money >= 1000)
            {
                player.GetComponent<PlayerStats>().money -= 1000;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                kirka.item.level += 1;
                costText.text = "2000";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }
        else if (kirka.item.level == 3)
        {
            if (player.GetComponent<PlayerStats>().money >= 2000)
            {
                player.GetComponent<PlayerStats>().money -= 2000;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                kirka.item.level += 1;
                costText.text = "MAX";
                upgradeButton.SetActive(false);
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }
    }
    public void UpgradeHealth()
    {
        bool healthAdded = true;
        if (healthLevel == 1 && player.GetComponent<PlayerStats>().money >= 300)
        {
            healthAdded = PermanentStatsBoost.AddMaxHealthBoost();
            healthLevel++;
            player.GetComponent<PlayerStats>().money -= 300;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            healthCost.text = "600";
        }

        else if (healthLevel == 2 && player.GetComponent<PlayerStats>().money >= 600)
        {
            healthAdded = PermanentStatsBoost.AddMaxHealthBoost();
            healthLevel++;
            player.GetComponent<PlayerStats>().money -= 600;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            healthCost.text = "1000";
        }

        else if (healthLevel == 3 && player.GetComponent<PlayerStats>().money >= 1000)
        {
            healthAdded = PermanentStatsBoost.AddMaxHealthBoost();
            healthLevel++;
            player.GetComponent<PlayerStats>().money -= 1000;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            healthCost.text = "1500";
        }

        else if (healthLevel == 4 && player.GetComponent<PlayerStats>().money >= 1500)
        {
            healthAdded = PermanentStatsBoost.AddMaxHealthBoost();
            healthLevel++;
            player.GetComponent<PlayerStats>().money -= 1500;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            healthCost.text = "MAX";
        }
        else if (healthAdded)
        {
            Guide.ShowMessage("Недостаточно средств");
        }

        if (!healthAdded)
        {
            Guide.ShowMessage("Достигнуто предельное значение здоровья");
        }
    }
    public void UpgradeIntel()
    {
        bool intelAdded = true;
        if (intelLevel == 1 && player.GetComponent<PlayerStats>().money >= 300)
        {
            intelAdded = PermanentStatsBoost.AddMaxIntellect();
            intelLevel++;
            player.GetComponent<PlayerStats>().money -= 300;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            intelCost.text = "600";
        }

        else if (intelLevel == 2 && player.GetComponent<PlayerStats>().money >= 600)
        {
            intelAdded = PermanentStatsBoost.AddMaxIntellect();
            intelLevel++;
            player.GetComponent<PlayerStats>().money -= 600;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            intelCost.text = "1000";
        }

        else if (intelLevel == 3 && player.GetComponent<PlayerStats>().money >= 1000)
        {
            intelAdded = PermanentStatsBoost.AddMaxIntellect();
            intelLevel++;
            player.GetComponent<PlayerStats>().money -= 1000;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            intelCost.text = "1500";
        }

        else if (intelLevel == 4 && player.GetComponent<PlayerStats>().money >= 1500)
        {
            intelAdded = PermanentStatsBoost.AddMaxIntellect();
            intelLevel++;
            player.GetComponent<PlayerStats>().money -= 1500;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            intelCost.text = "MAX";
        }
        else if (intelAdded)
        {
            Guide.ShowMessage("Недостаточно средств");
        }

        if (!intelAdded)
        {
            Guide.ShowMessage("Достигнуто предельное значение интеллекта");
        }
    }

    public void UpgradeDamage()
    {
        bool damageAdded = true;
        if (damageLevel == 1 && player.GetComponent<PlayerStats>().money >= 300)
        {
            damageAdded = PermanentStatsBoost.AddMaxDamage();
            damageLevel++;
            player.GetComponent<PlayerStats>().money -= 300;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            damageCost.text = "600";
        }

        else if (damageLevel == 2 && player.GetComponent<PlayerStats>().money >= 600)
        {
            damageAdded = PermanentStatsBoost.AddMaxDamage();
            damageLevel++;
            player.GetComponent<PlayerStats>().money -= 600;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            damageCost.text = "1000";
        }

        else if (damageLevel == 3 && player.GetComponent<PlayerStats>().money >= 1000)
        {
            damageAdded = PermanentStatsBoost.AddMaxDamage();
            damageLevel++;
            player.GetComponent<PlayerStats>().money -= 1000;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            damageCost.text = "1500";
        }

        else if (damageLevel == 4 && player.GetComponent<PlayerStats>().money >= 1500)
        {
            damageAdded = PermanentStatsBoost.AddMaxDamage();
            damageLevel++;
            player.GetComponent<PlayerStats>().money -= 1500;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            damageCost.text = "MAX";
        }
        else if (damageAdded)
        {
            Guide.ShowMessage("Недостаточно средств");
        }

        if (!damageAdded)
        {
            Guide.ShowMessage("Достигнуто предельное значение интеллекта");
        }
    }
    public void UpgradeMelee()
    {
        bool meleeAdded = true;
        if (meleeLevel == 1 && player.GetComponent<PlayerStats>().money >= 300)
        {
            meleeAdded = PermanentStatsBoost.AddMeleeResistance();
            meleeLevel++;
            player.GetComponent<PlayerStats>().money -= 300;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            meleeCost.text = "600";
        }

        else if (meleeLevel == 2 && player.GetComponent<PlayerStats>().money >= 600)
        {
            meleeAdded = PermanentStatsBoost.AddMeleeResistance();
            meleeLevel++;
            player.GetComponent<PlayerStats>().money -= 600;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            meleeCost.text = "1000";
        }

        else if (meleeLevel == 3 && player.GetComponent<PlayerStats>().money >= 1000)
        {
            meleeAdded = PermanentStatsBoost.AddMeleeResistance();
            meleeLevel++;
            player.GetComponent<PlayerStats>().money -= 1000;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            meleeCost.text = "1500";
        }

        else if (meleeLevel == 4 && player.GetComponent<PlayerStats>().money >= 1500)
        {
            meleeAdded = PermanentStatsBoost.AddMeleeResistance();
            meleeLevel++;
            player.GetComponent<PlayerStats>().money -= 1500;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            meleeCost.text = "MAX";
        }
        else if (meleeAdded)
        {
            Guide.ShowMessage("Недостаточно средств");
        }

        if (!meleeAdded)
        {
            Guide.ShowMessage("Достигнуто предельное значение интеллекта");
        }
    }
    public void UpgradeRange()
    {
        bool rangeAdded = true;
        if (rangeLevel == 1 && player.GetComponent<PlayerStats>().money >= 300)
        {
            rangeAdded = PermanentStatsBoost.AddRangeResistance();
            rangeLevel++;
            player.GetComponent<PlayerStats>().money -= 300;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            rangeCost.text = "600";
        }

        else if (rangeLevel == 2 && player.GetComponent<PlayerStats>().money >= 600)
        {
            rangeAdded = PermanentStatsBoost.AddRangeResistance();
            rangeLevel++;
            player.GetComponent<PlayerStats>().money -= 600;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            rangeCost.text = "1000";
        }

        else if (rangeLevel == 3 && player.GetComponent<PlayerStats>().money >= 1000)
        {
            rangeAdded = PermanentStatsBoost.AddRangeResistance();
            rangeLevel++;
            player.GetComponent<PlayerStats>().money -= 1000;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            rangeCost.text = "1500";
        }

        else if (rangeLevel == 4 && player.GetComponent<PlayerStats>().money >= 1500)
        {
            rangeAdded = PermanentStatsBoost.AddRangeResistance();
            rangeLevel++;
            player.GetComponent<PlayerStats>().money -= 1500;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            rangeCost.text = "MAX";
        }

        else if (rangeAdded)
        {
            Guide.ShowMessage("Недостаточно средств");
        }

        if (!rangeAdded)
        {
            Guide.ShowMessage("Достигнуто предельное значение интеллекта");
        }
    }
    public void UpgradeRegen()
    {
        bool regenAdded = true;
        if (regenLevel == 1 && player.GetComponent<PlayerStats>().money >= 300)
        {
            regenAdded = PermanentStatsBoost.AddRegenerationSpeed();
            regenLevel++;
            player.GetComponent<PlayerStats>().money -= 300;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            regenCost.text = "600";
        }

        if (regenLevel == 2 && player.GetComponent<PlayerStats>().money >= 600)
        {
            regenAdded = PermanentStatsBoost.AddRegenerationSpeed();
            regenLevel++;
            player.GetComponent<PlayerStats>().money -= 600;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            regenCost.text = "1000";
        }

        if (regenLevel == 3 && player.GetComponent<PlayerStats>().money >= 1000)
        {
            regenAdded = PermanentStatsBoost.AddRegenerationSpeed();
            regenLevel++;
            player.GetComponent<PlayerStats>().money -= 1000;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            regenCost.text = "1500";
        }

        if (regenLevel == 4 && player.GetComponent<PlayerStats>().money >= 1500)
        {
            regenAdded = PermanentStatsBoost.AddRegenerationSpeed();
            regenLevel++;
            player.GetComponent<PlayerStats>().money -= 1500;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            regenCost.text = "MAX";
        }

        else if (regenAdded)
        {
            Guide.ShowMessage("Недостаточно средств");
        }

        if (!regenAdded)
        {
            Guide.ShowMessage("Достигнуто предельное значение интеллекта");
        }
    }
    public void UpgradeRegenInt()
    {
        bool regenIntAdded = true;
        if (regenIntLevel == 1 && player.GetComponent<PlayerStats>().money >= 300)
        {
            regenIntAdded = PermanentStatsBoost.AddIntellectRegenSpeedSpeed();
            regenIntLevel++;
            player.GetComponent<PlayerStats>().money -= 300;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            regenIntCost.text = "600";
        }

        if (regenIntLevel == 2 && player.GetComponent<PlayerStats>().money >= 600)
        {
            regenIntAdded = PermanentStatsBoost.AddIntellectRegenSpeedSpeed();
            regenIntLevel++;
            player.GetComponent<PlayerStats>().money -= 600;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            regenIntCost.text = "1000";
        }

        if (regenIntLevel == 3 && player.GetComponent<PlayerStats>().money >= 1000)
        {
            regenIntAdded = PermanentStatsBoost.AddIntellectRegenSpeedSpeed();
            regenIntLevel++;
            player.GetComponent<PlayerStats>().money -= 1000;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            regenIntCost.text = "1500";
        }

        if (regenIntLevel == 4 && player.GetComponent<PlayerStats>().money >= 1500)
        {
            regenIntAdded = PermanentStatsBoost.AddIntellectRegenSpeedSpeed();
            regenIntLevel++;
            player.GetComponent<PlayerStats>().money -= 1500;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            regenIntCost.text = "MAX";
        }

        else if (regenIntAdded)
        {
            Guide.ShowMessage("Недостаточно средств");
        }

        if (!regenIntAdded)
        {
            Guide.ShowMessage("Достигнуто предельное значение интеллекта");
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
