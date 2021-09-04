using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gamePropEffectImageCD : MonoBehaviour
{
    public float timer = 0.0f;
    [SerializeField]
    private float duration = 10.0f;
    public string propEffect = "";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > duration)
        {
            Destroy(this.gameObject);
        }
    }
}
