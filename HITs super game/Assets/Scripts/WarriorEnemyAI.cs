using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WarriorEnemyAI : MonoBehaviour
{
    public float speed = 1;
    private Transform player;

    private Rigidbody2D rb;

    public int jumpForce = 500;
    public static float slowRate = 1;

    public int damage = 100;

    private bool onGround = false;
    private bool isFacedRight = true;

    private float flipTime = 0.2f;
    private float currentFlipTime = 0;

    private float jumpCooldown = 0.3f;
    private float currentJumpTime = 0;

    public Transform attackPoint;
    public float attackRange = 5;
    public LayerMask enemyLayers;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
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

        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        if (hitPlayer.Length > 0)
        {
            //Attack(hitPlayer);
        }
        //transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
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

    //private void Attack(Collider2D[] hitPlayer) // не назначены точки атаки, радиус и кого может бить
    //{
    //    foreach (Collider2D npc in hitPlayer)
    //    {
    //        PlayerStats hittedNpc = npc.GetComponent<PlayerStats>();
    //        hittedNpc.TakeDamage(CurrentDamage(), 0);
    //    }

    //}

    private int CurrentDamage()
    {
        return 1;
    }
}
