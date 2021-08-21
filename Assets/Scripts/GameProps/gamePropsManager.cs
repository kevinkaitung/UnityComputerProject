﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

//待修改：不同道具的生成時間，及障礙物生成範圍

public class gamePropsManager : MonoBehaviourPun
{
    //gamePropsManager Singleton
    //Only create gamePropsManager once
    private static gamePropsManager s_Instance = null;
    public static gamePropsManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(gamePropsManager)) as gamePropsManager;

                if (s_Instance == null)
                    Debug.Log("Could not locate a gamePropsManager " +
                              "object. \n You have to have exactly " +
                              "one gamePropsManager in the scene.");
            }
            return s_Instance;
        }
    }
    //timer for randomly placing game props
    private float timer = 0.0f;
    //time between placing game props
    [SerializeField]
    private float durationForProp1 = 20.0f;
    //team field for generating obstacle
    [SerializeField]
    private GameObject blueTeamBuildingField, redTeamBuildingField;
    //raise event code for player's slowdown or speedup effect
    public byte slowdownEffectEventCode = 2;
    public byte speedupEffectEventCode = 3;
    //panel for locking player during blackhole effect
    public GameObject blackholeEffectPanel;
    //text for locking player count down
    public GameObject blackholeEffectText;
    private Text blackholeEffectCountText;
    //respawn point for different players
    public Transform[] reSpawnPoints;
    //generate number of flame at once
    [SerializeField]
    private int numberofFlame = 10;
    //generate number of blackhole at once
    [SerializeField]
    private int numberofBlackhole = 10;
    //generate number of smoke at once
    [SerializeField]
    private int numberofSmoke = 10;
    //control Game Prop Effect Text tweening
    [SerializeField]
    private RectTransform gamePropEffectTextRectTransform;
    //Game Prop Effect Text and Image
    [SerializeField]
    private Text teamText;
    [SerializeField]
    private Text gamePropText;
    [SerializeField]
    private Image gamePropImage;
    // Start is called before the first frame update
    void Start()
    {
        blackholeEffectCountText = blackholeEffectText.GetComponent<Text>();
        randomPlaceGameProps();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > durationForProp1)
        {
            timer = 0.0f;
            //only master client randomly place game props
            if (PhotonNetwork.IsMasterClient)
            {
                randomPlaceGameProps();
            }
        }
    }

    //randomly place game props
    void randomPlaceGameProps()
    {
        float randPosX;
        float randPosZ;
        //生成道具範圍待調整
        //place game props
        for (int i = 0; i < 5; i++)
        {
            randPosX = Random.Range(-30.0f, 30.0f);
            randPosZ = Random.Range(-30.0f, 30.0f);
            PhotonNetwork.Instantiate("gameProps/itembox", new Vector3(randPosX, 1.5f, randPosZ), Quaternion.identity);
        }
    }

    //when players click game props, give one effect
    public void clickGameProps(string team)
    {
        //random give a game prop effect, if the player click
        int randGameProp = Random.Range(0, 5);
        string attackEffect = "";
        //slowdown effect
        if (randGameProp == 0)
        {
            //raise event to all players to judge whether puts the effect or not
            gamePropSlowdownEffect(team);
            attackEffect = "Slowdown";
        }
        //flame effect
        else if (randGameProp == 1)
        {
            //generate random position first, and pass to all players by RPC
            object[] obstacleRandPos = new object[2 * numberofFlame];
            for (int i = 0; i < numberofFlame; i++)
            {
                obstacleRandPos[2 * i] = (object)Random.Range(-30.0f, 30.0f);
                obstacleRandPos[2 * i + 1] = (object)Random.Range(-30.0f, 30.0f);
            }
            //RPC to generate obstacles at all players
            photonView.RPC("gamePropFlameEffect", RpcTarget.All, team, obstacleRandPos);
            attackEffect = "Flame";
        }
        //blackhole effect
        else if (randGameProp == 2)
        {
            //generate random position first, and pass to all players by RPC
            object[] obstacleRandPos = new object[2 * numberofBlackhole];
            for (int i = 0; i < numberofBlackhole; i++)
            {
                obstacleRandPos[2 * i] = (object)Random.Range(-30.0f, 30.0f);
                obstacleRandPos[2 * i + 1] = (object)Random.Range(-30.0f, 30.0f);
            }
            //RPC to generate obstacles at all players
            photonView.RPC("gamePropBlackholeEffect", RpcTarget.All, team, obstacleRandPos);
            attackEffect = "Blackhole";
        }
        //smoke effect
        else if (randGameProp == 3)
        {
            //generate random position first, and pass to all players by RPC
            object[] obstacleRandPos = new object[2 * numberofSmoke];
            for (int i = 0; i < numberofSmoke; i++)
            {
                obstacleRandPos[2 * i] = (object)Random.Range(-30.0f, 30.0f);
                obstacleRandPos[2 * i + 1] = (object)Random.Range(-30.0f, 30.0f);
            }
            //RPC to generate obstacles at all players
            photonView.RPC("gamePropSmokeEffect", RpcTarget.All, team, obstacleRandPos);
            attackEffect = "Smoke";
        }
        //speedup effect
        else if (randGameProp == 4)
        {
            //raise event to all players to judge whether puts the effect or not
            gamePropSpeedupEffect(team);
            attackEffect = "Speedup";
        }
        //show text of what game prop is
        if (randGameProp == 4)
        {
            if (team == "red")
            {
                teamText.text = "Red Team,";
                teamText.color = Color.red;
                gamePropText.color = Color.red;
            }
            else
            {
                teamText.text = "Blue Team,";
                teamText.color = Color.blue;
                gamePropText.color = Color.blue;
            }
            gamePropText.text = "All " + attackEffect + " !";
        }
        else
        {
            if (team == "red")
            {
                teamText.text = "Attack Blue Team,";
                teamText.color = Color.blue;
                gamePropText.color = Color.blue;
            }
            else
            {
                teamText.text = "Attack Red Team,";
                teamText.color = Color.red;
                gamePropText.color = Color.red;
            }
            gamePropText.text = "By " + attackEffect + " !";
        }
        //game prop effect tweening
        gamePropImage.sprite = Resources.Load<Sprite>("GamePropImg/" + attackEffect);
        gamePropEffectTextRectTransform.LeanSetLocalPosX(700.0f);
        gamePropEffectTextRectTransform.LeanMoveLocalX(0.0f, 0.8f).setEaseOutBack();
        gamePropEffectTextRectTransform.LeanMoveLocalX(-700.0f, 0.8f).setEaseInBack().setDelay(1.2f);
    }

    public void gamePropSlowdownEffect(string team)
    {
        //raise event
        //player speed slow down effect
        string effect = "slowdown";
        object[] content = new object[] { team, effect };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(slowdownEffectEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    //only master client call this method to instantiate obstacle
    [PunRPC]
    public void gamePropFlameEffect(string team, object[] obstaclePos)
    {
        //judge which team click the game prop, and give the effect to the other team
        GameObject teamField = null;
        if (team == "blue")
        {
            teamField = redTeamBuildingField;
        }
        else if (team == "red")
        {
            teamField = blueTeamBuildingField;
        }
        //flame effect
        for (int i = 0; i < numberofFlame; i++)
        {
            //load flame object
            GameObject clone = Instantiate(Resources.Load("gameObstacle/flame/flame", typeof(GameObject))) as GameObject;
            //assign flame object to the team field
            clone.transform.parent = teamField.transform;
            clone.transform.localPosition = new Vector3((float)obstaclePos[2 * i], 0.0f, (float)obstaclePos[2 * i + 1]);
        }
    }

    //only master client call this method to instantiate obstacle
    [PunRPC]
    public void gamePropBlackholeEffect(string team, object[] obstaclePos)
    {
        //judge which team click the game prop, and give the effect to the other team
        GameObject teamField = null;
        if (team == "blue")
        {
            teamField = redTeamBuildingField;
        }
        else if (team == "red")
        {
            teamField = blueTeamBuildingField;
        }
        //blackhole effect
        for (int i = 0; i < numberofBlackhole; i++)
        {
            //load blackhole object
            GameObject clone = Instantiate(Resources.Load("gameObstacle/blackhole/blackhole", typeof(GameObject))) as GameObject;
            //assign blackhole object to the team field
            clone.transform.parent = teamField.transform;
            clone.transform.localPosition = new Vector3((float)obstaclePos[2 * i], 0.0f, (float)obstaclePos[2 * i + 1]);
        }
    }

    //only master client call this method to instantiate obstacle
    [PunRPC]
    public void gamePropSmokeEffect(string team, object[] obstaclePos)
    {
        //judge which team click the game prop, and give the effect to the other team
        GameObject teamField = null;
        if (team == "blue")
        {
            teamField = redTeamBuildingField;
        }
        else if (team == "red")
        {
            teamField = blueTeamBuildingField;
        }
        //smoke effect
        for (int i = 0; i < numberofSmoke; i++)
        {
            //load smoke object
            GameObject clone = Instantiate(Resources.Load("gameObstacle/smoke/smoke", typeof(GameObject))) as GameObject;
            //assign smoke object to the team field
            clone.transform.parent = teamField.transform;
            clone.transform.localPosition = new Vector3((float)obstaclePos[2 * i], 0.0f, (float)obstaclePos[2 * i + 1]);
        }
    }

    public void gamePropSpeedupEffect(string team)
    {
        //raise event
        //player speed slow down effect
        string effect = "speedup";
        object[] content = new object[] { team, effect };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(speedupEffectEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    //give the player blackhole effect by lock the window
    //須修正至真正鎖住玩家的所有動作
    public void enableBlackholeEffecttoPlayer()
    {
        blackholeEffectPanel.SetActive(true);
    }

    //close the player blackhole effect
    public void disableBlackholeEffecttoPlayer()
    {
        blackholeEffectPanel.SetActive(false);
    }

    //display countdown seconds for the player
    public void blackholeEffectCountdown(float remainingSecs)
    {
        blackholeEffectCountText.text = "You are locked by Blackhole.\n Please Wait for " + ((int)remainingSecs).ToString() + " seconds.";
    }
}