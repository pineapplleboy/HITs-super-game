using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LaserDamage : MonoBehaviour
{
    public Transform firePoint;

    public static bool isActive = false;

    private float shootingTime = 0f;
    private int lastDigit = 0;

    public int baseDamage = 50;
    private int realDamage = 50;
    private float damageGrowthPercentagePerSecond = 0.2f;
    private int maxDamage = 152;

    public float laserCooldown = 0.5f;
    private float currentLaserAttackTime = 0f;

    public RaycastHit2D hitInfo;

    void Update()
    {
        if (!isActive) return;

        if (Input.GetMouseButton(0))
        {
            if (PlayerStats.intellectAmount > 0)
            {
                shootingTime += Time.deltaTime;
                Shoot();
            }

            if ((int)shootingTime != lastDigit)
            {
                realDamage += (int)(realDamage * damageGrowthPercentagePerSecond);
                realDamage = Mathf.Min(realDamage, maxDamage);
            }

            lastDigit = (int)shootingTime;
        }
        else
        {
            realDamage = baseDamage;
            shootingTime = 0f;
            currentLaserAttackTime = 0f;
        }
    }

    void Shoot()
    {
        if (currentLaserAttackTime > 0)
        {
            currentLaserAttackTime -= Time.deltaTime;
            return;
        }

        Vector2 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        hitInfo = Physics2D.Raycast(firePoint.position, difference);

        if (hitInfo)
        {
            StrikeEnemy();
        }

    }

    private void StrikeEnemy()
    {
        Enemy enemy = hitInfo.transform.GetComponent<Enemy>();

        if (enemy != null)
        {
            currentLaserAttackTime = laserCooldown;

            enemy.TakeDamage(CalculateDamage(), 2);
        }
    }

    int CalculateDamage()
    {
        return (int)Random.Range((realDamage) * 0.75f, (realDamage) * 1.25f + 1);
    }
}