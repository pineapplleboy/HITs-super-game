using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScript : MonoBehaviour
{
    [SerializeField] GameObject Camera;
    [SerializeField] float paralaxEffect;

    void Start()
    {

    }

    void FixedUpdate()
    {
        transform.position = new Vector3(Camera.transform.position.x, Camera.transform.position.y, transform.position.z);
    }
}
