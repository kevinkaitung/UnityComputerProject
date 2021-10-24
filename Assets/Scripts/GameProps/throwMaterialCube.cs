using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class throwMaterialCube : MonoBehaviourPun
{
    float timer = 0.0f;
    [SerializeField]
    float durationTime = 90.0f;
    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(0.0f, 0.0f, -90.0f);
        transform.LeanMoveLocalY(1.0f, 1.5f).setEaseInOutCubic().setLoopPingPong();
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

    [PunRPC]
    public void destroyObject()
    {
        //PhotonNetwork.Destroy(this.gameObject);
        //destroy by rpc (all clients)
        Destroy(this.gameObject);
    }
}
