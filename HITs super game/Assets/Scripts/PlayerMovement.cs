using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D rb;
    private bool onGround = false;

    [SerializeField] int speed = 1;
    [SerializeField] int jumpForce = 1;
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

        Vector2 move = new Vector2(moveX * speed, rb.velocity.y);
        rb.velocity = move;
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
}