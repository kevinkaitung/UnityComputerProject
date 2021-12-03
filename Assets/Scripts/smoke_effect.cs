using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smoke_effect : MonoBehaviour
{
    /*[SerializeField]
    private Image one;
    [SerializeField]
    private Image two;
    [SerializeField]
    private Image whree
    [SerializeField]
    private Image four;
    [SerializeField]
    private Image five;
    [SerializeField]
    private Image six;
    [SerializeField]
    private Image seven;
    [SerializeField]
    private Image eight;
    [SerializeField]
    private Image nine;
    [SerializeField]
    private Image ten;
    [SerializeField]
    private Image eleven;
    [SerializeField]
    private Image twelve;
    [SerializeField]
    private Image three;
    [SerializeField]
    private Image fourteen;
    [SerializeField]
    private Image fifteen;*/
    
    System.Random Posrandom = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 15; i++)
        {
            GameObject clone = Instantiate(Resources.Load("Assets/Resources/smokePanel", typeof(GameObject))) as GameObject;
            clone.transform.localPosition = new Vector2((int)Posrandom.Next(-200,200), (int)Posrandom.Next(-100,100));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
