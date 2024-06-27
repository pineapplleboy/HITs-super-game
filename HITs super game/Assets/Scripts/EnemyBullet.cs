using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;

    public float lifeTime = 10f;
    public float distance;
    public int damage;
    public LayerMask whatIsSolid;

    private float currentLifeTime = 0f;

    private int gunDamage = 50;

    private void Start()
    {
        rb.velocity = transform.right * speed;
    }

    void Update()
    {
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


        if (collision.name == "Tilemap" || collision.tag == "Player")
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name == "Tilemap" || collision.name == "Player")
        {
            Destroy(gameObject);
        }
    }

    int CalculateDamage()
    {
        return (int)Random.Range((damage + gunDamage) * 0.75f, (damage + gunDamage) * 1.25f + 1);
    }
}
