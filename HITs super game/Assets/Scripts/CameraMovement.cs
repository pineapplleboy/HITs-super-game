using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private GameObject Player;
    [SerializeField] private float leftBorder;
    [SerializeField] private float rightBorder;
    [SerializeField] private float UpBorder;
    [SerializeField] private float DownBorder;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(Mathf.Clamp(Player.transform.position.x, leftBorder, rightBorder), Mathf.Clamp(Player.transform.position.y, DownBorder, UpBorder), transform.position.z), 10 * Time.deltaTime);
    }
}