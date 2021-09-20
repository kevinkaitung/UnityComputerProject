using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gamePropEffectImageCD : MonoBehaviour
{
    public float timer = 0.0f;
    [SerializeField]
    private float duration = 10.0f;
    public string propEffect = "";
    [SerializeField]
    public Image mask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        mask.fillAmount = timer/duration;

        if(timer > duration)
        {
            Destroy(this.gameObject);
        }
    }
}
