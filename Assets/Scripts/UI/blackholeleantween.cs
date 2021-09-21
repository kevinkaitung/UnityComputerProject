using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class blackholeleantween : MonoBehaviour
{
    public GameObject blackhole; 
    int i = 10;

    // Start is called before the first frame update
    void Start()
    {
        blackhole.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
    }   

    // Update is called once per frame
    void Update()
    {
        blackhole.transform.rotation = Quaternion.Euler(0.0f, 0.0f, i);
        i+=i;
    }

    
    
}
