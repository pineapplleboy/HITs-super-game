using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class NpcAI : MonoBehaviour
{
    public static int damage = 30;

    public GameObject bullet;
    public Transform shotPoint;

    public float cooldown = 1f;
    private float currentCd = 0f;

    private Enemy targetEnemy;

    void Update()
    {
        currentCd -= Time.deltaTime;
        if (Spawner.allSpawnedEnemies.Count <= 0) return;
        targetEnemy = FindEnemy();

        if (targetEnemy == null) return;

        Flip();

        if (currentCd <= 0 && GetDistance(transform.position, targetEnemy.transform.position) < 30 && !GetComponent<NPCController>().isInCage)
        {
            Shoot();
        }
    }

    void Flip()
    {

        if (targetEnemy.transform.position.x >= transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    void Shoot()
    {
        Vector2 difference = targetEnemy.transform.position - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        currentCd = cooldown;

        Instantiate(bullet, shotPoint.position, Quaternion.Euler(0f, 0f, rotZ));
    }

    Enemy FindEnemy()
    {
        int ind = 0;
        float minDist = 10000;

        for (int i = 0; i < Spawner.allSpawnedEnemies.Count; i++)
        {
            if (Spawner.allSpawnedEnemies[i] == null) continue;

            float currentDist = GetDistance(transform.position, Spawner.allSpawnedEnemies[i].transform.position);

            if (currentDist < minDist)
            {
                minDist = currentDist;
                ind = i;
            }
        }

        return Spawner.allSpawnedEnemies[ind];
    }

    float GetDistance(Vector2 first, Vector2 second)
    {
        return Mathf.Sqrt(Mathf.Pow(first.x - second.x, 2) + Mathf.Pow(first.y - second.y, 2));
    }
}
