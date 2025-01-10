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
        // - Tính Vector từ đối tượng tới vị trí
        Vector2 _directionToPosition = _pos - (Vector2) transform.position;
        // - Tính tích vô hướng của vector này tới hướng đối tượng
        float _dotProduct = Vector2.Dot(transform.forward, _directionToPosition);
        // - Tính cosin của nửa góc tầm nhìn (visionAngle), chuyển từ độ sang radian. Góc này xác định kích thước của hình nón tầm nhìn
        float _cosHalfVisionAngle = Mathf.Cos(visionAngle * 0.5f * Mathf.Deg2Rad);
        // - So sánh kết quả với cosin của nửa góc tầm nhìn để xác định vị trí có nằm trong hình nón tầm nhìn hay không
        return _dotProduct >= _cosHalfVisionAngle;
    }

    private void OnDrawGizmosSelected()
    {
        // - Vẽ hình cầu dây ở vị trí của đối tượng với bán kính được xác định
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusDetect);

        var _boidsInRange = GetBoidsInRange();
        foreach (var _boid in _boidsInRange)
        {
            // Vẽ 1 đường thẳng từ vị trí hiện tại của đối tượng đến vị trí của từng boid, minh họa các boid khác nằm trong tầm nhìn.
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _boid.transform.position);
        }
    }
}
