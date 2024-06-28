using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentStatsBoost : MonoBehaviour
{
    public static int maxHealthBoost = 0;
    public static int maxIntellectBoost = 0;

    public static float damageBoost = 1;

    public static float meleeResistanceBoost = 0;
    public static float rangeResistanceBoost = 0;

    public static float regenerationSpeedBoost = 1;
    public static float intellectRegenSpeedBoost = 1;
 
    public static bool AddMaxHealthBoost()
    {
        if (maxHealthBoost < 100)
        {
            maxHealthBoost += 20;
            return true;
        }
        return false;
    }

    public static bool AddMaxIntellect()
    {
        if (maxIntellectBoost < 100)
        {
            maxIntellectBoost += 20;
            return true;
        }
        return false;
    }

    public static bool AddMaxDamage()
    {
        if (damageBoost < 1.5)
        {
            damageBoost += 0.05f;
            return true;
        }
        return false;
    }

    public static bool AddMeleeResistance()
    {
        if (meleeResistanceBoost < 0.5)
        {
            meleeResistanceBoost += 0.1f;
            return true;
        }
        return false;
    }

    public static bool AddRangeResistance()
    {
        if (rangeResistanceBoost < 0.5)
        {
            rangeResistanceBoost += 0.1f;
            return true;
        }
        return false;
    }

    public static bool AddRegenerationSpeed()
    {
        if (regenerationSpeedBoost < 2)
        {
            regenerationSpeedBoost += 0.1f;
            return true;
        }
        return false;
    }

    public static bool AddIntellectRegenSpeedSpeed()
    {
        if (intellectRegenSpeedBoost < 2)
        {
            intellectRegenSpeedBoost += 0.1f;
            return true;
        }
        return false;
    }
}
