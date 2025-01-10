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

    public List<BoidMovement> boids;
    [SerializeField] private BoidMovement boidPrefab;
    [SerializeField] private int boidCount;

    void Awake()
    {
        ins = this;
    }

    void Start()
    {
        if(boids.Count > 0){
            boids.Clear();
        }

        for(int i = 0; i < boidCount; i ++){
            float _direction = Random.Range(0, 360f);

            Vector3 _pos = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
            BoidMovement _boid = Instantiate(boidPrefab
                , _pos
                , Quaternion.Euler(Vector3.forward * _direction) * boidPrefab.transform.localRotation);
            boids.Add(_boid);
        }
    }
}
