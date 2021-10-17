using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class temp : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void joinRoomTemp()
    {
        PhotonNetwork.JoinRoom("AAA");
    }
}
