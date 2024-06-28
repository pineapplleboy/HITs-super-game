using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    private int damage = 2000;
    private int needIntellect = 20;

    private float attackCd = 5f;
    private float currentCd = 0f;

    public static bool readyToAttack = false;

    void Update()
    {
        currentCd -= Time.deltaTime;

        if (currentCd <= 0)
        {
            readyToAttack = true;
        }

        if (!IntellectConnection.startedConnection) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (IntellectConnection.targetEnemy == null)
            {
                Guide.ShowMessage("Цель не отмечена");
            }
            else if (currentCd > 0)
            {
                Guide.ShowMessage("Способность не готова");
            }
            else if (PlayerStats.intellectAmount < needIntellect)
            {
                Guide.ShowMessage("Недостаточно интеллекта");
                return;
            }

            if (currentCd <= 0)
            {
                currentCd = attackCd;
                PlayerStats.intellectAmount -= needIntellect;
                IntellectConnection.targetEnemy.TakeDamage(damage, -1);
            }
            
        }
    }
}
