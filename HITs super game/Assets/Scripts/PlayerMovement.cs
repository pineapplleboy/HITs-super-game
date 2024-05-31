using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D rb;
    private bool onGround = false;
    private bool mustAutoJump = false;
    private bool isFacedRight = true;

    [SerializeField] int speed = 1;
    [SerializeField] int jumpForce = 1;
    [SerializeField] float autoJumpForce = 1;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal");

        if(Input.GetKey(KeyCode.Space) && onGround)
        {
            rb.AddForce(Vector2.up * jumpForce);
            onGround = false;
        }

        if(Mathf.Abs(moveX) > 0.1 && mustAutoJump)
        {
            transform.Translate(0, autoJumpForce, 0);
        }

        if(moveX > 0)
        {
            isFacedRight = true;
        }
        else if(moveX < 0)
        {
            isFacedRight = false;
        }

        Vector2 move = new Vector2(moveX * speed, rb.velocity.y);
        rb.velocity = move;

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
        if(collision.collider.tag == "Ground")
        {
            onGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            onGround = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            mustAutoJump = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            mustAutoJump = false;
        }
    }
}