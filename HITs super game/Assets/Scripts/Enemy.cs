using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;

    public int maxHealth = 100;
    public int currentHealth;

    public List<int> damageResistance; // type = { 0 - melee, 1 - range, 2 - intelligence }

    void Start()
    {
        currentHealth = maxHealth;

        damageResistance = new List<int>() { 10, 0, 0 };

        rb = GetComponent<Rigidbody2D>();
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
        enabled = false;  
    }
}
