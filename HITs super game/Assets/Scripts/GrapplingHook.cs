using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    LineRenderer line;

    Vector3 target;
    RaycastHit2D raycast;

    public float dist = 10f;
    public LayerMask mask;

    public static bool isHooked = false;

    public Transform player;
    private float speed = 45;
    private Vector2 endHookPos;

    public static bool brokeRope = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        line = GetComponent<LineRenderer>();

        dist = 20;

        line.enabled = false;
    }

    private void Update()
    {
        if (isHooked)
        {
            MovePlayer(endHookPos);
        }

        if (brokeRope)
        {
            line.enabled = false;
            brokeRope = false;
        }

        if (Input.GetKey(KeyCode.T))
        {
            isHooked = false;
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            target.z = 0;

            raycast = Physics2D.Raycast(transform.position, target, dist, mask);

            if (raycast)
            {
                isHooked = true;
                DrawHook(transform.position, raycast.point, true);
            }

            else
            {
                DrawHook(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }

        else
        {
            
        }
    }

    void DrawHook(Vector2 startPos, Vector2 endPos, bool reach = false)
    {
        line.enabled = true;
        line.SetPosition(0, transform.position);

        Vector2 currentHookPosition = new Vector2(startPos.x, startPos.y);

        if (!reach)
        {
            float k = dist / Mathf.Sqrt(Mathf.Pow(endPos.x - startPos.x, 2) + Mathf.Pow(endPos.y - startPos.y, 2));

            float newX = startPos.x + (endPos.x - startPos.x) * k;
            float newY = startPos.y + (endPos.y - startPos.y) * k;

            endPos.x = newX;
            endPos.y = newY;
        }

        while (currentHookPosition.x < endPos.x)
        {
            currentHookPosition.x += 0.1f;
            currentHookPosition.y += 0.1f;

            line.SetPosition(1, currentHookPosition);
        }

        line.SetPosition(1, endPos);

        if (!reach)
        {
            HookBack();
        }
        else
        {
            endHookPos = endPos;
        }
    }

    void HookBack()
    {

    }

    void MovePlayer(Vector2 endPos)
    {
        if (CheckDist() < 1.5) return;
        player.transform.position = Vector2.MoveTowards(player.position, endPos, speed * Time.deltaTime);
        line.SetPosition(0, transform.position);
    }

    float CheckDist()
    {
        return Mathf.Sqrt(Mathf.Pow(endHookPos.x - player.transform.position.x, 2) + Mathf.Pow(endHookPos.y - player.transform.position.y, 2));
    }


}
