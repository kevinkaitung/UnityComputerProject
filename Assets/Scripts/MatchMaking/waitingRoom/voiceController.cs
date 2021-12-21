using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Photon.Voice.PUN;
using Photon.Voice.Unity;

public class voiceController : MonoBehaviourPunCallbacks
{
    //voiceController Singleton
    //Only create voiceController once
    private static voiceController s_Instance = null;
    public static voiceController instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(voiceController)) as voiceController;

                if (s_Instance == null)
                    Debug.Log("Could not locate a voiceController " +
                              "object. \n You have to have exactly " +
                              "one voiceController in the scene.");
            }
            return s_Instance;
        }
    }

    public GameObject muteBtn;
    public Recorder recorder;
    public PhotonVoiceNetwork punVoiceNetwork;
    bool isMute = false;
    // when flag == 0, it's red team
    // when flag == 1, it's blue team
    int flag = 0;
    // when index == 1, it's teamMsg
    // when index == 0, it's worldMsg
    int index = 0;
    int currentChannel = 1;

    void Awake()
    {
        //punVoiceNetwork = PhotonVoiceNetwork.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        //recorder = GameObject.Find("voiceRecorder").GetComponent<Recorder>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // mute or not
    public void muteOrNot()
    {
        if (!isMute)
        {
            Button mButton = muteBtn.GetComponent<Button>();
            mButton.image.sprite = Resources.Load<Sprite>("mute");
            recorder.TransmitEnabled = false;
            isMute = !isMute;
        }
        else
        {
            Button mButton = muteBtn.GetComponent<Button>();
            mButton.image.sprite = Resources.Load<Sprite>("voice");
            recorder.TransmitEnabled = true;
            isMute = !isMute;
        }
    }

    // changeVoiceChannel
    public void changeVoiceChannel()
    {
        // get team
        // when flag == 0, it's red team
        // when flag == 1, it's blue team
        object tmp;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("_pt", out tmp);
        if ((byte)tmp == 1)
        {
            flag = 1;
        }
        else
        {
            flag = 0;
        }
        // when index == 1, it's teamMsg
        // when index == 0, it's worldMsg
        // worldGroup = 0;
        // redGroup = 1;
        // blueGroup = 2;
        if (flag == 0)
        {
            index = (index + 1) % 2;
            if (index == 1)
            {
                //Photon.Voice.PUN.PhotonVoiceNetwork.Instance.Client.OpChangeGroups(worldGroup, redGroup);
                PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[0], null);
                PhotonVoiceNetwork.Instance.Client.GlobalInterestGroup = 2;
                currentChannel = 2;
                //PhotonNetwork.SetInterestGroups((byte)0, false);
                //PhotonNetwork.SetInterestGroups((byte)1, true);
                Debug.Log("切換到紅隊語音頻道");
            }
            else
            {
                //Photon.Voice.PUN.PhotonVoiceNetwork.Instance.Client.OpChangeGroups(redGroup, worldGroup);
                PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[0], null);
                PhotonVoiceNetwork.Instance.Client.GlobalInterestGroup = 1;
                currentChannel = 1;
                Debug.Log("切換到世界語音頻道");
            }
        }
        else
        {
            index = (index + 1) % 2;
            if (index == 1)
            {
                //Photon.Voice.PUN.PhotonVoiceNetwork.Instance.Client.OpChangeGroups(worldGroup, blueGroup);
                PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[0], null);
                PhotonVoiceNetwork.Instance.Client.GlobalInterestGroup = 3;
                currentChannel = 3;
                Debug.Log("切換到藍隊語音頻道");
            }
            else
            {
                //Photon.Voice.PUN.PhotonVoiceNetwork.Instance.Client.OpChangeGroups(blueGroup, worldGroup);
                PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[0], null);
                PhotonVoiceNetwork.Instance.Client.GlobalInterestGroup = 1;
                currentChannel = 1;
                Debug.Log("切換到世界語音頻道");
            }
        }
    }

    public void changeBackToWorldChannel()
    {
        //if change scene, set to world channel
        PhotonVoiceNetwork.Instance.Client.OpChangeGroups(new byte[0], null);
        PhotonVoiceNetwork.Instance.Client.GlobalInterestGroup = (byte)1;
    }


    /**無法銷毀dontdestroyonload物件?**/
    public void destroyRecorder()
    {
        Destroy(recorder.gameObject);
    }
}