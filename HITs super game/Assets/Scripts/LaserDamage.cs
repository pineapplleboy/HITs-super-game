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

    private float intellectPerTick = 10f;

    public float laserCooldown = 0.5f;
    private float currentLaserAttackTime = 0f;

    public RaycastHit2D hitInfo;

    private bool isShooting = false;
    private float necessaryIntellectAmount = 10f;

    public LayerMask mask;

    public ItemScriptableObject laser;

    void Update()
    {
        if (!isActive) return;

        if (laser.level == 1)
        {
            baseDamage = 20;
            maxDamage = 70;
            intellectPerTick = 10f;
        }
        else if (laser.level == 2)
        {
            baseDamage = 50;
            maxDamage = 152;
            intellectPerTick = 7f;
        }
        else if (laser.level == 3)
        {
            baseDamage = 60;
            maxDamage = 200;
            intellectPerTick = 5f;
        }

        if (isShooting)
        {
            necessaryIntellectAmount = 1f;
        }
        else
        {
            necessaryIntellectAmount = 10f;
        }

        if (Input.GetMouseButton(0) && PlayerStats.intellectAmount > necessaryIntellectAmount)
        {

            isShooting = true;

            shootingTime += Time.deltaTime;
            PlayerStats.intellectAmount -= intellectPerTick * Time.deltaTime;
            Shoot();
            GameObject.Find("Sound").GetComponent<AudioSource>().Play();

            if ((int)shootingTime != lastDigit)
            {
                realDamage += (int)(realDamage * damageGrowthPercentagePerSecond);
                realDamage = Mathf.Min(realDamage, maxDamage);
            }

            lastDigit = (int)shootingTime;
        }
        else
        {
            isShooting = false;
            necessaryIntellectAmount = 10f;

            realDamage = baseDamage;
            shootingTime = 0f;
            currentLaserAttackTime = 0f;

            GameObject.Find("Sound").GetComponent<AudioSource>().Stop();
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

        //hitInfo = Physics2D.Raycast(firePoint.position, difference);
        hitInfo = Physics2D.Raycast(firePoint.position, difference, 100000, mask);

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
        return (int)((Random.Range((realDamage) * 0.75f, (realDamage) * 1.25f + 1)) * PermanentStatsBoost.damageBoost);
    }
}