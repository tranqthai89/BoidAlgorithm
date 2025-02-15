using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidMovement : MonoBehaviour
{
    public float forwardSpeed;
    public float radiusDetect; // bán kính detect
    public float visionAngle; // Tầm nhìn 270 độ của boid
    public float turnSpeed; // Số lần cập nhật

    public Vector3 Velocity{get;private set;}

    void FixedUpdate()
    {
        // - Cập nhật Velocity dần dần theo hướng di chuyển
        Velocity = Vector2.Lerp(Velocity, CaculateVelocity(), turnSpeed / 2 * Time.fixedDeltaTime);
        
        // - Tính toán và cập nhật vị trí mới dựa trên vận tốc
        transform.position += Velocity * Time.fixedDeltaTime;

        // - Xoay đối tượng hướng theo vector vận tốc 
        transform.rotation = Quaternion.Slerp(transform.localRotation
            ,Quaternion.LookRotation(Velocity), turnSpeed * Time.fixedDeltaTime);
        
        CheckIfOutOfBoundary();
    }
    private void CheckIfOutOfBoundary(){
        // Kiểm tra các trục: nếu vượt ra vùng limit thì sẽ đặt lại vị trí là phía đối diện của trục
        if(Mathf.Abs(transform.position.x) > SpawnManager.Instance.boundery.XLimit){
            Vector3 _pos = transform.position;
            if(transform.position.x > 0){
                _pos.x = -SpawnManager.Instance.boundery.XLimit;
            }else{
                _pos.x = SpawnManager.Instance.boundery.XLimit;
            }
            transform.position = _pos;
        }
        if(Mathf.Abs(transform.position.y) > SpawnManager.Instance.boundery.YLimit){
            Vector3 _pos = transform.position;
            if(transform.position.y > 0){
                _pos.y = -SpawnManager.Instance.boundery.YLimit;
            }else{
                _pos.y = SpawnManager.Instance.boundery.YLimit;
            }
            transform.position = _pos;
        }
    }
    private Vector2 CaculateVelocity()
    {
        // Danh sách các boids nằm trong phạm vi ảnh hưởng
        // var _boidsInRange = GetBoidsInRange(); 
        // Vector2 _velocity = ((Vector2) transform.forward
        //         + 1.7f * Separation (_boidsInRange) // cộng thêm hướng luật Separation tránh va chạm, di chuyển đè chồng
        //         + 0.1f * Aligment(_boidsInRange) // cộng thêm hướng luật aligment di chuyển theo hướng cùng chiều
        //         + Cohesion(_boidsInRange) // cộng thêm hướng luật cohesion di chuyển theo trung tâm đàn
        //         ).normalized * forwardSpeed;
        // return _velocity; 

        var _boidsInRange = GetBoidsInRange(); 
        int _boidCount = _boidsInRange.Count;

        Vector2 _separation = Vector2.zero;
        Vector2 _direction_Aligment = Vector2.zero;
        Vector2 _center_Cohesion = Vector2.zero;
        for(int i = 0; i < _boidCount; i ++){
            _separation -= BoidAlgorithm.Separation(transform.position, _boidsInRange[i].transform.position, radiusDetect);
            _direction_Aligment += (Vector2) _boidsInRange[i].Velocity;
            _center_Cohesion += (Vector2) _boidsInRange[i].transform.position;
        }
        Vector2 _aligment = BoidAlgorithm.Aligment(_direction_Aligment, transform.forward, _boidCount);
        Vector2 _cohesion = BoidAlgorithm.Cohesion(_center_Cohesion, transform.position, _boidCount);
        Vector2 _velocity = ((Vector2) transform.forward
                + 1.7f * _separation
                + 0.1f * _aligment
                + _cohesion
                ).normalized * forwardSpeed;
        return _velocity; 
    }
    private List<BoidMovement> GetBoidsInRange(){
        List<BoidMovement> _boids = SpawnManager.Instance.ListBoids;
        float _powRadius = radiusDetect * radiusDetect;
        var _listBoids = _boids.FindAll(_boid => _boid != this
            && Vector2.SqrMagnitude((Vector2) transform.position - (Vector2) _boid.transform.position) <= _powRadius
            && MyConstant.CheckIfInVisionCone(transform.position, transform.forward, visionAngle, _boid.transform.position));
            // && InVisionCone(_boid.transform.position));
        return _listBoids;
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
    
    /// <summary>
    /// Tính ra Vector né các Boids, tránh va chạm vào nhau
    /// </summary>
    private Vector2 Separation (List<BoidMovement> _boids)
    {
        // Bắt đầu với một vector hướng không để tích lũy hướng ly tâm. 
        Vector2 _direction = Vector2.zero; 
        // Duyệt qua danh sách các boids để tính hưởng ly tâm đối với tùng bold.
        foreach (var _boid in _boids)
        {
            // Tỉnh tỷ lệ khoảng cách từ boid hiện tại đến boid trong danh sách so với bán kính quy định, giá trị này được giới hạn trong khoảng [O,1].
            float ratio = Mathf.Clamp01((_boid.transform.position - transform.position).magnitude / radiusDetect);
            // Giảm vector hướng bằng cách trừ đi một phần tỷ lệ của vector từ vị trí hiện tại đến vị trí boid, đảm bảo rằng boid sẽ di chuyển ra xa các boid khác.
            _direction -= ratio * (Vector2) (_boid.transform.position - transform.position);
        }
        // Chuẩn hóa vector hưởng cuối cùng để có độ lớn là 1, đảm bảo rằng chỉ hướng chuyển động được trả về, không phải độ lớn của nó.
        return _direction.normalized;
    }

    /// <summary>
    /// Tính ra Vector để di chuyển cùng hướng theo đàn. Mỗi boid sẽ cố gắng đi theo hướng chung của các boid khác
    /// </summary>
    private Vector2 Aligment (List<BoidMovement> _boids)
    {
        // Bắt đầu với một vector hướng không để tích lũy vận tốc trung bình.
        Vector2 _direction = Vector2.zero;

        // Duyệt qua danh sách các boids và cộng dồn vận tốc của từng boid vào vector hưởng.
        foreach (var _boid in _boids) {
            _direction += (Vector2) _boid. Velocity;
        }

        if (_boids.Count != 0) {
            // Nếu có boids trong danh sách, chia vector hướng cho số lượng boids để tính trung bình vận tốc.
            _direction /= _boids.Count;
        }
        else { 
            // Nếu không có boids nào khác, sử dụng vận tốc hiện tại của boid này.Chuẩn hóa vector hướng để đảm bảo chỉ hướng chuyển động được trả về, không phải độ lớn của nó.
            _direction = Velocity;
        }

        // Chuẩn hóa vector hướng để đảm bảo chỉ hướng chuyển động được trả về, không phải độ lớn của nó.
        return _direction.normalized;
    }

    /// <summary>
    /// 1 Boid sẽ tìm trung tâm các Boids lân cận và cố gắng hướng về nó
    /// </summary>
    private Vector2 Cohesion (List<BoidMovement> _boids)
    {
        // Chứa hướng di chuyển; center để tính trung tâm đàn, khởi tạo là vector không.
        Vector2 _direction;
        Vector2 _center = Vector2.zero;

        // Cộng dồn vị trí của tất cả boid trong vòng lặp.
        foreach (var _boid in _boids) _center += (Vector2)_boid.transform.position;

        
        if (_boids.Count != 0) {
            // Nếu có boid, center là trung bình vị trí 
            _center /= _boids.Count; 
        }
        else {
            // Nếu không, center là vị trí hiện tại của boid đang xét
            _center = transform.position;
        }

        // Tỉnh vector từ vị trí hiện tại đến trung tâm đàn.
        _direction = _center - (Vector2) transform.position;

        // Chuẩn hóa vector hướng và trả về vector này.
        return _direction.normalized;
    }

}
