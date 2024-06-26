using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public float speed = 10;
    private Transform player;

    private Rigidbody2D rb;

    public int jumpForce = 500;
    public static float slowRate = 1;

    public int damage = 1;

    private bool isFacedRight = true;

    private float flipTime = 0.2f;
    private float currentFlipTime = 0;

    private float sleepTime = 0f;

    public Transform attackPoint;
    public float attackRange = 5;
    public LayerMask enemyLayers;

    public float radius = 2f;
    public float angularSpeed = 2f;

    public float positionX, positionY, angle = 0;

    private bool nearPlayer = false;

    private float currentAttackCd = 0f;
    private float attackCd = 0.1f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        sleepTime -= Time.deltaTime;
        currentAttackCd -= Time.deltaTime;
        if (sleepTime > 0) return;

        Move();
        Flip();

        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        if (hitPlayer.Length > 0 && currentAttackCd <= 0)
        {
            Attack(hitPlayer);
        }

    }

    private void Attack(Collider2D[] hitPlayer)
    {
        currentAttackCd = attackCd;
        foreach (Collider2D npc in hitPlayer)
        {
            PlayerStats hittedNpc = npc.GetComponent<PlayerStats>();
            hittedNpc.TakeDamage(1, 0);
        }

    }

    private void Move() {
        Vector2 verticalMoving;

        if (!nearPlayer && CalculateDist() < 10)
        {
            if (player.position.x > transform.position.x)
            {
                angle = 180;
            }
            else
            {
                angle = 0;
            }

            nearPlayer = true;  
        }

        if (nearPlayer)
        {
            verticalMoving = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
        else
        {
            verticalMoving = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime * 10);
        }

        positionX = Mathf.Cos(angle) * radius;
        positionY = verticalMoving.y;

        if (nearPlayer)
        {
            transform.position = new Vector2(player.position.x + positionX, positionY);
        }
        else
        {
            transform.position = new Vector2(verticalMoving.x, positionY);
        }

        if (nearPlayer)
        {
            angle = angle + Time.deltaTime * angularSpeed;
            angle %= 360;
        }
        
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

    private float CalculateDist()
    {
        return Mathf.Sqrt(Mathf.Pow(transform.position.x - player.position.x, 2) +
            Mathf.Pow(transform.position.y - player.position.y, 2));
    }
}
