using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsSwitch : MonoBehaviour
{
    public GameObject sword;
    public GameObject gun;
    public GameObject laserGun;

    private void Start()
    {
        gun.SetActive(false);
        sword.SetActive(true);
        laserGun.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            if (!sword.activeInHierarchy)
            {
                SwordAttack.isActive = true;
                Gun.isActive = false;
                LaserGun.isActive = false;
                LaserDamage.isActive = false;

                sword.SetActive(true);
                gun.SetActive(false);
                laserGun.SetActive(false);
            }
        }

        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            if (!gun.activeInHierarchy)
            {
                SwordAttack.isActive = false;
                Gun.isActive = true;
                LaserGun.isActive = false;
                LaserDamage.isActive = false;

                gun.SetActive(true);
                sword.SetActive(false);
                laserGun.SetActive(false);
            }
        }

        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            if (!laserGun.activeInHierarchy)
            {
                SwordAttack.isActive = false;
                Gun.isActive = false;
                LaserGun.isActive = true;
                LaserDamage.isActive = true;

                laserGun.SetActive(true);
                sword.SetActive(false);
                gun.SetActive(false);
            }
        }
    }
}
