using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clickNotice : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable() {
        LeanTween.scale(this.gameObject, new Vector3(1.0f, 1.0f, 1.0f), 0.0f);   
        LeanTween.scale(this.gameObject, new Vector3(0.8f, 0.8f, 0.8f), 0.8f).setEaseInOutCubic().setLoopPingPong();
    }
    private void OnDisable() {
        LeanTween.cancel(this.gameObject);
    }
}
