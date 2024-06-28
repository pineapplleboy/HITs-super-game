using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D rb;
    private Animator animator;

    private bool onGround = false;
    private bool mustAutoJump = false;
    public static bool isFacedRight = true;

    public static bool canShootHook = false;

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

    private float jumpBoost = 1f;
    private bool boostFound = false;

    private bool isInBlock = false;

    private WorldGeneration world;

    [SerializeField] AudioSource StepSound;
    [SerializeField] AudioClip[] Steps;
    private float stepsSoundTime;

    void Start()
    {
        Time.fixedDeltaTime = Time.timeScale * 0.01f;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        world = GameObject.FindGameObjectWithTag("World").GetComponent<WorldGeneration>();
    }

    void Update()
    {
        if (PlayerStats.isDead)
            return;

        canShootHook = onGround;

        if (!LaserGun.isActive && !Gun.isActive) isShooting = false;

        if (boostFound) jumpBoost = 1.3f;
        else jumpBoost = 1f;

        CheckWhereIsPlayer();

        if (isInBlock)
        {
            rb.gravityScale = 0;
            rb.position = new Vector2(rb.position.x, rb.position.y + 3f);
        }
        else if (!(GrapplingHook.isHooked && GrapplingHook.ropeDrawen))
        {
            rb.gravityScale = 10;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GrapplingHook.isHooked = false;
            GrapplingHook.brokeRope = true;
            GrapplingHook.needToDraw = 0;
        }

        if (GrapplingHook.isHooked && GrapplingHook.ropeDrawen)
        {
            rb.gravityScale = 0;
            onGround = true;
        }
        else
        {
            rb.gravityScale = 10;
        }

        float moveX = 0;

        if (!GrapplingHook.isHooked && GrapplingHook.needToDraw == 0 && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            moveX = Input.GetAxis("Horizontal");
        }

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

        if (Input.GetKeyDown(KeyCode.Space) && onGround && currentJumpCd <= 0)
        {
            rb.AddForce(Vector2.up * (jumpForce * slowRate * jumpBoost));
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
            if (stepsSoundTime <= 0)
            {
                StepSound.clip = Steps[UnityEngine.Random.Range(0, Steps.Length)];
                stepsSoundTime = StepSound.clip.length + 0.3f;
                StepSound.Play();
            }
            else
            {
                stepsSoundTime -= Time.deltaTime;
            }
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
            if (moveX > 0)
            {
                move = new Vector2(300, rb.velocity.y);
            }
            else
            {
                move = new Vector2(-300, rb.velocity.y);
            }
            //move = new Vector2(moveX * (speed * 20 * slowRate * acceleration), rb.velocity.y);
            
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
    }

    private void CheckWhereIsPlayer()
    {
        int inBlock = 0;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (world.IsBlock((int)transform.position.x + i, (int)transform.position.y + j))
                {
                    inBlock++;
                }
            }
        }

        if (inBlock == 9)
        {
            isInBlock = true;
        }
        else
        {
            isInBlock = false;
        }

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
        if (collision.transform.name == "Tilemap")
        {
            foreach (ContactPoint2D contactPoint in collision.contacts)
            {
                Vector2 hitPoint = contactPoint.point;

                if (hitPoint.y - transform.position.y > -0.95)
                {
                    if (isFacedRight)
                    {
                        hitPoint.x += 1;
                    }
                    else
                    {
                        hitPoint.x -= 1f;
                        hitPoint.y -= 1f;
                    }

                    WorldBlock block = GameObject.FindGameObjectWithTag("World").GetComponent<WorldGeneration>().GetBlock((int)(hitPoint.x), (int)(hitPoint.y));

                    if (block == null)
                    {
                        hitPoint.y += 1f;
                        block = GameObject.FindGameObjectWithTag("World").GetComponent<WorldGeneration>().GetBlock((int)(hitPoint.x), (int)(hitPoint.y));
                    }

                    if (block != null && block.isDoor)
                    {
                        if (isFacedRight)
                        {
                            transform.position = new Vector3(transform.position.x + 1.5f, transform.position.y, transform.position.z);
                            return;
                        }
                        else
                        {
                            transform.position = new Vector3(transform.position.x - 1.5f, transform.position.y, transform.position.z);
                            return;
                        }
                    }
                }

            }
        }

        if (collision.collider.tag == "Ground" || collision.collider.tag == "Platform")
        {
            boostFound = false;

            if (collision.transform.name == "Tilemap")
            {
                foreach (ContactPoint2D contactPoint in collision.contacts)
                {
                    Vector2 hitPoint = contactPoint.point;

                    if (hitPoint.y - transform.position.y > -0.7 && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)))
                    {
                        boostFound = true;
                        break;
                    }

                }

                foreach (ContactPoint2D contactPoint in collision.contacts)
                {
                    Vector2 hitPoint = contactPoint.point;

                    if (hitPoint.y - transform.position.y <= -0.95)
                    {
                        onGround = true;
                        blockMoving = false;
                        return;
                    }

                }

                foreach (ContactPoint2D contactPoint in collision.contacts)
                {
                    Vector2 hitPoint = contactPoint.point;

                    if (hitPoint.y - transform.position.y > -0.95)
                    {
                        if ((lastDirection == 1 && Input.GetKey(KeyCode.D)) || (lastDirection == -1 && Input.GetKey(KeyCode.A)))
                        {
                            blockMoving = true;
                        }
                        else
                        {
                            blockMoving = false;
                        }

                        acceleration = 1;
                        timeOfWalking = 0f;

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