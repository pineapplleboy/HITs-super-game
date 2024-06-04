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

    public int amountOfBullets = 100;

    public static bool isActive = false;

    void Update()
    {
        if (!isActive) return;

        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        if (amountOfBullets > 0 && Input.GetMouseButton(0)) {
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);
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

        if (currentShotTime <= 0)
        {
            if (amountOfBullets > 0 && Input.GetMouseButton(0))
            {
                PlayerMovement.isShooting = true;
                amountOfBullets--;
                Instantiate(bullet, shotPoint.position, Quaternion.Euler(0f, 0f, rotZ));
                currentShotTime = shotSpeed;

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
}
