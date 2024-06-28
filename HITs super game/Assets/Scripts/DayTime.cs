using Newtonsoft.Json.Converters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayTime : MonoBehaviour
{
    public static int daysCounter;
    public static int nightsCounter;

    public static int raidsCounter = 0;

    private float dayTime = 300f;
    private float nightTime = 120f;

    private float raidTime = 120f;

    private float secondsInHour = 1f;

    private float secondsInHourDay;
    private float secondsInHourNight;
    private float secondsInHourRaid;

    private bool isDay = true;

    private int currentHours, currentMinutes;

    private float currentTime;

    private int raidFrequency = 1;

    void Start()
    {
        daysCounter = 0;
        nightsCounter = 0;

        currentTime = 0;

        secondsInHour = dayTime / 12;

        secondsInHourDay = dayTime / 12;
        secondsInHourNight = nightTime / 12;
        secondsInHourRaid = raidTime / 12;
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        if (isDay)
        {
            currentHours = 12 + (int)(currentTime / secondsInHourDay);
            currentMinutes = (int)((currentTime % secondsInHourDay) / (secondsInHourDay / 60));
        }
        else
        {
            if (Spawner.isAttack)
            {
                currentHours = (int)(currentTime / secondsInHourRaid);
                currentMinutes = (int)((currentTime % secondsInHourRaid) / (secondsInHourRaid / 60));
            }
            else
            {
                currentHours = (int)(currentTime / secondsInHourNight);
                currentMinutes = (int)((currentTime % secondsInHourNight) / (secondsInHourNight / 60));
            }
        }

        if (isDay && currentTime >= dayTime)
        {
            Camera.main.transform.Find("Background").GetComponent<Animator>().SetTrigger("End");
            isDay = false;
            daysCounter++;
            currentTime = 0;

            if (daysCounter % raidFrequency == 0 && !Spawner.isAttack)
            {
                Spawner.isAttack = true;
                Guide.ShowMessage("���� ���");
            }

            // Debug.Log("night" + " " + GetTime());

        }
        else if (!isDay && currentTime >= nightTime)
        {
            Camera.main.transform.Find("Background").GetComponent<Animator>().SetTrigger("Start");
            if (Spawner.isAttack)
            {
                Spawner.isAttack = false;
                raidsCounter++;
                Guide.ShowMessage("���� ��� ����������");
            }

            isDay = true;
            nightsCounter++;
            currentTime = 0;

            // Debug.Log("day" + " " + GetTime());

        }
    }

    string GetTime()
    {
        return currentHours + " " + currentMinutes;
    }
}
