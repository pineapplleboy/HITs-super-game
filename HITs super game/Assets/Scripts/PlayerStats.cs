using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;
using System.Runtime.CompilerServices;

public class PlayerStats : MonoBehaviour
{
    public static int healthAmount = 100;
    public int currHealth = 100;
    public int maxHealth = 100;
    public int money = 0;

    public static List<int> damageResistance;

    public static bool isBlocking = false;
    private static List<int> tempDamageResistance;

    public static int radiationResistance = 0;
    public static int currentRadiationImpact = 0;

    public static float intellectAmount = 100;
    public static float maxIntellectAmount = 100;

    private float whenStartHealing = 2.5f;
    private float currentTakeDamageTime = 0f;
    private float addHeartsAmount = 0f;

    public TMPro.TMP_Text HealthText;
    public TMPro.TMP_Text IntellectText;

    public GameObject[] Hearts;
    public GameObject[] Brains;

    public static bool isDead = false;
    private float currentPotionCd = 0f;
    private float healthPotionCd = 60f;

    private float currentSaveCd = 0f;
    private float saveCd = 300f;

    private string saveKey = "mainSaveMoney";
    private SaveData.Money GetSaveSnapshot()
    {
        var data = new SaveData.Money()
        {
            value = money,
        };

        return data;
    }

    public void Save()
    {
        SaveManager.Save(saveKey, GetSaveSnapshot());
    }

    public void Load()
    {
        var data = SaveManager.Load<SaveData.Money>(saveKey);
        money = data.value;
        GameObject.Find("MainCanvas").GetComponent<InventoryManager>().UpdateMoneyOnScreen();

    }
    void Start()
    {
        Load();

        damageResistance = new List<int>() { 0, 0, 0 };
        tempDamageResistance = new List<int>() { 0, 0, 0 };

        HealthText.text = Convert.ToInt32(healthAmount).ToString() + "/" + Convert.ToInt32(maxHealth).ToString();
        IntellectText.text = Convert.ToInt32(intellectAmount).ToString() + "/" + Convert.ToInt32(maxIntellectAmount).ToString();
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (Input.GetKey(KeyCode.J))
        {
            healthAmount -= 1000;
            GameObject.FindGameObjectWithTag("COMPUTER").GetComponent<COMPUTER>().TakeDamage(2000);
        }

        if (Input.GetKey(KeyCode.I))
        {
            money += 100;
            GameObject.Find("MainCanvas").GetComponent<InventoryManager>().UpdateMoneyOnScreen();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (currentPotionCd <= 0)
            {
                healthAmount += 50;
                currentPotionCd = healthPotionCd;
            }
            else
            {
                Guide.ShowMessage("Перезарядка. Осталось " + (int)currentPotionCd + " секунд");
            }
            
        }

        currentPotionCd -= Time.deltaTime;
        currentTakeDamageTime += Time.deltaTime;
        currentSaveCd -= Time.deltaTime;

        if (currentTakeDamageTime >= whenStartHealing)
        {
            Heal();
        }

        maxHealth = 100 + PermanentStatsBoost.maxHealthBoost;
        maxIntellectAmount = 100 + PermanentStatsBoost.maxIntellectBoost;

        damageResistance[0] = PermanentStatsBoost.meleeResistanceBoost;
        damageResistance[1] = PermanentStatsBoost.rangeResistanceBoost;

        healthAmount = (healthAmount < 0) ? 0 : healthAmount;
        currHealth = healthAmount;

        intellectAmount += 5 * Time.deltaTime * PermanentStatsBoost.intellectRegenSpeedBoost;
        intellectAmount = Mathf.Min(intellectAmount, maxIntellectAmount);

        HealthText.text = Convert.ToInt32(healthAmount).ToString() + "/" + Convert.ToInt32(maxHealth).ToString();
        IntellectText.text = Convert.ToInt32(intellectAmount).ToString() + "/" + Convert.ToInt32(maxIntellectAmount).ToString();

        int activeHearts = currHealth * Hearts.Length / maxHealth;
        for (int i = 0; i < activeHearts; i++)
        {
            Hearts[i].SetActive(true);
            SetAlpha(Hearts[i], 1);
        }

        if (activeHearts < Hearts.Length)
        {
            Hearts[activeHearts].SetActive(true);
            SetAlpha(Hearts[activeHearts], (currHealth - activeHearts * (maxHealth / Hearts.Length)) / (maxHealth / Hearts.Length));
        }

        for (int i = activeHearts + 1; i < Hearts.Length; ++i)
        {
            Hearts[i].SetActive(false);
        }

        int activeBrains = Convert.ToInt32(intellectAmount * Brains.Length / maxIntellectAmount);
        for (int i = 0; i < activeBrains; i++)
        {
            Brains[i].SetActive(true);
            SetAlpha(Brains[i], 1);
        }

        if (activeBrains < Brains.Length)
        {
            Brains[activeBrains].SetActive(true);
            SetAlpha(Brains[activeBrains], (intellectAmount - activeBrains * (maxIntellectAmount / Brains.Length)) / (maxIntellectAmount / Brains.Length));
        }

        for (int i = activeBrains + 1; i < Brains.Length; ++i)
        {
            Brains[i].SetActive(false);
        }

        if (healthAmount <= 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        isDead = true;
        GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        Guide.ShowMessage("Вы умерли");
        GameObject.FindGameObjectWithTag("World").GetComponent<WorldGeneration>().TPPLayerOnBase();

        yield return new WaitForSeconds(10);
        isDead = false;
        healthAmount = maxHealth;
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    private void Heal()
    {
        addHeartsAmount += 3 * Time.deltaTime * PermanentStatsBoost.regenerationSpeedBoost;

        healthAmount += (int)addHeartsAmount;
        healthAmount = Mathf.Min(healthAmount, maxHealth);

        addHeartsAmount %= 1;
    }

    private void SetAlpha(GameObject Image, float alpha)
    {
        var tempColor = Image.GetComponent<Image>().color;
        tempColor.a = alpha;
        Image.GetComponent<Image>().color = tempColor;
    }

    public static void Block()
    {
        isBlocking = true;
        PlayerMovement.slowRate = 0.2f;
        tempDamageResistance[0] += 50;
    }

    public static void UnBlock()
    {
        isBlocking = false;
        PlayerMovement.slowRate = 1f;
        tempDamageResistance[0] -= 50;
    }

    public void TakeDamage(int damage, int typeOfDamage)
    {
        currentTakeDamageTime = 0;
        healthAmount -= CalculateDamage(damage, typeOfDamage);
        addHeartsAmount = 0;

        if (healthAmount <= 0)
        {
            // ����������� ??
        }
    }

    public static void TakeTouchDamage(int damage, int typeOfDamage)
    {
        healthAmount -= CalculateDamage(damage, typeOfDamage);

        if (healthAmount <= 0)
        {
            // ����������� ??
        }
    }

    public static int CalculateDamage(int damage, int typeOfDamage)
    {
        return (int)(damage * ((100.0 - (damageResistance[typeOfDamage] + tempDamageResistance[typeOfDamage])) / 100));
    }
}