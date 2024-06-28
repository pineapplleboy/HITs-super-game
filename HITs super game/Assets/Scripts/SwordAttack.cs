using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

using Random = UnityEngine.Random;

public class SwordAttack : MonoBehaviour
{
    private Animator anim;

    private float timeBetweenAttack = 0;
    public float attackSpeed = 1;

    public Transform attackPoint;
    public float attackRange = 5;
    public LayerMask enemyLayers;

    public int damage = 50;
    public float critRate = 4;

    private float swingTime = 0f;
    private float maxSwingTime = 1f;

    public static bool isActive = true;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (PlayerStats.isDead)
            return;
        if (!isActive) return;

        if (PlayerStats.isBlocking && !Input.GetMouseButton(1)) PlayerStats.UnBlock();
        
        if (timeBetweenAttack <= 0) {
            if (Input.GetMouseButton(0))
            {
                if (PlayerStats.isBlocking) PlayerStats.UnBlock();

                swingTime += Time.deltaTime;
                swingTime = Math.Min(swingTime, maxSwingTime);
            }
            else if (Input.GetMouseButton(1) && !PlayerStats.isBlocking)
            {
                PlayerStats.Block();
            }
            else if (swingTime > 0f)
            {
                Attack();
                timeBetweenAttack = attackSpeed;
                swingTime = 0f;
            }
        }
        else
        {
            timeBetweenAttack -= Time.deltaTime;
        }
    }

    void Attack()
    {
        anim.SetTrigger("swordAtack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Enemy hittedEnemy = enemy.GetComponent<Enemy>();
            hittedEnemy.TakeDamage(CurrentDamage(), 0);
            //hittedEnemy.PushAway(attackPoint.position, 12500f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private int CurrentDamage()
    {
        int boostDamage = (int) (damage * (swingTime / maxSwingTime));
        return (int) ((Random.Range(damage * 0.75f, damage * 1.25f + 1) + boostDamage) * PermanentStatsBoost.damageBoost);
    }
}
