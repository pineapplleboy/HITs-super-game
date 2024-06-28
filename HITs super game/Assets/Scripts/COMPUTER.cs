using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class COMPUTER : MonoBehaviour
{
    private int health = 2000;
    private int startHealth = 2000;

    private void Start()
    {
        health = startHealth;
    }

    void Update()
    {
        if (health <= 0)
        {
            ThisIsTheEnd();
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    public void Restore()
    {
        health = startHealth;
    }

    void ThisIsTheEnd()
    {
        GameObject.Find("MainCanvas").transform.Find("DiedPanel").gameObject.SetActive(true);
        GameObject.FindGameObjectWithTag("Info").GetComponent<Text>().text = $"Вы пережили {DayTime.daysCounter} дней, {DayTime.raidsCounter} рейдов";
        Time.timeScale = 0;
    }
}
