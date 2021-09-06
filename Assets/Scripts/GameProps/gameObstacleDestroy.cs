using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameObstacleDestroy : MonoBehaviour
{
    float timer = 0.0f;
    [SerializeField]
    float durationTime = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > durationTime)
        {
            //time's up, destroy the obstacle
            Destroy(this.gameObject);
        }
    }
}
