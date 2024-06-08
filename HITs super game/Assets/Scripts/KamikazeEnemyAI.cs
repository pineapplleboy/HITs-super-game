using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeEnemyAI : MonoBehaviour
{
    public float speed = 10;
    private Transform player;

    public int damage = 100;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerStats.TakeDamage(damage, 0);
            Spawner.currentNearEnemies--;
            Destroy(gameObject);
        }
    }
}
