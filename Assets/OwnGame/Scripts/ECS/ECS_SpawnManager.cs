using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECS_SpawnManager : MonoBehaviour
{
    public static ECS_SpawnManager Instance{
        get{
            return ins;
        }
    }
    static ECS_SpawnManager ins;

    public List<ECS_BoidController> ListBoids {get;set;}
    [SerializeField] private ECS_BoidController boidPrefab;
    [SerializeField] private int boidCount;

    void Awake()
    {
        ins = this;
        if(ListBoids == null){
            ListBoids = new List<ECS_BoidController>();
        }else if(ListBoids.Count > 0){
            ListBoids.Clear();
        }

        float _yLimit = Camera.main.orthographicSize;
        float _xLimit = _yLimit * Screen.width / Screen.height;

        for(int i = 0; i < boidCount; i ++){
            float _direction = UnityEngine.Random.Range(0, 360f);

            Vector3 _pos = new Vector2(UnityEngine.Random.Range(-_xLimit, _xLimit) , UnityEngine.Random.Range(-_yLimit, _yLimit));
            ECS_BoidController _boid = Instantiate(boidPrefab
                , _pos
                , Quaternion.Euler(Vector3.forward * _direction) * boidPrefab.transform.rotation);

            // - Không nên set parent, vì mỗi lần check phải check localPosition => giảm performance
            // _boid.transform.SetParent(transform);

            ListBoids.Add(_boid);
        }
    }
}
