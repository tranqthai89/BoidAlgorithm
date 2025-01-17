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

    // void Awake()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     CheckIfOutOfBoundary();
    // }
    // private void CheckIfOutOfBoundary(){
    //     // Kiểm tra các trục: nếu vượt ra vùng limit thì sẽ đặt lại vị trí là phía đối diện của trục
    //     if(Mathf.Abs(transform.position.x) > ECS_SpawnManager.Instance.boundery.XLimit){
    //         Vector3 _pos = transform.position;
    //         if(transform.position.x > 0){
    //             _pos.x = -ECS_SpawnManager.Instance.boundery.XLimit;
    //         }else{
    //             _pos.x = ECS_SpawnManager.Instance.boundery.XLimit;
    //         }
    //         transform.position = _pos;
    //     }
    //     if(Mathf.Abs(transform.position.y) > ECS_SpawnManager.Instance.boundery.YLimit){
    //         Vector3 _pos = transform.position;
    //         if(transform.position.y > 0){
    //             _pos.y = -ECS_SpawnManager.Instance.boundery.YLimit;
    //         }else{
    //             _pos.y = ECS_SpawnManager.Instance.boundery.YLimit;
    //         }
    //         transform.position = _pos;
    //     }
    // }
}
