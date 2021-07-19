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
    [SerializeField] double timer = 100;
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
    }

    // Update is called once per frame
    void Update()
    {
        //count time and check
        if (!startTimer) return;
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

    

    
    
    public void playerPutThingsOnPoint(noticePoint pointInfo, string handyMaterial, string team)
    {
        //判斷哪隊傳入，並對該隊執行動作
        if(team == "blue")
        {
            blueTeam.playerPutThing(pointInfo, handyMaterial);
            if(blueTeam.isThisTeamFinish())
            {
                gameFinishDoing();
            }
        }
        else
        {
            redTeam.playerPutThing(pointInfo, handyMaterial);
            if(redTeam.isThisTeamFinish())
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
        holdMaterialText.text = "current hold material: " + holdMaterialtoPass;
    }

}
