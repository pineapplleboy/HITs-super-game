using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Guide : MonoBehaviour
{
    public static float time = 0;
    private GameObject obj;
    public static void ShowMessage(string msg)
    {
        GameObject.FindGameObjectWithTag("Guide").GetComponent<TMP_Text>().text = msg;
        time = 2;
    }

    private void Start()
    {
        obj = GameObject.FindGameObjectWithTag("Guide");
    }

    void Update()
    {
        if(time < 0)
        {
            obj.GetComponent<TMP_Text>().text = "";
        }
        else
        {
            time -= Time.deltaTime;
        }
    }

}
