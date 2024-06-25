using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class PlayerStats : MonoBehaviour
{
    public static int healthAmount = 100;
    public int maxHealth = 100;
    public int money = 0;

    public static List<int> damageResistance;

    public static bool isBlocking = false;
    private static List<int> tempDamageResistance;

    public static int radiationResistance = 0;
    public static int currentRadiationImpact = 0;

    public static float intellectAmount = 100;

    void Start()
    {
        damageResistance = new List<int>() { 0, 0, 0 };
        tempDamageResistance = new List<int>() { 0, 0, 0 };
    }

    private void Update()
    {
        maxHealth = healthAmount;
    }

    public static void Block()
    {
        isBlocking = true;
        PlayerMovement.slowRate = 0.2f;
        tempDamageResistance[0] += 5;
    }

    public static void UnBlock()
    {
        isBlocking = false;
        PlayerMovement.slowRate = 1f;
        tempDamageResistance[0] -= 5;
    }

    public static void TakeDamage(int damage, int typeOfDamage)
    {
        healthAmount -= CalculateDamage(damage, typeOfDamage);

        if (healthAmount <= 0)
        {
            // возрождение ??
        }
    }

    public static int CalculateDamage(int damage, int typeOfDamage)
    {
        return (int)(damage * ((100.0 - (damageResistance[typeOfDamage] + tempDamageResistance[typeOfDamage])) / 100));
    }
}
