using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IntellectConnection : MonoBehaviour
{
    public Transform head;
    public float attackRange = 20;
    public LayerMask enemyLayers;

    public static bool startedConnection = true;

    private static Collider2D hittedEnemy;
    public static Enemy targetEnemy;
    public static WarriorEnemyAI warrior;

    void Update()
    {
        if (startedConnection)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(head.position, attackRange, enemyLayers);

            if (hitEnemies.Length == 0)
            {
                hittedEnemy = null;
                targetEnemy = null;
                return;
            }

            if (!hitEnemies.Contains(hittedEnemy))
            {
                SetTarget(hitEnemies[0]);
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                NextTarget(hitEnemies);
            }

            if (ExplosionScript.readyToAttack)
            {
                DrawTarget();
            }
        }
    }

    private void DrawTarget()
    {

    }

    private void DeleteTarget()
    {

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

    private void NextTarget(Collider2D[] hitEnemies)
    {
        DeleteTarget();
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy != hittedEnemy)
            {
                SetTarget(enemy);
                return;
            }
        }
    }

    private void SetTarget(Collider2D enemy)
    {
        hittedEnemy = enemy;
        targetEnemy = enemy.GetComponent<Enemy>();
        warrior = enemy.GetComponent<WarriorEnemyAI>();
    }
}
