using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class RangeEnemy : MonoBehaviour
{

    public float speed = 1;
    private Transform player;

    private Rigidbody2D rb;
    private Animator anim;

    public int jumpForce = 500;
    public static float slowRate = 1;

    public int damage = 10;

    private bool onGround = false;
    private bool isFacedRight = true;

    private float flipTime = 0.2f;
    private float currentFlipTime = 0;

    private float jumpCooldown = 0.3f;
    private float currentJumpTime = 0;

    private float sleepTime = 0f;

    private float maxReach = 30f;

    public float cooldown = 1f;
    private float currentCd = 0f;
    private bool isShooting = false;

    public GameObject bullet;
    public Transform shotPoint;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        sleepTime -= Time.deltaTime;
        currentCd -= Time.deltaTime;
        if (sleepTime > 0) return;
        
        Flip();
        if (!onGround)
        {
            speed = 3;
        }
        else
        {
            speed = 6;
        }

        currentJumpTime -= Time.deltaTime;

        if (CheckDistance() <= maxReach && CheckReach())
        {
            isShooting = true;
            Shoot();
            anim.SetBool("isRunning", false);
        }
        else if (currentCd <= 0)
        {
            isShooting = false;
            Move();
            anim.SetBool("isRunning", true);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {

        if (collision.collider.tag == "Ground" || collision.collider.tag == "Platform")
        {
            onGround = true;
        }

        if (collision.gameObject.tag == "Player")
        {
            //PlayerStats.TakeDamage(damage, 0);
            //Destroy(gameObject);
        }

        if (isShooting) return;

        if (collision.transform.name == "Tilemap")
        {
            foreach (ContactPoint2D contactPoint in collision.contacts)
            {
                Vector2 hitPoint = contactPoint.point;

                if (hitPoint.y - transform.position.y > 0)
                {
                    Jump();
                    break;
                }

            }

        }
    }

    private void Jump()
    {
        if (onGround && currentJumpTime <= 0)
        {
            currentJumpTime = jumpCooldown;
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
        }
        else
        {
            isFacedRight = false;
            moveX = -1;
        }

        Vector2 move = new Vector2(moveX * (speed), rb.velocity.y);

        rb.velocity = move;

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

        if (player.position.x >= transform.position.x)
        {
            isFacedRight = true;
        }
        else
        {
            isFacedRight = false;
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

    private int CurrentDamage()
    {
        return 1;
    }

    public void Sleep(float newSleepingTime)
    {
        sleepTime = newSleepingTime;
    }

    private void Shoot()
    {
        Vector2 difference = player.position - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        if (currentCd > 0)
        {
            return;
        }
        currentCd = cooldown;

        PlayerMovement.isShooting = true;
        Instantiate(bullet, shotPoint.position, Quaternion.Euler(0f, 0f, rotZ));
    }

    private float CheckDistance()
    {
        return Mathf.Sqrt(Mathf.Pow(player.position.x - transform.position.x, 2) + Mathf.Pow(player.position.y - transform.position.y, 2));
    }

    private bool CheckReach()
    {
        Vector2 difference = player.position - transform.position;
        RaycastHit2D hitInfo = Physics2D.Raycast(shotPoint.position, difference);
        return hitInfo.transform.name == "Player";
    }
}