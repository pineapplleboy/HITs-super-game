using UnityEngine;

namespace SaveData
{
    [System.Serializable]
    public class World
    {
        public Block[,] world;
        public Block[,] bgWorld;
        public Block[,] fgWorld;
        public int[,] lightMap;

        public World()
        {
            world = null;
            bgWorld = null;
            fgWorld = null;
            lightMap = null;
        }
    }
}
