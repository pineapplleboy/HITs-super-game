using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Block
{
    [SerializeField] public string name;
    [SerializeField] public TileBase tile;
    [SerializeField] private float timeToBreak;
    [SerializeField] private int lightLvl = 15;

    public Block(Block block)
    {
        this.name = block.name;
        this.tile = block.tile;
        this.timeToBreak = block.timeToBreak;
        this.lightLvl = block.lightLvl;
    }

    public float GetTimeToBreak()
    {
        return timeToBreak;
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
            if(block.name == name)
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

    [SerializeField] int worldWidth;
    [SerializeField] int worldHeight;
    [SerializeField] int chunkSize;
    [SerializeField] int renderDistance;

    public Block[,] world;
    public Block[,] bgWorld;
    public Block[,] fgWorld;
    public int[,] lightMap;
    private int[] maxHeights;

    private float seed;
    private GameObject Player;

    private int playerChunkX;
    private int playerChunkY;

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

    private float blockPressedTime;
    private Vector3Int blockPressedCoords;

    public Block[,] GenerateArray(int width, int height, bool empty)
    {
        Block[,] map = new Block[width, height];

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
                    map[x, y] = blocks.GetBlock("cobblestone");
                }
            }
        }

        return map;
    }

    public int[,] RenderLight(int[,] lightMap, int x, int y, int lightLvl, bool reduceLight)
    {
        if(reduceLight && lightLvl != lightMap[x, y])
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

    public void RenderMap(Block[,] map, Block[,] bgMap, Block[,] fgMap, int[,] lightMap, Tilemap tilemap, Tilemap bgTilemap, Tilemap fgTilemap, int chunkX, int chunkY, int chunkSize)
    {
        //tilemap.ClearAllTiles();

        for (int x = chunkX * chunkSize; x < (chunkX + 1) * chunkSize; x++)
        {
            for (int y = chunkY * chunkSize; y < (chunkY + 1) * chunkSize; y++)
            {

                if (map[x, y] != null)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), map[x, y].tile);
                    tilemap.SetColor(new Vector3Int(x, y, 0), new Color(lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f));
                }

                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), null);
                }

                if (bgMap[x, y] != null)
                {
                    bgTilemap.SetTile(new Vector3Int(x, y, 0), bgMap[x, y].tile);
                    bgTilemap.SetColor(new Vector3Int(x, y, 0), new Color(lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f));
                }

                if (fgMap[x, y] != null)
                {
                    fgTilemap.SetTile(new Vector3Int(x, y, 0), fgMap[x, y].tile);
                    fgTilemap.SetColor(new Vector3Int(x, y, 0), new Color(lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f, lightMap[x, y] / 15.0f));
                }
            }
        }
    }

    public Block[,] CreateBGWall(Block[,] world)
    {
        Block[,] bgWorld = new Block[world.GetUpperBound(0), world.GetUpperBound(1)];

        for(int i = 0; i < world.GetUpperBound(0); ++i)
        {
            for(int j = 0; j < world.GetUpperBound(1); ++j)
            {

                if (world[i, j] != null)
                {
                    bgWorld[i, j] = blocks.GetBlock("cobblestone");
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

    public void SetBlock(Block[,] map, Block block, int x, int y)
    {
        map[x, y] = block;
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

    public Block[,] PerlinNoise(Block[,] map, float seed)
    {
        int newPoint;
        float reduction = 0.5f;

        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            newPoint = Mathf.FloorToInt((Mathf.PerlinNoise(x, seed) - reduction) * map.GetUpperBound(1));

            newPoint += (map.GetUpperBound(1) / 2);

            for (int y = newPoint; y >= 0; y--)
            {
                map[x, y] = blocks.GetBlock("cobblestone");
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

    public Block[,] RandomWalkTopSmoothed(Block[,] map, float seed, int minSectionWidth)
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
                map[x, y] = blocks.GetBlock("cobblestone");
            }
        }

        return map;
    }

    public Block[,] PerlinNoiseCave(Block[,] map, int[] maxHeights, float modifier)
    {
        int newPoint;
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < maxHeights[x] - 30; y++)
            {
                newPoint = Mathf.RoundToInt(Mathf.PerlinNoise(x * modifier, y * modifier));
                map[x, y] = newPoint == 1 ? blocks.GetBlock("cobblestone") : null;
            }
        }
        return map;
    }

    int GetMooreSurroundingTiles(Block[,] map, int x, int y, bool edgesAreWalls)
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

    public Block[,] SmoothMooreCellularAutomata(Block[,] map, float seed, int fillPercent, bool edgesAreWalls, int smoothCount)
    {
        System.Random rand = new System.Random(seed.GetHashCode());

        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1) / 2 - 30; y++)
            {
                if (edgesAreWalls && (x == 0 || x == map.GetUpperBound(0) - 1 || y == 0 || y == map.GetUpperBound(1) / 2 - 31))
                {
                    map[x, y] = map[x, y] = blocks.GetBlock("cobblestone");
                }
                else
                {
                    map[x, y] = (rand.Next(0, 100) < fillPercent) ? map[x, y] = blocks.GetBlock("cobblestone") : null;
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
                        map[x, y] = blocks.GetBlock("cobblestone");
                    }
                    else if (surroundingTiles > 4)
                    {
                        map[x, y] = blocks.GetBlock("cobblestone");
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

    public Block[,] OresGeneration(Block[,] map, float seed)
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
                        map[x, y] = blocks.GetBlock("dirt");
                    }
                    else if (oreNoise > 0.9)
                    {
                        map[x, y] = blocks.GetBlock("dirt");
                    }

                    oreNoise = Mathf.PerlinNoise((x / frequencies[0]) + oreSeeds[0], (y / frequencies[0]) + oreSeeds[0]);
                    if (y < maxHeights[x] - 15 && oreNoise > 0.8)
                    {
                        map[x, y] = blocks.GetBlock("lead");
                    }

                    oreNoise = Mathf.PerlinNoise((x / frequencies[1]) + oreSeeds[1], (y / frequencies[1]) + oreSeeds[1]);
                    if (y < maxHeights[x] - 30 && oreNoise > 0.8)
                    {
                        map[x, y] = blocks.GetBlock("aluminum");
                    }

                    oreNoise = Mathf.PerlinNoise((x / frequencies[2]) + oreSeeds[2], (y / frequencies[2]) + oreSeeds[2]);
                    if (y < maxHeights[x] - 30 && oreNoise > 0.9)
                    {
                        map[x, y] = blocks.GetBlock("uranus");
                    }

                    oreNoise = Mathf.PerlinNoise((x / frequencies[3]) + oreSeeds[3], (y / frequencies[3]) + oreSeeds[3]);
                    if (y < maxHeights[x] - 30 && oreNoise > 0.8)
                    {
                        map[x, y] = blocks.GetBlock("zink");
                    }
                }
            }
        }

        return map;
    }

    public Block[,] TreesGeneration(Block[,] map)
    {
        Block[,] fgMap = new Block[map.GetUpperBound(0), map.GetUpperBound(1)];

        for(int x = 0; x < fgMap.GetLength(0); x++)
        {
            if(UnityEngine.Random.Range(0, 100) > 90)
            {
                int maxY = map.GetUpperBound(1);
                while (map[x, maxY] == null)
                {
                    maxY--;
                }

                int treeHeight = UnityEngine.Random.Range(0, 4);
                fgMap[x, maxY + 1] = blocks.GetBlock("tree_4");
                for(int i = 0; i < treeHeight; i++)
                {
                    fgMap[x, maxY + i + 2] = UnityEngine.Random.Range(0, 100) > 50 ? blocks.GetBlock("tree_3") : blocks.GetBlock("tree_2");
                }
                fgMap[x, maxY + treeHeight + 2] = blocks.GetBlock("tree_1");
            }
        }

        return fgMap;
    }

    public int[] GetMaxHeights(Block[,] map)
    {
        int[] maxHeights = new int[map.GetUpperBound(0)];
        for(int x = 0; x < map.GetUpperBound(0); ++x)
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

    public void DetectTilePressed(float distanceToBreak)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = tilemap.WorldToCell(worldPoint);
        TileBase clickedTile = tilemap.GetTile(cellPosition);

        if(Vector3.Distance(Player.transform.position, cellPosition) <= distanceToBreak)
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                RenderLight(lightMap, blockPressedCoords.x, blockPressedCoords.y, 15, false);
            }

            if (clickedTile != null && blockPressedCoords == cellPosition)
            {
                blockPressedTime += Time.deltaTime;
            }
            else
            {
                blockPressedTime = 0;
                blockPressedCoords = cellPosition;
            }

            if (/*blockPressedTime >= world[blockPressedCoords.x, blockPressedCoords.y].GetTimeToBreak()*/true)
            {
                world[blockPressedCoords.x, blockPressedCoords.y] = null;
            }
        }
    }

    public int[,] SetDayLight(int[,] light, Block[,] map)
    {
        for(int x = 0; x < map.GetUpperBound(0); x++)
        {
            for(int y = map.GetUpperBound(1); y >= 0 ; y--)
            {
                light = RenderLight(light, x, y, 15, false);

                if (map[x, y] != null)
                {
                    break;
                }
            }
        }

        return light;
    }

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");

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

        playerChunkX = Mathf.Clamp((int)Player.transform.position.x / chunkSize, 0, worldWidth / chunkSize);
        playerChunkY = Mathf.Clamp((int)Player.transform.position.y / chunkSize, 0, worldHeight / chunkSize);

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

        for (int i = renderLeftBorder; i < renderRightBorder; ++i)
        {
            for (int j = renderDownBorder; j < renderUpBorder; ++j)
            {
                RenderMap(world, bgWorld, fgWorld, lightMap, tilemap, bgTilemap, fgTilemap, i, j, chunkSize);
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

        if(prevPlayerPositionX != playerPositionX || prevPlayerPositionY != playerPositionY)
        {
            //lightMap = RenderLight(lightMap, prevPlayerPositionX, prevPlayerPositionY, 15, true);
            lightMap = RenderLight(lightMap, playerPositionX, playerPositionY, 15, false);
        }

        prevRenderLeftBorder = renderLeftBorder;
        prevRenderRightBorder = renderRightBorder;
        prevRenderDownBorder = renderDownBorder;
        prevRenderUpBorder = renderUpBorder;

        prevPlayerPositionX = playerPositionX;
        prevPlayerPositionY = playerPositionY;

        if (Input.GetKey(KeyCode.Mouse0))
        {
            DetectTilePressed(5);
        }
    }
}