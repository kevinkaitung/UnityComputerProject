using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class objLifetime : MonoBehaviourPunCallbacks
{
    public static GameObject instance;
    void Awake()
    {
        //don't destroy team manager when entering the main game
        //check if the team manger is duplicate or not
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this.gameObject;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //when the player left room, destroy team manager
    public override void OnLeftRoom()
    {
        Destroy(this.gameObject);
        Debug.Log("destroy this");
    }
}
