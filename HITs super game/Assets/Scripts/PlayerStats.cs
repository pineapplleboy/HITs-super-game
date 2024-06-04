using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class PlayerStats : MonoBehaviour
{
    public int healthAmount = 100;
    public int maxHealth = 100;

    public List<int> damageResistance;

    public static bool isBlocking = false;
    private static List<int> tempDamageResistance;


    void Start()
    {
        damageResistance = new List<int>() { 0, 0, 0 };
        tempDamageResistance = new List<int>() { 0, 0, 0 };
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

    public void TakeDamage(int damage, int typeOfDamage)
    {
        healthAmount -= CalculateDamage(damage, typeOfDamage);

        if (healthAmount <= 0)
        {
            // возрождение ??
        }
    }

    public int CalculateDamage(int damage, int typeOfDamage)
    {
        return (int)(damage * ((100.0 - (damageResistance[typeOfDamage] + tempDamageResistance[typeOfDamage])) / 100));
    }
}
