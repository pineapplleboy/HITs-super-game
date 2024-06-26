using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private bool ropeDrawen = false;
    public static bool stopMoving = false;
    private Vector2 ropeDrawCoord;

    public static bool brokeRope = false;

    public static int needToDraw = 0;

    private bool ropeBack = false;
    private Vector2 positionWhenThrowen;
    private bool catchToPlayer = false;

    private List<float> delta = new List<float>() { 0, 0 };

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        line = GetComponent<LineRenderer>();

        dist = 20;

        line.enabled = false;
    }

    private void Update()
    {
        stopMoving = ropeDrawen;
        if (isHooked && ropeDrawen)
        {
            MovePlayer(endHookPos);
            needToDraw = 0;
        }
        else if (isHooked && needToDraw == 1)
        {
            DrawHook();
        }
        else if (needToDraw == 2)
        {
            HookBack();
        }
        else
        {
            needToDraw = 0;
            ropeDrawen = false;
            brokeRope = true;
        }

        if (brokeRope)
        {
            line.enabled = false;
            brokeRope = false;
        }

        if (Input.GetKey(KeyCode.T) && needToDraw == 0)
        {
            isHooked = false;
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            target.z = 0;

            raycast = Physics2D.Raycast(transform.position, target, dist, mask);

            if (raycast)
            {
                isHooked = true;
                endHookPos = GetEndCoord(transform.position, raycast.point, true);
                ropeDrawCoord = transform.position;
                needToDraw = 1;
                delta = CalculateDelta(transform.position, raycast.point);
            }

            else
            {
                endHookPos = GetEndCoord(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                needToDraw = 2;
                delta = CalculateDelta(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                ropeDrawCoord = transform.position;
                positionWhenThrowen = transform.position;
            }
        }
    }

    Vector2 GetEndCoord(Vector2 startPos, Vector2 endPos, bool reach = false)
    {
        if (!reach)
        {
            float k = dist / Mathf.Sqrt(Mathf.Pow(endPos.x - startPos.x, 2) + Mathf.Pow(endPos.y - startPos.y, 2));

            float newX = startPos.x + (endPos.x - startPos.x) * k;
            float newY = startPos.y + (endPos.y - startPos.y) * k;

            endPos.x = newX;
            endPos.y = newY;
        }

        return endPos;
    }

    List<float> CalculateDelta(Vector2 startPos, Vector2 endPos)
    {
        List <float> res = new List<float>() { 0, 0 };

        float gipo = Mathf.Sqrt(Mathf.Pow(endPos.x - startPos.x, 2) + Mathf.Pow(endPos.y - startPos.y, 2));

        res[0] = (endPos.x - startPos.x) / 20;
        res[1] = (endPos.y - startPos.y) / 20;
        return res;
    }

    List<float> CalculatRopeBackToPlayerDelta(Vector2 startPos)
    {
        List<float> res = new List<float>() { 0, 0 };

        float gipo = Mathf.Sqrt(Mathf.Pow(player.position.x - startPos.x, 2) + Mathf.Pow(player.position.y - startPos.y, 2));

        res[0] = (player.position.x - startPos.x) / 10;
        res[1] = (player.position.y - startPos.y) / 10;
        return res;
    }

    void DrawHook()
    {
        line.enabled = true;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, ropeDrawCoord);

        if (CheckDist(ropeDrawCoord, endHookPos) < 1.5)
        {
            ropeDrawen = true;
        }

        ropeDrawCoord.x += delta[0];
        ropeDrawCoord.y += delta[1];
    }

    void HookBack()
    {
        line.enabled = true;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, ropeDrawCoord);

        if (CheckDist(ropeDrawCoord, endHookPos) < 1.5)
        {
            ropeBack = true;
        }

        if (!ropeBack)
        {
            ropeDrawCoord.x += delta[0];
            ropeDrawCoord.y += delta[1];
        }
        else
        {
            ropeDrawCoord.x -= delta[0];
            ropeDrawCoord.y -= delta[1];
        }

        if (ropeBack && CheckDist(ropeDrawCoord, positionWhenThrowen) < 1.5)
        {
            ropeBack = false;
            catchToPlayer = true;
            delta = CalculatRopeBackToPlayerDelta(positionWhenThrowen);
        }
        
        if (catchToPlayer && CheckDist(ropeDrawCoord, transform.position) < 1.5)
        {
            needToDraw = 0;
            line.enabled = false;
            catchToPlayer = false;
        }

        if (CheckDist(ropeDrawCoord, transform.position) > 50)
        {
            needToDraw = 0;
            line.enabled = false;
            catchToPlayer = false;
        }

    }

    void MovePlayer(Vector2 endPos)
    {
        if (CheckDist(player.transform.position, endHookPos) < 1.5) return;
        player.transform.position = Vector2.MoveTowards(player.position, endPos, speed * Time.deltaTime);
        line.SetPosition(0, transform.position);
        line.SetPosition(1, endPos);
    }

    float CheckDist(Vector2 fistVec, Vector2 secVec)
    {
        return Mathf.Sqrt(Mathf.Pow(secVec.x - fistVec.x, 2) + Mathf.Pow(secVec.y - fistVec.y, 2));
    }
}
