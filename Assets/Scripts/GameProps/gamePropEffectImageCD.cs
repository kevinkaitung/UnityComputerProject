using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gamePropEffectImageCD : MonoBehaviour
{
    public float timer = 0.0f;
    [SerializeField]
    private float duration = 10.0f;
    //record the start time (time since level load)
    public float startTime = 0.0f;
    public string propEffect = "";
    [SerializeField]
    public Image mask;

    void Awake()
    {
        //if init after awake, set start time first
        if(startTime <= 0.0f)
        {
            startTime = Time.timeSinceLevelLoad;
        }
    }

    public void init(float startTimeInput, string propEffectInput)
    {
        startTime = startTimeInput;
        propEffect = propEffectInput;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        mask.fillAmount = timer / duration;

        if (timer > duration)
        {
            Destroy(this.gameObject);
        }
    }

    //back from this object previous inactive; after active, set timer to correct seconds
    void OnEnable()
    {
        timer = Time.timeSinceLevelLoad - startTime;
    }
}
