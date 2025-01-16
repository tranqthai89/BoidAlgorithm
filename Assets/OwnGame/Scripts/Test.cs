using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class Test : MonoBehaviour
{
    public struct AAA{
        public int bbbb;
    }
    // Start is called before the first frame update
    void Start()
    {
        NativeList<AAA> _boidsInRange = new NativeList<AAA>(Allocator.Temp);

        for(int i = 0; i < 5; i ++){
            AAA _tmp = new AAA{
                bbbb = i
            };
            _boidsInRange.Add(_tmp);
        }
        for(int i = 0; i < _boidsInRange.Length; i ++){
            for(int j = 0; j < _boidsInRange.Length; j ++){
                if(!_boidsInRange[i].Equals(_boidsInRange[j])){
                    Debug.LogError("hehehehehe");
                }
            }
        }
        _boidsInRange.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
