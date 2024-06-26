using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemies;
    private int randEnemy;
    private Vector2 spawnPosition;

    public static int spawnRate = 3;
    private int maxNearEnemies = 0;

    public static int currentNearEnemies = 0;

    private float timer = 0f;

    public static bool isAttack = false;
    public static List<int> listOfAttackEnemies;

    private Transform player;

    private WorldGeneration world;

    private void Start()
    {
        listOfAttackEnemies = new List<int>(enemies.Length) { 0 };
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        world = GameObject.FindGameObjectWithTag("World").GetComponent<WorldGeneration>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (!isAttack)
        {
            if (timer >= 3 && currentNearEnemies < maxNearEnemies)
            {

                SpawnEnemy();

            }
        }
        else
        {
            AttackMode();
        }
        
    }

    private void AttackMode()
    {

    }

    private void SpawnEnemy()
    {
        randEnemy = Random.Range(0, enemies.Length);
        //randEnemy = 3;
        //spawnPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spawnPosition = GeneratePosition();

        if (!world.IsBlock((int)spawnPosition.x, (int)spawnPosition.y))
        {
            Instantiate(enemies[randEnemy], spawnPosition, Quaternion.identity);
            currentNearEnemies++;
            timer = 0f;
        }
        
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
}
