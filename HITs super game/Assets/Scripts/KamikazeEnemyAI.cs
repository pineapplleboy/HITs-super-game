using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeEnemyAI : MonoBehaviour
{
    public float speed = 1;
    private Transform player;

    private Rigidbody2D rb;
    private Animator anim;

    public int jumpForce = 500;
    public static float slowRate = 1;

    public int damage = 100;

    private bool onGround = false;
    private bool isFacedRight = true;

    private float flipTime = 0.2f;
    private float currentFlipTime = 0;

    private float sleepTime = 0f;

    private float goBackJumpingCd = 1f;
    private float currentGoBackJumpingCd = 0f;

    private float jumpCooldown = 0.3f;
    private float currentJumpTime = 0;

    private int stepCounter = 0;

    private bool nearWall = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        sleepTime -= Time.deltaTime;
        currentGoBackJumpingCd -= Time.deltaTime;

        if (sleepTime > 0) return;

        if (stepCounter == 0)
        {
            Move();
            Flip();
        }
        else
        {
            GoBackAndJump();
        }

        if (!onGround)
        {
            speed = 4;
        }
        else
        {
            speed = 8;
        }

        currentJumpTime -= Time.deltaTime;
        //transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerStats.TakeTouchDamage(damage, 0);
            Spawner.currentNearEnemies--;
            Destroy(gameObject);
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {

        if (collision.collider.tag == "Ground" || collision.collider.tag == "Platform")
        {
            onGround = true;
        }

        if (collision.transform.name == "Tilemap")
        {
            foreach (ContactPoint2D contactPoint in collision.contacts)
            {
                Vector2 hitPoint = contactPoint.point;

                if (hitPoint.y - transform.position.y > -0.95)
                {
                    if (currentGoBackJumpingCd <= 0)
                    {
                        currentGoBackJumpingCd = goBackJumpingCd;
                        stepCounter = 10;
                    }
                    else
                    {
                        nearWall = true;
                    }

                    onGround = false;
                    return;
                }

            }
        }

        nearWall = false;
    }

    private void GoBackAndJump()
    {
        stepCounter--;
        float moveX;

        if (isFacedRight)
        {
            moveX = -1;
        }
        else
        {
            moveX = 1;
        }

        Vector2 move = new Vector2(moveX * (speed), rb.velocity.y);

        rb.velocity = move;

        if (stepCounter == 0)
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (onGround)
        {
            rb.AddForce(Vector2.up * (jumpForce * slowRate));
            onGround = false;
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground" || collision.collider.tag == "Platform")
        {
            onGround = false;
        }
    }

    private void Move()
    {
        Vector2 playerPosition = player.position;
        float moveX;

        if (playerPosition.x >= transform.position.x)
        {
            isFacedRight = true;
            moveX = 1;
            anim.SetBool("isRunning", true);
        }
        else
        {
            isFacedRight = false;
            moveX = -1;
            anim.SetBool("isRunning", true);
        }

        Vector2 move = new Vector2(moveX * (speed), rb.velocity.y);

        if (!((nearWall && isFacedRight && moveX == 1) || (nearWall && !isFacedRight && moveX == -1))) rb.velocity = move;

        if (Mathf.Abs(playerPosition.x - transform.position.x) < 10 && playerPosition.y > transform.position.y + 2)
        {
            Jump();
        }

        else if (playerPosition.y > transform.position.y + 5)
        {
            Jump();
        }

        currentFlipTime += Time.deltaTime;

    }

    private void Flip()
    {
        if (currentFlipTime < flipTime)
        {
            return;
        }

        if (isFacedRight)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            currentFlipTime = 0;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            currentFlipTime = 0;
        }
    }

    public void Sleep(float newSleepingTime)
    {
        sleepTime = newSleepingTime;
    }
}
