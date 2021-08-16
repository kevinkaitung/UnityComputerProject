using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class gameLogicController : MonoBehaviourPunCallbacks, IOnEventCallback
{
    //gameLogicController Singleton
    //Only create NodeManager once
    private static gameLogicController s_Instance = null;
    public static gameLogicController instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(gameLogicController)) as gameLogicController;

                if (s_Instance == null)
                    Debug.Log("Could not locate a gameLogicController " +
                              "object. \n You have to have exactly " +
                              "one gameLogicController in the scene.");
            }
            return s_Instance;
        }
    }

    //how many point to put in this stage
    private int thisStageCount;
    //how many points have been clicked by player (already build that point)
    private int thisStagePutCount;
    //current stage
    private int currentStageNumber;
    //game data nodes' length
    private int dataNodeLen;
    //index for access game data nodes
    private int index;
    //index for notice points' photon view ID array
    private int viewidIndex = 0;
    //calculate accuracy
    private float accuracy;
    //count correct step
    private int correctCount;

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
        currentStageNumber = 1;
        dataNodeLen = nodeManager.instance.dataRoot.gameDataNodes.Length;
        index = 0;
        accuracy = 0.0f;
        correctCount = 0;
        //regist for the custom data type serialization
        customTypes.register();
        if (PhotonNetwork.IsMasterClient)
        {
            //set timer before start
            object[] content = new object[] { PhotonNetwork.Time }; // Array contains the master client's start time
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
            PhotonNetwork.RaiseEvent(gameTimerEventCode, content, raiseEventOptions, SendOptions.SendReliable);
            //setting first stage
            //photonView.RPC("otherSettingStage", RpcTarget.AllBuffered, currentStageNumber);
            settingStage(currentStageNumber);
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
        timerText.text = "text " + min.ToString() + ":" + sec.ToString("00");
        if (timerIncrementValue >= timer)
        {
            //time's up, game finish
            Debug.Log("time's up");
            gameFinishDoing();
        }
    }

    void settingStage(int stageNumber)
    {
        //notice points' photon view ID list
        List<int> viewIDs = new List<int>();
        //int[] viewIDs = new int[20];
        //viewidIndex = 0;
        thisStageCount = 0;
        thisStagePutCount = 0;
        if (index < dataNodeLen)
        {
            //show notice points info on blueprint
            bluePrint.instance.bluePrintText.text = "stage : " + stageNumber.ToString() + "\n";
            //put notice points of this stage
            while (nodeManager.instance.dataRoot.gameDataNodes[index].stage == stageNumber)
            {
                GameObject clone = Instantiate(Resources.Load("noticePoint", typeof(GameObject)), nodeManager.instance.dataRoot.gameDataNodes[index].position, Quaternion.identity) as GameObject;
                noticePoint tmp = clone.GetComponent<noticePoint>();
                tmp.pos = nodeManager.instance.dataRoot.gameDataNodes[index].position;
                tmp.rot = nodeManager.instance.dataRoot.gameDataNodes[index].rotation;
                tmp.sca = nodeManager.instance.dataRoot.gameDataNodes[index].scale;
                tmp.objShap = nodeManager.instance.dataRoot.gameDataNodes[index].objShape;
                tmp.materialNam = nodeManager.instance.dataRoot.gameDataNodes[index].materialName;
                tmp.stag = nodeManager.instance.dataRoot.gameDataNodes[index].stage;
                //show each notice point info
                bluePrint.instance.bluePrintText.text += "The material of " + tmp.objShap + " is " + tmp.materialNam + "\n";
                PhotonView PV = clone.GetComponent<PhotonView>();
                if (PhotonNetwork.AllocateViewID(PV))
                {
                    //viewIDs[viewidIndex] = PV.ViewID;
                    //viewidIndex++;
                    viewIDs.Add(PV.ViewID);
                }
                index++;
                thisStageCount++;
                if (index == dataNodeLen)
                {
                    break;
                }
            }
        }
        else
        {
            gameFinishDoing();
            Debug.Log("Game Finish!!!");
        }
        //when passing parameter via network, need to change list to array
        photonView.RPC("otherSettingStage", RpcTarget.OthersBuffered, currentStageNumber, viewIDs.ToArray());
    }

    //only master client need to load settingStage()
    [PunRPC]
    void otherSettingStage(int stageNumber, int[] viewIDs)
    {
        viewidIndex = 0;
        //dataNodeLen = nodeManager.instance.dataRoot.gameDataNodes.Length;
        thisStageCount = 0;
        thisStagePutCount = 0;
        if (index < dataNodeLen)
        {
            //show notice points info on blueprint
            bluePrint.instance.bluePrintText.text = "stage : " + stageNumber.ToString() + "\n";
            //put notice points of this stage
            while (nodeManager.instance.dataRoot.gameDataNodes[index].stage == stageNumber)
            {
                GameObject clone = Instantiate(Resources.Load("noticePoint", typeof(GameObject)), nodeManager.instance.dataRoot.gameDataNodes[index].position, Quaternion.identity) as GameObject;
                noticePoint tmp = clone.GetComponent<noticePoint>();
                tmp.pos = nodeManager.instance.dataRoot.gameDataNodes[index].position;
                tmp.rot = nodeManager.instance.dataRoot.gameDataNodes[index].rotation;
                tmp.sca = nodeManager.instance.dataRoot.gameDataNodes[index].scale;
                tmp.objShap = nodeManager.instance.dataRoot.gameDataNodes[index].objShape;
                tmp.materialNam = nodeManager.instance.dataRoot.gameDataNodes[index].materialName;
                tmp.stag = nodeManager.instance.dataRoot.gameDataNodes[index].stage;
                //show each notice point info
                bluePrint.instance.bluePrintText.text += "The material of " + tmp.objShap + " is " + tmp.materialNam + "\n";
                PhotonView PV = clone.GetComponent<PhotonView>();
                PV.ViewID = viewIDs[viewidIndex];
                viewidIndex++;
                index++;
                thisStageCount++;
                if (index == dataNodeLen)
                {
                    break;
                }
            }
        }
        else
        {
            gameFinishDoing();
            Debug.Log("Game Finish!!!");
        }
    }
    
    public void playerPutThingsOnPoint(noticePoint pointInfo, string handyMaterial)
    {
        thisStagePutCount++;
        //Put right game object on the player clicked point
        Debug.Log("obj shape: " + pointInfo.objShap + " obj pos: " + pointInfo.pos);
        //create part of the building
        GameObject partOfBuildingClone = Instantiate(Resources.Load("house/" + pointInfo.objShap, typeof(GameObject)), pointInfo.pos, Quaternion.Euler(pointInfo.rot)) as GameObject;

        //set the texture to handyMaterial
        Renderer cloneRend = partOfBuildingClone.GetComponent<Renderer>();
        cloneRend.material.mainTexture = Resources.Load("texture of building/" + handyMaterial) as Texture;
        //check player put is correct or not (calculate accuracy)
        if (handyMaterial == pointInfo.materialNam)
        {
            correctCount++;
        }
        //call others to deal with the game logic
        photonView.RPC("otherPlayerPutThingOnPoint", RpcTarget.Others, pointInfo, handyMaterial);
        //if all notice points have been clicked (put), go next stage
        if (PhotonNetwork.IsMasterClient)
        {
            if (thisStagePutCount == thisStageCount)
            {
                currentStageNumber++;
                Debug.Log("go next stage: " + currentStageNumber);
                settingStage(currentStageNumber);
                //photonView.RPC("otherSettingStage", RpcTarget.Others, currentStageNumber);
            }
        }
        Debug.Log("Put Count: " + thisStagePutCount + " stage total count: " + thisStageCount);
    }

    [PunRPC]
    public void otherPlayerPutThingOnPoint(noticePoint pointInfo, string handyMaterial)
    {
        thisStagePutCount++;
        //create part of the building
        GameObject partOfBuildingClone = Instantiate(Resources.Load("house/" + pointInfo.objShap, typeof(GameObject)), pointInfo.pos, Quaternion.Euler(pointInfo.rot)) as GameObject;
        //set the texture to handyMaterial
        Renderer cloneRend = partOfBuildingClone.GetComponent<Renderer>();
        cloneRend.material.mainTexture = Resources.Load("texture of building/" + handyMaterial) as Texture;
        //check player put is correct or not (calculate accuracy)
        if (handyMaterial == pointInfo.materialNam)
        {
            correctCount++;
        }
        //the thing that player put on point was created by that player, don't need to put it again!
        if (PhotonNetwork.IsMasterClient)
        {
            if (thisStagePutCount == thisStageCount)
            {
                currentStageNumber++;
                Debug.Log("go next stage: " + currentStageNumber);
                settingStage(currentStageNumber);
                //photonView.RPC("otherSettingStage", RpcTarget.Others, currentStageNumber);
            }
        }
        Debug.Log("Put Count: " + thisStagePutCount + " stage total count: " + thisStageCount);
    }

    void gameFinishDoing()
    {
        //disable timer
        startTimer = false;
        gameFinishPanel.SetActive(true);
        mainGamePanel.SetActive(false);
        accuracy = (float)correctCount / (float)dataNodeLen;
        scoreText.text = "正確率：" + accuracy.ToString("p");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //Destroy Player, 5/12 查看看這樣寫是否最好
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }
        Debug.Log("accuracy:" + accuracy);
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

}
