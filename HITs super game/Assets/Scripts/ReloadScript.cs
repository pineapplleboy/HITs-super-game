using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadScript : MonoBehaviour
{

    private int damage = 10;
    private int needIntellect = 30;
    private float sleepTime = 3f;

    void Update()
    {
        if (!IntellectConnection.startedConnection) return;

        if (PlayerStats.intellectAmount < needIntellect) return;

        if (Input.GetMouseButtonDown(0))
        {
            PlayerStats.intellectAmount -= needIntellect;
            IntellectConnection.warrior.Sleep(sleepTime);
            IntellectConnection.targetEnemy.TakeDamage(damage, -1);
        }
    }

}
