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
    public TMP_Text upgradeSword;
    public TMP_Text upgradeGun;
    public TMP_Text upgradeLaser;
    public TMP_Text swordCost;
    public TMP_Text gunCost;
    public TMP_Text laserCost;
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

    public ItemScriptableObject[] items;

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
        sword.GetComponent<Item>().item.level = 0;
        gun.GetComponent<Item>().item.level = 0;
        laserGun.GetComponent<Item>().item.level = 0;
        kirk.GetComponent<Item>().item.level = 1;
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
        if (player.GetComponent<PlayerStats>().money >= 200 && sword.GetComponent<Item>().item.level == 0)
        {
            player.GetComponent<PlayerStats>().money -= 200;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            Item weapon = sword.GetComponent<Item>();
            sword.GetComponent<Item>().item.level++;
            AddItem(weapon.item, weapon.amount);
            upgradeSword.text = "УЛУЧШИТЬ";
            swordCost.text = "1000";
        }
        else if (player.GetComponent<PlayerStats>().money >= 1000 && sword.GetComponent<Item>().item.level == 1)
        {
            player.GetComponent<PlayerStats>().money -= 1000;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            sword.GetComponent<Item>().item.level++;
            swordCost.text = "1500";
        }
        else if (player.GetComponent<PlayerStats>().money >= 1500 && sword.GetComponent<Item>().item.level == 2)
        {
            player.GetComponent<PlayerStats>().money -= 1500;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            sword.GetComponent<Item>().item.level++;
            swordCost.text = "MAX";
            upgradeSword.text = "MAX";
        }
        else if (sword.GetComponent<Item>().item.level == 3)
        {
            Guide.ShowMessage("Меч достиг максимального уровня");
        }
        else
        {
            Guide.ShowMessage("Недостаточно денег");
        }
    }
    public void BuyGun()
    {
        if (player.GetComponent<PlayerStats>().money >= 1000 && gun.GetComponent<Item>().item.level == 0)
        {
            player.GetComponent<PlayerStats>().money -= 1000;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            Item weapon = gun.GetComponent<Item>();
            gun.GetComponent<Item>().item.level++;
            AddItem(weapon.item, weapon.amount);
            upgradeGun.text = "УЛУЧШИТЬ";
            gunCost.text = "2000";
        }
        else if (player.GetComponent<PlayerStats>().money >= 2000 && gun.GetComponent<Item>().item.level == 1)
        {
            player.GetComponent<PlayerStats>().money -= 2000;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            gun.GetComponent<Item>().item.level++;
            gunCost.text = "3000";
        }
        else if (player.GetComponent<PlayerStats>().money >= 3000 && gun.GetComponent<Item>().item.level == 2)
        {
            player.GetComponent<PlayerStats>().money -= 3000;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            gun.GetComponent<Item>().item.level++;
            gunCost.text = "MAX";
            upgradeGun.text = "MAX";
        }
        else if (gun.GetComponent<Item>().item.level == 3)
        {
            Guide.ShowMessage("Пушка достигла максимального уровня");
        }
        else
        {
            Guide.ShowMessage("Недостаточно денег");
        }
    }
    public void BuyStoneBricks()
    {
        if (player.GetComponent<PlayerStats>().money >= 10)
        {
            player.GetComponent<PlayerStats>().money -= 10;
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
        if (player.GetComponent<PlayerStats>().money >= 20)
        {
            player.GetComponent<PlayerStats>().money -= 20;
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
        if (player.GetComponent<PlayerStats>().money >= 30)
        {
            player.GetComponent<PlayerStats>().money -= 30;
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
        if (player.GetComponent<PlayerStats>().money >= 2000 && laserGun.GetComponent<Item>().item.level == 0)
        {
            player.GetComponent<PlayerStats>().money -= 2000;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            Item weapon = laserGun.GetComponent<Item>();
            AddItem(weapon.item, weapon.amount);
            laserGun.GetComponent<Item>().item.level++;
            upgradeLaser.text = "УЛУЧШИТЬ";
            laserCost.text = "3500";
        }
        else if (player.GetComponent<PlayerStats>().money >= 3500 && laserGun.GetComponent<Item>().item.level == 1)
        {
            player.GetComponent<PlayerStats>().money -= 3500;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            laserGun.GetComponent<Item>().item.level++;
            laserCost.text = "5000";
        }
        else if (player.GetComponent<PlayerStats>().money >= 5000 && laserGun.GetComponent<Item>().item.level == 2)
        {
            player.GetComponent<PlayerStats>().money -= 5000;
            wallet.text = player.GetComponent<PlayerStats>().money.ToString();
            laserGun.GetComponent<Item>().item.level++;
            laserCost.text = "MAX";
            upgradeLaser.text = "MAX";
        }
        else if (laserGun.GetComponent<Item>().item.level == 3)
        {
            Guide.ShowMessage("Лазер достиг максимального уровня");
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
        if (healthLevel == 1)
        {
            if (player.GetComponent<PlayerStats>().money >= 300)
            {
                PermanentStatsBoost.AddMaxHealthBoost();
                healthLevel++;
                player.GetComponent<PlayerStats>().money -= 300;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                healthCost.text = "600";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (healthLevel == 2)
        {
            if (player.GetComponent<PlayerStats>().money >= 600)
            {
                PermanentStatsBoost.AddMaxHealthBoost();
                healthLevel++;
                player.GetComponent<PlayerStats>().money -= 600;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                healthCost.text = "1000";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (healthLevel == 3)
        {
            if (player.GetComponent<PlayerStats>().money >= 1000)
            {
                PermanentStatsBoost.AddMaxHealthBoost();
                healthLevel++;
                player.GetComponent<PlayerStats>().money -= 1000;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                healthCost.text = "1500";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (healthLevel == 4)
        {
            if (player.GetComponent<PlayerStats>().money >= 1500)
            {
                PermanentStatsBoost.AddMaxHealthBoost();
                healthLevel++;
                player.GetComponent<PlayerStats>().money -= 1500;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                healthCost.text = "2000";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (healthLevel == 5)
        {
            if (player.GetComponent<PlayerStats>().money >= 2000)
            {
                PermanentStatsBoost.AddMaxHealthBoost();
                healthLevel++;
                player.GetComponent<PlayerStats>().money -= 2000;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                healthCost.text = "MAX";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (healthLevel >= 6)
        {
            Guide.ShowMessage("Достигнуто предельное значение здоровья");
        }
    }
    public void UpgradeIntel()
    {
        
        if (intelLevel == 1)
        {
            if (player.GetComponent<PlayerStats>().money >= 300)
            {
                PermanentStatsBoost.AddMaxIntellect();
                intelLevel++;
                player.GetComponent<PlayerStats>().money -= 300;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                intelCost.text = "600";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (intelLevel == 2)
        {
            if (player.GetComponent<PlayerStats>().money >= 600)
            {
                PermanentStatsBoost.AddMaxIntellect();
                intelLevel++;
                player.GetComponent<PlayerStats>().money -= 600;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                intelCost.text = "1000";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (intelLevel == 3)
        {
            if (player.GetComponent<PlayerStats>().money >= 1000)
            {
                PermanentStatsBoost.AddMaxIntellect();
                intelLevel++;
                player.GetComponent<PlayerStats>().money -= 1000;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                intelCost.text = "1500";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (intelLevel == 4)
        {
            if (player.GetComponent<PlayerStats>().money >= 1500)
            {
                PermanentStatsBoost.AddMaxIntellect();
                intelLevel++;
                player.GetComponent<PlayerStats>().money -= 1500;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                intelCost.text = "2000";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (intelLevel == 5)
        {
            if (player.GetComponent<PlayerStats>().money >= 2000)
            {
                PermanentStatsBoost.AddMaxIntellect();
                intelLevel++;
                player.GetComponent<PlayerStats>().money -= 2000;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                intelCost.text = "MAX";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (intelLevel >= 6)
        {
            Guide.ShowMessage("Достигнуто предельное значение интеллекта");
        }
    }

    public void UpgradeDamage()
    {
        if (damageLevel == 1)
        {
            if (player.GetComponent<PlayerStats>().money >= 300)
            {
                PermanentStatsBoost.AddMaxDamage();
                damageLevel++;
                player.GetComponent<PlayerStats>().money -= 300;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                damageCost.text = "600";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (damageLevel == 2)
        {
            if (player.GetComponent<PlayerStats>().money >= 600)
            {
                PermanentStatsBoost.AddMaxDamage();
                damageLevel++;
                player.GetComponent<PlayerStats>().money -= 600;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                damageCost.text = "1000";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (damageLevel == 3)
        {
            if (player.GetComponent<PlayerStats>().money >= 1000)
            {
                PermanentStatsBoost.AddMaxDamage();
                damageLevel++;
                player.GetComponent<PlayerStats>().money -= 1000;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                damageCost.text = "1500";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (damageLevel == 4)
        {
            if (player.GetComponent<PlayerStats>().money >= 1500)
            {
                PermanentStatsBoost.AddMaxDamage();
                damageLevel++;
                player.GetComponent<PlayerStats>().money -= 1500;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                damageCost.text = "2000";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (damageLevel == 5)
        {
            if (player.GetComponent<PlayerStats>().money >= 2000)
            {
                PermanentStatsBoost.AddMaxDamage();
                damageLevel++;
                player.GetComponent<PlayerStats>().money -= 2000;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                damageCost.text = "MAX";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (damageLevel >= 6)
        {
            Guide.ShowMessage("Достигнуто предельное значение урона");
        }
    }
    public void UpgradeMelee()
    {
        if (meleeLevel == 1)
        {
            if (player.GetComponent<PlayerStats>().money >= 300)
            {
                PermanentStatsBoost.AddMeleeResistance();
                meleeLevel++;
                player.GetComponent<PlayerStats>().money -= 300;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                meleeCost.text = "600";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (meleeLevel == 2)
        {
            if (player.GetComponent<PlayerStats>().money >= 600)
            {
                PermanentStatsBoost.AddMeleeResistance();
                meleeLevel++;
                player.GetComponent<PlayerStats>().money -= 600;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                meleeCost.text = "1000";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (meleeLevel == 3)
        {
            if (player.GetComponent<PlayerStats>().money >= 1000)
            {
                PermanentStatsBoost.AddMeleeResistance();
                meleeLevel++;
                player.GetComponent<PlayerStats>().money -= 1000;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                meleeCost.text = "1500";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (meleeLevel == 4)
        {
            if (player.GetComponent<PlayerStats>().money >= 1500)
            {
                PermanentStatsBoost.AddMeleeResistance();
                meleeLevel++;
                player.GetComponent<PlayerStats>().money -= 1500;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                meleeCost.text = "2000";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (meleeLevel == 5)
        {
            if (player.GetComponent<PlayerStats>().money >= 2000)
            {
                PermanentStatsBoost.AddMeleeResistance();
                meleeLevel++;
                player.GetComponent<PlayerStats>().money -= 2000;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                meleeCost.text = "MAX";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if(meleeLevel >= 6) 
        {
            Guide.ShowMessage("Достигнуто предельное сопротивление к ближнему урону");
        }
    }
    public void UpgradeRange()
    {
        if (rangeLevel == 1)
        {
            if (player.GetComponent<PlayerStats>().money >= 300)
            {
                PermanentStatsBoost.AddRangeResistance();
                rangeLevel++;
                player.GetComponent<PlayerStats>().money -= 300;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                rangeCost.text = "600";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (rangeLevel == 2)
        {
            if (player.GetComponent<PlayerStats>().money >= 600)
            {
                PermanentStatsBoost.AddRangeResistance();
                rangeLevel++;
                player.GetComponent<PlayerStats>().money -= 600;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                rangeCost.text = "1000";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (rangeLevel == 3)
        {
            if (player.GetComponent<PlayerStats>().money >= 1000)
            {
                PermanentStatsBoost.AddRangeResistance();
                rangeLevel++;
                player.GetComponent<PlayerStats>().money -= 1000;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                rangeCost.text = "1500";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (rangeLevel == 4)
        {
            if (player.GetComponent<PlayerStats>().money >= 1500)
            {
                PermanentStatsBoost.AddRangeResistance();
                rangeLevel++;
                player.GetComponent<PlayerStats>().money -= 1500;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                rangeCost.text = "2000";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (rangeLevel == 5)
        {
            if (player.GetComponent<PlayerStats>().money >= 2000)
            {
                PermanentStatsBoost.AddRangeResistance();
                rangeLevel++;
                player.GetComponent<PlayerStats>().money -= 2000;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                rangeCost.text = "MAX";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (rangeLevel >= 6)
        {
            Guide.ShowMessage("Достигнуто предельное сопротивление к дальнему урону");
        }
    }
    public void UpgradeRegen()
    {
        
        if (regenLevel == 1)
        {
            if (player.GetComponent<PlayerStats>().money >= 300)
            {
                PermanentStatsBoost.AddRangeResistance();
                regenLevel++;
                player.GetComponent<PlayerStats>().money -= 300;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                regenCost.text = "600";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (regenLevel == 2)
        {
            if (player.GetComponent<PlayerStats>().money >= 600)
            {
                PermanentStatsBoost.AddRangeResistance();
                regenLevel++;
                player.GetComponent<PlayerStats>().money -= 600;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                regenCost.text = "1000";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (regenLevel == 3)
        {
            if (player.GetComponent<PlayerStats>().money >= 1000)
            {
                PermanentStatsBoost.AddRangeResistance();
                regenLevel++;
                player.GetComponent<PlayerStats>().money -= 1000;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                regenCost.text = "1500";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (regenLevel == 4)
        {
            if (player.GetComponent<PlayerStats>().money >= 1500)
            {
                PermanentStatsBoost.AddRangeResistance();
                regenLevel++;
                player.GetComponent<PlayerStats>().money -= 1500;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                regenCost.text = "2000";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (regenLevel == 5)
        {
            if (player.GetComponent<PlayerStats>().money >= 2000)
            {
                PermanentStatsBoost.AddRangeResistance();
                regenLevel++;
                player.GetComponent<PlayerStats>().money -= 2000;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                regenCost.text = "MAX";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (regenLevel >= 6)
        {
            Guide.ShowMessage("Достигнуто предельное значение регенерации здоровья");
        }
    }
    public void UpgradeRegenInt()
    {
        
        if (regenIntLevel == 1)
        {
            if (player.GetComponent<PlayerStats>().money >= 300)
            {
                PermanentStatsBoost.AddIntellectRegenSpeedSpeed();
                regenIntLevel++;
                player.GetComponent<PlayerStats>().money -= 300;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                regenIntCost.text = "600";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (regenIntLevel == 2)
        {
            if (player.GetComponent<PlayerStats>().money >= 600)
            {
                PermanentStatsBoost.AddIntellectRegenSpeedSpeed();
                regenIntLevel++;
                player.GetComponent<PlayerStats>().money -= 600;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                regenIntCost.text = "1000";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (regenIntLevel == 3)
        {
            if (player.GetComponent<PlayerStats>().money >= 1000)
            {
                PermanentStatsBoost.AddIntellectRegenSpeedSpeed();
                regenIntLevel++;
                player.GetComponent<PlayerStats>().money -= 1000;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                regenIntCost.text = "1500";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (regenIntLevel == 4)
        {
            if (player.GetComponent<PlayerStats>().money >= 1500)
            {
                PermanentStatsBoost.AddIntellectRegenSpeedSpeed();
                regenIntLevel++;
                player.GetComponent<PlayerStats>().money -= 1500;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                regenIntCost.text = "2000";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (regenIntLevel == 5)
        {
            if (player.GetComponent<PlayerStats>().money >= 2000)
            {
                PermanentStatsBoost.AddIntellectRegenSpeedSpeed();
                regenIntLevel++;
                player.GetComponent<PlayerStats>().money -= 2000;
                wallet.text = player.GetComponent<PlayerStats>().money.ToString();
                regenIntCost.text = "MAX";
            }
            else
            {
                Guide.ShowMessage("Недостаточно средств");
            }
        }

        else if (regenLevel >= 6)
        {
            Guide.ShowMessage("Достигнуто предельное значение регенерации интеллекта");
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
