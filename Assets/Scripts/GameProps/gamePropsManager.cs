using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

//待修改：不同道具的生成時間，及障礙物生成範圍

public class gamePropsManager : MonoBehaviourPunCallbacks
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
    //which team belong
    private string myTeam;
    //timer for randomly placing game props
    private float timer1 = 0.0f;
    //timer for randomly placing game props (removal tool)
    private float timer2 = 0.0f;
    //time between placing game props
    [SerializeField]
    private float durationForProp1 = 20.0f;
    //time between placing game props (removal tool)
    [SerializeField]
    private float durationForProp2 = 30.0f;
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
    //panel for player's smoke effect
    public GameObject smokeEffectPanel;
    public int myRespawnPointIndex = 0;
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
    [SerializeField]
    private GameObject CanvasGameObject;
    [SerializeField]
    private GameObject gamePropCountdownContainer;
    private Transform gamePropCountdownContainerTransform;
    // Start is called before the first frame update
    void Start()
    {
        //get team
        object tmp;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("_pt", out tmp);
        if ((byte)tmp == 1)
        {
            myTeam = "blue";
        }
        else
        {
            myTeam = "red";
        }
        blackholeEffectCountText = blackholeEffectText.GetComponent<Text>();
        if (PhotonNetwork.IsMasterClient)
        {
            randomPlaceGameProps();
        }
        gamePropCountdownContainerTransform = gamePropCountdownContainer.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        timer1 += Time.deltaTime;
        timer2 += Time.deltaTime;
        if (timer1 > durationForProp1)
        {
            timer1 = 0.0f;
            //only master client randomly place game props
            if (PhotonNetwork.IsMasterClient)
            {
                randomPlaceGameProps();
            }
        }
        if (timer2 > durationForProp2)
        {
            timer2 = 0.0f;
            //only master client randomly place game props (removal tool)
            if (PhotonNetwork.IsMasterClient)
            {
                randomPlaceRemovalTool();
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
            randPosX = Random.Range(-15.0f, 15.0f);
            randPosZ = Random.Range(-20.0f, 20.0f);
            PhotonNetwork.Instantiate("gameProps/itembox", new Vector3(randPosX, 1.5f, randPosZ), Quaternion.identity);
        }
    }

    //randomly place game props (removal tool)
    void randomPlaceRemovalTool()
    {
        float randPosX = Random.Range(-15.0f, 15.0f);
        float randPosZ = Random.Range(-20.0f, 20.0f);
        PhotonNetwork.Instantiate("gameProps/removalToolMyself", new Vector3(randPosX, 1.0f, randPosZ), Quaternion.identity);
        randPosX = Random.Range(-15.0f, 15.0f);
        randPosZ = Random.Range(-20.0f, 20.0f);
        PhotonNetwork.Instantiate("gameProps/removalToolOther", new Vector3(randPosX, 1.0f, randPosZ), Quaternion.identity);
    }

    //when players click game props, give one effect
    public void clickGameProps(string team)
    {
        //random give a game prop effect, if the player click
        int randGameProp = Random.Range(0, 5);
        string attackedTeam = "";
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
                obstacleRandPos[2 * i] = (object)Random.Range(-22.0f, 22.0f);
                obstacleRandPos[2 * i + 1] = (object)Random.Range(-22.0f, 22.0f);
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
                obstacleRandPos[2 * i] = (object)Random.Range(-22.0f, 22.0f);
                obstacleRandPos[2 * i + 1] = (object)Random.Range(-22.0f, 22.0f);
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
                obstacleRandPos[2 * i] = (object)Random.Range(-22.0f, 22.0f);
                obstacleRandPos[2 * i + 1] = (object)Random.Range(-22.0f, 22.0f);
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

        //receive effect team
        if (randGameProp == 4)
        {
            attackedTeam = team;
        }
        else
        {
            if (team == "blue")
            {
                attackedTeam = "red";
            }
            else
            {
                attackedTeam = "blue";
            }
        }
        //game prop publisher show effect of what game prop type is
        clickShowGamePropTypeTextEffect(attackedTeam, attackEffect);
        //receive game prop effect team players show text effect of what game prop type is 
        photonView.RPC("showGamePropTypeTextEffect", RpcTarget.Others, attackedTeam, attackEffect);
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

    //game prop effect publisher show text
    private void clickShowGamePropTypeTextEffect(string attackedTeam, string attackEffect)
    {
        //instantiate new UI prefab to show text and image
        GameObject UIPrefabClone = Instantiate(Resources.Load("UIPrefab/GamePropEffectText", typeof(GameObject)), new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
        UIPrefabClone.transform.SetParent(CanvasGameObject.transform);
        //call the effect
        UIPrefabClone.GetComponent<gamePropTypeText>().activateTextEffect(attackedTeam, attackEffect);

        //if speed up effect, publisher show the countdown effect too
        if (attackEffect == "Speedup")
        {
            //check the game prop effect current does effect (是否改成foreach較安全?)
            for (int i = 0; i < gamePropCountdownContainerTransform.childCount; i++)
            {
                gamePropEffectImageCD currentEffect = gamePropCountdownContainerTransform.GetChild(i).GetComponent<gamePropEffectImageCD>();
                if (currentEffect.propEffect == attackEffect)
                {
                    //set timer 0 and set new start time
                    currentEffect.timer = 0.0f;
                    currentEffect.startTime = Time.timeSinceLevelLoad;
                    return;
                }
            }
            //game prop effect countdown image
            GameObject ImageCountdownEffect = Instantiate(Resources.Load("UIPrefab/GamePropCountdownImage", typeof(GameObject)), gamePropCountdownContainerTransform) as GameObject;
            //initiate the start time and attack effect
            ImageCountdownEffect.GetComponent<gamePropEffectImageCD>().init(Time.timeSinceLevelLoad, attackEffect);
            Debug.Log(attackEffect);
            ImageCountdownEffect.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("GamePropImg/" + attackEffect);
        }
    }


    //game prop effect receiver show text
    [PunRPC]
    private void showGamePropTypeTextEffect(string attackedTeam, string attackEffect)
    {
        if (myTeam != attackedTeam)
        {
            return;
        }
        //instantiate new UI prefab to show text and image
        GameObject UIPrefabClone = Instantiate(Resources.Load("UIPrefab/GamePropEffectText", typeof(GameObject)), new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
        UIPrefabClone.transform.SetParent(CanvasGameObject.transform);
        //call the effect
        UIPrefabClone.GetComponent<gamePropTypeText>().activateTextEffect(attackedTeam, attackEffect);


        //check the game prop effect current does effect (是否改成foreach較安全?)
        for (int i = 0; i < gamePropCountdownContainerTransform.childCount; i++)
        {
            gamePropEffectImageCD currentEffect = gamePropCountdownContainerTransform.GetChild(i).GetComponent<gamePropEffectImageCD>();
            if (currentEffect.propEffect == attackEffect)
            {
                //set timer 0 and set new start time
                currentEffect.timer = 0.0f;
                currentEffect.startTime = Time.timeSinceLevelLoad;
                return;
            }
        }
        //game prop effect countdown image
        GameObject ImageCountdownEffect = Instantiate(Resources.Load("UIPrefab/GamePropCountdownImage", typeof(GameObject)), gamePropCountdownContainerTransform) as GameObject;
        //initiate the start time and attack effect
        ImageCountdownEffect.GetComponent<gamePropEffectImageCD>().init(Time.timeSinceLevelLoad, attackEffect);
        Debug.Log(attackEffect);
        ImageCountdownEffect.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("GamePropImg/" + attackEffect);
    }

    public void gameFinishPropsDoing()
    {
        //when the game is end, disable the blackhole effect
        disableBlackholeEffecttoPlayer();
        timer1 = -80.0f;
        timer2 = -80.0f;
    }
}