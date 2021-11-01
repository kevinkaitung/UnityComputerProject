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
using Photon.Voice;

public class voiceController : MonoBehaviourPunCallbacks
{
    public GameObject voice;
    public GameObject muteBtn;
    bool isMute = false;
    // when flag == 0, it's red team
    // when flag == 1, it's blue team
    int flag = 0;
    // when index == 1, it's teamMsg
    // when index == 0, it's worldMsg
    int index = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // mute or not
    public void muteOrNot()
    {
        if(!isMute)
        {
            Button mButton = muteBtn.GetComponent<Button>();
            mButton.image.sprite = Resources.Load<Sprite>("voice");
            voice.SetActive(true);
            isMute = !isMute;
        }
        else
        {
            Button mButton = muteBtn.GetComponent<Button>();
            mButton.image.sprite = Resources.Load<Sprite>("mute");
            voice.SetActive(false);
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
        if((byte)tmp == 1)
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
        if(flag == 0)
        {
            index = (index + 1) % 2;
            if(index == 1)
            {
                //Photon.Voice.PUN.PhotonVoiceNetwork.Instance.Client.OpChangeGroups(worldGroup, redGroup);
                Photon.Voice.PUN.PhotonVoiceNetwork.Instance.Client.GlobalAudioGroup = 1;
                //PhotonNetwork.SetInterestGroups((byte)0, false);
                //PhotonNetwork.SetInterestGroups((byte)1, true);
                Debug.Log("切換到紅隊語音頻道");
            }
            else
            {
                //Photon.Voice.PUN.PhotonVoiceNetwork.Instance.Client.OpChangeGroups(redGroup, worldGroup);
                Photon.Voice.PUN.PhotonVoiceNetwork.Instance.Client.GlobalAudioGroup = 0;
                Debug.Log("切換到世界語音頻道");
            }
        }
        else
        {
            index = (index + 1) % 2;
            if(index == 1)
            {
                //Photon.Voice.PUN.PhotonVoiceNetwork.Instance.Client.OpChangeGroups(worldGroup, blueGroup);
                Photon.Voice.PUN.PhotonVoiceNetwork.Instance.Client.GlobalAudioGroup = 2;
                Debug.Log("切換到藍隊語音頻道");
            }
            else
            {
                //Photon.Voice.PUN.PhotonVoiceNetwork.Instance.Client.OpChangeGroups(blueGroup, worldGroup);
                Photon.Voice.PUN.PhotonVoiceNetwork.Instance.Client.GlobalAudioGroup = 0;
                Debug.Log("切換到世界語音頻道");
            }
        }
    }
}