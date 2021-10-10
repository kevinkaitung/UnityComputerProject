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

public class voiceController : MonoBehaviour
{
    public GameObject voice;
    public GameObject muteBtn;
    bool isMute = false;

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
}