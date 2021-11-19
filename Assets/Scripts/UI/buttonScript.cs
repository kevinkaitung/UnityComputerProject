﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonScript : MonoBehaviour
{
    public GameObject blueprintPanel;
    public GameObject optionPanel;
    public GameObject chatroomPanel;
    public GameObject godCamera;
    public GameObject teachingPanel;

    bool clicktimeBP;
    bool clicktimeOP;
    bool clicktimeCP;
    bool clicktimeGC;

    // Start is called before the first frame update
    void Start()
    {
        blueprintPanel.SetActive(false);
        optionPanel.SetActive(false);
        chatroomPanel.SetActive(false);
        godCamera.SetActive(false);
        GodViewPlayersInfo.instance.GodViewInfoPanel.SetActive(false);
        /*LeanTween.scale(blueprintPanel, new Vector3(0, 0, 0), 0.1f);
        LeanTween.moveX(blueprintPanel.GetComponent<RectTransform>(), 415f, 0.1f);
        LeanTween.moveY(blueprintPanel.GetComponent<RectTransform>(), -148f, 0.1f);*/
        clicktimeBP = false;
        clicktimeOP = false;
        clicktimeCP = false;
        clicktimeGC = false;
    }

    void Update()
    {
        if (PlayerInputActionMode.instance.state == 1 || PlayerInputActionMode.instance.state == 2)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                PlayerInputActionMode.instance.stateThree();
                blueprintPanel.SetActive(true);
                clicktimeBP = true;
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                PlayerInputActionMode.instance.stateThree();
                optionPanel.SetActive(true);
                clicktimeOP = true;
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                PlayerInputActionMode.instance.stateThree();
                chatroomPanel.SetActive(true);
                clicktimeCP = true;
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                PlayerInputActionMode.instance.stateThree();
                godCamera.SetActive(true);
                clicktimeGC = true;
                GodViewPlayersInfo.instance.GodViewInfoPanel.SetActive(true);
            }
        }
        //state 3
        else if (PlayerInputActionMode.instance.state == 3)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                blueprintPanel.SetActive(false);
                optionPanel.SetActive(false);
                chatroomPanel.SetActive(false);
                teachingPanel.SetActive(false);
                clicktimeBP = false;
                clicktimeOP = false;
                clicktimeCP = false;
                //check for whether other actions are active or not
                if (!clicktimeBP && !clicktimeOP && !clicktimeCP && !clicktimeGC)
                {
                    PlayerInputActionMode.instance.stateOne();
                }
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                godCamera.SetActive(false);
                clicktimeGC = false;
                GodViewPlayersInfo.instance.GodViewInfoPanel.SetActive(false);
                //check for whether other actions are active or not
                if (!clicktimeBP && !clicktimeOP && !clicktimeCP && !clicktimeGC)
                {
                    PlayerInputActionMode.instance.stateOne();
                }
            }
        }
    }


    public void callBlueprintPanel()
    {
        if (clicktimeBP == false)
        {
            blueprintPanel.SetActive(true);
            clicktimeBP = true;
        }
        else if (clicktimeBP == true)
        {
            blueprintPanel.SetActive(false);
            clicktimeBP = false;
            /*Debug.Log("freecount " + freecount);
            freecount = StageNumber;
            Debug.Log("StageNumber " + StageNumber);
            switch (StageNumber)
            {
                case 1:
                    Blueprint.sprite = one_front;
                    break;
                case 2:
                    Blueprint.sprite = one_front;
                    break;
                case 3:
                    Blueprint.sprite = three_front;
                    break;
                case 4:
                    Blueprint.sprite = three_front;
                    break;
                case 5:
                    Blueprint.sprite = five_front;
                    break;
                case 6:
                    Blueprint.sprite = six;
                    break;
            }*/
        }
    }

    public void callOptionPanel()
    {
        if (clicktimeOP == false)
        {
            optionPanel.SetActive(true);
            clicktimeOP = true;
        }
        else if (clicktimeOP == true)
        {
            optionPanel.SetActive(false);
            clicktimeOP = false;
        }
    }

    public void callChatRoomPanel()
    {
        if (clicktimeCP == false)
        {
            chatroomPanel.SetActive(true);
            clicktimeCP = true;
        }
        else if (clicktimeCP == true)
        {
            chatroomPanel.SetActive(false);
            clicktimeCP = false;
        }
    }

    public void callUIChangeState()
    {
        if (clicktimeBP == true || clicktimeOP == true || clicktimeCP == true || clicktimeGC == true)
        {
            PlayerInputActionMode.instance.stateThree();
            Debug.Log("state 3");
        }
        else
        {
            PlayerInputActionMode.instance.stateOne();
            Debug.Log("state 1");
        }
    }
}
