using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance{
        get{
            return ins;
        }
    }
    static SpawnManager ins;

    public Boundary boundery;
    public List<BoidMovement> ListBoids{get;set;}
    [SerializeField] private BoidMovement boidPrefab;
    [SerializeField] private int boidCount;

    void Awake()
    {
        ins = this;

        boundery = new Boundary();
        
        if(ListBoids == null){
            ListBoids = new List<BoidMovement>();
        }else if(ListBoids.Count > 0){
            ListBoids.Clear();
        }

        float _yLimit = Camera.main.orthographicSize;
        float _xLimit = _yLimit * Screen.width / Screen.height;
        
        for(int i = 0; i < boidCount; i ++){
            float _direction = Random.Range(0, 360f);

            Vector3 _pos = new Vector2(Random.Range(-_xLimit, _xLimit) , Random.Range(-_yLimit, _yLimit));
            BoidMovement _boid = Instantiate(boidPrefab
                , _pos
                , Quaternion.Euler(Vector3.forward * _direction) * boidPrefab.transform.localRotation);

            // - Không nên set parent, vì mỗi lần check phải check localPosition => giảm performance
            // _boid.transform.SetParent(transform);
            ListBoids.Add(_boid);
        }
    }
}
