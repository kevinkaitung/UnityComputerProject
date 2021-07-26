using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class gamePropsDestroy : MonoBehaviour
{
    float timer = 0.0f;
    //life time of the game prop existing in the scene
    [SerializeField]
    float durationTime = 20.0f;
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
            Destroy(this.gameObject);
        }
    }

    //if the game prop was clicked, destroy it (only master client do it) 
    [PunRPC]
    public void destroyObject()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }
}
