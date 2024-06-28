using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 15f;
    public Rigidbody2D rb;

    public float lifeTime = 10f;
    public float distance;
    private int damage = 1;
    public LayerMask whatIsSolid;

    private float currentLifeTime = 0f;

    private int gunDamage = 15;

    private WorldGeneration world;

    private void Start()
    {
        rb.velocity = transform.right * speed;

        world = GameObject.FindGameObjectWithTag("World").GetComponent<WorldGeneration>();
    }

    void Update()
    {
        if (world.IsBlock((int)transform.position.x, (int)transform.position.y))
        {
            Destroy(gameObject);
        }

        currentLifeTime += Time.deltaTime;

        if (currentLifeTime >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerStats player = collision.GetComponent<PlayerStats>();
        if (player != null)
        {
            player.TakeDamage(CalculateDamage(), 1);
        }

        COMPUTER computer = collision.GetComponent<COMPUTER>();
        if (computer != null)
        {
            computer.TakeDamage(70);
        }

        if (collision.name == "Tilemap" || collision.tag == "Player" || collision.tag == "COMPUTER")
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name == "Tilemap" || collision.name == "Player" || collision.tag == "COMPUTER")
        {
            Destroy(gameObject);
        }
    }

    int CalculateDamage()
    {
        return (int)Random.Range((damage + gunDamage) * 0.75f, (damage + gunDamage) * 1.25f + 1);
    }
}
