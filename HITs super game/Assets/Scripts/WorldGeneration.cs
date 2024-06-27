using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[System.Serializable]
public class WorldBlock
{
    [SerializeField] public string name;
    [SerializeField] public float timeToBreak;
    [SerializeField] public bool isImportantForBase = false;

    public WorldBlock()
    {
        name = null;
        timeToBreak = 0;
    }

    public WorldBlock(Block block)
    {
        this.name = block.name;
        this.timeToBreak = block.timeToBreak;
    }

    public bool getImportance()
    {
        return isImportantForBase;
    }

    public void setImportance(bool importance)
    {
        isImportantForBase = importance;
    }
    public float GetTimeToBreak()
    {
        return timeToBreak;
    }
    public bool CheckByName(string name)
    {
        if (this.name == name) return true;
        return false;
    }
}

[System.Serializable]
public class Block
{
    [SerializeField] public string name;
    [SerializeField] public TileBase tile;
    [SerializeField] public float timeToBreak;
    [SerializeField] public ItemScriptableObject item;
    [SerializeField] private Sprite icon;

    public Block(Block block)
    {
        this.name = block.name;
        this.tile = block.tile;
        this.timeToBreak = block.timeToBreak;
        this.item = block.item;
        this.icon = block.icon;
    }
}

[System.Serializable]
public class Blocks
{
    [SerializeField] public Block[] blocks;
    public Block GetBlock(string name)
    {
        foreach (Block block in blocks)
        {
            if (block.name == name)
            {
                return new Block(block);
            }
        }

        return null;
    }
}

public class WorldGeneration : MonoBehaviour
{
    [SerializeField] public Blocks blocks;

    [SerializeField] public Tilemap tilemap;
    [SerializeField] Tilemap bgTilemap;
    [SerializeField] Tilemap fgTilemap;

    [SerializeField] public int worldWidth;
    [SerializeField] int worldHeight;
    [SerializeField] int chunkSize;
    [SerializeField] int renderDistance;

    public WorldBlock[,] world;
    public WorldBlock[,] bgWorld;
    public WorldBlock[,] fgWorld;
    public int[,] lightMap;

    public GameObject tileDrop;
    private int[] maxHeights;

    private float seed;
    private GameObject Player;

    private int playerChunkX;
    private int playerChunkY;

    private int prevPlayerChunkX;
    private int prevPlayerChunkY;

    private int playerPositionX;
    private int playerPositionY;

    private int prevPlayerPositionX;
    private int prevPlayerPositionY;

    private int renderLeftBorder = 0;
    private int renderRightBorder = 0;
    private int renderDownBorder = 0;
    private int renderUpBorder = 0;

    private int prevRenderLeftBorder = 0;
    private int prevRenderRightBorder = 0;
    private int prevRenderDownBorder = 0;
    private int prevRenderUpBorder = 0;

    public Transform QuickslotPanel;
    public Sprite selectedSprite;

    private float blockPressedTime;
    private Vector3Int blockPressedCoords;

    public GameObject computer;
    private string[] bricks = new string[]{"lead_bricks", "stone_bricks", "aluminum_bricks"};

    private string saveKey = "mainSave";

    private SaveData.World GetSaveSnapshot()
    {
        var data = new SaveData.World()
        {
            fgWorld = fgWorld,
            bgWorld = bgWorld,
            world = world,
            lightMap = lightMap,
        };

        return data;
    }

    public void Save()
    {
        SaveManager.Save(saveKey, GetSaveSnapshot());
    }

    public void Load()
    {
        var data = SaveManager.Load<SaveData.World>(saveKey);
        fgWorld = data.fgWorld;
        bgWorld = data.bgWorld;
        world = data.world;
        lightMap = data.lightMap;
    }

    public bool IsBlock(int x, int y)
    {
        return world[x, y] != null;
    }

    public WorldBlock[,] GenerateArray(int width, int height, bool empty)
    {
        WorldBlock[,] map = new WorldBlock[width, height];

        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {

                if (empty)
                {
                    map[x, y] = null;
                }
                else
                {
                    map[x, y] = new WorldBlock(blocks.GetBlock("cobblestone"));
                }
            }
        }

        return map;
    }

    public int[,] RenderLight(int[,] lightMap, int x, int y, int lightLvl, bool reduceLight)
    {
        if (reduceLight && lightLvl != lightMap[x, y])
        {
            return lightMap;
        }

        Queue<(int x, int y, int lightLvl)> queue = new Queue<(int x, int y, int lightLvl)>();
        queue.Enqueue((x, y, lightLvl));

        while (queue.Count > 0)
        {
            var (currentX, currentY, currentLightLvl) = queue.Dequeue();

            if (currentLightLvl == 0 || currentX < 0 || currentX >= lightMap.GetUpperBound(0)
                || currentY < 0 || currentY >= lightMap.GetUpperBound(1))
                continue;

            if (!reduceLight && currentLightLvl > lightMap[currentX, currentY] || reduceLight && currentLightLvl > 0)
            {
                if (!reduceLight)
                {
                    lightMap[currentX, currentY] = currentLightLvl;
                }
                else
                {
                    lightMap[currentX, currentY] = (currentX == x && currentY == y) ? 0 :
                        Mathf.Max(Mathf.Max(lightMap[x + 1, y], lightMap[x, y + 1]), Mathf.Max(lightMap[x, y - 1], lightMap[x - 1, y])) >= lightMap[currentX, currentY] ?
                        Mathf.Max(Mathf.Max(lightMap[x + 1, y], lightMap[x, y + 1]), Mathf.Max(lightMap[x, y - 1], lightMap[x - 1, y])) - 1 : 0;
                }

                queue.Enqueue((currentX + 1, currentY, currentLightLvl - 1));
                queue.Enqueue((currentX, currentY + 1, currentLightLvl - 1));
                queue.Enqueue((currentX - 1, currentY, currentLightLvl - 1));
                queue.Enqueue((currentX, currentY - 1, currentLightLvl - 1));
            }
        }

        return lightMap;
    }

    public void RenderMapByPosition(Vector3 pos)
    {
        int objX = Mathf.Clamp((int)pos.x / chunkSize, 0, worldWidth / chunkSize);
        int objY = Mathf.Clamp((int)pos.y / chunkSize, 0, worldHeight / chunkSize);

        renderLeftBorder = Mathf.Clamp(objX - renderDistance, 0, worldWidth / chunkSize);
        renderRightBorder = Mathf.Clamp(objX + renderDistance, 0, worldWidth / chunkSize);
        renderDownBorder = Mathf.Clamp(objY - renderDistance, 0, worldHeight / chunkSize);
        renderUpBorder = Mathf.Clamp(objY + renderDistance, 0, worldHeight / chunkSize);

        for (int i = renderLeftBorder; i < renderRightBorder; ++i)
        {
            for (int j = renderDownBorder; j < renderUpBorder; ++j)
            {
                RenderMap(i, j, chunkSize);
            }
        }
    }

    public void ClearMapByPosition(Vector3 pos)
    {
        int objX = Mathf.Clamp((int)pos.x / chunkSize, 0, worldWidth / chunkSize);
        int objY = Mathf.Clamp((int)pos.y / chunkSize, 0, worldHeight / chunkSize);

        renderLeftBorder = Mathf.Clamp(objX - renderDistance, 0, worldWidth / chunkSize);
        renderRightBorder = Mathf.Clamp(objX + renderDistance, 0, worldWidth / chunkSize);
        renderDownBorder = Mathf.Clamp(objY - renderDistance, 0, worldHeight / chunkSize);
        renderUpBorder = Mathf.Clamp(objY + renderDistance, 0, worldHeight / chunkSize);

        for (int i = renderLeftBorder; i < renderRightBorder; ++i)
        {
            for (int j = renderDownBorder; j < renderUpBorder; ++j)
            {
                ClearTileMap(tilemap, bgTilemap, fgTilemap, i, j, chunkSize);
            }
        }

        Render();
    }

    public void RenderMap(int chunkX, int chunkY, int chunkSize)
    {
        //tilemap.ClearAllTiles();

        for (int x = chunkX * chunkSize; x < (chunkX + 1) * chunkSize; x++)
        {
            for (int y = chunkY * chunkSize; y < (chunkY + 1) * chunkSize; y++)
            {

                if (world[x, y] != null)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), blocks.GetBlock(world[x, y].name).tile);
                    tilemap.SetColor(new Vector3Int(x, y, 0), new Color(lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f));
                }

                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), null);
                }

                if (bgWorld[x, y] != null)
                {
                    bgTilemap.SetTile(new Vector3Int(x, y, 0), blocks.GetBlock(bgWorld[x, y].name).tile);
                    bgTilemap.SetColor(new Vector3Int(x, y, 0), new Color(lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f));
                }

                else
                {
                    bgTilemap.SetTile(new Vector3Int(x, y, 0), null);
                }

                if (fgWorld[x, y] != null)
                {
                    fgTilemap.SetTile(new Vector3Int(x, y, 0), blocks.GetBlock(fgWorld[x, y].name).tile);
                    fgTilemap.SetColor(new Vector3Int(x, y, 0), new Color(lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f));
                }

                else
                {
                    fgTilemap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
    }

    public void RenderLightWorld(int plPosX, int plPosY)
    {
        for (int x = plPosX - 16; x < plPosX + 16; x++)
        {
            for (int y = plPosY - 16; y < plPosY + 16; y++)
            {

                if (world[x, y] != null)
                {
                    tilemap.SetColor(new Vector3Int(x, y, 0), new Color(lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f));
                }

                if (bgWorld[x, y] != null)
                {
                    bgTilemap.SetColor(new Vector3Int(x, y, 0), new Color(lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f));
                }

                if (fgWorld[x, y] != null)
                {
                    fgTilemap.SetColor(new Vector3Int(x, y, 0), new Color(lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f));
                }
            }
        }
    }

    public WorldBlock[,] CreateBGWall(WorldBlock[,] world)
    {
        WorldBlock[,] bgWorld = new WorldBlock[world.GetUpperBound(0), world.GetUpperBound(1)];

        for (int i = 0; i < world.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < world.GetUpperBound(1); ++j)
            {

                if (world[i, j] != null)
                {
                    bgWorld[i, j] = new WorldBlock(blocks.GetBlock("cobblestone"));
                }
            }
        }

        return bgWorld;
    }

    public static void ClearTileMap(Tilemap tilemap, Tilemap bgTilemap, Tilemap fgTilemap, int chunkX, int chunkY, int chunkSize)
    {
        for (int x = chunkX * chunkSize; x < (chunkX + 1) * chunkSize; x++)
        {
            for (int y = chunkY * chunkSize; y < (chunkY + 1) * chunkSize; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), null);
                bgTilemap.SetTile(new Vector3Int(x, y, 0), null);
                fgTilemap.SetTile(new Vector3Int(x, y, 0), null);
            }
        }
    }

    public void SetBlock(WorldBlock[,] map, Block block, int x, int y)
    {
        map[x, y] = new WorldBlock(block);
        Render();
    }

    //public void UpdateMap(int[,] map, Tilemap tilemap, int chunkX, int chunkY, int chunkSize)
    //{
    //    for (int x = chunkX * chunkSize; x < (chunkX + 1) * chunkSize; x++)
    //    {
    //        for (int y = chunkY * chunkSize; y < (chunkY + 1) * chunkSize; y++)
    //        {

    //            if (map[x, y] == 0)
    //            {
    //                tilemap.SetTile(new Vector3Int(x, y, 0), null);
    //            }
    //        }
    //    }
    //}

    public WorldBlock[,] PerlinNoise(WorldBlock[,] map, float seed)
    {
        int newPoint;
        float reduction = 0.5f;

        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            newPoint = Mathf.FloorToInt((Mathf.PerlinNoise(x, seed) - reduction) * map.GetUpperBound(1));

            newPoint += (map.GetUpperBound(1) / 2);

            for (int y = newPoint; y >= 0; y--)
            {
                map[x, y] = new WorldBlock(blocks.GetBlock("cobblestone"));
            }
        }
        return map;
    }

    //public Block[,] PerlinNoiseSmooth(Block[,] map, float seed, int interval)
    //{
    //    int newPoint, points;
    //    float reduction = 0.8f;
    //    float amplitude = 0.2f;
    //    Vector2Int currentPos, lastPos;
    //    List<int> noiseX = new List<int>();
    //    List<int> noiseY = new List<int>();

    //    for (int x = 0; x < map.GetUpperBound(0); x += interval)
    //    {
    //        newPoint = Mathf.FloorToInt((Mathf.PerlinNoise(x, (seed * reduction))) * map.GetUpperBound(1));
    //        noiseY.Add(newPoint);
    //        noiseX.Add(x);
    //    }

    //    points = noiseY.Count;
    //    for (int i = 1; i < points; i++)
    //    {
    //        currentPos = new Vector2Int(noiseX[i], noiseY[i]);
    //        lastPos = new Vector2Int(noiseX[i - 1], noiseY[i - 1]);

    //        Vector2 diff = currentPos - lastPos;

    //        float heightChange = diff.y / interval;
    //        float currHeight = lastPos.y;

    //        for (int x = lastPos.x; x < currentPos.x; x++)
    //        {
    //            for (int y = Mathf.FloorToInt(currHeight); y > 0; y--)
    //            {
    //                map[x, y] = blocks.GetBlock("cobblestone");
    //            }
    //            currHeight += heightChange;
    //        }
    //    }

    //    return map;
    //}

    public WorldBlock[,] RandomWalkTopSmoothed(WorldBlock[,] map, float seed, int minSectionWidth)
    {
        System.Random rand = new System.Random(seed.GetHashCode());

        int lastHeight = map.GetUpperBound(1) / 2;

        int nextMove = 0;
        int sectionWidth = 0;

        for (int x = 0; x <= map.GetUpperBound(0); x++)
        {
            nextMove = rand.Next(2);

            if (nextMove == 0 && lastHeight > 0 && sectionWidth > minSectionWidth)
            {
                lastHeight--;
                sectionWidth = 0;
            }
            else if (nextMove == 1 && lastHeight < map.GetUpperBound(1) && sectionWidth > minSectionWidth)
            {
                lastHeight++;
                sectionWidth = 0;
            }
            sectionWidth++;

            for (int y = lastHeight; y >= 0; y--)
            {
                map[x, y] = new WorldBlock(blocks.GetBlock("cobblestone"));
            }
        }

        return map;
    }

    public WorldBlock[,] PerlinNoiseCave(WorldBlock[,] map, int[] maxHeights, float modifier)
    {
        int newPoint;
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < maxHeights[x] - 30; y++)
            {
                newPoint = Mathf.RoundToInt(Mathf.PerlinNoise(x * modifier, y * modifier));
                map[x, y] = newPoint == 1 ? new WorldBlock(blocks.GetBlock("cobblestone")) : null;
            }
        }
        return map;
    }

    int GetMooreSurroundingTiles(WorldBlock[,] map, int x, int y, bool edgesAreWalls)
    {
        int tileCount = 0;

        for (int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++)
        {
            for (int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < map.GetUpperBound(0) && neighbourY >= 0 && neighbourY < map.GetUpperBound(1))
                {
                    if (neighbourX != x || neighbourY != y)
                    {
                        tileCount += map[neighbourX, neighbourY] == null ? 0 : 1;
                    }
                }
            }
        }
        return tileCount;
    }

    public WorldBlock[,] SmoothMooreCellularAutomata(WorldBlock[,] map, float seed, int fillPercent, bool edgesAreWalls, int smoothCount)
    {
        System.Random rand = new System.Random(seed.GetHashCode());

        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1) / 2 - 30; y++)
            {
                if (edgesAreWalls && (x == 0 || x == map.GetUpperBound(0) - 1 || y == 0 || y == map.GetUpperBound(1) / 2 - 31))
                {
                    map[x, y] = map[x, y] = new WorldBlock(blocks.GetBlock("cobblestone"));
                }
                else
                {
                    map[x, y] = (rand.Next(0, 100) < fillPercent) ? map[x, y] = new WorldBlock(blocks.GetBlock("cobblestone")) : null;
                }
            }
        }

        for (int i = 0; i < smoothCount; i++)
        {
            for (int x = 0; x < map.GetUpperBound(0); x++)
            {
                for (int y = 0; y < map.GetUpperBound(1) / 2 - 30; y++)
                {
                    int surroundingTiles = GetMooreSurroundingTiles(map, x, y, edgesAreWalls);

                    if (edgesAreWalls && (x == 0 || x == (map.GetUpperBound(0) - 1) || y == 0 || y == (map.GetUpperBound(1) / 2 - 31)))
                    {
                        map[x, y] = new WorldBlock(blocks.GetBlock("cobblestone"));
                    }
                    else if (surroundingTiles > 4)
                    {
                        map[x, y] = new WorldBlock(blocks.GetBlock("cobblestone"));
                    }
                    else if (surroundingTiles < 4)
                    {
                        map[x, y] = null;
                    }
                }
            }
        }

        return map;
    }

    public WorldBlock[,] OresGeneration(WorldBlock[,] map, float seed)
    {
        float[] frequencies = new float[4] { 10, 8, 2, 8 };
        float[] oreSeeds = new float[4]
        {
            UnityEngine.Random.Range(0f, 10f),
            UnityEngine.Random.Range(10f, 20f),
            UnityEngine.Random.Range(20f, 30f),
            UnityEngine.Random.Range(30f, 40f)
        };

        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); ++y)
            {
                if (map[x, y] != null)
                {
                    float oreNoise = Mathf.PerlinNoise((x / frequencies[0]) + seed, (y / frequencies[0]) + seed);

                    if (y > maxHeights[x] - 30 && oreNoise > 0.3)
                    {
                        map[x, y] = new WorldBlock(blocks.GetBlock("dirt"));
                    }
                    else if (oreNoise > 0.9)
                    {
                        map[x, y] = new WorldBlock(blocks.GetBlock("dirt"));
                    }

                    oreNoise = Mathf.PerlinNoise((x / frequencies[0]) + oreSeeds[0], (y / frequencies[0]) + oreSeeds[0]);
                    if (y < maxHeights[x] - 15 && oreNoise > 0.8)
                    {
                        map[x, y] = new WorldBlock(blocks.GetBlock("lead"));
                    }

                    oreNoise = Mathf.PerlinNoise((x / frequencies[1]) + oreSeeds[1], (y / frequencies[1]) + oreSeeds[1]);
                    if (y < maxHeights[x] - 30 && oreNoise > 0.8)
                    {
                        map[x, y] = new WorldBlock(blocks.GetBlock("aluminum"));
                    }

                    oreNoise = Mathf.PerlinNoise((x / frequencies[2]) + oreSeeds[2], (y / frequencies[2]) + oreSeeds[2]);
                    if (y < maxHeights[x] - 30 && oreNoise > 0.9)
                    {
                        map[x, y] = new WorldBlock(blocks.GetBlock("uranus"));
                    }

                    oreNoise = Mathf.PerlinNoise((x / frequencies[3]) + oreSeeds[3], (y / frequencies[3]) + oreSeeds[3]);
                    if (y < maxHeights[x] - 30 && oreNoise > 0.8)
                    {
                        map[x, y] = new WorldBlock(blocks.GetBlock("zink"));
                    }
                }
            }
        }

        return map;
    }

    public WorldBlock[,] TreesGeneration(WorldBlock[,] map)
    {
        WorldBlock[,] fgMap = new WorldBlock[map.GetUpperBound(0), map.GetUpperBound(1)];

        for (int x = 0; x < fgMap.GetLength(0); x++)
        {
            if (UnityEngine.Random.Range(0, 100) > 90)
            {
                int maxY = map.GetUpperBound(1);
                while (map[x, maxY] == null)
                {
                    maxY--;
                }

                int treeHeight = UnityEngine.Random.Range(0, 4);
                fgMap[x, maxY + 1] = new WorldBlock(blocks.GetBlock("tree_4"));
                for (int i = 0; i < treeHeight; i++)
                {
                    fgMap[x, maxY + i + 2] = UnityEngine.Random.Range(0, 100) > 50 ? new WorldBlock(blocks.GetBlock("tree_3")) : new WorldBlock(blocks.GetBlock("tree_2"));
                }
                fgMap[x, maxY + treeHeight + 2] = new WorldBlock(blocks.GetBlock("tree_1"));
            }
        }

        return fgMap;
    }

    public int[] GetMaxHeights(WorldBlock[,] map)
    {
        int[] maxHeights = new int[map.GetUpperBound(0)];
        for (int x = 0; x < map.GetUpperBound(0); ++x)
        {
            int highestPoint = map.GetUpperBound(1) - 1;

            for (int y = map.GetUpperBound(1) - 1; y > 0; y--)
            {
                if (map[x, y] != null)
                {
                    highestPoint = y;
                    maxHeights[x] = highestPoint;
                    break;
                }
            }
        }

        return maxHeights;
    }

    public void BreakBlock(float distanceToBreak)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = tilemap.WorldToCell(worldPoint);
        TileBase clickedTile = tilemap.GetTile(cellPosition);

        if (Vector3.Distance(Player.transform.position, cellPosition) <= distanceToBreak)
        {
            //if (Input.GetKey(KeyCode.Mouse1))
            //{
            //    RenderLight(lightMap, blockPressedCoords.x, blockPressedCoords.y, 15, false);
            //}

            if (clickedTile != null && blockPressedCoords == cellPosition)
            {
                blockPressedTime += Time.deltaTime;
            }
            else
            {
                blockPressedTime = 0;
                blockPressedCoords = cellPosition;
            }

            if (world[blockPressedCoords.x, blockPressedCoords.y] != null && blockPressedTime >= world[blockPressedCoords.x, blockPressedCoords.y].GetTimeToBreak())
            {
                GameObject newTileDrop = Instantiate(tileDrop, new Vector2(blockPressedCoords.x + 0.5f, blockPressedCoords.y + 1), Quaternion.identity);
                newTileDrop.GetComponent<SpriteRenderer>().sprite = tilemap.GetSprite(cellPosition);
                newTileDrop.GetComponent<Item>().item = blocks.GetBlock(world[blockPressedCoords.x, blockPressedCoords.y].name).item;

                if (world[blockPressedCoords.x, blockPressedCoords.y].getImportance())
                {
                    DestroyRoom(blockPressedCoords.x, blockPressedCoords.y);
                }

                world[blockPressedCoords.x, blockPressedCoords.y] = null;
            }
        }

        Render();
    }

    public bool SetBlockOnMap(float distanceToBreak, ItemScriptableObject inputObject)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = tilemap.WorldToCell(worldPoint);
        TileBase clickedTile = tilemap.GetTile(cellPosition);

        if (Vector3.Distance(Player.transform.position, cellPosition) <= distanceToBreak)
        {
            Debug.Log($"{inputObject.itemName}");
            world[cellPosition.x, cellPosition.y] = new WorldBlock(blocks.GetBlock(inputObject.itemName));
            Render();
            return true;
        }

        Render();
        return false;
    }
    public bool IsBlockOnCursor()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = tilemap.WorldToCell(worldPoint);
        return world[cellPosition.x, cellPosition.y] != null;
    }

    public int[,] SetDayLight(int[,] light, WorldBlock[,] map)
    {
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = map.GetUpperBound(1); y >= 0; y--)
            {
                light = RenderLight(light, x, y, 15, false);

                if (map[x, y] != null)
                {
                    break;
                }
            }
        }

        Render();
        return light;
    }

    public void TpObjectOnSurface(GameObject obj, int x)
    {
        for (int i = worldHeight - 1; i >= 0; i--)
        {
            if (world[x, i] != null)
            {
                obj.transform.position = new Vector3(x, i + 5, obj.transform.position.z);
                return;
            }
        }
    }

    public int GetFloorHeight()
    {
        int floorHeight = 0;
        for (int i = worldHeight - 1; i >= 0; i--)
        {
            if (world[worldWidth / 2, i] != null)
            {
                floorHeight = i;
                break;
            }
        }

        return floorHeight;
    }

    public void GenerateBase()
    {
        int floorHeight = 0;
        int baseWidth = 20;
        int baseHeight = 10;

        for (int i = worldHeight - 1; i >= 0; i--)
        {
            if (world[worldWidth / 2, i] != null)
            {
                floorHeight = i;
                break;
            }
        }

        for (int x = worldWidth / 2; x < worldWidth / 2 + baseWidth; x++)
        {
            world[x, floorHeight] = new WorldBlock(blocks.GetBlock("lead_bricks"));
            fgWorld[x, floorHeight] = null;
            bgWorld[x, floorHeight] = null;

            int currHeight = floorHeight - 1;

            while (world[x, currHeight] == null)
            {
                world[x, currHeight] = new WorldBlock(blocks.GetBlock("lead_bricks"));
                bgWorld[x, currHeight] = null;
                fgWorld[x, currHeight] = null;

                currHeight--;
            }

            for (int y = floorHeight + 1; y < worldHeight - 1; y++)
            {
                world[x, y] = null;
                bgWorld[x, y] = null;
                fgWorld[x, y] = null;
            }

            world[x, floorHeight + baseHeight] = new WorldBlock(blocks.GetBlock("lead_bricks"));
        }

        for (int y = floorHeight; y < floorHeight + baseHeight; y++)
        {
            world[worldWidth / 2, y] = new WorldBlock(blocks.GetBlock("lead_bricks"));
            world[worldWidth / 2 + baseWidth - 1, y] = new WorldBlock(blocks.GetBlock("lead_bricks"));
        }

        for (int x = worldWidth / 2; x < worldWidth / 2 + baseWidth; x++)
        {
            for (int y = floorHeight; y < floorHeight + baseHeight; y++)
            {
                bgWorld[x, y] = new WorldBlock(blocks.GetBlock("lead_bricks"));
            }
        }
        Instantiate(computer, new Vector2(worldWidth / 2 + baseWidth / 2, floorHeight + 2), Quaternion.identity);
        Render();
    }

    public Vector2Int[] CheckPlaceForRoom(Vector3Int cellPosition)
    {
        int leftBorderX = cellPosition.x;
        int leftBorderTop = cellPosition.y;
        int leftBorderDown = cellPosition.y;
        while (leftBorderX >= 0)
        {
            if (world[leftBorderX, cellPosition.y] != null && bricks.Any(brick => world[leftBorderX, cellPosition.y].CheckByName(brick)))
            {
                while (world[leftBorderX, leftBorderTop] != null && bricks.Any(brick => world[leftBorderX, leftBorderTop].CheckByName(brick)))
                {
                    leftBorderTop++;
                }
                leftBorderTop--;

                while (world[leftBorderX, leftBorderDown] != null && bricks.Any(brick => world[leftBorderX, leftBorderDown].CheckByName(brick)))
                {
                    leftBorderDown--;
                }
                leftBorderDown++;

                break;
            }

            leftBorderX--;
        }

        if (leftBorderX < 0)
            return null;

        int rightBorderX = cellPosition.x;
        int rightBorderTop = cellPosition.y;
        int rightBorderDown = cellPosition.y;
        while (rightBorderX < worldWidth)
        {
            if (world[rightBorderX, cellPosition.y] != null && bricks.Any(brick => world[rightBorderX, cellPosition.y].CheckByName(brick)))
            {
                while (world[rightBorderX, rightBorderTop] != null && bricks.Any(brick => world[rightBorderX, rightBorderTop].CheckByName(brick)))
                {
                    rightBorderTop++;
                }
                rightBorderTop--;

                while (world[rightBorderX, rightBorderDown] != null && bricks.Any(brick => world[rightBorderX, rightBorderDown].CheckByName(brick)))
                {
                    rightBorderDown--;
                }
                rightBorderDown++;

                break;
            }

            rightBorderX++;
        }

        if (rightBorderX >= worldWidth)
            return null;

        int roofY = Mathf.Min(leftBorderTop, rightBorderTop);
        while (roofY >= 0)
        {
            int currX = leftBorderX;
            while (currX < rightBorderX && world[currX, roofY] != null && bricks.Any(brick => world[currX, roofY].CheckByName(brick)))
            {
                currX++;
            }

            if (currX == rightBorderX)
                break;

            roofY--;
        }

        if (roofY < 0)
            return null;

        int floorY = Mathf.Max(leftBorderDown, rightBorderDown);
        while (floorY < worldHeight)
        {
            int currX = leftBorderX;
            while (currX < rightBorderX && world[currX, floorY] != null && bricks.Any(brick => world[currX, floorY].CheckByName(brick)))
            {
                currX++;
            }

            if (currX == rightBorderX)
                break;

            floorY++;
        }

        if (floorY >= worldHeight)
            return null;

        if (cellPosition.x < leftBorderX || cellPosition.x > rightBorderX || cellPosition.y < floorY || cellPosition.y > roofY)
            return null;

        Debug.Log(leftBorderX + " " + leftBorderDown + " " + leftBorderTop + " " + rightBorderX + " " + rightBorderDown + " " + rightBorderTop + " " + roofY + " " + floorY);

        return new Vector2Int[] { new Vector2Int(leftBorderX, floorY), new Vector2Int(rightBorderX, roofY) };
    }

    public void SetRoom(Room room)
    {
        for (int x = room.GetLeftDownCorner().x; x <= room.GetRightUpCorner().x; x++)
        {
            world[x, room.GetLeftDownCorner().y].setImportance(true);
            world[x, room.GetRightUpCorner().y].setImportance(true);
        }

        for (int y = room.GetLeftDownCorner().y; y <= room.GetRightUpCorner().y; y++)
        {
            world[room.GetLeftDownCorner().x, y].setImportance(true);
            world[room.GetRightUpCorner().x, y].setImportance(true);
        }

        for (int x = room.GetLeftDownCorner().x; x <= room.GetRightUpCorner().x; x++)
        {
            for (int y = room.GetLeftDownCorner().y; y <= room.GetRightUpCorner().y; y++)
            {
                bgWorld[x, y] = new WorldBlock(blocks.GetBlock("lead"));
            }
        }
        Render();
    }

    public void DestroyRoom(int blockX, int blockY)
    {
        Room room = Base.GetRoomFromCoords(blockX, blockY);

        for (int x = room.GetLeftDownCorner().x; x <= room.GetRightUpCorner().x; x++)
        {
            world[x, room.GetLeftDownCorner().y].setImportance(false);
            world[x, room.GetRightUpCorner().y].setImportance(false);
        }

        for (int y = room.GetLeftDownCorner().y; y <= room.GetRightUpCorner().y; y++)
        {
            world[room.GetLeftDownCorner().x, y].setImportance(false);
            world[room.GetRightUpCorner().x, y].setImportance(false);
        }

        for (int x = room.GetLeftDownCorner().x; x <= room.GetRightUpCorner().x; x++)
        {
            for (int y = room.GetLeftDownCorner().y; y <= room.GetRightUpCorner().y; y++)
            {
                Debug.Log(bgWorld[x, y].name);
                bgWorld[x, y] = null;
                Debug.Log(x + " " + y);
            }
        }

        Base.DestroyRoom(room);
        Render();
    }

    private void RenderAllMapLight()
    {
        for(int x = 0; x < worldWidth - 1; x++)
        {
            for(int y = 0; y < worldHeight - 1; y++)
            {
                if (world[x, y] != null)
                {
                    tilemap.SetColor(new Vector3Int(x, y, 0), new Color(lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f));
                }

                if (bgWorld[x, y] != null)
                {
                    bgTilemap.SetColor(new Vector3Int(x, y, 0), new Color(lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f));
                }

                if (fgWorld[x, y] != null)
                {
                    fgTilemap.SetColor(new Vector3Int(x, y, 0), new Color(lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f));
                }
            }
        }
    }

    private void Render()
    {
        for (int i = renderLeftBorder; i < renderRightBorder; ++i)
        {
            for (int j = renderDownBorder; j < renderUpBorder; ++j)
            {
                RenderMap(i, j, chunkSize);
            }
        }

        for (int i = prevRenderLeftBorder; i < prevRenderRightBorder; ++i)
        {
            for (int j = prevRenderDownBorder; j < prevRenderUpBorder; ++j)
            {
                if (i < renderLeftBorder || i >= renderRightBorder || j < renderDownBorder || j >= renderUpBorder)
                {
                    ClearTileMap(tilemap, bgTilemap, fgTilemap, i, j, chunkSize);
                }
            }
        }
    }

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");

        Load();
        if (world == null)
        {
            seed = UnityEngine.Random.Range(0.05f, 0.15f);
            Debug.Log(seed);

            world = GenerateArray(worldWidth, worldHeight, true);
            //world = PerlinNoiseSmooth(world, seed, 100);
            world = RandomWalkTopSmoothed(world, seed, 10);
            bgWorld = CreateBGWall(world);
            maxHeights = GetMaxHeights(world);
            //world = PerlinNoiseCave(world, maxHeights, seed * 1.4f);
            world = SmoothMooreCellularAutomata(world, seed, 50, true, 10);
            world = OresGeneration(world, seed);
            fgWorld = TreesGeneration(world);
            lightMap = new int[worldWidth, worldHeight];
            lightMap = SetDayLight(lightMap, bgWorld);

            GenerateBase();
        }

        TpObjectOnSurface(Player, worldWidth / 2);
        TpObjectOnSurface(GameObject.FindGameObjectWithTag("NPC"), worldWidth / 2 - 10);

        playerChunkX = Mathf.Clamp((int)Player.transform.position.x / chunkSize, 0, worldWidth / chunkSize);
        playerChunkY = Mathf.Clamp((int)Player.transform.position.y / chunkSize, 0, worldHeight / chunkSize);

        prevPlayerChunkX = playerChunkX;
        prevPlayerChunkY = playerChunkY;

        RenderAllMapLight();
        Render();

        prevPlayerPositionX = (int)Player.transform.position.x;
        prevPlayerPositionY = (int)Player.transform.position.y;

        prevRenderLeftBorder = Mathf.Clamp(playerChunkX - renderDistance, 0, worldWidth / chunkSize);
        prevRenderRightBorder = Mathf.Clamp(playerChunkX + renderDistance, 0, worldWidth / chunkSize);
        prevRenderDownBorder = Mathf.Clamp(playerChunkY - renderDistance, 0, worldHeight / chunkSize);
        prevRenderUpBorder = Mathf.Clamp(playerChunkY + renderDistance, 0, worldHeight / chunkSize);
    }

    private void Update()
    {
        playerChunkX = Mathf.Clamp((int)Player.transform.position.x / chunkSize, 0, worldWidth / chunkSize);
        playerChunkY = Mathf.Clamp((int)Player.transform.position.y / chunkSize, 0, worldHeight / chunkSize);

        playerPositionX = (int)Player.transform.position.x;
        playerPositionY = (int)Player.transform.position.y;

        renderLeftBorder = Mathf.Clamp(playerChunkX - renderDistance, 0, worldWidth / chunkSize);
        renderRightBorder = Mathf.Clamp(playerChunkX + renderDistance, 0, worldWidth / chunkSize);
        renderDownBorder = Mathf.Clamp(playerChunkY - renderDistance, 0, worldHeight / chunkSize);
        renderUpBorder = Mathf.Clamp(playerChunkY + renderDistance, 0, worldHeight / chunkSize);

        if(prevPlayerChunkX != playerChunkX || prevPlayerChunkY != playerChunkY)
        {
            Render();
        }

        if (prevPlayerPositionX != playerPositionX || prevPlayerPositionY != playerPositionY)
        {
            //lightMap = RenderLight(lightMap, prevPlayerPositionX, prevPlayerPositionY, 15, true);
            lightMap = RenderLight(lightMap, playerPositionX, playerPositionY, 15, false);
            RenderLightWorld(playerPositionX, playerPositionY);
        }

        prevRenderLeftBorder = renderLeftBorder;
        prevRenderRightBorder = renderRightBorder;
        prevRenderDownBorder = renderDownBorder;
        prevRenderUpBorder = renderUpBorder;

        prevPlayerPositionX = playerPositionX;
        prevPlayerPositionY = playerPositionY;

        prevPlayerChunkX = playerChunkX;
        prevPlayerChunkY = playerChunkY;
        for (int i = 0; i < QuickslotPanel.childCount; i++)
        {
            if (QuickslotPanel.GetChild(i).GetComponent<InventorySlot>().item != null)
            {
                if (QuickslotPanel.GetChild(i).GetComponent<Image>().sprite == selectedSprite)
                {
                    if (QuickslotPanel.GetChild(i).GetComponent<InventorySlot>().item.itemName == "kirk")
                    {
                        if (Input.GetKey(KeyCode.Mouse0))
                        {
                            BreakBlock(5);
                        }
                    }
                }
            }
        }
    }
}