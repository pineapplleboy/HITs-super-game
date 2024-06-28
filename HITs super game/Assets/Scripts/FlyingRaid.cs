using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FlyingRaid : MonoBehaviour
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

    private WorldGeneration world;

    private COMPUTER computer;

    private Vector2 target;

    private string targetedObject;

    private float hitBlockCd = 0.3f;
    private float currentHitBlockCd = 0f;

    private int blockDamage = 4;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();

        world = GameObject.FindGameObjectWithTag("World").GetComponent<WorldGeneration>();

        computer = GameObject.FindGameObjectWithTag("COMPUTER").GetComponent<COMPUTER>();

        target = player.position;
    }

    void FixedUpdate()
    {
        sleepTime -= Time.deltaTime;
        currentAttackCd -= Time.deltaTime;
        currentHitBlockCd -= Time.deltaTime;
        if (sleepTime > 0) return;

        FindTarget();

        Move();
        Flip();

        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        if (hitPlayer.Length > 0 && currentAttackCd <= 0)
        {
            Attack(hitPlayer);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.name == "Tilemap")
        {
            foreach (ContactPoint2D contactPoint in collision.contacts)
            {
                Vector2 hitPoint = contactPoint.point;
                HitWall(hitPoint);

            }
        }
    }

    private void HitWall(Vector2 hitPoint)
    {
        if (currentHitBlockCd > 0) return;

        currentHitBlockCd = hitBlockCd;

        WorldBlock block = world.GetBlock((int)(hitPoint.x), (int)(hitPoint.y));

        if (block == null)
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
        }
        block = world.GetBlock((int)(hitPoint.x), (int)(hitPoint.y));

        if (block == null)
        {
            hitPoint.y += 1f;
            block = world.GetBlock((int)(hitPoint.x), (int)(hitPoint.y));
        }

        if (block != null)
        {
            block.TakeDamage(1);
            //Debug.Log(block.GetHealth());
            if (block.GetHealth() <= 0)
            {
                world.EnemyBreakBlock(hitPoint);
            }
        }

    }

    private void FindTarget()
    {
        if (nearPlayer && targetedObject == "player")
        {
            target = player.position;
            return;
        }
        else if (nearPlayer && targetedObject == "computer")
        {
            target = computer.transform.position;
            return;
        }

        if (CheckDistance(transform.position, player.transform.position) < CheckDistance(transform.position, computer.transform.position))
        {
            target = player.position;
            targetedObject = "player";
        }
        else
        {
            target = computer.transform.position;
            targetedObject = "computer";
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

    private void Move()
    {
        Vector2 verticalMoving;

        if (!nearPlayer && CalculateDist() < 10)
        {
            if (target.x > transform.position.x)
            {
                angle = 180;
            }
            else
            {
                angle = 0;
            }

            nearPlayer = true;
        }

        if (nearPlayer && CalculateDist() > 20)
        {
            nearPlayer = false;
            angle = 0;
        }

        if (nearPlayer)
        {
            verticalMoving = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
        else
        {
            verticalMoving = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime * 10);
        }

        positionX = Mathf.Cos(angle) * radius;
        positionY = verticalMoving.y;

        if (nearPlayer)
        {
            transform.position = new Vector2(target.x + positionX, positionY);
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

        if (target.x >= transform.position.x)
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
        return Mathf.Sqrt(Mathf.Pow(transform.position.x - target.x, 2) +
            Mathf.Pow(transform.position.y - target.y, 2));
    }

    private float CheckDistance(Vector2 first, Vector2 second)
    {
        return Mathf.Sqrt(Mathf.Pow(first.x - second.x, 2) + Mathf.Pow(first.y - second.y, 2));
    }
}
