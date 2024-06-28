using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float offset;
    public GameObject bullet;
    public Transform shotPoint;

    public float shotSpeed = 0.3f;
    private float currentShotTime = 0f;

    public static int damage = 10;

    public int amountOfBullets = 10000;

    public static bool isActive = false;

    public AudioSource Shot;
    public AudioClip ShotSound;

    public ItemScriptableObject gun;

    public static float bulletSpeed;

    void Update()
    {
        if (!isActive) return;

        if (gun.level == 1)
        {
            shotSpeed = 0.5f;
            damage = 15;
            bulletSpeed = 15f;
        }
        else if (gun.level == 2)
        {
            shotSpeed = 0.3f;
            damage = 20;
            bulletSpeed = 20f;
        }
        else if (gun.level == 3)
        {
            shotSpeed = 0.15f;
            damage = 17;
            bulletSpeed = 23f;
        }

        Vector2 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        ChangePosition();

        if (currentShotTime <= 0)
        {
            if (Input.GetMouseButton(0))
            {
                PlayerMovement.isShooting = true;
                amountOfBullets--;
                Instantiate(bullet, shotPoint.position, Quaternion.Euler(0f, 0f, rotZ));
                currentShotTime = shotSpeed;

                Shot.clip = ShotSound;
                Shot.Play();

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
        else
        {
            currentShotTime -= Time.deltaTime;
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
    }
}
