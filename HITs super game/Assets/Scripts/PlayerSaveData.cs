using System.Collections.Generic;
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

        public World()
        {
            world = null;
            bgWorld = null;
            fgWorld = null;
            lightMap = null;
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