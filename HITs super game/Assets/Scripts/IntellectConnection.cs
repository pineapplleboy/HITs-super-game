using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IntellectConnection : MonoBehaviour
{
    public Transform head;
    public float attackRange = 20;
    public LayerMask enemyLayers;

    public static bool startedConnection = false;
    public static bool foundConnection = false;

    private static Collider2D hittedEnemy;
    public static Enemy targetEnemy;

    void Update()
    {
        if (startedConnection)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(head.position, attackRange, enemyLayers);

            if (hitEnemies.Contains(hittedEnemy))
            {

            }

            foreach (Collider2D enemy in hitEnemies)
            {
                targetEnemy = enemy.GetComponent<Enemy>();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (head == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(head.position, attackRange);
    }
}
