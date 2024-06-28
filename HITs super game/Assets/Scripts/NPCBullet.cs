using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBullet : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;

    public float lifeTime = 10f;
    public float distance;
    private int damage = 20;
    public LayerMask whatIsSolid;

    private float currentLifeTime = 0f;

    private int gunDamage;

    private void Start()
    {
        rb.velocity = transform.right * speed;
        gunDamage = NpcAI.damage;
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

        if (collision.tag == "Enemy" || collision.tag == "KamikazeEnemy" || collision.tag == "FlyingEnemy")
        {
            Destroy(gameObject);
        }

    }

    int CalculateDamage()
    {
        return (int)Random.Range((damage + gunDamage) * 0.75f, (damage + gunDamage) * 1.25f + 1);
    }
}
