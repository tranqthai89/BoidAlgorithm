using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECS_BoidController : MonoBehaviour
{
    public float forwardSpeed;
    public float radiusDetect; // bán kính detect
    public float visionAngle; // Tầm nhìn 270 độ của boid
    public float turnSpeed; // Số lần cập nhật

    public Vector3 Velocity{get; set;}

    // Start is called before the first frame update
    // void Awake()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }
}
