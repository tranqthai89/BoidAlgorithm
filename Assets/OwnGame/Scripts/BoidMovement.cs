using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidMovement : MonoBehaviour
{
    public float forwardSpeed;
    public float radiusDetect; // bán kính detect
    public float visionAngle; // Tầm nhìn 270 độ của boid

    public Vector3 Velocity{get;private set;}

    void FixedUpdate()
    {
        // - Cập nhật Velocity dần dần theo hướng di chuyển
        Velocity = Vector2.Lerp(Velocity, transform.forward * forwardSpeed, 10* Time.fixedDeltaTime);
        
        // - Tính toán và cập nhật vị trí mới dựa trên vận tốc
        transform.position += Velocity * Time.fixedDeltaTime;

        // - Xoay đối tượng hướng theo vector vận tốc 
        transform.rotation = Quaternion.Slerp(transform.localRotation
            ,Quaternion.LookRotation(Velocity), Time.fixedDeltaTime);
    }
    private List<BoidMovement> GetBoidsInRange(){
        List<BoidMovement> _boids = SpawnManager.Instance.boids;
        float _powRadius = radiusDetect * radiusDetect;
        var _listBoids = _boids.FindAll(_boid => _boid != this
            && Vector2.SqrMagnitude((Vector2) transform.position - (Vector2) _boid.transform.position) <= _powRadius
            && InVisionCone(_boid.transform.position));
        return _listBoids;
    }
    private bool InVisionCone(Vector2 _pos){
        return false;
    }
}
