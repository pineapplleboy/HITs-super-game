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
        sword.SetActive(false);
        laserGun.SetActive(false);
    }

}
