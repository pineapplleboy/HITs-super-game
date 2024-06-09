using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
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

    public RaycastHit2D hitInfo;
    Thread strikeThread;

    private void Start()
    {
        //strikeThread = new Thread(StrikeEnemy);

        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;
    }

    void Update()
    {
        if (!isActive) return;

        ChangePosition();

        if (Input.GetMouseButton(0))
        {
            //if (!strikeThread.IsAlive)
            //{
            //    strikeThread.Start();
            //}

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

            lineRenderer.startWidth += 0.002f;
            lineRenderer.endWidth += 0.002f;

            lineRenderer.startWidth = Mathf.Min(lineRenderer.startWidth, 1);
            lineRenderer.endWidth = Mathf.Min(lineRenderer.endWidth, 1);
        }
        else
        {

            //if (strikeThread.IsAlive)
            //{
            //    strikeThread.Abort();
            //}

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
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ - 90);
        }
        else
        {
            PlayerMovement.isShooting = false;
            if (PlayerMovement.isFacedRight)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, 0f, -90f);
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

        hitInfo = Physics2D.Raycast(firePoint.position, difference);

        if (hitInfo)
        {
            //Debug.Log(hitInfo.transform.name);
            //StrikeEnemy();

            Draw2DRay(firePoint.position, hitInfo.point);
        }
        else
        {
            Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log(mousePoint + " " + mousePoint * 2);

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
