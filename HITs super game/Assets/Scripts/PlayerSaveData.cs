using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SaveData
{
    [System.Serializable]
    public class World
    {
        public WorldBlock[,] world;
        public WorldBlock[,] bgWorld;
        public WorldBlock[,] fgWorld;
        public int[,] lightMap;
        public float compPosX;
        public float compPosY;

        public World()
        {
            world = null;
            bgWorld = null;
            fgWorld = null;
            lightMap = null;
            compPosX = 0;
            compPosY = 0;
        }
    }

    [System.Serializable]
    public class PermanentStats
    {
        public int maxHealthBoost;
        public int maxIntellectBoost;

        public float damageBoost;

        public int meleeResistanceBoost;
        public int rangeResistanceBoost;

        public float regenerationSpeedBoost;
        public float intellectRegenSpeedBoost;

        public PermanentStats()
        {
            maxHealthBoost = 0;
            maxIntellectBoost = 0;

            damageBoost = 1;

            meleeResistanceBoost = 0;
            rangeResistanceBoost = 0;

            regenerationSpeedBoost = 1;
            intellectRegenSpeedBoost = 1;
        }
    }

    [SerializeField]
    public class PermStatsLvls
    {
        public int healthLevel;
        public int intelLevel;
        public int damageLevel;
        public int meleeLevel;
        public int rangeLevel;
        public int regenLevel;
        public int regenIntLevel;

        public PermStatsLvls()
        {
            healthLevel = 1;
            intelLevel = 1;
            damageLevel = 1;
            meleeLevel = 1;
            rangeLevel = 1;
            regenLevel = 1;
            regenIntLevel = 1;
        }
    }

    [System.Serializable]
    public class Base
    {
        public List<Room> rooms;

        public Base()
        {
            rooms = new List<Room>();
        }
    }

    [System.Serializable]
    public class Inventory
    {
        public List<SingleSlot> slots;
        public List<SingleSlot> quickSlots;

        public Inventory()
        {
            slots = new List<SingleSlot>();
            quickSlots = new List<SingleSlot>();
        }
    }
}