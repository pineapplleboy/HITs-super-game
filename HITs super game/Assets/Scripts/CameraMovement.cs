using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private GameObject Player;
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(Player.transform.position.x, Player.transform.position.y, transform.position.z), 10 * Time.deltaTime);
    }
}