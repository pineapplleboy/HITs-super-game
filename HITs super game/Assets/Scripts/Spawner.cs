using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemies;
    private List<int> enemiesSpawnProbability;

    public GameObject[] raidEnemies;
    private List<int> raidEnemiesSpawnProbability;

    public static List<Enemy> allSpawnedEnemies;

    private List<int> enemiesMinHp;
    private List<int> enemiesMaxHp;

    private List<List<int>> enemiesResistance;

    private int randEnemy;
    private int randEnemyValue;
    private Vector2 spawnPosition;

    public static int spawnRate = 3;
    private int maxNearEnemies = 5;
    private int raidMaxNearEnemies = 10;

    private float raidEnemiesGrowth = 1.7f;

    private float spawnCd = 4.5f;
    private float raidSpawnCd = 2.5f;

    public static int currentNearEnemies = 0;

    private float timer = 0f;

    public static bool isAttack = false;
    public static List<int> listOfAttackEnemies;

    private Transform player;

    private WorldGeneration world;

    private COMPUTER computer;

    private Vector2 computerCoord;

    private void Start()
    {
        currentNearEnemies = 0;

        listOfAttackEnemies = new List<int>(enemies.Length) { 0 };
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        world = GameObject.FindGameObjectWithTag("World").GetComponent<WorldGeneration>();

        enemiesMinHp = new List<int>() { 30, 150, 75, 40 };
        enemiesMaxHp = new List<int>() { 40, 200, 90, 60 };

        enemiesResistance = new List<List<int>>() { new List<int>() { 20, 20, 20 }, new List<int>() { 30, 0, 0 }, 
            new List<int>() { 0, 15, 0 }, new List<int>() { 0, 0, 0 } };

        enemiesSpawnProbability = new List<int>() { 10, 45, 35, 10 };
        raidEnemiesSpawnProbability = new List<int>() { 25, 25, 25, 25 };

        allSpawnedEnemies = new List<Enemy>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        raidMaxNearEnemies = (int) (10 * Mathf.Pow(raidEnemiesGrowth, DayTime.raidsCounter));
        raidSpawnCd = 2.5f - DayTime.raidsCounter / 10;
        raidSpawnCd = Mathf.Max(raidSpawnCd, 2f);

        if (DayTime.raidsCounter == 0)
        {
            raidEnemiesSpawnProbability = new List<int>() { 0, 100, 0, 0 };
        }
        else if (DayTime.raidsCounter == 1)
        {
            raidEnemiesSpawnProbability = new List<int>() { 30, 40, 30, 0 };
        }
        else if (DayTime.raidsCounter == 2)
        {
            raidEnemiesSpawnProbability = new List<int>() { 20, 40, 30, 10 };
        }
        else if (DayTime.raidsCounter == 3)
        {
            raidEnemiesSpawnProbability = new List<int>() { 0, 0, 0, 100 };
        }
        else
        {
            raidEnemiesSpawnProbability = new List<int>() { 25, 35, 25, 15 };
        }

        //computerCoord = new Vector2(world.worldWidth / 2 + 20 / 2, world.GetFloorHeight() / 2);
        computerCoord = GameObject.FindGameObjectWithTag("COMPUTER").GetComponent<COMPUTER>().transform.position;

        if (!isAttack)
        {
            if (timer >= spawnCd && currentNearEnemies < maxNearEnemies)
            {

                if (CheckDistance(computerCoord, player.transform.position) > 100)
                {
                    SpawnEnemy();
                    //SpawnRaidEnemy();
                }

            }
        }
        else
        {
            SickoMode();
        }
        
    }

    private void SickoMode()
    {
        if (timer >= raidSpawnCd && currentNearEnemies < raidMaxNearEnemies)
        {

            SpawnRaidEnemy();

        }
    }

    private void SpawnRaidEnemy()
    {
        randEnemyValue = Random.Range(1, 101);
        int currentSum = 0;
        for (int i = 0; i < raidEnemies.Length; i++)
        {
            currentSum += raidEnemiesSpawnProbability[i];
            if (randEnemyValue <= currentSum)
            {
                randEnemy = i;
                break;
            }
        }
        //randEnemy = 1;

        //spawnPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spawnPosition = GenerateRaidPosition();
        //spawnPosition = GeneratePosition();

        if (!world.IsBlock((int)spawnPosition.x, (int)spawnPosition.y))
        {
            Enemy newEnemy = raidEnemies[randEnemy].GetComponent<Enemy>();
            newEnemy.SetOnRaid();

            newEnemy.SetHp(Random.Range(enemiesMinHp[randEnemy], enemiesMaxHp[randEnemy] + 1));
            newEnemy.SetDamageResistance(enemiesResistance[randEnemy]);

            newEnemy.SetIndex(allSpawnedEnemies.Count);

            allSpawnedEnemies.Add(Instantiate(newEnemy, spawnPosition, Quaternion.identity));

            currentNearEnemies++;
            timer = 0f;
        }

    }

    private void SpawnEnemy()
    {
        //randEnemy = Random.Range(0, enemies.Length);
        //randEnemy = 1;

        randEnemyValue = Random.Range(1, 101);
        int currentSum = 0;
        for (int i = 0; i < enemies.Length; i++)
        {
            currentSum += enemiesSpawnProbability[i];
            if (randEnemyValue <= currentSum)
            {
                randEnemy = i;
                break;
            }
        }
        //randEnemy = 1;

        //spawnPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spawnPosition = GeneratePosition();

        if (!world.IsBlock((int)spawnPosition.x, (int)spawnPosition.y))
        {
            Enemy newEnemy = enemies[randEnemy].GetComponent<Enemy>();
            newEnemy.SetHp(Random.Range(enemiesMinHp[randEnemy], enemiesMaxHp[randEnemy] + 1));
            newEnemy.SetDamageResistance(enemiesResistance[randEnemy]);

            newEnemy.SetIndex(allSpawnedEnemies.Count);

            allSpawnedEnemies.Add(Instantiate(newEnemy, spawnPosition, Quaternion.identity));

            currentNearEnemies++;
            timer = 0f;
        }
        
    }

    private Vector2 GenerateRaidPosition()
    {
        float deltaX = Random.Range(40, 60);
        float deltaY = Random.Range(0, 10);
        
        float positionX, positionY;

        if (Random.Range(0, 2) == 0)
        {
            positionX = computerCoord.x + deltaX;
        }
        else
        {
            positionX = computerCoord.x - deltaX;
        }

        positionY = computerCoord.y + deltaY;

        Vector2 position = new Vector2(positionX, positionY);

        return position;
    }

    private Vector2 GeneratePosition()
    {
        float deltaX = Random.Range(30, 50);
        float deltaY = Random.Range(0, 10);
        //deltaX = 10;
        float positionX, positionY;

        if (Input.GetKey(KeyCode.D))
        {
            positionX = player.position.x + deltaX;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            positionX = player.position.x - deltaX;
        } 
        else if (Random.Range(0, 2) == 0)
        {
            positionX = player.position.x + deltaX;
        }
        else
        {
            positionX = player.position.x - deltaX;
        }

        if (Random.Range(0, 2) == 0)
        {
            positionY = player.position.y - deltaY;
        }
        else
        {
            positionY = player.position.y + deltaY;
        }

        Vector2 position = new Vector2(positionX, positionY);

        return position;
    }

    private float CheckDistance(Vector2 first, Vector2 second)
    {
        return Mathf.Sqrt(Mathf.Pow(first.x - second.x, 2) + Mathf.Pow(first.y - second.y, 2));
    }
}
