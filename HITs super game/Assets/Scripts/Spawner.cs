using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemies;
    private int randEnemy;
    private Vector2 spawnPosition;

    public static int spawnRate = 3;
    private int maxNearEnemies = 3;

    public static int currentNearEnemies = 0;

    private float timer = 0f;

    void Update()
    {
        if (timer >= 3 && currentNearEnemies < maxNearEnemies)
        {
            timer = 0f;

            SpawnEnemy();

            //int randomValue = Random.Range(1, 11);
            //if (randomValue >= 5)
            //{
            //    SpawnEnemy();
            //}

        }

        timer += Time.deltaTime;
    }

    private void SpawnEnemy()
    {
        currentNearEnemies++;
        randEnemy = Random.Range(0, enemies.Length);
        //randEnemy = 1;
        spawnPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Instantiate(enemies[randEnemy], spawnPosition, Quaternion.identity);
    }

    private float CheckDistance()
    {
        return 0;
    }
}
