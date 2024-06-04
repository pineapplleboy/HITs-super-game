using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsSwitch : MonoBehaviour
{
    public GameObject sword;
    public GameObject gun;

    private void Start()
    {
        gun.SetActive(false);
        sword.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            if (gun.activeInHierarchy)
            {
                SwordAttack.isActive = true;
                Gun.isActive = false;
                gun.SetActive(false);
                sword.SetActive(true);
            }
        }

        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            if (sword.activeInHierarchy)
            {
                SwordAttack.isActive = false;
                Gun.isActive = true;
                sword.SetActive(false);
                gun.SetActive(true);
            }
        }
    }
}
