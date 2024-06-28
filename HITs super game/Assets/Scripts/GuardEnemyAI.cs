using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public float spped = 2;
    public int positionOfPatrol = 10;
    public static Vector2 point;

    private bool isFacedRight = true;
    private bool isMovingRight = true;

    void Start()
    {
        point = transform.position;
    }

    void Update()
    {
        Flip();
        if (Vector2.Distance(transform.position, point) < positionOfPatrol)
        {
            Wait();
        }
    }

    private void Wait()
    {
        if (transform.position.x > point.x + positionOfPatrol)
        {
            isMovingRight = true;
        }
        else if (transform.position.x < point.x - positionOfPatrol)
        {
            isMovingRight = false;
        }

        if (isMovingRight)
        {
            transform.position = new Vector2(transform.position.x + spped * Time.deltaTime, transform.position.y);
        }
        else
        {
            transform.position = new Vector2(transform.position.x - spped * Time.deltaTime, transform.position.y);
        }
        

    }

    private void RunToPlayer()
    {

    }

    private void GoBack()
    {

    }

    private void Flip()
    {
        if (isFacedRight)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

}
