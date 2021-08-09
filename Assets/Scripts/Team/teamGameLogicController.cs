using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    //for blue and red team to set stage and count score
    [SerializeField]
    private GameObject blueTeamBuildingField, redTeamBuildingField;
    private teamProcessing blueTeam, redTeam;

    //enable and disable timer
    bool startTimer = false;
    //local player timer
    double timerIncrementValue;
    //master client start time shared with others
    double startTime;
    //countdown time
    [SerializeField] double timer = 10;
    [SerializeField] Text timerText;
    [SerializeField] Text holdMaterialText;

    public const byte gameTimerEventCode = 1;   //raise event for timer

    int min;    //for display timer min
    int sec;    //for display timer sec
    double tempTimer;   //for tmp store timer value

    [SerializeField] Text scoreText;    //score text, show accuracy of the this round

    [SerializeField]
    private GameObject gameFinishPanel;  //game finish panel, showing when game finish
    [SerializeField]
    private GameObject mainGamePanel;  //lobby panel, showing when game is progressing

    private float getMaterialTimer = 0.0f;  //timer for player getting material count down
    [SerializeField]
    private float getMaterialTimeDuration = 0.5f;   //max time between two click when the player gets material
    private bool startGetMaterialTimer = false;     //start player getting material timer
    [SerializeField]
    public GameObject actionInfoPanel;      //panel for the player to show some action info (ex. get what material, some warnings)
    [SerializeField]
    public GameObject actionInfoText;
    private Text actionInfoTextProp;
    [SerializeField]
    public GameObject cancelButton;     //for the player canceling getting material
    private string gettingMaterial;     //which material is the player getting now
    public bool isGetMat = false;       //if sucessfully get material or not
    private int getMatClickCount = 0;   //click count for checking if the player can get material or not
    [SerializeField]
    private int getMatClickTimes = 10;  //min click times when the player wants to get material

    [SerializeField]
    public Image Barmask;   //for progress bar

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
    }

    public void Start()
    {
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
        actionInfoTextProp = actionInfoText.GetComponent<Text>();
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
            timerText.text = "Timer  " + min.ToString() + ":" + sec.ToString("00");
            if (timerIncrementValue >= timer)
            {
                //time's up, game finish
                Debug.Log("time's up");
                gameFinishDoing();
            }
        }
    }

    public void playerPutThingsOnPoint(noticePoint pointInfo, string handyMaterial, string team)
    {
        //判斷哪隊傳入，並對該隊執行動作
        if (team == "blue")
        {
            blueTeam.playerPutThing(pointInfo, handyMaterial);
            if (blueTeam.isThisTeamFinish())
            {
                gameFinishDoing();
            }
        }
        else
        {
            redTeam.playerPutThing(pointInfo, handyMaterial);
            if (redTeam.isThisTeamFinish())
            {
                gameFinishDoing();
            }
        }
    }

    void gameFinishDoing()
    {
        //disable timer
        startTimer = false;
        gameFinishPanel.SetActive(true);
        mainGamePanel.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //顯示兩隊正確率
        scoreText.text = "正確率：\n藍隊：" + blueTeam.accuracyCount().ToString("p") + "\n紅隊：" + redTeam.accuracyCount().ToString("p");
        //Destroy Player, 5/12 查看看這樣寫是否最好
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }
        //Debug.Log("accuracy:" + accuracy);
    }

    public void backToWaitingRoomOnClick()
    {
        //5/9 test, redundant?
        //5/12待修改
        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.CurrentRoom.IsVisible = true;
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("waitingRoomScene");
        }
    }

    public void showPlayerHandyMaterial(string holdMaterialtoPass)
    {
        holdMaterialText.text = holdMaterialtoPass;
    }

    //for player call to get material
    public void playerGetMaterial(string getMaterial)
    {
        startGetMaterialTimer = true;
        getMaterialTimer = 0.0f;
        actionInfoPanel.SetActive(true);
        cancelButton.SetActive(true);
        gettingMaterial = getMaterial;
        isGetMat = false;
        getMatClickCount++;
        //show getting material count down
        actionInfoTextProp.text = getMatClickCount.ToString() + " / " + getMatClickTimes;
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
        actionInfoPanel.SetActive(false);
        cancelButton.SetActive(false);
    }

    public void playerSuccessfullyGetMat()
    {
        Debug.Log("successfully get mat");
        isGetMat = true;
    }

    //if player click the obj. too far
    public void tooFarClickNotice()
    {
        //start coroutine to show warnings
        StartCoroutine(showNotice());
    }

    IEnumerator showNotice()
    {
        actionInfoPanel.SetActive(true);
        actionInfoTextProp.text = "Closer to click the object.";
        yield return new WaitForSeconds(1);
        actionInfoPanel.SetActive(false);
    }
}