using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class voiceSpeakerLifetime : MonoBehaviourPunCallbacks
{   
    //once enter waiting room, set dontDestroyOnLoad (use this one until the player leave room)
    //will be destroy by PhotonNetwork.DestroyAll()
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //once the player leave room, destroy this speaker
    public override void OnLeftRoom()
    {
        Destroy(this.gameObject);
    }
}
