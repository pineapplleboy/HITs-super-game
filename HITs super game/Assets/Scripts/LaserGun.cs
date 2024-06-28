using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LaserGun : MonoBehaviour
{
    public Transform firePoint;
    public LineRenderer lineRenderer;

    public static bool isActive = false;

    private float shootingTime = 0f;
    private int lastDigit = 0;

    public int baseDamage = 50;
    private int realDamage = 50;
    private float damageGrowthPercentagePerSecond = 0.2f;
    private int maxDamage = 152;

    public float laserCooldown = 0.5f;
    private float currentLaserAttackTime = 0f;

    private float laserWidth = 0.2f;
    private float widthGrowth = 0f;

    private float intellectPerTick = 10f;

    public RaycastHit2D hitInfo;

    private bool isShooting = false;
    private float necessaryIntellectAmount = 10f;

    public LayerMask mask;

    public ItemScriptableObject laser;

    private void Start()
    {
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;
    }

    void Update()
    {
        if (!isActive) return;

        if (laser.level == 1)
        {
            laserWidth = 0.1f;
            intellectPerTick = 10f;
            widthGrowth = 0;
        }
        else if (laser.level == 2)
        {
            laserWidth = 0.15f;
            intellectPerTick = 7f;
            widthGrowth = 0.001f;
        }
        else if (laser.level == 3)
        {
            laserWidth = 0.2f;
            intellectPerTick = 5f;
            widthGrowth = 0.002f;
        }

        if (isShooting)
        {
            necessaryIntellectAmount = 1f;
        }
        else
        {
            necessaryIntellectAmount = 10f;
        }

        ChangePosition();

        if (Input.GetMouseButton(0) && PlayerStats.intellectAmount > necessaryIntellectAmount)
        {
            isShooting = true;

            shootingTime += Time.deltaTime;
            PlayerStats.intellectAmount -= intellectPerTick * Time.deltaTime;
            PlayerStats.intellectAmount = Mathf.Max(PlayerStats.intellectAmount, 0);
            Shoot();

            if ((int)shootingTime != lastDigit)
            {
                realDamage += (int)(realDamage * damageGrowthPercentagePerSecond);
                realDamage = Mathf.Min(realDamage, maxDamage);
            }

            lastDigit = (int)shootingTime;

            lineRenderer.startWidth += widthGrowth;
            lineRenderer.endWidth += widthGrowth;

            lineRenderer.startWidth = Mathf.Min(lineRenderer.startWidth, 1);
            lineRenderer.endWidth = Mathf.Min(lineRenderer.endWidth, 1);
        }
        else
        {
            isShooting = false;
            necessaryIntellectAmount = 10f;

            Draw2DRay(firePoint.position, firePoint.position);
            realDamage = baseDamage;
            shootingTime = 0f;
            currentLaserAttackTime = 0f;

            lineRenderer.startWidth = laserWidth;
            lineRenderer.endWidth = laserWidth;
        }
    }

    void ChangePosition()
    {
        Vector2 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        if (Input.GetMouseButton(0))
        {
            if (PlayerMovement.isFacedRight)
                transform.rotation = Quaternion.Euler(0f, 0f, rotZ - 90);
            else
                transform.rotation = Quaternion.Euler(0f, 180f, 90 - rotZ);
        }
        else
        {
            PlayerMovement.isShooting = false;
            if (PlayerMovement.isFacedRight)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            }
            else
            {
                transform.rotation = Quaternion.Euler(180f, 0f, 90f);
            }
        }

        if (PlayerStats.intellectAmount > 0 && Input.GetMouseButton(0))
        {
            PlayerMovement.isShooting = true;

            if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= transform.position.x)
            {
                PlayerMovement.isFacedRight = true;
            }
            else
            {
                PlayerMovement.isFacedRight = false;
            }
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
        //hitInfo = Physics2D.Raycast(firePoint.position, difference, 100000, mask);

        if (hitInfo)
        {
            Draw2DRay(firePoint.position, hitInfo.point);
        }
        else
        {
            Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Draw2DRay(firePoint.position, mousePoint + difference * 50);
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

    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    int CalculateDamage()
    {
        return (int)Random.Range((realDamage) * 0.75f, (realDamage) * 1.25f + 1);
    }
}
