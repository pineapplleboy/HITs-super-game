using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COMPUTER : MonoBehaviour
{
    private int health = 5000;
    private int startHealth = 5000;

    private void Start()
    {
        health = startHealth;
    }

    void Update()
    {
        if (health <= 0)
        {
            ThisIsTheEnd();
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    public void Restore()
    {
        health = startHealth;
    }

    void ThisIsTheEnd()
    {
        Debug.Log("OH NO U DEAD");
        Guide.ShowMessage("OH NO U DEAD");
    }
}
