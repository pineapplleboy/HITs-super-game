using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Block
{
    [SerializeField] public string name;
    [SerializeField] public TileBase tile;
    [SerializeField] private float timeToBreak;

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
                return block;
            }
        }

        return null;
    }
}

public class WorldGeneration : MonoBehaviour
{
    [SerializeField] public Blocks blocks;

    [SerializeField] Tilemap tilemap;

    [SerializeField] int worldWidth;
    [SerializeField] int worldHeight;
    [SerializeField] int chunkSize;
    [SerializeField] int renderDistance;

    public static Block[,] world;
    private int[] maxHeights;

    private float seed;
    private GameObject Player;

    private int playerChunkX;
    private int playerChunkY;

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

    public void RenderMap(Block[,] map, Tilemap tilemap, int chunkX, int chunkY, int chunkSize)
    {
        //tilemap.ClearAllTiles();

        for (int x = chunkX * chunkSize; x < (chunkX + 1) * chunkSize; x++)
        {
            for (int y = chunkY * chunkSize; y < (chunkY + 1) * chunkSize; y++)
            {

                if (map[x, y] != null)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), map[x, y].tile);
                }

                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
    }
    public static void ClearTileMap(Tilemap tilemap, int chunkX, int chunkY, int chunkSize)
    {
        for (int x = chunkX * chunkSize; x < (chunkX + 1) * chunkSize; x++)
        {
            for (int y = chunkY * chunkSize; y < (chunkY + 1) * chunkSize; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), null);
            }
        }
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

    public Block[,] PerlinNoiseSmooth(Block[,] map, float seed, int interval)
    {
        if (interval > 1)
        {
            int newPoint, points;
            float reduction = 0.5f;

            Vector2Int currentPos, lastPos;
            List<int> noiseX = new List<int>();
            List<int> noiseY = new List<int>();

            for (int x = 0; x < map.GetUpperBound(0); x += interval)
            {
                newPoint = Mathf.FloorToInt((Mathf.PerlinNoise(x, (seed * reduction))) * map.GetUpperBound(1));
                noiseY.Add(newPoint);
                noiseX.Add(x);
            }

            points = noiseY.Count;
            for (int i = 1; i < points; i++)
            {
                currentPos = new Vector2Int(noiseX[i], noiseY[i]);
                lastPos = new Vector2Int(noiseX[i - 1], noiseY[i - 1]);

                Vector2 diff = currentPos - lastPos;

                float heightChange = diff.y / interval;
                float currHeight = lastPos.y;

                for (int x = lastPos.x; x < currentPos.x; x++)
                {
                    for (int y = Mathf.FloorToInt(currHeight); y > 0; y--)
                    {
                        map[x, y] = blocks.GetBlock("cobblestone");
                    }
                    currHeight += heightChange;
                }
            }
        }
        else
        {
            map = PerlinNoise(map, seed);
        }

        return map;
    }

    public Block[,] PerlinNoiseCave(Block[,] map, int[] maxHeights, float modifier)
    {
        int newPoint;
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < maxHeights[x] - 50; y++)
            {
                newPoint = Mathf.RoundToInt(Mathf.PerlinNoise(x * modifier, y * modifier));
                map[x, y] = newPoint == 1 ? blocks.GetBlock("cobblestone") : null;
            }
        }
        return map;
    }

    public Block[,] OresGeneration(Block[,] map, int[] maxHeights, float seed)
    {
        float frequency = 10;
        for(int x = 0; x < map.GetUpperBound(0); x++)
        {
            for(int y = 0; y < map.GetUpperBound(1); ++y)
            {
                if (map[x, y] != null)
                {
                    float oreNoise = Mathf.PerlinNoise((x / frequency) + seed, (y / frequency) + seed);

                    if (y > maxHeights[x] - 30 && oreNoise > 0.3)
                    {
                        map[x, y] = blocks.GetBlock("dirt");
                    }
                    else if(oreNoise > 0.9)
                    {
                        map[x, y] = blocks.GetBlock("dirt");
                    }

                    if (y < maxHeights[x] - 15 && oreNoise > 0.8)
                    {
                        map[x, y] = map[x, y] = blocks.GetBlock("iron");

                    }

                    if (y < maxHeights[x] - 30 && oreNoise > 0.65)
                    {
                        map[x, y] = map[x, y] = blocks.GetBlock("diamond");
                    }
                }
            }
        }

        return map;
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

    void DetectTilePressed(float distanceToBreak)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = tilemap.WorldToCell(worldPoint);
        TileBase clickedTile = tilemap.GetTile(cellPosition);

        if(Vector3.Distance(Player.transform.position, cellPosition) <= distanceToBreak)
        {

            if (clickedTile != null && blockPressedCoords == cellPosition)
            {
                blockPressedTime += Time.deltaTime;
            }
            else
            {
                blockPressedTime = 0;
                blockPressedCoords = cellPosition;
            }

            if (blockPressedTime >= world[blockPressedCoords.x, blockPressedCoords.y].GetTimeToBreak())
            {
                world[blockPressedCoords.x, blockPressedCoords.y] = null;
            }
        }
    }

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");

        seed = UnityEngine.Random.Range(0.05f, 0.15f);
        Debug.Log(seed);

        world = GenerateArray(worldWidth, worldHeight, true);
        world = PerlinNoiseSmooth(world, seed, 25);
        maxHeights = GetMaxHeights(world);
        world = PerlinNoiseCave(world, maxHeights, seed * 0.5f);
        world = OresGeneration(world, maxHeights, seed);

        playerChunkX = Mathf.Clamp((int)Player.transform.position.x / chunkSize, 0, worldWidth / chunkSize);
        playerChunkY = Mathf.Clamp((int)Player.transform.position.y / chunkSize, 0, worldHeight / chunkSize);

        prevRenderLeftBorder = Mathf.Clamp(playerChunkX - renderDistance, 0, worldWidth / chunkSize);
        prevRenderRightBorder = Mathf.Clamp(playerChunkX + renderDistance, 0, worldWidth / chunkSize);
        prevRenderDownBorder = Mathf.Clamp(playerChunkY - renderDistance, 0, worldHeight / chunkSize);
        prevRenderUpBorder = Mathf.Clamp(playerChunkY + renderDistance, 0, worldHeight / chunkSize);
    }

    private void Update()
    {
        playerChunkX = Mathf.Clamp((int)Player.transform.position.x / chunkSize, 0, worldWidth / chunkSize);
        playerChunkY = Mathf.Clamp((int)Player.transform.position.y / chunkSize, 0, worldHeight / chunkSize);
        
        renderLeftBorder = Mathf.Clamp(playerChunkX - renderDistance, 0, worldWidth / chunkSize);
        renderRightBorder = Mathf.Clamp(playerChunkX + renderDistance, 0, worldWidth / chunkSize);
        renderDownBorder = Mathf.Clamp(playerChunkY - renderDistance, 0, worldHeight / chunkSize);
        renderUpBorder = Mathf.Clamp(playerChunkY + renderDistance, 0, worldHeight / chunkSize);

        for (int i = renderLeftBorder; i < renderRightBorder; ++i)
        {
            for (int j = renderDownBorder; j < renderUpBorder; ++j)
            {
                RenderMap(world, tilemap, i, j, chunkSize);
            }
        }

        for (int i = prevRenderLeftBorder; i < prevRenderRightBorder; ++i)
        {
            for (int j = prevRenderDownBorder; j < prevRenderUpBorder; ++j)
            {
                if (i < renderLeftBorder || i >= renderRightBorder || j < renderDownBorder || j >= renderUpBorder)
                {
                    ClearTileMap(tilemap, i, j, chunkSize);
                }
            }
        }

        prevRenderLeftBorder = renderLeftBorder;
        prevRenderRightBorder = renderRightBorder;
        prevRenderDownBorder = renderDownBorder;
        prevRenderUpBorder = renderUpBorder;

        if (Input.GetKey(KeyCode.Mouse0))
        {
            DetectTilePressed(5);
        }
    }
}