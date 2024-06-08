using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D rb;
    private bool onGround = false;
    private bool mustAutoJump = false;
    public static bool isFacedRight = true;

    [SerializeField] int speed = 1;
    [SerializeField] int jumpForce = 1;
    [SerializeField] float autoJumpForce = 1;

    private float acceleration = 1f; // ускорение персонажа
    private float timeOfWalking = 0f; // время, которое он двигается - нужно для ускорения
    private float lastMovingDirection = 0f;

    private Vector2 tempWalkingSlide;
    private int amountOfSlides = 0;

    public static float slowRate = 1f; // замедление при постановке блока

    public static bool isShooting = false; // для поворота при стрельбе

    public float dashCooldown = 2f; // перезарядка рывка
    private float currentDashCD = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal");

        if(Input.GetKey(KeyCode.Space) && onGround)
        {
            rb.AddForce(Vector2.up * (jumpForce * slowRate));
            onGround = false;
        }

        if(Mathf.Abs(moveX) > 0.1 && mustAutoJump)
        {
            transform.Translate(0, autoJumpForce, 0);
        }

        if (!isShooting)
        {
            if (moveX > 0)
            {
                isFacedRight = true;
            }
            else if (moveX < 0)
            {
                isFacedRight = false;
            }
        }

        Vector2 move;

        if (currentDashCD <= 0 && Input.GetKey(KeyCode.LeftShift) && moveX != 0)
        {
            move = new Vector2(moveX * (speed * 20 * slowRate * acceleration), rb.velocity.y);
            currentDashCD = dashCooldown;
            acceleration = 2f;
            timeOfWalking = 10f;
        }
        else
        {
            move = new Vector2(moveX * (speed * slowRate * acceleration), rb.velocity.y);
        }

        if ((moveX >= 0 && lastMovingDirection < 0) || (moveX <= 0 && lastMovingDirection > 0))
        {
            tempWalkingSlide = new Vector2(lastMovingDirection * (speed * slowRate * acceleration), rb.velocity.y);
            amountOfSlides = 5;
            timeOfWalking = 0f;
            acceleration = 1f;
        }

        if (amountOfSlides > 0)
        {
            amountOfSlides--;
            move = tempWalkingSlide;
        }

        if (moveX == 0) timeOfWalking = 0;
        else timeOfWalking += Time.deltaTime;

        acceleration = Mathf.Min(2, 1 + (timeOfWalking / 3));

        currentDashCD -= Time.deltaTime;
        
        rb.velocity = move;

        lastMovingDirection = moveX;

        Flip();
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.collider.tag == "Ground" || collision.collider.tag == "Platform")
        {
            onGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground" || collision.collider.tag == "Platform")
        {
            onGround = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Ground" || collision.tag == "Platform")
        {
            mustAutoJump = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground" || collision.tag == "Platform")
        {
            mustAutoJump = false;
        }
    }
}