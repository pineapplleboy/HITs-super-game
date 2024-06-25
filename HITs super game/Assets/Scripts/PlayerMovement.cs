using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D rb;
    private Animator animator;

    private bool onGround = false;
    private bool mustAutoJump = false;
    public static bool isFacedRight = true;

    [SerializeField] int speed = 1;
    [SerializeField] int jumpForce = 1;
    [SerializeField] float autoJumpForce = 1;

    private float acceleration = 1f;
    private float timeOfWalking = 0f;
    private float lastMovingDirection = 0f;

    private Vector2 tempWalkingSlide;
    private int amountOfSlides = 0;

    public static float slowRate = 1f;

    public static bool isShooting = false;

    public float dashCooldown = 2f;
    private float currentDashCD = 0f;

    private int lastDirection = 0;
    private bool blockMoving = false;

    private float jumpCd = 0.3f;
    private float currentJumpCd = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.A))
        {
            if (lastDirection != -1) blockMoving = false;
            lastDirection = -1;
        }
        else if (Input.GetKey(KeyCode.D)) 
        {
            if (lastDirection != 1) blockMoving = false;
            lastDirection = 1;
        }
        else
        {
            blockMoving = false;
            lastDirection = 0;
        }

        if(Input.GetKeyDown(KeyCode.Space) && onGround && currentJumpCd <= 0)
        {
            rb.AddForce(Vector2.up * (jumpForce * slowRate));
            currentJumpCd = jumpCd;
            onGround = false;
        }

        currentJumpCd -= Time.deltaTime;

        if (Mathf.Abs(moveX) > 0.1 && mustAutoJump && onGround)
        {
            transform.Translate(0, autoJumpForce, 0);
        }

        if(moveX != 0){
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
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

        if (!blockMoving)
        {
            acceleration = Mathf.Min(2, 1 + (timeOfWalking / 3));
            rb.velocity = move;
        }
        else
        {
            acceleration = 1;
        }

        currentDashCD -= Time.deltaTime;

        lastMovingDirection = moveX;

        Flip();

        //Debug.Log(blockMoving);
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

            if (collision.transform.name == "Tilemap")
            {
                foreach (ContactPoint2D contactPoint in collision.contacts)
                {
                    Vector2 hitPoint = contactPoint.point;

                    //Debug.Log(hitPoint.x + " " + transform.position.x);

                    if (hitPoint.y - transform.position.y <= -0.5)
                    {
                        onGround = true;
                        blockMoving = false;
                        return;
                    }

                }

                foreach (ContactPoint2D contactPoint in collision.contacts)
                {
                    Vector2 hitPoint = contactPoint.point;

                    if (hitPoint.y - transform.position.y > 0)
                    {
                        if ((lastDirection == 1 && Input.GetKey(KeyCode.D)) || (lastDirection == -1 && Input.GetKey(KeyCode.A)))
                        {
                            blockMoving = true;
                        }
                        else
                        {
                            blockMoving = false;
                        }


                        onGround = false;
                        return;
                    }

                }
            }

            onGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        blockMoving = false;
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