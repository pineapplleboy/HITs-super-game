using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COMPUTER : MonoBehaviour
{
    private int health = 100;

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

    void ThisIsTheEnd()
    {
        Debug.Log("OH NO U DEAD");
    }
}
