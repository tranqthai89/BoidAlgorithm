using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Thuật toán tính vector di chuyển của đàn cá boid
/// </summary>
public static class BoidAlgorithm
{
    /// <summary>
    /// Tính ra Vector né các Boids, tránh va chạm vào nhau
    /// </summary>
    public static Vector2 Separation (Vector2 _currentPosition, Vector2 _boidOtherPosition, float _radiusDetect)
    {         
        // Tỉnh tỷ lệ khoảng cách từ boid hiện tại đến boid trong danh sách so với bán kính quy định, giá trị này được giới hạn trong khoảng [O,1].
        float _ratio = Mathf.Clamp01((_boidOtherPosition - _currentPosition).magnitude / _radiusDetect);

        // Giảm vector hướng bằng cách trừ đi một phần tỷ lệ của vector từ vị trí hiện tại đến vị trí boid, đảm bảo rằng boid sẽ di chuyển ra xa các boid khác.
        // (1-ratio) => càng gần boid khác thì lực đẩy mạnh hơn
        Vector2 _direction = (1 - _ratio) * (_boidOtherPosition - _currentPosition);
        return _direction.normalized;
    }
    /// <summary>
    /// Tính ra Vector để di chuyển cùng hướng theo đàn. Mỗi boid sẽ cố gắng đi theo hướng chung của các boid khác
    /// </summary>
    public static Vector2 Aligment (Vector2 _direction, Vector2 _forward, int _boidCount)
    {
        if (_boidCount != 0) {
            // Nếu có boids trong danh sách, chia vector hướng cho số lượng boids để tính trung bình vận tốc.
            _direction /= _boidCount;
        }
        else { 
            // Nếu không có boids nào khác, sử dụng vận tốc hiện tại của boid này.Chuẩn hóa vector hướng để đảm bảo chỉ hướng chuyển động được trả về, không phải độ lớn của nó.
            _direction = _forward;
        }

        // Chuẩn hóa vector hướng để đảm bảo chỉ hướng chuyển động được trả về, không phải độ lớn của nó.
        return _direction.normalized;
    }
    /// <summary>
    /// 1 Boid sẽ tìm trung tâm các Boids lân cận và cố gắng hướng về nó
    /// </summary>
    public static Vector2 Cohesion (Vector2 _center, Vector2 _currentPosition, int _boidCount)
    {
        if (_boidCount != 0) {
            // Nếu có boid, center là trung bình vị trí 
            _center /= _boidCount; 
        }
        else {
            // Nếu không, center là vị trí hiện tại của boid đang xét
            _center = _currentPosition;
        }

        // Tỉnh vector từ vị trí hiện tại đến trung tâm đàn.
        // Chuẩn hóa vector hướng và trả về vector này.
        return (_center - _currentPosition).normalized;
    }

    /* Example:
    private Vector2 CaculateVelocity()
    {
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
    }*/
}
