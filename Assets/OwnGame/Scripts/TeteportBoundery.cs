using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeteportBoundery : MonoBehaviour
{
    [SerializeField] private Boundary boundery;

    void Start()
    {
        boundery = new Boundary();
    }

    void FixedUpdate()
    {
        // Kiểm tra các trục: nếu vượt ra vùng limit thì sẽ đặt lại vị trí là phía đối diện của trục
        if(Mathf.Abs(transform.position.x) > boundery.XLimit){
            Vector3 _pos = transform.position;
            if(transform.position.x > 0){
                _pos.x = -boundery.XLimit;
            }else{
                _pos.x = boundery.XLimit;
            }
            transform.position = _pos;
        }
        if(Mathf.Abs(transform.position.y) > boundery.YLimit){
            Vector3 _pos = transform.position;
            if(transform.position.y > 0){
                _pos.y = -boundery.YLimit;
            }else{
                _pos.y = boundery.YLimit;
            }
            transform.position = _pos;
        }
    }    
}
