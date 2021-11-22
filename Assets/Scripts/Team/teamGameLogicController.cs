using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class teamGameLogicController : MonoBehaviourPunCallbacks, IOnEventCallback
{
    //teamGameLogicController Singleton
    //Only create NodeManager once
    private static teamGameLogicController s_Instance = null;
    public static teamGameLogicController instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(teamGameLogicController)) as teamGameLogicController;

                if (s_Instance == null)
                    Debug.Log("Could not locate a teamGameLogicController " +
                              "object. \n You have to have exactly " +
                              "one teamGameLogicController in the scene.");
            }
            return s_Instance;
        }
    }


    //for change scene
    [SerializeField]
    private GameObject blackPanel;
    //for blue and red team to set stage and count score
    [SerializeField]
    private GameObject blueTeamBuildingField, redTeamBuildingField;
    private teamProcessing blueTeam, redTeam;

    //enable and disable timer
    public bool startTimer = false;
    //local player timer
    double timerIncrementValue;
    //master client start time shared with others
    double startTime;
    //countdown time
    [SerializeField] double timer = 10;
    [SerializeField] Text timerText_left;
    [SerializeField] Text timerText_mid;
    [SerializeField] Text timerText_right;
    [SerializeField] Text holdMaterialText;
    [SerializeField] Text TPtimerText_left;
    [SerializeField] Text TPtimerText_mid;
    [SerializeField] Text TPtimerText_right;

    bool backtowaitingroomclick = false;

    public const byte gameTimerEventCode = 1;   //raise event for timer
    private byte callOthersBackEventCode = 5;   //raise event for calling others to back to waiting room

    int min;    //for display timer min
    int sec;    //for display timer sec
    double millisec;    //for display timer millisec
    double tempTimer;   //for tmp store timer value

    [SerializeField] Text scoreText;    //score text, show accuracy of the this round

    [SerializeField]
    private GameObject gameFinishPanel;  //game finish panel, showing when game finish
    [SerializeField]
    private GameObject mainGamePanel;  //lobby panel, showing when game is progressing

    [SerializeField]
    private GameObject holdMaterialImage;   //show hold material image
    private Image holdMaterialImageComponent;

    private float getMaterialTimer = 0.0f;  //timer for player getting material count down
    [SerializeField]
    private float getMaterialTimeDuration = 0.5f;   //max time between two click when the player gets material
    private bool startGetMaterialTimer = false;     //start player getting material timer
    [SerializeField]
    public GameObject takeMatActionPanel;      //panel for the player to show what material to get
    [SerializeField]
    public GameObject takeMatActionText;
    public GameObject takeMatActionImage;
    private Text takeMatActionTextComponent;
    private Image takeMatActionImageComponent;
    [SerializeField]
    public GameObject actionWarningPanel;      //panel for the player to show action warnings
    [SerializeField]
    public GameObject actionWarningText;
    private Text actionWarningTextComponent;
    private string gettingMaterial;     //which material is the player getting now
    public bool isGetMat = false;       //if sucessfully get material or not
    private int getMatClickCount = 0;   //click count for checking if the player can get material or not
    [SerializeField]
    private int getMatClickTimes = 10;  //min click times when the player wants to get material
    [SerializeField]
    public GameObject throwMaterialCube;

    [SerializeField]
    public Image Barmask;   //for progress bar

    public GameObject blackhole;

    double ringspeed;

    public GameObject BackToWaitRoomButton;

    public int countdown = 0;
    public List<string> playerlist;
    public List<string> layerlist;

    //register for raise event
    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    //receive raise event
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        //receive master client's start time
        if (eventCode == gameTimerEventCode)
        {
            //if receive time, start timer
            object[] data = (object[])photonEvent.CustomData;
            startTime = (double)data[0];
            startTimer = true;
        }
        //receive master client's call to change scene anim 
        else if (eventCode == callOthersBackEventCode)
        {
            LeanTween.scale(blackPanel, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutCubic);
        }
    }

    public void Start()
    {
        blackPanel.SetActive(true);
        LeanTween.scale(blackPanel, Vector3.zero, 0.5f).setEase(LeanTweenType.easeOutCubic);
        blueTeam = blueTeamBuildingField.GetComponent<teamProcessing>();
        redTeam = redTeamBuildingField.GetComponent<teamProcessing>();
        blueTeam.team = "blue";
        redTeam.team = "red";
        //regist for the custom data type serialization
        customTypes.register();
        if (PhotonNetwork.IsMasterClient)
        {
            //set timer before start
            object[] content = new object[] { PhotonNetwork.Time }; // Array contains the master client's start time
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
            PhotonNetwork.RaiseEvent(gameTimerEventCode, content, raiseEventOptions, SendOptions.SendReliable);
        }
        holdMaterialImageComponent = holdMaterialImage.GetComponent<Image>();
        takeMatActionTextComponent = takeMatActionText.GetComponent<Text>();
        takeMatActionImageComponent = takeMatActionImage.GetComponent<Image>();
        actionWarningTextComponent = actionWarningText.GetComponent<Text>();
        /*foreach(var i in PhotonNetwork.PlayerList)
        {
            string temp;
            temp = i.ToString().Remove(i.ToString().Length-1);
            temp = temp.Remove(0,5);
            playerlist.Add(temp);
            temp = i.ToString().Remove(3);
            layerlist.Add(temp);
        }
        foreach(var i in playerlist)
        {
            Debug.Log(i);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        //player get material count down
        if (startGetMaterialTimer)
        {
            getMaterialTimer += Time.deltaTime;
            //time out, get material again
            if (getMaterialTimer > getMaterialTimeDuration)
            {
                //closing the info panel
                playerFinishGetMaterial();
                //fail to get the material
                getMatClickCount = 0;
            }
        }
        //count time and check
        if (startTimer)
        {
            timerIncrementValue = PhotonNetwork.Time - startTime;
            tempTimer = timer - timerIncrementValue;
            min = (int)tempTimer / 60;
            sec = (int)tempTimer % 60;
            millisec = (float)tempTimer % 1 * 100;
            ringspeed = (float)tempTimer % 1 * 180;
            if (tempTimer > 30)
            {
                timerText_left.color = Color.white;
                timerText_mid.color = Color.white;
                timerText_right.color = Color.white;
                timerText_left.text = TPtimerText_left.text = min.ToString("00");
                timerText_right.text = TPtimerText_right.text = sec.ToString("00");
            }
            else
            {
                timerText_left.color = TPtimerText_left.color = Color.red;
                timerText_mid.color = TPtimerText_mid.color = Color.red;
                timerText_right.color = TPtimerText_right.color = Color.red;
                timerText_left.text = TPtimerText_left.text = min.ToString("00");
                timerText_right.text = TPtimerText_right.text = sec.ToString("00");
            }
            if (timerIncrementValue >= timer)
            {
                //time's up, game finish
                Debug.Log("time's up");
                gameFinishDoing();
            }
        }

        blackhole.transform.rotation = Quaternion.Euler(0.0f, 0.0f, (float)ringspeed);
    }

    public void playerPutThingsOnPoint(noticePoint pointInfo, string handyMaterial, string team)
    {
        //判斷哪隊傳入，並對該隊執行動作
        if (team == "blue")
        {
            blueTeam.playerPutThing(pointInfo, handyMaterial);
            if (blueTeam.isThisTeamFinish())
            {
                //call all players to finish the game
                photonView.RPC("gameFinishDoing", RpcTarget.All);
            }
        }
        else
        {
            redTeam.playerPutThing(pointInfo, handyMaterial);
            if (redTeam.isThisTeamFinish())
            {
                //call all players to finish the game
                photonView.RPC("gameFinishDoing", RpcTarget.All);
            }
        }
    }

    //if player use removal tool
    public void playerRemoveThing(string team)
    {
        //affected team
        if (team == "blue")
        {
            blueTeam.playerRemoveBuildingTexture();
        }
        else
        {
            redTeam.playerRemoveBuildingTexture();
        }
    }

    [PunRPC]
    /*async*/
    void gameFinishDoing()
    {
        //disable timer
        startTimer = false;
        gameFinishPanel.SetActive(true);
        mainGamePanel.SetActive(false);
        PlayerInputActionMode.instance.stateFour();
        //after networked-objects destroy (time's up), clear the playersInfo (which are instantiated with networked-objects)
        GodViewPlayersInfo.instance.playersInfo.Clear();
        //顯示兩隊正確率
        scoreText.text = "正確率：\n藍隊：" + blueTeam.accuracyCount().ToString("p") + "\n紅隊：" + redTeam.accuracyCount().ToString("p");
        //Destroy Player, 5/12 查看看這樣寫是否最好
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
            //await Task.Delay(10000);
            //game fight result
            StartCoroutine(showBackToWaitRoomButton());
        }
        //while (backtowaitingroomclick == false)
        {
            //await Task.Delay(1000);
            countdown++;
            //if (countdown == 20)
            {
                //backToWaitingRoomOnClick();
                //break;
            }
        }
        //Debug.Log("accuracy:" + accuracy);
    }

    IEnumerator showBackToWaitRoomButton()
    {
        //wait for 5 seconds to show back button
        yield return new WaitForSeconds(5);
        BackToWaitRoomButton.SetActive(true);
        //if player didn't click the button, auto back to waiting room
        yield return new WaitForSeconds(25);
        backToWaitingRoomOnClick();
    }



    public void backToWaitingRoomOnClick()
    {
        //5/9 test, redundant?
        //5/12待修改
        backtowaitingroomclick = true;
        Debug.Log(tempTimer);
        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.CurrentRoom.IsVisible = true;
        if (PhotonNetwork.IsMasterClient)
        {
            //raise event
            //call other players show the scene change anim (back to waiting room)
            object content = null;
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(callOthersBackEventCode, content, raiseEventOptions, SendOptions.SendReliable);
            //self scene change anim before back to waiting room
            LeanTween.scale(blackPanel, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutCubic).setOnComplete(loadNewLevel);
        }
    }

    private void loadNewLevel()
    {
        PhotonNetwork.LoadLevel("waitingRoomScene");
    }

    public void showPlayerHandyMaterial(string holdMaterialtoPass)
    {
        holdMaterialText.text = holdMaterialtoPass;
        if (holdMaterialtoPass == "empty")
        {
            holdMaterialImageComponent.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
        else
        {
            holdMaterialImageComponent.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            holdMaterialImageComponent.sprite = Resources.Load<Sprite>("materialSprite/" + holdMaterialtoPass);
        }
    }

    //for player call to get material
    public void playerGetMaterial(string getMaterial)
    {
        startGetMaterialTimer = true;
        getMaterialTimer = 0.0f;
        takeMatActionPanel.SetActive(true);
        gettingMaterial = getMaterial;
        isGetMat = false;
        getMatClickCount++;
        if (getMatClickCount > getMatClickTimes)
        {
            getMatClickCount = getMatClickTimes;
        }
        //show getting material count down
        takeMatActionTextComponent.text = getMatClickCount.ToString() + " / " + getMatClickTimes;
        //only first time click to load material image
        if (getMatClickCount == 1)
        {
            takeMatActionImageComponent.sprite = Resources.Load<Sprite>("materialSprite/" + getMaterial);
        }
        Barmask.fillAmount = (float)getMatClickCount / (float)getMatClickTimes;
        //check if the player's click count is enough
        if (getMatClickCount >= getMatClickTimes)
        {
            playerSuccessfullyGetMat();
        }
    }

    //if player sucessfully get material or cancel, close the panel 
    public void playerFinishGetMaterial()
    {
        startGetMaterialTimer = false;
        takeMatActionPanel.SetActive(false);
    }

    public void playerSuccessfullyGetMat()
    {
        Debug.Log("successfully get mat");
        isGetMat = true;
    }

    //instantiate throw material cube by player position and hold material
    public void throwMaterialCubeInstantiate(Vector3 instPos, string materialType)
    {
        GameObject throwMaterialCubeClone = Instantiate(throwMaterialCube, instPos, Quaternion.identity) as GameObject;
        throwMaterialCubeClone.GetComponent<MeshRenderer>().material = Resources.Load("materialTexture/Materials/" + materialType) as Material;
        throwMaterialCubeClone.tag = materialType;
        //assign photonview id for networked-destroy
        PhotonView PV = throwMaterialCubeClone.GetComponent<PhotonView>();
        if (PhotonNetwork.AllocateViewID(PV))
        {
            Debug.Log("success allocate view ID");
        }
        //call other players instantiate throw material cube by RPC
        photonView.RPC("OtherThrowMaterialCubeInstantiate", RpcTarget.Others, instPos, materialType, PV.ViewID);
    }

    [PunRPC]
    public void OtherThrowMaterialCubeInstantiate(Vector3 instPos, string materialType, int viewId)
    {
        GameObject throwMaterialCubeClone = Instantiate(throwMaterialCube, instPos, Quaternion.identity) as GameObject;
        throwMaterialCubeClone.GetComponent<MeshRenderer>().material = Resources.Load("materialTexture/Materials/" + materialType) as Material;
        throwMaterialCubeClone.tag = materialType;
        PhotonView PV = throwMaterialCubeClone.GetComponent<PhotonView>();
        PV.ViewID = viewId;
    }

    //player action warnings
    Coroutine warningsCoroutine = null;
    public void actionWarnings(string warnings)
    {
        //start coroutine to show warnings
        if (warningsCoroutine == null)
        {
            warningsCoroutine = StartCoroutine(showWarnings(warnings));
            Debug.Log("start coroutine");
        }
        else
        {
            //if there is coroutine active, stop it and start a new
            StopCoroutine(warningsCoroutine);
            warningsCoroutine = StartCoroutine(showWarnings(warnings));
            Debug.Log("new coroutine");
        }
    }

    IEnumerator showWarnings(string warnings)
    {
        actionWarningPanel.SetActive(true);
        actionWarningTextComponent.text = warnings;
        yield return new WaitForSeconds(1);
        actionWarningPanel.SetActive(false);
    }

}