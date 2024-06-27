using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;

    public float lifeTime = 10f;
    public float distance;
    public int damage;
    public LayerMask whatIsSolid;

    private float currentLifeTime = 0f;

    private int gunDamage;

    private void Start()
    {
        // определяем, какая при создании пули была пушка в руках, в зависимости от этого ставим дамаг
        rb.velocity = transform.right * speed;
        gunDamage = Gun.damage;
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
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(CalculateDamage(), 1);
        }

        if (collision.name != "Player" && collision.tag != "NPC")
        {
            Destroy(gameObject);
        }
        
    }

    void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.name != "Player" && collision.tag != "NPC")
        {
            Destroy(gameObject);
        }

    }

    int CalculateDamage()
    {
        return (int)Random.Range((damage + gunDamage) * 0.75f, (damage + gunDamage) * 1.25f + 1);
    }
}
