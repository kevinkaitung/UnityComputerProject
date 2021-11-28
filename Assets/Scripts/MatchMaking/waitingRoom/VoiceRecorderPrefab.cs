using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Photon.Voice.PUN;
using UnityEngine.SceneManagement;
using Photon.Voice.Unity;

public class VoiceRecorderPrefab : MonoBehaviourPunCallbacks
{
    public override void OnEnable()
    {
        PhotonVoiceNetwork.Instance.Client.StateChanged += this.VoiceClientStateChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        PhotonVoiceNetwork.Instance.Client.StateChanged -= this.VoiceClientStateChanged;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //back to launch scene, destroy this object
        //?因無法呼叫onLeft函式，所以由此執行銷毀
        if (scene.buildIndex == 0)
        {
            //Destroy(this.gameObject);
            return;
        }
        //if change scenes (waiting room or main game), reassign this recorder to voice controller
        voiceController.instance.recorder = this.gameObject.GetComponent<Recorder>();
        Debug.Log("On Scene Loaded call back function");
    }

    private void VoiceClientStateChanged(Photon.Realtime.ClientState fromState, Photon.Realtime.ClientState toState)
    {
        int currentChannel = 1; //set channel to 1
        //once connected, set the voice channel to number 1 (our default world channel)
        if (toState == Photon.Realtime.ClientState.Joined)
        {
            PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[0], null);
            PhotonVoiceNetwork.Instance.Client.GlobalInterestGroup = (byte)currentChannel;
            Debug.Log("On Connect Event!");
        }
        //leave room (disconnected), destroy this object
        else if (toState == Photon.Realtime.ClientState.Disconnected)
        {
            Destroy(this.gameObject);
            Debug.Log("Disconnected!");
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


    //離開房間時，並不會呼叫此函式?(目前須由OnSceneLoaded去銷毀?)
    public override void OnLeftRoom()
    {
        Debug.Log("I'm left room");
        Destroy(this.gameObject);
    }
}
