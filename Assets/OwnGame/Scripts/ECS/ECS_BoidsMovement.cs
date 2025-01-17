using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

/// <summary>
/// Class có ứng dụng ECS
/// </summary>
public class ECS_BoidsMovement : MonoBehaviour
{
    private TransformAccessArray transformAccessArray;
    private NativeArray<BoidData> listBoids;

    private struct BoidData{
        public int index;
        // Vì get_transform không thể lấy trong Job được
        public float3 position;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        var _boidCount = ECS_SpawnManager.Instance.ListBoids.Count;
        transformAccessArray = new TransformAccessArray(_boidCount);

        //Allocator.Persistent : bộ nhớ dài hạn
        //Allocator.Temp : bộ nhớ tạm giải phóng sau mỗi khung hình
        //Allocator.TempJob : bộ nhớ tạm cho job, giải phóng sau khi job hoàn thành
        listBoids = new NativeArray<BoidData>(_boidCount, Allocator.Persistent);
        for(int i = 0; i < _boidCount; i ++){
            transformAccessArray.Add(ECS_SpawnManager.Instance.ListBoids[i].transform);
            listBoids[i] = new BoidData{
                index = i,
                position = ECS_SpawnManager.Instance.ListBoids[i].transform.position,
            };
        }
    }

    void Update()
    {
        BoidMovementsJob _boidMovementsJob = new BoidMovementsJob
        {
            listBoids = listBoids,
            deltaTime = Time.deltaTime,
        };
        JobHandle _boidMovementsJobHandle = _boidMovementsJob.Schedule(transformAccessArray);
        _boidMovementsJobHandle.Complete();
    }

    void OnDestroy()
    {
        transformAccessArray.Dispose();
        listBoids.Dispose();
    }

    [BurstCompile]
    private struct BoidMovementsJob : IJobParallelForTransform
    {
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<BoidData> listBoids;
        public float deltaTime;

        public void Execute(int _index, TransformAccess _transform)
        {
            Vector3 _velocity = ECS_SpawnManager.Instance.ListBoids[_index].Velocity;

            // - Cập nhật Velocity dần dần theo hướng di chuyển
            _velocity = Vector2.Lerp(_velocity, CaculateVelocity(_index, _transform), ECS_SpawnManager.Instance.ListBoids[_index].turnSpeed / 2 * deltaTime);
            
            // - Tính toán và cập nhật vị trí mới dựa trên vận tốc
            _transform.position += _velocity * deltaTime;

            CheckIfOutOfBoundary(_transform);

            if(_velocity != Vector3.zero){
                // - Xoay đối tượng hướng theo vector vận tốc 
                _transform.rotation = Quaternion.Slerp(_transform.rotation
                    ,Quaternion.LookRotation(_velocity), ECS_SpawnManager.Instance.ListBoids[_index].turnSpeed * deltaTime);
            }

            // - Cập nhật lại data
            ECS_SpawnManager.Instance.ListBoids[_index].Velocity = _velocity;
            listBoids[_index] = new BoidData{
                index = _index,
                position = _transform.position,
            };

        }
        private Vector2 CaculateVelocity(int _index, TransformAccess _transform)
        {
            // Danh sách các boids nằm trong phạm vi ảnh hưởng
            Vector2 _currentForward = _transform.localToWorldMatrix.MultiplyVector(Vector3.forward);
            var _boidsInRange = GetBoidsInRange(_index, _transform.position, _currentForward, ECS_SpawnManager.Instance.ListBoids[_index].radiusDetect, ECS_SpawnManager.Instance.ListBoids[_index].visionAngle); 
            int _boidCount = _boidsInRange.Length;

            Vector2 _separation = Vector2.zero;
            Vector2 _direction_Aligment = Vector2.zero;
            Vector2 _center_Cohesion = Vector2.zero;
            for(int i = 0; i < _boidCount; i ++){
                _separation -= BoidAlgorithm.Separation(_transform.position, _boidsInRange[i].position.xy, ECS_SpawnManager.Instance.ListBoids[_index].radiusDetect);
                _direction_Aligment += (Vector2) ECS_SpawnManager.Instance.ListBoids[_boidsInRange[i].index].Velocity;
                _center_Cohesion += (Vector2) _boidsInRange[i].position.xy;
            }
            Vector2 _aligment = BoidAlgorithm.Aligment(_direction_Aligment, _currentForward, _boidCount);
            Vector2 _cohesion = BoidAlgorithm.Cohesion(_center_Cohesion, _transform.position, _boidCount);
            Vector2 _velocity = (_currentForward
                    + 1.7f * _separation
                    + 0.1f * _aligment
                    + _cohesion
                    ).normalized * ECS_SpawnManager.Instance.ListBoids[_index].forwardSpeed;
            return _velocity; 
        }
        private NativeArray<BoidData> GetBoidsInRange(int _currentIndex, float3 _currentPosition, float2 _currentForward, float _currentRadiusDetect, float _visionAngle){
            NativeList<BoidData> _boidsInRange = new NativeList<BoidData>(Allocator.Temp);
            float _powRadius = _currentRadiusDetect * _currentRadiusDetect;
            for(int i = 0; i < listBoids.Length; i ++){
                if(_currentIndex != listBoids[i].index
                    // && math.distance(_currentPosition, listBoids[i].position) < ECS_SpawnManager.Instance.boids[listBoids[i].index].radiusDetect
                    && Vector2.SqrMagnitude(_currentPosition.xy - listBoids[i].position.xy) <= _powRadius
                    && MyConstant.CheckIfInVisionCone(_currentPosition.xy, _currentForward, _visionAngle, listBoids[i].position.xy)){
                    _boidsInRange.Add(listBoids[i]);
                }
            }
            NativeArray<BoidData> _boids = new NativeArray<BoidData>(_boidsInRange.AsArray(), Allocator.Temp);
            _boidsInRange.Dispose();
            return _boids;
        }
        private void CheckIfOutOfBoundary(TransformAccess _transform){
            // Kiểm tra các trục: nếu vượt ra vùng limit thì sẽ đặt lại vị trí là phía đối diện của trục
            if(Mathf.Abs(_transform.position.x) > ECS_SpawnManager.Instance.boundery.XLimit){
                Vector3 _pos = _transform.position;
                if(_transform.position.x > 0){
                    _pos.x = -ECS_SpawnManager.Instance.boundery.XLimit;
                }else{
                    _pos.x = ECS_SpawnManager.Instance.boundery.XLimit;
                }
                _transform.position = _pos;
            }
            if(Mathf.Abs(_transform.position.y) > ECS_SpawnManager.Instance.boundery.YLimit){
                Vector3 _pos = _transform.position;
                if(_transform.position.y > 0){
                    _pos.y = -ECS_SpawnManager.Instance.boundery.YLimit;
                }else{
                    _pos.y = ECS_SpawnManager.Instance.boundery.YLimit;
                }
                _transform.position = _pos;
            }
        }
    }
}
