using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;

    public int maxHealth = 100;
    public int currentHealth;

    public List<int> damageResistance; // type = { 0 - melee, 1 - range, 2 - intelligence }

    private Transform player;

    private bool died = false;
    private float dieTime = 0f;

    private int enemyIndex;

    public bool onRaid = false;

    private COMPUTER computer;

    void Start()
    {
        currentHealth = maxHealth;

        //damageResistance = new List<int>() { 10, 0, 0 };

        rb = GetComponent<Rigidbody2D>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        computer = GameObject.FindGameObjectWithTag("COMPUTER").GetComponent<COMPUTER>();
    }

    private void Update()
    {
        if (died)
        {
            dieTime += Time.deltaTime;

            if (dieTime >= 2)
            {
                Destroy(gameObject);
            }
        }

        if (IsFarAway())
        {
            Spawner.currentNearEnemies--;
            Spawner.allSpawnedEnemies.RemoveAt(enemyIndex);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.layer == 8 && collision.gameObject.tag == "Player")
        {
            if (onRaid)
            {
                PlayerStats.TakeTouchDamage(100, 0);
            }
            else
            {
                PlayerStats.TakeTouchDamage(70, 0);
            }
            
            Spawner.currentNearEnemies--;
            Spawner.allSpawnedEnemies.RemoveAt(enemyIndex);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.layer == 8 && collision.gameObject.tag == "COMPUTER")
        {
            computer.TakeDamage(100);

            Spawner.currentNearEnemies--;
            Spawner.allSpawnedEnemies.RemoveAt(enemyIndex);
            Destroy(gameObject);
        }
    }

    public void SetHp(int health)
    {
        maxHealth = health;

        if (onRaid)
        {
            maxHealth = (int)(maxHealth * 1.2);
        }
    }

    public void SetDamageResistance(List<int> res)
    {
        damageResistance = res;
    }

    public void SetOnRaid()
    {
        onRaid = true;
    }

    public void TakeDamage(int damage, int typeOfDamage)
    {
        currentHealth -= CalculateDamage(damage, typeOfDamage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public int CalculateDamage(int damage, int typeOfDamage)
    {
        if (typeOfDamage == -1) return damage;
        return (int)(damage * ((100.0 - damageResistance[typeOfDamage]) / 100));
    }

    public void PushAway(Vector2 pushFrom, float pushPower)
    {
        if (pushPower == 0)
        {
            return;
        }

        Vector2 pushDirection = -(pushFrom - (Vector2)transform.position);
        pushDirection.x *= pushPower;

        rb.AddForce(pushDirection);
    }

    void Die()
    {
        Spawner.currentNearEnemies--;
        GetComponent<Collider2D>().enabled = false;
        died = true;

        GetComponent<Rigidbody2D>().gravityScale = 3;

        Spawner.allSpawnedEnemies.RemoveAt(enemyIndex);
    }

    private bool IsFarAway()
    {
        if (onRaid)
        {
            //return Mathf.Min(Mathf.Sqrt(Mathf.Pow(transform.position.x - player.position.x, 2) +
            //Mathf.Pow(transform.position.y - player.position.y, 2)),

            //Mathf.Sqrt(Mathf.Pow(computer.transform.position.x - player.position.x, 2) +
            //Mathf.Pow(computer.transform.position.y - player.position.y, 2))) > 150;
            return false;
        }

        return Mathf.Sqrt(Mathf.Pow(transform.position.x - player.position.x, 2) + 
            Mathf.Pow(transform.position.y - player.position.y, 2)) > 150;
    }

    public void SetIndex(int index)
    {
        enemyIndex = index;
    }

}
