using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private int damage = 2000;
    private int needIntellect = 70;

    void Update()
    {
        if (!IntellectConnection.startedConnection) return;

        if (PlayerStats.intellectAmount < needIntellect) return;

        if (Input.GetMouseButtonDown(0))
        {
            PlayerStats.intellectAmount -= needIntellect;
            IntellectConnection.targetEnemy.TakeDamage(damage, -1);
        }
    }
}
