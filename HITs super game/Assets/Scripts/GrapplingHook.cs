using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    LineRenderer line;

    Vector3 target;
    RaycastHit2D raycast;

    public float dist = 10f;
    public LayerMask mask;

    private void Start()
    {
        line = GetComponent<LineRenderer>();

        line.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.T))
        {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = 0;

            raycast = Physics2D.Raycast(transform.position, target, dist, mask);

            if (raycast)
            {
                Debug.Log(raycast.transform.name);

                line.enabled = true;
                line.SetPosition(0, transform.position);
                line.SetPosition(1, raycast.point);
            }
        }

        else
        {
            line.enabled = false;
        }
    }
}
